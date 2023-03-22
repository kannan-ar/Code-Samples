import { Injectable } from "@angular/core";
import { User, UserManager } from 'oidc-client-ts';
import { AppConfig } from "../models";
import { selectConfig } from '../store/platform';
import { Store } from '@ngrx/store';
import { filter, first } from 'rxjs/operators';


export class AuthService {
    userManager: UserManager;

    constructor(config: AppConfig) {
        this.userManager = new UserManager({
            authority: config.auth_authority,
            client_id: config.auth_client_id,
            response_type: 'code',
            scope: config.auth_scope,
            redirect_uri: config.auth_redirect_uri
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