import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

@Injectable()
export class DataService {

    constructor(
        private http: HttpClient,
    ) {
    }
  getData() {
    return this.http.get("https://localhost:44318/api/data");
  }
}