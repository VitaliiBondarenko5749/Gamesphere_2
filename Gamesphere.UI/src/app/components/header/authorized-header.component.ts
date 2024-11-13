import { Component, OnInit } from "@angular/core";
import { AuthenticationService } from "../../services/authentication.service";
import { UserAuthorizedModel } from "../../models/user-authorized.model";
import { RouterOutlet, RouterLink, Router } from "@angular/router";
import { FileService } from "../../services/file.service";

@Component({
    selector: 'authorized-header',
    standalone: true,
    imports: [RouterOutlet, RouterLink],
    templateUrl: './authorized-header.component.html',
    styleUrl: './authorized-header.component.css'
})
export class AuthorizedHeaderComponent implements OnInit {
    userData: UserAuthorizedModel = new UserAuthorizedModel('', '', '', '', '');
    imageUrl: string = '';
    textColor: string = 'white';

    constructor(private authenticationService: AuthenticationService, private fileService: FileService,
      private router: Router) { }

    ngOnInit(): void {
        this.userData = this.authenticationService.getAuthorizeHeaderData();

        this.fileService.downloadFileFromDropbox(this.userData.imageDirectory)
        .subscribe(
          (link: string) => {
            this.imageUrl = link;
          });
    }

    onMouseEnter(){
        this.textColor = 'black'
    }

    onMouseLeave(){
        this.textColor = 'white'
    }

    navigateTo(route: string) {
      this.router.navigate([route]);
  }
}