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

  constructor(private authService: AuthService) {
  }

  async ngOnInit() {
    this.authService.signinRedirect();
  }
}
