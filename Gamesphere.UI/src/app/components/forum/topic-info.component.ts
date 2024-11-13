import { Component, OnInit, ViewChild, ElementRef } from "@angular/core";
import { Location, NgIf, NgFor } from "@angular/common";
import { ActivatedRoute, Router, RouterModule } from "@angular/router";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { QuillEditorModule } from "../../quill-editor.module";

// Components
import { AuthorizedHeaderComponent } from "../header/authorized-header.component";
import { UnauthorizedHeaderComponent } from "../header/unauthorized-header.component";
import { FooterComponent } from "../footer/footer.component";

// Models
import { UserAuthorizedModel } from "../../models/user-authorized.model";
import { AuthorizationResponse } from "../../extensions/authorization-response";
import { FullPostInfoModel } from "../../models/full-post-info-model";
import { SendEmailModel } from "../../models/send-email.model";
import { SendReplyModel } from "../../models/send-reply.model";
import { Pagination } from "../../extensions/pagination";

// Services
import { AuthenticationService } from "../../services/authentication.service";
import { ForumService } from "../../services/forum.service";
import { FileService } from "../../services/file.service";
import { EmailService } from "../../services/email.service";
import { ReplyInfoModel } from "../../models/reply-info.model";
import { ReplyOptions } from "../../extensions/reply-options";
import { ShortPostInfoModel } from "../../models/short-post-info.model";

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [AuthorizedHeaderComponent, UnauthorizedHeaderComponent, FooterComponent, NgIf, NgFor, QuillEditorModule, FormsModule,
        ReactiveFormsModule, RouterModule],
    templateUrl: './topic-info.component.html',
    styleUrl: './topic-info.component.css'
})
export class TopicInfoComponent implements OnInit {
    ownUserData: UserAuthorizedModel;
    isAuthorized: AuthorizationResponse;
    authorizationResponse = AuthorizationResponse;
    isVisibleHeader: boolean;
    post: FullPostInfoModel | null;
    isSavedPost: boolean;
    isReplyToPostFormVisible: boolean;
    earnLimitSymbols: boolean;
    replyToPostModel: SendReplyModel;
    resizedImageSrc: string | null = null;

    pagination: Pagination;
    replies: ReplyInfoModel[];
    similarPosts: ShortPostInfoModel[];


    quillConfig = {
        modules: {
            toolbar: [
                [{ 'header': [1, 2, false] }],
                ['bold', 'italic', 'underline', 'strike'],
                ['link', 'image', 'video'],
                [{ 'list': 'ordered' }, { 'list': 'bullet' }],
                ['clean'], // Кнопка для очищення форматування
                [{ 'align': [] }],
                [{ 'color': [] }, { 'background': [] }],
                ['code-block'],
                ['blockquote'],
                ['formula'], // Додає підтримку формул
            ],
        }
    };

    constructor(private authenticationService: AuthenticationService, private location: Location, private activatedRoute: ActivatedRoute,
        private router: Router, private forumService: ForumService, private fileService: FileService, private emailService: EmailService
    ) {
        this.ownUserData = new UserAuthorizedModel('', '', '', '', '');
        this.isVisibleHeader = false;
        this.isAuthorized = AuthorizationResponse.TokenNotFound;
        this.post = null;
        this.isSavedPost = false;
        this.isReplyToPostFormVisible = false;
        this.earnLimitSymbols = false;
        this.replyToPostModel = new SendReplyModel('', '', '', null);
        this.pagination = new Pagination(1, 0, 5);
        this.replies = [];
        this.similarPosts = [];
    }

    navigateTo(link: string) {
        this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
            this.router.navigate([link]);
        });
    }

    ngOnInit(): void {
        this.isAuthorized = this.authenticationService.isAuthorized();

        if (this.isAuthorized === this.authorizationResponse.TokenExpired) {
            let refreshToken = confirm('Your session has finished. Do you want to reset your session?');

            if (refreshToken) {
                this.authenticationService.refreshToken();

                setTimeout(() => {
                    window.location.reload();
                }, 2000);

            } else {
                this.authenticationService.clearLocalStorage();
                alert('You have logged out from the account!');
            }
        }

        this.ownUserData = this.authenticationService.getAuthorizeHeaderData();
        this.isVisibleHeader = true;
        let postId = null;
        this.activatedRoute.params.subscribe(params => postId = params['id']);

        if (postId !== null) {
            this.getPost(postId);
            this.replyToPostModel.postId = postId;
        } else {
            this.router.navigateByUrl('/error');
        }

        this.replyToPostModel.userId = this.ownUserData.id;
    }
    // Налаштування частини компоненту для посту

    private getPost(postId: string) {
        this.forumService.getPostInfo(postId)
            .subscribe(response => {
                // get user icon
                this.fileService.downloadFileFromDropbox(response.user.iconDirectory)
                    .subscribe(
                        (link: string) => {
                            response.user.iconDirectory = link;
                        });

                // get game icon
                if (response.game) {
                    this.fileService.downloadFileFromDropbox('/' + response.game.mainImageDirectory)
                        .subscribe(
                            (link: string) => {
                                response.game.mainImageDirectory = link;
                            });
                }

                this.post = response;

                this.checkPostInFavoriteList();
                this.getReplies();
                this.getSimilarPosts();
            }, error => { });
    }

    private getSimilarPosts() {
        if(this.post){
            const clearText = this.post.content.replace(/<[^>]*>/g, '');

            if(clearText.length === 0){
                return;
            }

            const gameId = (this.post.game) ? this.post.game.id : null;

            this.forumService.getSimilarTopics(this.post.id, clearText, gameId, 5)
            .subscribe(response => {
                this.similarPosts = response;

                this.similarPosts.forEach(post => {
                    this.fileService.downloadFileFromDropbox(post.userInfo.iconDirectory)
                    .subscribe(
                        (link: string) => {
                            post.userInfo.iconDirectory = link;
                        });
                });
            }, error => {});
        }
    }

    deletePost() {
        let accessToken = this.authenticationService.getAccessToken();
        if (this.post) {
            this.forumService.deletePost(accessToken, this.post?.id)
                .subscribe(response => {
                    if (response.isSuccess) {
                        alert(response.message);
                        this.location.back();
                    }
                });
        }
    }

    private checkPostInFavoriteList() {
        if (this.post) {
            this.forumService.checkPostInFavorite(this.post.id, this.ownUserData.id)
                .subscribe(response => this.isSavedPost = response.isSuccess, error => { });
        }
    }

    addPostToFavorite() {
        if (this.post) {
            let accessToken = this.authenticationService.getAccessToken();
            this.forumService.addPostToFavorite(accessToken, this.post.id, this.ownUserData.id)
                .subscribe(response => {
                    if (response.isSuccess) {
                        this.checkPostInFavoriteList();
                    }
                }, error => { });
        }
    }

    deletePostFromFavorite() {
        if (this.post) {
            let accessToken = this.authenticationService.getAccessToken();
            this.forumService.deletePostFromFavorite(accessToken, this.post.id, this.ownUserData.id)
                .subscribe(response => {
                    if (response) {
                        this.checkPostInFavoriteList();
                    }
                }, error => { });
        }
    }

    removePostAsPrivilegedUser() {
        if (this.post && (this.ownUserData.role === 'Creator' || this.ownUserData.role === 'Admin')) {
            let reasonText = prompt('Reason:');

            if (!reasonText || reasonText.length === 0) {
                alert('Describe your reason!');
                return;
            }

            let removedBy = `${this.ownUserData.username}-${this.ownUserData.role}`;
            let emailMessage = `<p>Dear <b>${this.post?.user.userName}</b>,</p>`;
            emailMessage += `<br><p>Your post has been removed by ${removedBy}</p>`;
            emailMessage += `<br><p>Topic: <b>${this.post?.topic}</b></p>`;
            emailMessage += `<br><p>Reason: <i>${reasonText}</i></p>`;
            emailMessage += `<br><p>Our team hopes that you will be a good person in future posts next time. Good luck! :)</p>`;
            emailMessage += `<br><br><b>GAMESPHERE TEAM</b>`;

            let emailModel = new SendEmailModel(this.post.user.email, 'Removed Post', emailMessage, true);

            this.emailService.sendEmail(emailModel)
                .subscribe();

            this.deletePost();
        }
    }

    reportPost() {
        if (this.post) {
            let description = prompt('Description:');

            if (!description || description.length === 0) {
                alert('Describe your reason!');
                return;
            }

            let emailMessage = `<p>${this.ownUserData.role} ${this.ownUserData.username} sent post report to user ${this.post?.user.userName}.</p><br>`;
            emailMessage += `<p>Reason of the report: <i>${description}</i>.</p>`;
            emailMessage += `<a href="http://localhost:4200/forum/${this.post.id}" target='_blank'>Post link</a>`;

            let emailModel = new SendEmailModel('gamesphere7438@gmail.com', 'Post Report', emailMessage, true);

            this.emailService.sendEmail(emailModel)
                .subscribe();
        }
    }

    onContentChanged(event: any): void {
        // Отримуємо HTML-код з редактора
        const htmlContent = event.html;

        if (htmlContent) {
            // Видаляємо всі HTML-теги, залишаючи лише текст
            const plainText = htmlContent.replace(/<[^>]*>/g, '');
            const textLength = plainText.length;

            this.earnLimitSymbols = textLength > 10000;
        }
    }

    triggerFileInput() {
        const fileInput = document.getElementById('fileInput') as HTMLInputElement;
        if (fileInput) {
            fileInput.click(); // Викликаємо input
        }
    }

    onFileSelected(event: any): void {
        const file = event.target.files[0];
        if (file) {
            const fileName = file.name.split('.')[0]; // Отримуємо назву файлу без розширення
            const fileExtension = file.name.split('.').pop(); // Отримуємо розширення файлу
            const reader = new FileReader();
            reader.onload = (e: any) => {
                const image = new Image();
                image.src = e.target.result;

                image.onload = () => {
                    const newHeight = parseInt(prompt('Enter the new height for the image:', `${image.height}`) || '0', 10);
                    const newWidth = parseInt(prompt('Enter the new width for the image:', `${image.width}`) || '0', 10);

                    if (newHeight > 0 && newWidth > 0) {
                        const canvas = document.createElement('canvas');
                        canvas.width = newWidth;
                        canvas.height = newHeight;
                        const ctx = canvas.getContext('2d');

                        if (ctx) {
                            ctx.drawImage(image, 0, 0, newWidth, newHeight);
                            this.resizedImageSrc = canvas.toDataURL('image/jpeg');

                            // Trigger download
                            const link = document.createElement('a');
                            link.href = this.resizedImageSrc;
                            link.download = fileName.includes('-resized') ? `${fileName}.${fileExtension}` : `${fileName}-resized.${fileExtension}`;
                            link.click();
                        }
                    } else {
                        alert('Invalid height or width!');
                    }
                };
            };

            reader.readAsDataURL(file);
        }
    }

    // TODO: Поведінка під час отримання відповідей
    sendReply(reply: SendReplyModel) {
        if (reply.content && reply.content.length > 0) {
            let accessToken = this.authenticationService.getAccessToken();

            this.forumService.sendReply(accessToken, reply)
                .subscribe(response => {
                    if (response.isSuccess) {
                        if (reply.replyToId === null) {
                            this.isReplyToPostFormVisible = false;
                            this.replyToPostModel.content = '';
                        }

                        alert(response.message);

                        this.updateReplies();
                    }
                }, error => { })

        } else {
            alert('You cannot send an empty text!');
        }
    }

    // Налаштування частини компоненту для відповідей

    getReplies() {
        if (this.post) {
            this.forumService.getReplies(this.pagination, this.post.id)
                .subscribe(response => {
                    this.pagination.count = response.count
                    this.replies = response.items as ReplyInfoModel[];

                    this.replies.forEach(reply => {
                        this.fileService.downloadFileFromDropbox(reply.user.iconDirectory)
                            .subscribe(
                                (link: string) => {
                                    reply.user.iconDirectory = link;
                                });

                        reply.options = new ReplyOptions(false, 'Replied to', '');

                        if (reply.replyToId === null && this.post) {
                            reply.options.replyToText += ` ${this.post.user.userName}`;
                        } else {
                            const userName = this.replies.find(r => r.id === reply.replyToId)?.user.userName;

                            if (userName) {
                                reply.options.replyToText += ` ${userName}`;
                            }
                        }
                    });
                }, error => { });
        }
    }

    deleteReply(replyId: string) {
        let accessToken = this.authenticationService.getAccessToken();

        this.forumService.deleteReply(accessToken, replyId)
            .subscribe(response => {
                if (response.isSuccess) {
                    alert(response.message);

                    if (this.replies.length > 1) {
                        this.updateReplies();
                    } else {
                        this.replies = [];
                    }
                }
            }, error => { });
    }

    updateReplies() {
        let page = this.pagination.page;
        let pageSize = this.pagination.pageSize;
        this.pagination.page = 1;
        this.pagination.pageSize = pageSize * page;

        this.getReplies();

        this.pagination.page = page;
        this.pagination.pageSize = pageSize;
    }

    @ViewChild('replyList', { static: true }) replyList!: ElementRef;

    scrollToReply(reply: ReplyInfoModel) {
        // Якщо replyToId відсутній, скролимо до початку сторінки
        if (!reply.replyToId) {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
            return;
        }

        // Шукаємо елемент за його ID
        const targetElement = document.getElementById('reply-' + reply.replyToId);

        if (targetElement) {
            // Плавний скрол до елемента
            targetElement.scrollIntoView({ behavior: 'smooth', block: 'center' });

            // Додаємо клас для анімації
            targetElement.classList.add('jump-animation');

            // Після завершення анімації знімаємо клас
            setTimeout(() => {
                targetElement.classList.remove('jump-animation');
            }, 3000); // Тривалість анімації
        }
    }

    removeReplyAsPrivilegedUser(reply: ReplyInfoModel) {
        if (this.ownUserData.role === 'Creator' || this.ownUserData.role === 'Admin') {
            let reasonText = prompt('Reason:');

            if (!reasonText || reasonText.length === 0) {
                alert('Describe your reason!');
                return;
            }

            let removedBy = `${this.ownUserData.username}-${this.ownUserData.role}`;
            let emailMessage = `<p>Dear <b>${this.post?.user.userName}</b>,</p>`;
            emailMessage += `<br><p>Your reply to post has been removed by ${removedBy}</p>`;
            emailMessage += `<br><p>Topic: <b>${this.post?.topic}</b></p>`;
            emailMessage += `<br><p>Reason: <i>${reasonText}</i></p>`;
            emailMessage += `<br><p>Reply text:</p><br>${reply.content}`
            emailMessage += `<br><p>Our team hopes that you will be a good person in future posts next time. Good luck! :)</p>`;
            emailMessage += `<br><br><b>GAMESPHERE TEAM</b>`;

            let emailModel = new SendEmailModel(reply.user.email, 'Removed Reply', emailMessage, true);

            this.emailService.sendEmail(emailModel)
                .subscribe();

            this.deleteReply(reply.id);
        }
    }

    reportReply(text: string) {
        if (this.post) {
            let description = prompt('Description:');

            if (!description || description.length === 0) {
                alert('Describe your reason!');
                return;
            }

            let emailMessage = `<p>${this.ownUserData.role} ${this.ownUserData.username} sent post report to user ${this.post?.user.userName}.</p><br>`;
            emailMessage += `<p>Reason of the report: <i>${description}</i>.</p>`;
            emailMessage += `<a href="http://localhost:4200/forum/${this.post.id}" target='_blank'>Post link</a>`;
            emailMessage += `<br><p>Reply text:</p><br>${text}`

            let emailModel = new SendEmailModel('gamesphere7438@gmail.com', 'Reply Report', emailMessage, true);

            this.emailService.sendEmail(emailModel)
                .subscribe();
        }
    }

    earnReplyTextLimitSymbols(text: string): boolean {
        const plainText = text.replace(/<[^>]*>/g, '');
        const textLength = plainText.length;

        return textLength > 5000;
    }

    sendReplyToReply(reply: ReplyInfoModel) {
        let sendRtoR = new SendReplyModel(this.ownUserData.id, reply.postId, reply.options.replyToContent, reply.id);

        this.sendReply(sendRtoR);
    }

    moreRepliesAvailable(): boolean {
        return (this.pagination.page * this.pagination.pageSize) <= this.pagination.count;
    }
}