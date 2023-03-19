import { Injectable } from "@angular/core";
import { User, UserManager } from 'oidc-client-ts';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    userManager: UserManager;

    constructor() {
        this.userManager = new UserManager({
                authority: 'https://localhost:9000/',
                client_id: 'angularclient',
                response_type: 'code',
                scope: 'myApi.read',
                redirect_uri: 'http://localhost:7000/callback'
            });
    }

    public login(): Promise<void> {
        return this.userManager!.signinRedirect();
    }

    public renewToken(): Promise<User | null> {
        return this.userManager!.signinSilent();
    }

    public logout(): Promise<void> {
        return this.userManager!.signoutRedirect();
    }

    public async signinCallback() {
        console.log(this.userManager);
        await this.userManager!.signinCallback();
    }

    public signinRedirect() {
        this.userManager!.signinRedirectCallback().then(function () {
            window.location.href = "/";
        }).catch(function (e: any) {
            console.error(e);
        });
    }

    public async isLogged(): Promise<boolean> {
        return await this.userManager!.getUser() !== null;
    }

    public async getUser(): Promise<User | null> {
        return await this.userManager!.getUser();
    }
}