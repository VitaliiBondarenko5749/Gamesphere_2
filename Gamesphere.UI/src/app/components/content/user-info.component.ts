import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { Location, NgIf, NgStyle } from "@angular/common";
import { Subscription } from "rxjs";
import { AuthorizationResponse } from "../../extensions/authorization-response";
import { AuthenticationService } from "../../services/authentication.service";
import { NavigationService } from "../../services/navigation.service";
import { UserAuthorizedModel } from "../../models/user-authorized.model";
import { AuthorizedHeaderComponent } from "../header/authorized-header.component";
import { FooterComponent } from "../footer/footer.component";
import { FileService } from "../../services/file.service";
import { response } from "express";
import { BackendServerResponse } from "../../extensions/backend-server-response";
import { error } from "console";

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [AuthorizedHeaderComponent, FooterComponent, NgIf, NgStyle],
    templateUrl: './user-info.component.html',
    styleUrl: './user-info.component.css'
})

export class UserInfoComponent implements OnInit {
    isAuthorized: AuthorizationResponse = AuthorizationResponse.TokenNotFound;
    authorizationResponse = AuthorizationResponse;
    id: string | undefined;
    private subscription: Subscription;
    userData: UserAuthorizedModel;
    imageUrl: string | undefined;
    isUsernameAvailable: boolean;

    constructor(private authenticationService: AuthenticationService, private navigationService: NavigationService,
        private router: Router, private location: Location, private activateRoute: ActivatedRoute,
        private fileService: FileService) {
        this.subscription = activateRoute.params.subscribe(params => this.id = params["id"]);
        this.userData = new UserAuthorizedModel('', '', '', '', '');
        this.isUsernameAvailable = true;
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

        this.userData = this.authenticationService.getAuthorizeHeaderData();

        if (this.userData.id !== this.id && this.isAuthorized === this.authorizationResponse.Authorized) {
            this.router.navigateByUrl('/error');
        }

        if (this.userData.imageDirectory) {
            this.fileService.downloadFileFromDropbox(this.userData.imageDirectory)
                .subscribe(
                    (link: string) => {
                        this.imageUrl = link;
                    });
        }

        if (this.isAuthorized === this.authorizationResponse.TokenNotFound) {
            this.router.navigateByUrl(this.navigationService.getLastRoute());
        }
    }

    goBack() {
        this.location.back();
    }

    onFileSelected(event: Event): void {
        const fileInput = event.target as HTMLInputElement;
        if (fileInput.files && fileInput.files.length > 0) {
            const file = fileInput.files[0];

            if (this.id && file) {
                this.authenticationService.changeAvatar(this.id, file)
                    .subscribe(
                        (response: BackendServerResponse) => {
                            if (response.isSuccess === true) {
                                this.authenticationService.refreshToken();
                                setTimeout(() => {
                                    window.location.reload();
                                }, 2000);
                            }
                        },
                        (error) => {
                            console.log(error);
                        }
                    );
            }
        }
    }

    changeUsername() {
        this.isUsernameAvailable = true;
        let username = prompt('Enter a new username...');

        if (username) {
            this.authenticationService.checkUsernameExistence(username)
                .subscribe(
                    (response: BackendServerResponse) => {
                        this.isUsernameAvailable = response.isSuccess;
                    }, (error) => {
                        console.error('Error checking username availability:', error);
                    }
                );

            if (this.isUsernameAvailable === true && this.id) {
                this.authenticationService.changeUsername(this.id, username)
                    .subscribe((response: BackendServerResponse) => {
                        if (response.isSuccess === true) {
                            alert('Username has been changed successfully!');
                            this.authenticationService.refreshToken();

                            setTimeout(() => {
                                window.location.reload();
                            }, 2000);
                        } else {
                            alert(response.message);
                        }
                    }, (error) => {
                        console.log(error);
                    });
            } else {
                alert('Username is already taken!');
            }
        }
    }

    changeEmail() {
        let email = prompt('Enter a new email...')

        if (email && this.testEmail(email) && this.id) {
            this.authenticationService.generateChangeEmailToken(this.id, email)
            .subscribe((response: BackendServerResponse) => {
                alert(response.message);
            }, (error) =>{
                console.log(error);
            })
        } else {
            alert('Email is not valid!');
        }
    }

    private testEmail(value: string): boolean {
        const emailRegex = /^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$/;
        return emailRegex.test(value);
    }

    redirectToManageUsersPage(){
        this.router.navigateByUrl('/manage-users')
    }

    logout() {
        let logout = confirm('Are you sure you want to log out from your account?');

        if (logout) {
            this.authenticationService.clearLocalStorage();
            this.router.navigateByUrl(this.navigationService.getLastRoute());
        }
    }
}