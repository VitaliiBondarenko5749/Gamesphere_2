// Angular
import { Component, OnInit } from "@angular/core";
import { NgIf, NgFor, NgStyle } from "@angular/common";
import { Router } from "@angular/router";
import { FormsModule } from "@angular/forms";

// Another components
import { AuthorizedHeaderComponent } from "../../header/authorized-header.component";
import { FooterComponent } from "../../footer/footer.component";
import { AddGameComponent } from "./add-game.component";

// Extensions
import { AuthorizationResponse } from "../../../extensions/authorization-response";

// Models
import { UserAuthorizedModel } from "../../../models/user-authorized.model";
import { CategoryInfoModel } from "../../../models/category-info.model";
import { DeveloperInfoModel } from "../../../models/developer-info.model";
import { LanguageInfoModel } from "../../../models/language-info.model";
import { PlatformInfoModel } from "../../../models/platform-info.model";
import { PublisherInfoModel } from "../../../models/publisher-info.model";

// Services
import { NavigationService } from "../../../services/navigation.service";
import { AuthenticationService } from "../../../services/authentication.service";
import { GameService } from "../../../services/game.service";

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [AuthorizedHeaderComponent, FooterComponent, NgIf, FormsModule, NgFor, NgStyle, AddGameComponent],
    templateUrl: './edit-game-data.component.html',
    styleUrl: './edit-game-data.component.css'
})

export class EditGameDataComponent implements OnInit {
    isAuthorized: AuthorizationResponse = AuthorizationResponse.TokenNotFound;
    authorizationResponse = AuthorizationResponse;
    ownUserData: UserAuthorizedModel;
    searchText: string;
    top10Categories: CategoryInfoModel[];
    top10Developers: DeveloperInfoModel[];
    top10Languages: LanguageInfoModel[];
    top10Platforms: PlatformInfoModel[];
    top10Publishers: PublisherInfoModel[];

    constructor(private navigationService: NavigationService, private router: Router,
        private authenticationService: AuthenticationService, private gameService: GameService) {
        this.ownUserData = new UserAuthorizedModel('', '', '', '', '');
        this.searchText = '';
        this.top10Categories = [];
        this.top10Developers = [];
        this.top10Languages = [];
        this.top10Platforms = [];
        this.top10Publishers = [];
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
                this.router.navigateByUrl(this.navigationService.getLastRoute());
            }
        }

        this.ownUserData = this.authenticationService.getAuthorizeHeaderData();

        if (this.ownUserData.role === 'User' || this.isAuthorized === this.authorizationResponse.TokenNotFound) {
            this.router.navigateByUrl(this.navigationService.getLastRoute());
        }
    }

    onChangeOption(event: Event): void {
        const forms = document.querySelectorAll('form');

        forms.forEach((form: any) => {
            form.style.display = 'none';
        });

        const selectElement = event.target as HTMLSelectElement;

        if (selectElement instanceof HTMLSelectElement) {
            const formId = selectElement.value + 'Form';
            const selectedForm = document.getElementById(formId);

            if (selectedForm) {
                selectedForm.style.display = 'flex';
            } else {
                console.error(`Form with id '${formId}' not found.`);
            }
        }

        this.searchText = '';
        this.top10Categories = [];
        this.top10Developers = [];
        this.top10Languages = [];
        this.top10Platforms = [];
        this.top10Publishers =[];
    }

    resetForm() {
        this.searchText = '';
        this.top10Categories = [];
        this.top10Developers = [];
        this.top10Languages = [];
        this.top10Platforms = [];
        this.top10Publishers = [];
    }

    // Categories

    getTop10Categories() {
        if (this.searchText !== '' && this.searchText.length > 0) {
            this.gameService.getTop10Categories(this.searchText)
                .subscribe(response => {
                    this.top10Categories = response;
                }, error => { });
        }
    }

    deleteCategory(category: CategoryInfoModel) {
        let accessToken = this.authenticationService.getAccessToken();

        if (accessToken !== '') {
            let confirmDelete = confirm(`Are you sure you want to delete category "${category.name}"?`);

            if (confirmDelete) {
                this.gameService.deleteCategory(category.id, accessToken)
                    .subscribe(response => {
                        if (response.isSuccess) {
                            alert(`Category ${category.name} has been removed!`);
                            this.getTop10Categories();
                        }
                    }, error => { });
            }
        }
    }

    addCategory() {
        let accessToken = this.authenticationService.getAccessToken();

        if (accessToken !== '') {
            this.gameService.addCategory(this.searchText, accessToken)
                .subscribe(response => {
                    if (response.isSuccess) {
                        alert(`Category ${this.searchText} has been added!`);
                        this.searchText = '';
                    } else {
                        alert(response.errors);
                    }
                }, error => { })
        }
    }

    existsInCategories(): boolean {
        return this.top10Categories.some(c => c.name.toLowerCase() === this.searchText.toLocaleLowerCase())
    }

    // DEVELOPERS

    getTop10Developers() {
        if (this.searchText !== '' && this.searchText.length > 0) {
            this.gameService.getTop10Developers(this.searchText)
                .subscribe(response => {
                    this.top10Developers = response;
                }, error => { });
        }
    }

    deleteDeveloper(developer: DeveloperInfoModel) {
        let accessToken = this.authenticationService.getAccessToken();

        if (accessToken !== '') {
            let confirmDelete = confirm(`Are you sure you want to delete developer "${developer.name}"?`);

            if (confirmDelete) {
                this.gameService.deleteDeveloper(developer.id, accessToken)
                    .subscribe(response => {
                        if (response.isSuccess) {
                            alert(`Developer ${developer.name} has been removed!`);
                            this.getTop10Developers();
                        }
                    }, error => { });
            }
        }
    }

    addDeveloper() {
        let accessToken = this.authenticationService.getAccessToken();

        if (accessToken !== '') {
            this.gameService.addDeveloper(this.searchText, accessToken)
                .subscribe(response => {
                    if (response.isSuccess) {
                        alert(`Developer ${this.searchText} has been added!`);
                        this.searchText = '';
                    } else {
                        alert(response.errors);
                    }
                }, error => { })
        }
    }

    existsInDevelopers(): boolean {
        return this.top10Developers.some(d => d.name.toLowerCase() === this.searchText.toLocaleLowerCase())
    }

    // Languages

    getTop10Languages() {
        if (this.searchText !== '' && this.searchText.length > 0) {
            this.gameService.getTop10Languages(this.searchText)
                .subscribe(response => {
                    this.top10Languages = response;
                }, error => { });
        }
    }

    deleteLanguage(language: LanguageInfoModel) {
        let accessToken = this.authenticationService.getAccessToken();

        if (accessToken !== '') {
            let confirmDelete = confirm(`Are you sure you want to delete language "${language.name}"?`);

            if (confirmDelete) {
                this.gameService.deleteLanguage(language.id, accessToken)
                    .subscribe(response => {
                        if (response.isSuccess) {
                            alert(`Language ${language.name} has been removed!`);
                            this.getTop10Languages();
                        }
                    }, error => { });
            }
        }
    }

    addLanguage() {
        let accessToken = this.authenticationService.getAccessToken();

        if (accessToken !== '') {
            this.gameService.addLanguage(this.searchText, accessToken)
                .subscribe(response => {
                    if (response.isSuccess) {
                        alert(`Language ${this.searchText} has been added!`);
                        this.searchText = '';
                    } else {
                        alert(response.errors);
                    }
                }, error => { })
        }
    }

    existsInLanguages(): boolean {
        return this.top10Languages.some(l => l.name.toLowerCase() === this.searchText.toLocaleLowerCase())
    }

    // PLATFORMS

    getTop10Platforms() {
        if (this.searchText !== '' && this.searchText.length > 0) {
            this.gameService.getTop10Platforms(this.searchText)
                .subscribe(response => {
                    this.top10Platforms = response;
                }, error => { });
        }
    }

    deletePlatform(platform: PlatformInfoModel) {
        let accessToken = this.authenticationService.getAccessToken();

        if (accessToken !== '') {
            let confirmDelete = confirm(`Are you sure you want to delete platform "${platform.name}"?`);

            if (confirmDelete) {
                this.gameService.deletePlatform(platform.id, accessToken)
                    .subscribe(response => {
                        if (response.isSuccess) {
                            alert(`Platform ${platform.name} has been removed!`);
                            this.getTop10Platforms();
                        }
                    }, error => { });
            }
        }
    }

    addPlatform() {
        let accessToken = this.authenticationService.getAccessToken();

        if (accessToken !== '') {
            this.gameService.addPlatform(this.searchText, accessToken)
                .subscribe(response => {
                    if (response.isSuccess) {
                        alert(`Platform ${this.searchText} has been added!`);
                        this.searchText = '';
                    } else {
                        alert(response.errors);
                    }
                }, error => { })
        }
    }

    existsInPlatforms(): boolean {
        return this.top10Platforms.some(p => p.name.toLowerCase() === this.searchText.toLocaleLowerCase())
    }

    // PUBLISHERS

    getTop10Publishers() {
        if (this.searchText !== '' && this.searchText.length > 0) {
            this.gameService.getTop10Publishers(this.searchText)
                .subscribe(response => {
                    this.top10Publishers = response;
                }, error => { });
        }
    }

    deletePublisher(publisher: PublisherInfoModel) {
        let accessToken = this.authenticationService.getAccessToken();

        if (accessToken !== '') {
            let confirmDelete = confirm(`Are you sure you want to delete publisher "${publisher.name}"?`);

            if (confirmDelete) {
                this.gameService.deletePublisher(publisher.id, accessToken)
                    .subscribe(response => {
                        if (response.isSuccess) {
                            alert(`Publisher ${publisher.name} has been removed!`);
                            this.getTop10Publishers();
                        }
                    }, error => { });
            }
        }
    }

    addPublisher() {
        let accessToken = this.authenticationService.getAccessToken();

        if (accessToken !== '') {
            this.gameService.addPublisher(this.searchText, accessToken)
                .subscribe(response => {
                    if (response.isSuccess) {
                        alert(`Publisher ${this.searchText} has been added!`);
                        this.searchText = '';
                    } else {
                        alert(response.errors);
                    }
                }, error => { })
        }
    }

    existsInPublishers(): boolean {
        return this.top10Publishers.some(p => p.name.toLowerCase() === this.searchText.toLocaleLowerCase())
    }
}