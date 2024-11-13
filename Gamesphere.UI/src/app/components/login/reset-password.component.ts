import { Component, OnInit } from "@angular/core";
import { AuthenticationService } from "../../services/authentication.service";
import { FormsModule } from "@angular/forms";
import { ResetPasswordModel } from "../../models/reset-password.model";
import { NgIf, NgClass } from "@angular/common";
import { ActivatedRoute, Router } from "@angular/router";
import { BackendServerResponse } from "../../extensions/backend-server-response";


@Component({
    selector: 'app-root',
    standalone: true,
    imports:[FormsModule, NgIf, NgClass],
    templateUrl: './reset-password.component.html',
    styleUrl: './reset-password.component.css'
})

export class ResetPasswordComponent implements OnInit{
    model: ResetPasswordModel;
    hiddenPassword: boolean;
    hiddenConfirmPassword: boolean;
 
    constructor(private authenticationService: AuthenticationService, private route: ActivatedRoute,
        private router: Router){
        this.model = new ResetPasswordModel('', '', '', '');
        this.hiddenPassword = true;
        this.hiddenConfirmPassword = true;
    }

    ngOnInit(): void {
        this.route.queryParams.subscribe( params => {
            this.model.code = params['code'];
        });
    }

    inverseBool(value: boolean): boolean {
        return !value;
    }

    passwordConfirmed(): boolean{
        return this.model.password===this.model.confirmPassword;
    }

    resetPassword(){
        this.authenticationService.resetPassword(this.model)
        .subscribe((response: BackendServerResponse) =>{
            if(response.isSuccess){
                alert(response.message);
                this.router.navigateByUrl('/sign-in');
            } else{
                alert(response.message);
                this.router.navigateByUrl('/error');
            }
        })
    }
}