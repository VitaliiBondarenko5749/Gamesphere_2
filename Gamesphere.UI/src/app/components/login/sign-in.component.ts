import { Component, ViewChild, OnInit } from "@angular/core";
import { FooterComponent } from "../footer/footer.component";
import { UnauthorizedHeaderComponent } from "../header/unauthorized-header.component";
import { FormsModule, NgForm } from "@angular/forms";
import { NgClass, NgIf } from "@angular/common";
import { AuthenticationService } from "../../services/authentication.service";
import { LoginModel } from "../../models/login.model";
import { RouterLink, Router } from "@angular/router";
import { NavigationService } from "../../services/navigation.service";
import { BackendServerResponse } from "../../extensions/backend-server-response";
import { AuthorizationResponse } from "../../extensions/authorization-response";

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [FooterComponent, UnauthorizedHeaderComponent, FormsModule, NgClass, NgIf, RouterLink],
    templateUrl: './sign-in.component.html',
    styleUrl: './sign-in.component.css',
    providers: [AuthenticationService]
})

export class SignInComponent implements OnInit {
    loginModel: LoginModel = new LoginModel('', '');
    hiddenPassword: boolean;
    lastNavigationPath: string;
    private email: string = '';
    isAuthorized: AuthorizationResponse = AuthorizationResponse.TokenNotFound;

    ngOnInit(): void {
        this.isAuthorized = this.authenticationService.isAuthorized();

        if (this.isAuthorized === AuthorizationResponse.Authorized) {
            let logout = confirm('You have already logged in the platform. Exit?');

            if (logout) {
                this.authenticationService.clearLocalStorage();
                this.router.navigateByUrl('sign-in');
            } else {
                this.goBack();
            }
        }

    }

    @ViewChild('myForm') myForm!: NgForm

    constructor(private authenticationService: AuthenticationService, private navigationService: NavigationService,
        private router: Router) {
        this.hiddenPassword = true;
        this.lastNavigationPath = this.navigationService.getLastRoute();
    }

    goBack() {
        this.router.navigate([this.lastNavigationPath]);
    }

    showOrHidePassword() {
        this.hiddenPassword = !this.hiddenPassword;
    }

    login() {
        if (this.myForm.valid) {
            this.authenticationService.login(this.loginModel)
                .subscribe(
                    (response: BackendServerResponse) => {
                        if (response.isSuccess && response.accessToken && response.refreshToken) {
                            localStorage.setItem("accessToken", response.accessToken);
                            localStorage.setItem("refreshToken", response.refreshToken);
                            alert(response.message);
                            this.goBack();
                        } else {
                            alert(response.message);
                        }
                    },
                    (error) => {
                        console.error('Error during log in:', error);
                    });
        } else {
            alert('Form is not valid')
        }
    }

    openPrompt() {
        this.email = window.prompt('Enter your email to reset a password...') || '';

        if (this.email && this.testEmail(this.email)) {
            this.authenticationService.forgotPassword(this.email)
                .subscribe((response: BackendServerResponse) => {
                    alert(response.message);
                    this.goBack();
                }, (error) => {
                    console.log(error);
                });
        } else if (this.email && !this.testEmail(this.email)) {
            alert('Email is not valid!');
        }
    }

    private testEmail(value: string): boolean {
        const emailRegex = /^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$/;
        return emailRegex.test(value);
    }
}