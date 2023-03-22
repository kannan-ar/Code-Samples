import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { filter, first } from 'rxjs/operators';

import { AppConfig } from '../models';
import { AuthService } from '../services/auth.service';
import { selectConfig } from '../store/platform';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  authService: AuthService | undefined;

  constructor(
    private store: Store<{ config: AppConfig }>) {
  }

  async ngOnInit() {
    this.store.select(selectConfig).pipe(filter(x => x.auth_authority !== "")).pipe(first()).subscribe(x => {
      console.log(x);
      this.authService = new AuthService(x);
    });
  }

  public async onLogin() {
    await this.authService!.login();
  }

  public async onGetUser() {
    const user = await this.authService!.getUser();
    console.log(user);
  }
}
