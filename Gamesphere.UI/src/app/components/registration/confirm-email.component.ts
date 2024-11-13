import { Component, OnInit } from "@angular/core";
import { AuthenticationService } from "../../services/authentication.service";
import { ActivatedRoute, Router } from "@angular/router";
import { BackendServerResponse } from "../../extensions/backend-server-response";
import {} from "@angular/common/http";
import { NgIf } from "@angular/common";

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [NgIf],
    templateUrl: './confirm-email.component.html',
    styleUrl: './confirm-email.component.css'
})

export class ConfirmEmailComponent implements OnInit {
    userId: string | null = null;
    code: string | null = null;
    isEmailConfirmed: boolean

    constructor(private route: ActivatedRoute, private router: Router, private authenticationService: AuthenticationService) {
        this.isEmailConfirmed = false;
    }

    ngOnInit(): void {
        this.route.queryParams.subscribe(params => {
            this.userId = params['userId'];
            this.code = params['code'];
        });

        this.confirmEmail();
    }

    confirmEmail() {
        if (this.userId && this.code) {
            this.authenticationService.confirmEmail(this.userId, this.code)
                .subscribe((responce: BackendServerResponse) => {
                    if (responce.isSuccess) {
                        this.isEmailConfirmed = true;
                    }
                    else {
                        this.router.navigate(['/error']);
                    }
                });
        } else {
            this.router.navigate(['/error']);
        }
    }

    redirectToMainPage() {
        this.router.navigate([''])
    }
}