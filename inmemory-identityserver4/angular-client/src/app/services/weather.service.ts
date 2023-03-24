import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})
export class WeatherService {
    constructor(public http: HttpClient) {}

    public getWeather(): Promise<string[]> {
        return this.http.get<string[]>('http://localhost:8000/WeatherForecast').toPromise();
    }
}