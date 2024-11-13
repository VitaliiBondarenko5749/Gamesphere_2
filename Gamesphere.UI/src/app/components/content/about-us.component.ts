import { Component, OnInit } from "@angular/core";
import { FooterComponent } from "../footer/footer.component";
import { UnauthorizedHeaderComponent } from "../header/unauthorized-header.component";
import { NavigationService } from "../../services/navigation.service";
import { Location, NgIf } from "@angular/common";
import { AuthorizationResponse } from "../../extensions/authorization-response";
import { AuthenticationService } from "../../services/authentication.service";
import { AuthorizedHeaderComponent } from "../header/authorized-header.component";
import { Router } from "@angular/router";

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [UnauthorizedHeaderComponent, FooterComponent, NgIf, AuthorizedHeaderComponent],
    templateUrl: '/src/app/components/content/about-us.component.html',
    styleUrl: './about-us.component.css'
})

export class AboutUsComponent implements OnInit{
    isVisibleHeader: boolean = false;
    isAuthorized : AuthorizationResponse = AuthorizationResponse.TokenNotFound;
    authorizationResponse = AuthorizationResponse;

    constructor(private navigationService: NavigationService, private location: Location, 
        private authenticationService: AuthenticationService, private router: Router){
    }

    ngOnInit(): void {
        this.navigationService.addToStack(this.location.path());
        this.isAuthorized = this.authenticationService.isAuthorized(); 
        this.isVisibleHeader = true;

        if(this.isAuthorized === this.authorizationResponse.TokenExpired){
            let refreshToken = confirm('Your session has finished. Do you want to reset your session?');

            if(refreshToken){
                this.authenticationService.refreshToken();
                
                setTimeout(() =>{
                    window.location.reload();
                }, 2000);

            } else {
                this.authenticationService.clearLocalStorage();
                alert('You have logged out from the account!');
            }
        }
    }

    navigateTo(link: string) {
        this.router.navigateByUrl(link);
    }
}