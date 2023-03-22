import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";

import { AppConfig } from '../models';
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
  })
export class ConfigService {
    constructor(private http: HttpClient) {
    }

    public loadConfiguration(): Observable<AppConfig> {
        let configPath = `${window.location.origin}/assets/config.json`;
        console.log(configPath);
        return this.http.get<AppConfig>(configPath);
    }
}