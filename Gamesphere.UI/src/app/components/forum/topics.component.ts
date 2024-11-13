import { Component, OnInit } from "@angular/core";
import { NgIf, Location, NgStyle, NgFor } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { Pagination } from "../../extensions/pagination";
import { NgxPaginationModule } from "ngx-pagination";
import { Router } from "@angular/router";

// Components
import { AuthorizedHeaderComponent } from "../header/authorized-header.component";
import { UnauthorizedHeaderComponent } from "../header/unauthorized-header.component";
import { FooterComponent } from "../footer/footer.component";

// Models
import { UserAuthorizedModel } from "../../models/user-authorized.model";
import { GameListModel } from "../../models/game-list.model";
import { TopicSorter } from "../../extensions/topic-sorter.enum";
import { TopicViewer } from "../../extensions/topic-viewer.enum";
import { ShortPostInfoModel } from "../../models/short-post-info.model";

// Services
import { AuthenticationService } from "../../services/authentication.service";
import { NavigationService } from "../../services/navigation.service";
import { AuthorizationResponse } from "../../extensions/authorization-response";
import { GameService } from "../../services/game.service";
import { ForumService } from "../../services/forum.service";
import { FileService } from "../../services/file.service";


@Component({
    selector: 'app-root',
    standalone: true,
    imports: [AuthorizedHeaderComponent, UnauthorizedHeaderComponent, FooterComponent, NgIf, FormsModule, NgFor, NgStyle,
        NgxPaginationModule],
    templateUrl: './topics.component.html',
    styleUrl: './topics.component.css'
})

export class TopicsComponent implements OnInit {
    ownUserData: UserAuthorizedModel;
    isAuthorized: AuthorizationResponse;
    authorizationResponse = AuthorizationResponse;
    isVisibleHeader: boolean;

    topicText: string;
    gameText: string;
    games: GameListModel[];
    chosenGames: GameListModel[];

    topicSorter = TopicSorter;
    selectedTopicSort: TopicSorter = TopicSorter.DateByDescending;
    topicViewer = TopicViewer;
    selectedTopicView = TopicViewer.All;
    chosenSpecisalTopics = false;

    pagination: Pagination;
    posts: ShortPostInfoModel[];

    constructor(private authenticationService: AuthenticationService, private navigationService: NavigationService,
        private location: Location, private gameService: GameService, private forumService: ForumService,
        private fileService: FileService, private router: Router) {
        this.ownUserData = new UserAuthorizedModel('', '', '', '', '');
        this.isVisibleHeader = false;
        this.isAuthorized = AuthorizationResponse.TokenNotFound;
        this.topicText = '';
        this.gameText = '';
        this.games = [];
        this.chosenGames = [];
        this.pagination = new Pagination(1, 0, 10);
        this.posts = [];
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

        this.getTopics(this.selectedTopicView)
    }

    getTop10GamesByName() {
        if (this.gameText !== '') {
            this.gameService.getTop10Games(this.gameText)
                .subscribe(response => this.games = response.slice(0, 5), error => { })
        } else {
            this.games = [];
        }
    }

    existsInGameStack(game: GameListModel): boolean {
        return this.chosenGames.some(g => g.name === game.name);
    }

    onPageChange(event: number) {
        this.pagination.page = event;
        this.getTopics(this.selectedTopicView);
    }

    getTopics(topicView: TopicViewer) {
        this.chosenSpecisalTopics = (topicView === TopicViewer.All) ? false : true;
        this.selectedTopicView = topicView;
        const gameIds = this.chosenGames.map(g => g.id);
        const userId = (this.isAuthorized === AuthorizationResponse.Authorized) ? this.ownUserData.id : null;

        this.forumService.getTopics(this.pagination, this.selectedTopicView, this.selectedTopicSort, this.topicText, gameIds, this.ownUserData.id)
            .subscribe(response => {
                this.posts = response.items as ShortPostInfoModel[];
                this.pagination.count = response.count;

                this.posts.forEach(post => {
                    this.fileService.downloadFileFromDropbox(post.userInfo.iconDirectory)
                    .subscribe(
                        (link: string) => {
                            post.userInfo.iconDirectory = link;
                        });
                });
            }, error => { });
    }

    navigateTo(link: string) {
        this.router.navigateByUrl(link);
    }
}