import { Component, OnInit } from "@angular/core";
import { Location, NgIf } from "@angular/common";

// Models
import { AuthorizationResponse } from "../../extensions/authorization-response";

// Services
import { NavigationService } from "../../services/navigation.service";
import { AuthenticationService } from "../../services/authentication.service";
import { GameService } from "../../services/game.service";

// Components
import { AuthorizedHeaderComponent } from "../header/authorized-header.component";
import { UnauthorizedHeaderComponent } from "../header/unauthorized-header.component";
import { FooterComponent } from "../footer/footer.component";
import { response } from "express";
import { error } from "console";

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [AuthorizedHeaderComponent, UnauthorizedHeaderComponent, NgIf, FooterComponent],
    templateUrl: './predict-game.component.html',
    styleUrl: './predict-game.component.css'
})

export class PredictGameComponent implements OnInit {
    isVisibleHeader: boolean = false;
    isAuthorized: AuthorizationResponse = AuthorizationResponse.TokenNotFound;
    authorizationResponse = AuthorizationResponse;
    selectedFile: any;
    predictionResult: string = '';
    imgFile: File | null = null;

    constructor(private navigationService: NavigationService, private location: Location,
        private authenticationService: AuthenticationService, private gameService: GameService) {
    }

    ngOnInit(): void {
        this.navigationService.addToStack(this.location.path())
        this.isAuthorized = this.authenticationService.isAuthorized();
        this.isVisibleHeader = true;

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
    }

    onFileSelected(event: Event): void {
        const fileInput = event.target as HTMLInputElement;
        if (fileInput.files && fileInput.files.length > 0) {
            const file = fileInput.files[0];
            this.imgFile = file;
            const reader = new FileReader();
            reader.onload = () => {
                this.selectedFile = reader.result;
            };
            reader.readAsDataURL(file);
            this.predictionResult = '';
        }
    }

    predictGame() {
        if (this.imgFile) {
            this.gameService.predictGame(this.imgFile)
            .subscribe(response => {
                this.predictionResult = response.result;
            }, error => {
                console.error('Error:', error);
                this.predictionResult = "Помилка під час класифікації зображення.";
            });
        }
    }
}