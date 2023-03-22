import { Component } from '@angular/core';
import { Store } from '@ngrx/store';

import { AppConfig } from './models';
import { getConfig } from './store/platform';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'angular-client';

  constructor(
    private store: Store<{ config: AppConfig }>) {
      store.dispatch(getConfig());
  }
}
