import { Component } from "@angular/core";
import { Router } from "@angular/router";


@Component({
    selector: 'app-root',
    standalone: true,
    imports:[],
    templateUrl: './error.component.html',
    styleUrl: './error.component.css'
})

export class ErrorComponent{
    constructor(private router: Router){}

    redirectToMainPage(){
        this.router.navigate([''])
    }
}