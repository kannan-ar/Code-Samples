import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
  })
  export class HostedServiceApi {
    private baseUrl = 'https://localhost:44318/api/Payroll';
  
    constructor(private http: HttpClient) {}
  
    triggerProcess(delay): Observable<{ processId: string }> {
      return this.http.post<{ processId: string }>(`${this.baseUrl}/trigger/${delay}`, {});
    }
  
    getProcessStatus(processId: string): Observable<{ status: string; result: string }> {
      return this.http.get<{ status: string; result: string }>(`${this.baseUrl}/status/${processId}`);
    }
  }