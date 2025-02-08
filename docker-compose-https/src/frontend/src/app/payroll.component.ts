import { Component } from '@angular/core';
import { HostedServiceApi } from './payroll.service';
import { interval } from 'rxjs';
import { takeWhile, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-my-component',
  template: `
    <button (click)="startProcess()">Start Process</button>
    <p>Status: {{ status }}</p>
    <p *ngIf="result">Result: {{ result }}</p>
  `,
})
export class MyComponent {
  status: string = '';
  result: string | null = null;

  constructor(private hostedServiceApi: HostedServiceApi) {}

  startProcess(): void {
    this.status = 'Starting...';
    this.result = null;
  
    /*this.hostedServiceApi.triggerProcess().subscribe(({ processId }) => {
      this.pollProcessStatus(processId);
    });*/
  }

  pollProcessStatus(processId: string): void {
    interval(1000)
      .pipe(
        switchMap(() => this.hostedServiceApi.getProcessStatus(processId)),
        takeWhile((response) => response.status !== 'Completed', true)
      )
      .subscribe(
        (response) => {
          this.status = response.status;
          if (response.status === 'Completed') {
            this.result = response.result;
          }
        },
        (error) => {
          console.error('Error polling process status:', error);
        }
      );
  }
}