import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

import { AppConfig } from '../models';
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";

@Injectable({
    providedIn: 'root'
  })
export class ConfigService {
    constructor(private http: HttpClient) {
    }

    private _appConfig: AppConfig | undefined;

    get appConfig(): AppConfig | undefined {
        return this._appConfig;
    }
    
    public loadConfiguration(): Observable<AppConfig> {
        let configPath = `${window.location.origin}/assets/config.json`;
        return this.http.get<AppConfig>(configPath);
    }

    public getConfiguration(): Observable<AppConfig> {
        return this.loadConfiguration().pipe(tap(config => (this._appConfig = config)));
    }
}