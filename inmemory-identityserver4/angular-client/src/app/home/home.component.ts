import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { filter, first } from 'rxjs/operators';

import { AppConfig } from '../models';
import { AuthService } from '../services/auth.service';
import { ConfigService } from '../services/config.service';
import { selectConfig } from '../store/platform';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  isLoggedIn: Promise<boolean> | null = null;

  constructor(private authService: AuthService) {
  }

  async ngOnInit() {
    this.isLoggedIn = new Promise((resolve, reject) => {
      this.authService!.isLogged().then(result => resolve(result));
    });
  }

  public async onLogin() {
    await this.authService!.login();
  }

  public async onLogout() {
    await this.authService!.logout();
  }

  public async onGetUser() {
    const user = await this.authService!.getUser();
    console.log(user);
  }
}
