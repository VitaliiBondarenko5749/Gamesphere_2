import { Component, OnInit } from "@angular/core";
import { NgIf, Location, NgFor } from "@angular/common";
import { Router } from "@angular/router";

import { UnauthorizedHeaderComponent } from "../header/unauthorized-header.component";
import { AuthorizedHeaderComponent } from "../header/authorized-header.component";
import { UserAuthorizedModel } from "../../models/user-authorized.model";
import { AuthorizationResponse } from "../../extensions/authorization-response";
import { FooterComponent } from "../footer/footer.component";
import { NavigationService } from "../../services/navigation.service";
import { AuthenticationService } from "../../services/authentication.service";
import { GameService } from "../../services/game.service";
import { CategoryInfoModel } from "../../models/category-info.model";


@Component({
    selector: 'app-root',
    standalone: true,
    imports: [UnauthorizedHeaderComponent, AuthorizedHeaderComponent, NgIf, FooterComponent, NgFor],
    templateUrl: './categories.component.html',
    styleUrl: './categories.component.css'
})

export class CategoriesComponent implements OnInit {
    isAuthorized: AuthorizationResponse = AuthorizationResponse.TokenNotFound;
    authorizationResponse = AuthorizationResponse;
    ownUserData: UserAuthorizedModel;
    isVisibleHeader: boolean = false;
    categories: CategoryInfoModel[];


    constructor(private navigationService: NavigationService, private authenticationService: AuthenticationService,
        private location: Location, private router: Router, private gameService: GameService) {
        this.ownUserData = new UserAuthorizedModel('', '', '', '', '');
        this.categories = [];
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

        this.getCategories();
    }

    getCategories() {
        this.gameService.getCategories()
            .subscribe(response => {
                this.categories = response;
            }, error => { })
    }

    addCategory() {
        let accessToken = this.authenticationService.getAccessToken();

        if (accessToken !== '') {
            let categoryName = prompt('Enter a name: ') || '';

            if (categoryName !== '') {
                this.gameService.addCategory(categoryName, accessToken)
                    .subscribe(response => {
                        if (response.isSuccess) {
                            alert(`Category ${categoryName} has been added!`);
                            this.getCategories();
                        } else{
                            alert(response.errors);
                        }
                    }, error => { })

            } else {
                alert('Wrong name...');
            }
        }
    }

    deleteCategory(category: CategoryInfoModel) {
        let accessToken = this.authenticationService.getAccessToken();

        if (accessToken !== '') {
            let confirmDelete = confirm(`Are you sure you want to delete category "${category.name}"?`);

            if (confirmDelete) {
                this.gameService.deleteCategory(category.id, accessToken)
                .subscribe(response => {
                    if(response.isSuccess){
                        alert(`Category ${category.name} has been removed!`);
                        this.getCategories();
                    }
                }, error => {});
            }
        }
    }

    navigateTo(category: string){
        category = category.replace(' ', '_');
        this.router.navigateByUrl('categories/' + category);
    }
}