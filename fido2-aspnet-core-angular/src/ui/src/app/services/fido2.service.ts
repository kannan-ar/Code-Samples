import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class Fido2Service {
  private apiUrl = 'https://localhost:7027/api/fido2';
  constructor(private http: HttpClient) {}

  register(username: string) {
    return this.http.post(`${this.apiUrl}/register/options`, { username });
  }

  completeRegistration(response: any) {
    return this.http.post(`${this.apiUrl}/register/complete`, response);
  }
}
