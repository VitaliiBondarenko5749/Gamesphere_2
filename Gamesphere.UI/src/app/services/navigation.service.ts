import { Injectable } from "@angular/core";

@Injectable({
    "providedIn": "root"
})
export class NavigationService {
    private navigationStack: string[] = [];

    addToStack(route: string) {
        if (this.navigationStack.length >= 10) {
            this.clearStack();
        }

        this.navigationStack.push(route);
    }

    getLastRoute(): string {
        return this.navigationStack.length > 0 ? this.navigationStack[this.navigationStack.length - 1] : '/';
    }

    private clearStack() {
        this.navigationStack = [];
    }
}