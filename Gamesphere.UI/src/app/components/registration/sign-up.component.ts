import { Component, ViewChild, OnInit } from "@angular/core";
import { FormsModule, NgForm } from "@angular/forms";
import { FooterComponent } from "../footer/footer.component";
import { UnauthorizedHeaderComponent } from "../header/unauthorized-header.component";
import { NgIf, NgStyle, NgClass } from "@angular/common";
import { RegisterModel } from "../../models/register.model";
import { RouterOutlet, RouterLink, Router } from "@angular/router";
import { AuthenticationService } from "../../services/authentication.service";
import { BackendServerResponse } from "../../extensions/backend-server-response";
import {} from "@angular/common/http";
import { NavigationService } from "../../services/navigation.service";
import { AuthorizationResponse } from "../../extensions/authorization-response";

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [FormsModule, FooterComponent, UnauthorizedHeaderComponent, NgIf, NgStyle, NgClass,
        RouterLink, RouterOutlet],
    templateUrl: './sign-up.component.html',
    styleUrl: './sign-up.component.css',
    providers: [AuthenticationService]
})

export class SignUpComponent implements OnInit {
    selectedFile: any;
    hiddenPassword: boolean
    isUsernameAvailable: boolean
    isEmailAvailable: boolean
    lastNavigationPath: string
    user: RegisterModel = new RegisterModel(null, '', '', '');
    isAuthorized: AuthorizationResponse = AuthorizationResponse.TokenExpired;

    constructor(private authenticationService: AuthenticationService, private navigationService: NavigationService,
        private router: Router) {
        this.hiddenPassword = true;
        this.isUsernameAvailable = true;
        this.isEmailAvailable = true;
        this.lastNavigationPath = this.navigationService.getLastRoute();
    }

    ngOnInit(): void {
        this.isAuthorized = this.authenticationService.isAuthorized();

        if(this.isAuthorized === AuthorizationResponse.Authorized){
            let logout = confirm('You have already logged in the platform. Exit?');

            if (logout) {
                this.authenticationService.clearLocalStorage();
                this.router.navigateByUrl('sign-in');
            } else {
                this.goBack();
            }
        }
    }

    register() {
        if (this.myForm.valid) {
            this.authenticationService.registerUser(this.user)
                .subscribe((responce: BackendServerResponse) => {
                    if (responce.isSuccess) {
                        alert(responce.message);
                        this.goBack();
                    } else {
                        alert('Registration failed');
                    }
                },
                    (error) => {
                        console.error('Error checking username availability:', error);
                    });
        } else {
            alert("Form is not valid!");
        }
    }

    goBack() {
        this.router.navigate([this.lastNavigationPath]);
    }

    onFileSelected(event: Event): void {
        const fileInput = event.target as HTMLInputElement;
        if (fileInput.files && fileInput.files.length > 0) {
            const file = fileInput.files[0];
            this.user.icon = fileInput.files[0];
            const reader = new FileReader();
            reader.onload = () => {
                this.selectedFile = reader.result;
            };
            reader.readAsDataURL(file);
        }
    }

    @ViewChild('myForm') myForm!: NgForm;

    resetForm(): void {
        this.user = new RegisterModel(null, '', '', '');
        this.selectedFile = null;
        this.myForm.resetForm();
    }

    showOrHidePassword() {
        this.hiddenPassword = !this.hiddenPassword;
    }

    checkUsernameExistence() {
        this.isUsernameAvailable = true;
        if (this.myForm.controls['username'].valid) {
            this.authenticationService.checkUsernameExistence(this.user.username)
                .subscribe((response: BackendServerResponse) => {
                    console.log(response);
                    this.isUsernameAvailable = response.isSuccess
                },
                    (error) => {
                        console.error('Error checking username availability:', error);
                    });
        }
    }

    checkEmailExistence() {
        this.isEmailAvailable = true;
        if (this.myForm.controls['email'].valid) {
            this.authenticationService.checkEmailExistence(this.user.email)
                .subscribe((response: BackendServerResponse) => {
                    console.log(response);
                    this.isEmailAvailable = response.isSuccess;
                },
                    (error) => {
                        console.error('Error cheking email availability: ', error);
                    }
                );
        }
    }
}