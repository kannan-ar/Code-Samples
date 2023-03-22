import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AppConfig } from '../models';
import { AuthService } from '../services/auth.service';
import { selectConfig } from '../store/platform';
import { filter, first } from 'rxjs/operators';

@Component({
  selector: 'app-auth-callback',
  templateUrl: './auth-callback.component.html',
  styleUrls: ['./auth-callback.component.css']
})
export class AuthCallbackComponent implements OnInit {

  authService: AuthService | undefined;

  constructor(private store: Store<{ config: AppConfig }>) {
  }

  async ngOnInit() {
    this.store.select(selectConfig).pipe(filter(x => x.auth_authority !== "")).pipe(first()).subscribe(x => {
      this.authService = new AuthService(x);
      this.authService.signinRedirect();
    });
  }
}
