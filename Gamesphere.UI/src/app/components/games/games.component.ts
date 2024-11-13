// Angular
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Location, NgIf, NgFor } from "@angular/common";
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

// Models
import { UserAuthorizedModel } from "../../models/user-authorized.model";
import { ShortGameModel } from "../../models/short-game.model";

// Extensions
import { AuthorizationResponse } from "../../extensions/authorization-response";
import { GameSorter } from "../../extensions/game-sorter.enum";
import { Pagination } from "../../extensions/pagination";

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [AuthorizedHeaderComponent, UnauthorizedHeaderComponent, FooterComponent, NgIf, FormsModule,
        NgxPaginationModule, NgFor],
    templateUrl: './games.component.html',
    styleUrl: './games.component.css'
})

export class GamesComponent implements OnInit {
    category: string = '';
    ownUserData: UserAuthorizedModel;
    isAuthorized: AuthorizationResponse;
    authorizationResponse = AuthorizationResponse;
    isVisibleHeader: boolean;
    searchText: string = '';
    selectedSort: GameSorter = GameSorter.DateByDescending;
    sorter = GameSorter;
    games: ShortGameModel[];
    pagination: Pagination;
    recommendGames: ShortGameModel[];

    constructor(private authenticationService: AuthenticationService, private gameService: GameService,
        private navigationService: NavigationService, private activateRoute: ActivatedRoute,
        private location: Location, private router: Router, private fileService: FileService) {
        this.ownUserData = new UserAuthorizedModel('', '', '', '', '');
        this.isVisibleHeader = false;
        this.isAuthorized = AuthorizationResponse.TokenNotFound;
        this.games = [];
        this.pagination = new Pagination(1, 0, 18);
        this.recommendGames = [];
    }

    ngOnInit(): void {
        this.recommendGames = [];
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
        this.activateRoute.params.subscribe(params => this.category = params["category"]);
        this.category = this.category.replace('_', ' ');

        this.getGames();
    }

    getGames() {
        if (this.category === 'All games') {
            this.gameService.getAllGames(this.selectedSort, this.searchText || '', this.pagination)
                .subscribe(response => {
                    this.games = response.items as ShortGameModel[];
                    this.pagination.count = response.count;

                    this.games.forEach(game => {
                        this.fileService.downloadFileFromDropbox('/' + game.mainImageDirectory)
                            .subscribe(
                                (link: string) => {
                                    game.mainImageDirectory = link;
                                });
                    });
                }, error => { });
        } else if (this.category === 'Favorite games') {
            let accessToken = this.authenticationService.getAccessToken();

            if(accessToken){
                this.gameService.getFavoriteGames(accessToken, this.selectedSort, this.searchText || '', this.ownUserData.id, 
                this.pagination)
                .subscribe(response => {
                    this.games = response.items as ShortGameModel[];
                    this.pagination.count = response.count;

                    this.games.forEach(game => {
                        this.fileService.downloadFileFromDropbox('/' + game.mainImageDirectory)
                            .subscribe(
                                (link: string) => {
                                    game.mainImageDirectory = link;
                                });
                    });
                }, error => {});

                this.gameService.getRecGames(accessToken, this.ownUserData.id)
                .subscribe(response => {
                    this.recommendGames = response;

                    this.recommendGames.forEach(game =>{
                        this.fileService.downloadFileFromDropbox('/' + game.mainImageDirectory)
                            .subscribe(
                                (link: string) => {
                                    game.mainImageDirectory = link;
                                });
                    });
                }, error => {});
            }

        } else {
            this.gameService.getGamesByCategory(this.selectedSort, this.searchText || '', this.category, this.pagination)
                .subscribe(response => {
                    this.games = response.items as ShortGameModel[];
                    this.pagination.count = response.count;

                    this.games.forEach(game => {
                        this.fileService.downloadFileFromDropbox('/' + game.mainImageDirectory)
                            .subscribe(
                                (link: string) => {
                                    game.mainImageDirectory = link;
                                });
                    });
                }, error => { });
        }
    }

    getGamesWithPagination() {
        this.pagination.page = 1;
        this.getGames();
    }

    navigateTo(link: string) {
        this.router.navigateByUrl(link);
    }

    onPageChange(event: number) {
        this.pagination.page = event;
        this.getGames();
    }
}