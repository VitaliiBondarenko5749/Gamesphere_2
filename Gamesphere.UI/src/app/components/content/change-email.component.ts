import { Component, OnInit } from "@angular/core";
import { ChangeEmailModel } from "../../models/change-email.model";
import { ActivatedRoute, Route, Router } from "@angular/router";
import { NgIf } from "@angular/common";
import { NavigationService } from "../../services/navigation.service";
import { AuthenticationService } from "../../services/authentication.service";
import { BackendServerResponse } from "../../extensions/backend-server-response";

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [NgIf],
    templateUrl: './change-email.component.html',
    styleUrl: './change-email.component.css'
})

export class ChangeEmailComponent implements OnInit {
    model: ChangeEmailModel;
    isEmailChanged: boolean;

    constructor(private route: ActivatedRoute, private navigationService: NavigationService, private router: Router,
        private authenticationService: AuthenticationService) {
        this.model = new ChangeEmailModel('', '', '');
        this.isEmailChanged = false;
    }

    ngOnInit(): void {
        this.route.queryParams.subscribe(params => {
            this.model.userId = params['userId'];
            this.model.email = params['email'];
            this.model.code = params['code'];
        });

        this.changeEmail();
    }

    changeEmail() {
        if (this.model.userId && this.model.email && this.model.code) {
            this.authenticationService.changeEmail(this.model)
                .subscribe((response: BackendServerResponse) => {
                    if (response.isSuccess) {
                        this.isEmailChanged = true;
                    } else {
                        this.router.navigateByUrl('/error');
                    }
                })
        }
    }

    redirectToPreviousPage() {
        this.authenticationService.clearLocalStorage();

        this.router.navigateByUrl('/sign-in');
    }
}