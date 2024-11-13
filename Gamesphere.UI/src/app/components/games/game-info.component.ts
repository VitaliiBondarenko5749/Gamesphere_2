// Angular
import { Component, OnInit, Sanitizer } from "@angular/core";
import { NgIf, NgFor, Location } from "@angular/common";
import { ActivatedRoute, Router } from "@angular/router";
import { DomSanitizer } from "@angular/platform-browser";
import { forkJoin, tap } from "rxjs";
import { FormsModule } from "@angular/forms";
import { NgxPaginationModule } from "ngx-pagination";

// Components
import { AuthorizedHeaderComponent } from "../header/authorized-header.component";
import { UnauthorizedHeaderComponent } from "../header/unauthorized-header.component";
import { FooterComponent } from "../footer/footer.component";

// Services
import { NavigationService } from "../../services/navigation.service";
import { AuthenticationService } from "../../services/authentication.service";
import { GameService } from "../../services/game.service";
import { FileService } from "../../services/file.service";
import { EmailService } from "../../services/email.service";

// Models
import { UserAuthorizedModel } from "../../models/user-authorized.model";
import { GameInfoModel } from "../../models/game-info.model";
import { CommentInfoModel } from "../../models/comment-info.model";
import { SendEmailModel } from "../../models/send-email.model";

// Extensions
import { AuthorizationResponse } from "../../extensions/authorization-response";
import { Pagination } from "../../extensions/pagination";
import { error } from "node:console";

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [AuthorizedHeaderComponent, UnauthorizedHeaderComponent, FooterComponent, NgIf, NgFor, FormsModule,
        NgxPaginationModule],
    templateUrl: './game-info.component.html',
    styleUrl: './game-info.component.css'
})

export class GameInfoComponent implements OnInit {
    ownUserData: UserAuthorizedModel;
    isAuthorized: AuthorizationResponse;
    authorizationResponse = AuthorizationResponse;
    isVisibleHeader: boolean;
    private gameId: string;
    game: GameInfoModel;
    imageChanged: boolean = false;
    isGameInFavoriteList: boolean = false;
    commentText: string = '';
    pagination: Pagination;
    comments: CommentInfoModel[];
    category: string;

    constructor(private authenticationService: AuthenticationService, private gameService: GameService,
        private navigationService: NavigationService, private activateRoute: ActivatedRoute,
        private location: Location, private fileService: FileService, private sanitizer: DomSanitizer,
        private emailService: EmailService, private router: Router) {
        this.ownUserData = new UserAuthorizedModel('', '', '', '', '');
        this.isVisibleHeader = false;
        this.isAuthorized = AuthorizationResponse.TokenNotFound;
        this.gameId = '';
        this.game = new GameInfoModel('', '', '', '', '', 0, '', [], [], [], [], [], []);
        this.pagination = new Pagination(1, 0, 10);
        this.comments = [];
        this.category = '';
    }

    ngOnInit(): void {
        this.navigationService.addToStack(this.location.path());
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
        this.activateRoute.params.subscribe(params => this.gameId = params["gameId"]);
        this.activateRoute.params.subscribe(params => this.category = params["category"]);

        this.gameService.getGameById(this.gameId)
            .subscribe(response => {
                this.game = response;

                this.downloadImages();
                console.log(this.game);
            }, error => { })

        setTimeout(() => {
            this.downloadIconImage();
        }, 2000);

        if (this.isAuthorized === this.authorizationResponse.Authorized) {
            setTimeout(() => {
                this.checkGameInFavoriteList();
            }, 1200);
        }

        this.getComments();
    }

    getComments() {
        this.gameService.getComments(this.gameId, this.pagination)
            .subscribe(response => {
                this.comments = response.items as CommentInfoModel[];
                this.pagination.count = response.count;

                this.comments.forEach(comment => {
                    this.fileService.downloadFileFromDropbox(comment.iconDirectory)
                        .subscribe(
                            (link: string) => {
                                comment.iconDirectory = link;
                            });
                });

                if (this.isAuthorized === this.authorizationResponse.Authorized) {
                    this.comments.forEach(comment => {
                        this.gameService.checkLikeExistence(comment.id, this.ownUserData.id)
                            .subscribe(response => {
                                comment.isLiked = response.isSuccess;
                            }, error => { });

                    });
                }

                this.comments.forEach(comment => {
                    this.gameService.getCountOfLikes(comment.id)
                        .subscribe(response => {
                            comment.countOfLikes = response;
                        }, error => { });
                });
            }, error => { })
    }

    doLikeOperation(comment: CommentInfoModel) {
        let accessToken = this.authenticationService.getAccessToken();

        if (accessToken) {
            this.gameService.doLikeOperation(accessToken, comment.id, this.ownUserData.id)
                .subscribe(response => {
                    if (response.isSuccess) {
                        let index = this.comments.indexOf(comment);

                        this.gameService.checkLikeExistence(comment.id, this.ownUserData.id)
                            .subscribe(response => {
                                comment.isLiked = response.isSuccess;

                                this.gameService.getCountOfLikes(comment.id)
                                    .subscribe(response => {
                                        comment.countOfLikes = response;
                                        this.comments[index] = comment;
                                    }, error => { });

                            }, error => { });
                    }
                }, error => { });
        }
    }

    deleteComment(comment: CommentInfoModel) {
        let accessToken = this.authenticationService.getAccessToken();

        if (accessToken) {

            if (this.ownUserData.username !== comment.userName) {
                let reasonOfReport = prompt('Describe a reason of deleting comment:');

                if (reasonOfReport) {
                    let emailMessage = `<p>${this.ownUserData.role} ${this.ownUserData.username} removed your comment.</p><br>`;
                    emailMessage += `<p>Reason: <i>${reasonOfReport}</i>.</p><br>`;
                    emailMessage += `<p>Comment content: <i>${comment.content}</i>.</p>`;

                    let emailModel = new SendEmailModel(comment.email, 'Your comment was deleted', emailMessage, true);

                    this.emailService.sendEmail(emailModel)
                        .subscribe();
                } else {
                    return;
                }
            }

            this.gameService.deleteComment(accessToken, comment.id)
                .subscribe(response => {
                    this.getComments();
                }, error => { });
        }
    }

    onPageChange(event: number) {
        this.pagination.page = event;
        this.getComments();
    }

    addComment(content: string) {
        let accessToken = this.authenticationService.getAccessToken();

        if (accessToken && this.commentText !== '') {
            this.gameService.addComment(accessToken, this.ownUserData.id, this.gameId, content)
                .subscribe(response => {
                    if (response.isSuccess) {
                        alert('Comment has benn added!');
                        this.getComments();
                    }
                }, error => { });
        } else {
            alert('Something went wrong!');
        }

        this.commentText = '';
    }

    private downloadIconImage() {
        this.fileService.downloadFileFromDropbox('/' + this.game.mainImageDirectory)
            .subscribe(
                (link: string) => {
                    this.game.mainImageDirectory = link;
                });
    }

    private downloadImages(): void {
        const imageLinks: string[] = [];
        const imageObservables = this.game.images.map((image, index) =>
            this.fileService.downloadFileFromDropbox('/' + image)
                .pipe(tap(link => imageLinks[index] = link))
        );

        forkJoin(imageObservables).subscribe({
            next: () => {
                this.game.images = imageLinks;
            },
            error: err => {
                console.error('Error downloading images:', err);
            }
        });
    }

    getYoutubeFrameLink(videoId: string) {
        let link = `https://www.youtube.com/embed/${videoId}`;
        return this.sanitizer.bypassSecurityTrustResourceUrl(link);
    }

    private checkGameInFavoriteList() {
        this.gameService.checkGameInFavoriteList(this.game.id, this.ownUserData.id)
            .subscribe(response => {
                this.isGameInFavoriteList = response.isSuccess;
            }, error => { })
    }

    addGameInFavoriteList() {
        let accessToken = this.authenticationService.getAccessToken();

        if (accessToken) {
            this.gameService.addGameToFavoriteList(accessToken, this.game.id, this.ownUserData.id)
                .subscribe(response => {
                    if (response.isSuccess) {
                        setTimeout(() => {
                            this.checkGameInFavoriteList();
                        }, 1000)
                    }
                }, error => { });
        }
    }

    removeGameFromFavoriteList() {
        let accessToken = this.authenticationService.getAccessToken();

        if (accessToken) {
            this.gameService.removeGameFromFavoriteList(accessToken, this.game.id, this.ownUserData.id)
                .subscribe(response => {
                    if (response.isSuccess) {
                        setTimeout(() => {
                            this.checkGameInFavoriteList();
                        }, 1000)
                    }
                }, error => { });
        }
    }

    deleteGame() {
        let accessToken = this.authenticationService.getAccessToken();

        if (accessToken) {
            this.gameService.deleteGame(accessToken, this.game.id)
                .subscribe(response => {
                    if (response.isSuccess) {
                        alert(`Game ${this.game.name} has been removed!`);
                        this.router.navigateByUrl('/categories/' + this.category);
                    }
                }, error => { });
        }
    }

    banUser(comment: CommentInfoModel) {
        let confirmBan = confirm(`Are you sure you want to ban user ${comment.userName}?`);

        if (confirmBan) {
            let bannedBy = `${this.ownUserData.username}-${this.ownUserData.role}`;
            let description = prompt('Write a reason of the ban') || '';

            this.authenticationService.banUser(comment.userId, description, bannedBy)
                .subscribe(response => {
                    if (response.isSuccess) {
                        alert(`User ${comment.userName} has been banned`);
                        this.getComments();
                    }
                }, error => { });
        }
    }

    reportUser(comment: CommentInfoModel) {
        let confirmReport = confirm(`Are you sure you want to report ${comment.userName}?`);

        if (confirmReport) {
            let reasonOfReport = prompt('Enter a message. Describe a reason of the report:');

            if (reasonOfReport !== null && reasonOfReport.length > 0) {
                let emailMessage = `<p>${this.ownUserData.role} ${this.ownUserData.username} sent report to user ${comment.userName}-${comment.role}.</p><br>`;
                emailMessage += `<p>Reason of the report: <i>${reasonOfReport}</i>.</p><br>`;
                emailMessage += `<p>Comment content: <i>${comment.content}</i></p>`;

                let emailModel = new SendEmailModel('gamesphere7438@gmail.com', 'Report', emailMessage, true);

                this.emailService.sendEmail(emailModel)
                    .subscribe();

                alert('Your report has been sent!');
            } else {
                alert('You can not send a report without reason!');
            }
        }
    }
}