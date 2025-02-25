import { Component } from '@angular/core';
import { Fido2Service } from '../services/fido2.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  username = '';

  constructor(private fido2Service: Fido2Service) { }

  async register() {
    const credentialCreationOptions: any = {
      "rp": {
          "id": "webapplication",
          "name": "webapplication"
      },
      "user": {
          "name": "xxx",
          "id": "cq2iz1cZpkaOPt3nqm9Drw",
          "displayName": "xxx"
      },
      "challenge": "aGjv0mXgDhHjSggCbQ_WfA",
      "pubKeyCredParams": [
          {
              "type": "public-key",
              "alg": -8
          }
      ],
      "timeout": 60000,
      "attestation": "none",
      "authenticatorSelection": {
          "residentKey": "discouraged",
          "requireResidentKey": false,
          "userVerification": "preferred"
      },
      "hints": [],
      "excludeCredentials": [],
      "extensions": {
          "exts": true,
          "uvm": true,
          "devicePubKey": {
              "attestation": "none",
              "attestationFormats": []
          },
          "credProps": true
      },
      "status": "ok",
      "errorMessage": ""
  };
    const credential = await navigator.credentials.create(credentialCreationOptions);
    /*this.fido2Service.register(this.username).subscribe(async (options: any) => {
      const credential = await navigator.credentials.create(options);
      this.fido2Service.completeRegistration(credential).subscribe((res) => {
        console.log('Registration successful!', res);
      });
    });*/
  }
}
