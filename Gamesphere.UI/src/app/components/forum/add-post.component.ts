import { Component, OnInit, ViewChild, AfterViewInit } from "@angular/core";
import { Location, NgIf, NgFor, NgClass } from "@angular/common";
import { FormsModule, NgForm, ReactiveFormsModule } from "@angular/forms";
import { QuillEditorModule } from "../../quill-editor.module";
import { Router } from "@angular/router";

// Components
import { AuthorizedHeaderComponent } from "../header/authorized-header.component";
import { FooterComponent } from "../footer/footer.component";

// Models
import { UserAuthorizedModel } from "../../models/user-authorized.model";
import { AuthorizationResponse } from "../../extensions/authorization-response";
import { GameListModel } from "../../models/game-list.model";
import { AddPostModel } from "../../models/add-post.model";

// Services
import { AuthenticationService } from "../../services/authentication.service";
import { GameService } from "../../services/game.service";
import { ForumService } from "../../services/forum.service";

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [AuthorizedHeaderComponent, FooterComponent, NgIf, NgFor, FormsModule, NgClass, ReactiveFormsModule, QuillEditorModule],
    templateUrl: './add-post.component.html',
    styleUrls: ['./add-post.component.css']
})
export class AddPostComponent implements OnInit, AfterViewInit {
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
            clipboard: { // Модуль для роботи з буфером обміну
                matchVisual: false,
            },
            syntax: true, // Включає підсвітку синтаксису
            imageResize: true
        },
        theme: 'snow', // Встановлення теми
        placeholder: 'Compose an epic...',
    };

    ownUserData: UserAuthorizedModel;
    isAuthorized: AuthorizationResponse;
    authorizationResponse = AuthorizationResponse;
    isVisibleHeader: boolean;
    @ViewChild('myForm') myForm!: NgForm;

    gameText: string;
    games: GameListModel[];
    selectedGame: GameListModel | null;
    topicText: string;
    mainText: string;
    post: AddPostModel;
    earnLimitSymbols: boolean;
    resizedImageSrc: string | null = null;

    constructor(
        private authenticationService: AuthenticationService,
        private location: Location,
        private gameService: GameService,
        private forumService: ForumService,
        private router: Router
    ) {
        this.ownUserData = new UserAuthorizedModel('', '', '', '', '');
        this.isVisibleHeader = false;
        this.isAuthorized = AuthorizationResponse.TokenNotFound;
        this.games = [];
        this.selectedGame = null;
        this.gameText = '';
        this.topicText = '';
        this.mainText = '';
        this.post = new AddPostModel('', '', null, '');
        this.earnLimitSymbols = false;
    }

    ngOnInit(): void {
        this.isAuthorized = this.authenticationService.isAuthorized();

        if (this.isAuthorized === this.authorizationResponse.TokenExpired) {
            const refreshToken = confirm('Your session has finished. Do you want to reset your session?');

            if (refreshToken) {
                this.authenticationService.refreshToken();
                setTimeout(() => {
                    window.location.reload();
                }, 2000);
            } else {
                this.authenticationService.clearLocalStorage();
                alert('You have logged out from the account!');
                this.router.navigateByUrl('/forum');
            }
        }

        this.ownUserData = this.authenticationService.getAuthorizeHeaderData();

        if (this.isAuthorized === this.authorizationResponse.TokenNotFound) {
            this.router.navigateByUrl('/forum');
        }

        this.isVisibleHeader = true;
        this.post.userId = this.ownUserData.id;
    }

    ngAfterViewInit() {
        console.log(this.myForm); // Check if myForm is correctly initialized
    }

    getTop10GamesByName() {
        if (this.gameText !== '') {
            this.gameService.getTop10Games(this.gameText)
                .subscribe(response => this.games = response.slice(0, 5), error => { });
        } else {
            this.games = [];
        }
    }

    selectGame(game: GameListModel) {
        this.post.gameId = game.id;
        this.selectedGame = game;
        this.gameText = '';
        this.games = [];
    }

    cleanSelectedGame() {
        this.post.gameId = null;
        this.selectedGame = null;
    }

    onSubmit(myForm: NgForm) {
        console.log(myForm); // Check form state and validity
        if (myForm.valid && !this.earnLimitSymbols) {
            const accessToken = this.authenticationService.getAccessToken();
            console.log(accessToken);
            this.forumService.addPost(this.post, accessToken)
                .subscribe(response => {
                    if (response.isSuccess) {
                        alert(response.message);
                        this.toPrevPage();
                    } else {
                        alert(response.errors);
                    }
                });
        } else {
            console.log('Invalid form!', myForm.errors);
            alert('Invalid form!');
        }
    }

    toPrevPage() {
        this.location.back();
    }

    cleanForm() {
        this.cleanSelectedGame();
        this.post.subject = '';
        this.post.text = '';
        this.myForm.reset();
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
}