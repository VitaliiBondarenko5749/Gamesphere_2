import { Component } from "@angular/core";
import { RouterOutlet, RouterLink, Router } from "@angular/router";

@Component({
    selector: 'unauthorized-header',
    standalone: true,
    imports: [RouterOutlet, RouterLink],
    templateUrl: './unauthorized-header.component.html',
    styleUrl: './unauthorized-header.component.css'
})
export class UnauthorizedHeaderComponent{
    constructor(private router: Router){}
    
    navigateTo(route: string) {
        this.router.navigate([route]);
    }

    openPrompt(){
        const email = window.prompt('Enter your email to reset password...');
    }
} 