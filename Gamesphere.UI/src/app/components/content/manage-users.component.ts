import { Component, OnInit } from "@angular/core";
import { AuthorizationResponse } from "../../extensions/authorization-response";
import { UserAuthorizedModel } from "../../models/user-authorized.model";
import { AuthorizedHeaderComponent } from "../header/authorized-header.component";
import { AuthenticationService } from "../../services/authentication.service";
import { NavigationService } from "../../services/navigation.service";
import { Router } from "@angular/router";
import { FooterComponent } from "../footer/footer.component";
import { NgIf, NgFor } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { UserInfoModel } from "../../models/user-info.model";
import { Pagination } from "../../extensions/pagination";
import { NgxPaginationModule } from "ngx-pagination";
import { SendEmailModel } from "../../models/send-email.model";
import { EmailService } from "../../services/email.service";

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [NgIf, AuthorizedHeaderComponent, FooterComponent, FormsModule, NgFor, NgxPaginationModule],
    templateUrl: './manage-users.component.html',
    styleUrl: './manage-users.component.css'
})

export class ManageUsersComponent implements OnInit{
    isAuthorized: AuthorizationResponse = AuthorizationResponse.TokenNotFound;
    authorizationResponse = AuthorizationResponse;
    ownUserData: UserAuthorizedModel;
    searchText?: string;
    users: UserInfoModel[];
    pagination: Pagination;

    constructor(private authenticationService: AuthenticationService, private navigationService: NavigationService,
        private router: Router, private emailService: EmailService){
        this.ownUserData = new UserAuthorizedModel('', '', '', '', '');
        this.pagination = new Pagination(1, 0, 10);
        this.users = [];
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
        
        if(this.ownUserData.role === 'User' || this.isAuthorized === this.authorizationResponse.TokenNotFound){
            this.router.navigateByUrl(this.navigationService.getLastRoute());
        }

        this.getUsers();
    }

    getUsers(){
        this.authenticationService.getUsers(this.searchText || '', this.pagination)
        .subscribe( response => {
            this.users = response.items as UserInfoModel[];
            this.pagination.count = response.count;
        }, error => {});
    }

    getUserWithPagination(){
        this.pagination.page = 1;
        this.getUsers();
    }

    onPageChange(event: number){
        this.pagination.page = event;
        this.getUsers();
    }

    banUser(user: UserInfoModel){
        let confirmBan = confirm(`Are you sure you want to ban user ${user.userName}?`);

        if(confirmBan){
            let bannedBy = `${this.ownUserData.username}-${this.ownUserData.role}`;
            let description = prompt('Write a reason of the ban') || '';

            this.authenticationService.banUser(user.id, description, bannedBy)
            .subscribe(response => {
                if(response.isSuccess){
                    alert(`User ${user.userName} has been banned`);
                    this.getUsers()
                }
            }, error => {});
        }
    }

    upgradeRole(user: UserInfoModel){
        let confirmUpgradeRole = confirm(`Are you sure you want to give the role "Admin" for user ${user.userName}?`);

        if(confirmUpgradeRole){
            this.authenticationService.upgradeRole(user.id)
            .subscribe(response => {
                if(response.isSuccess){
                    alert(`User ${user.userName} is already admin!`);
                    this.getUsers();
                }
            }, error => {})
        }
    }

    downgradeRole(user: UserInfoModel){
        let confirmDowngradeRole = confirm(`Are you sure you want to downgrade ${user.userName} to "User"?`);

        if(confirmDowngradeRole){
            let description = prompt('Write a reason of the downgrade') || '';

            this.authenticationService.downgradeRole(user.id, description)
            .subscribe(response => {
                if(response.isSuccess){
                    alert(`User ${user.userName} has already role "User"`);
                    this.getUsers()
                }
            }, error => {});
        }
    }

    reportUser(user: UserInfoModel){
        let confirmReport = confirm(`Are you sure you want to report ${user.userName}?`);

        if(confirmReport){
            let reasonOfReport = prompt('Enter a message. Describe a reason of the report:');

            if(reasonOfReport !== null && reasonOfReport.length > 0){
                let emailMessage = `<p>${this.ownUserData.role} ${this.ownUserData.username} sent report to user ${user.userName}.</p><br>`;
                emailMessage += `<p>Reason of the report: <i>${reasonOfReport}</i>.</p>`;

                let emailModel = new SendEmailModel('gamesphere7438@gmail.com', 'Report', emailMessage, true);

                this.emailService.sendEmail(emailModel)
                .subscribe();
            } else {
                alert('You can not send a report without reason!');
            }
        }
    }

    upgradeRoleRequest(user: UserInfoModel){
        let confirmRequest = confirm(`Are you sure you want to send request to upgrade ${user.userName} to role "Admin"?`);

        if(confirmRequest){
            let reasonOfRequest = prompt('Enter a message. Describe why do you want to send request:');

            if(reasonOfRequest !== null && reasonOfRequest.length > 0){
                let emailMessage = `<p>${this.ownUserData.role} ${this.ownUserData.username} sent request to upgrade ${user} to Admin.</p><br>`;
                emailMessage += `<p>Reason of the report: <i>${reasonOfRequest}</i>.</p>`;

                let emailModel = new SendEmailModel('gamesphere7438@gmail.com', 'Upgrade role request', emailMessage, true);

                this.emailService.sendEmail(emailModel)
                .subscribe();
            } else {
                alert('You can not send a request without explanation!');
            }
        }
    }
}