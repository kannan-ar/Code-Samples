import { NgModule, Component, enableProdMode, ViewChild } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { DxDataGridComponent, DxDataGridModule } from 'devextreme-angular';
import { Service, Order } from './app.service';
import { DataService } from './data.service';
import CustomStore from 'devextreme/data/custom_store';
import { HostedServiceApi } from './payroll.service';
import { interval } from 'rxjs';
import { switchMap, takeWhile } from 'rxjs/operators';

@Component({
  selector: 'demo-app',
  templateUrl: `app.component.html`,
  styleUrls: [`app.component.css`],
  providers: [Service, DataService],
})
export class AppComponent {

  dataSource: any = {};
  columns = [];
  status: string = '';
  result: string | null = null;
  delay = 0;

  @ViewChild(DxDataGridComponent, { static: false }) dataGrid!: DxDataGridComponent;

  constructor(private dataService: DataService, private hostedServiceApi: HostedServiceApi) {

  }

  startProcess(): void {
    this.status = 'Starting...';
    this.result = null;

    this.hostedServiceApi.triggerProcess(this.delay).subscribe(({ processId }) => {
      this.pollProcessStatus(processId);
    });
  }

  pollProcessStatus(processId: string): void {
    interval(15000)
      .pipe(
        switchMap(() => this.hostedServiceApi.getProcessStatus(processId)),
        takeWhile((response) => response.status !== 'Completed', true)
      )
      .subscribe(
        (response) => {
          console.log(response)
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

  updateColumns() {
    this.columns = [{ dataField: 'Name', caption: 'The Name' }];
  }
  updateDataSource() {


    this.dataSource = new CustomStore({
      key: "Id",
      load: function (loadOptions) {
        const pageNumber = (loadOptions.skip / loadOptions.take) + 1;
        const pageSize = loadOptions.take;

        return fetch(`https://localhost:44318/api/data/GetData/${pageNumber}/${pageSize}`)
          .then(response => response.json())
          .then(data => {

            //this.dataGrid.instance.repaint();
            //this.columns = data.columns;
            return {
              data: data.Data,
              totalCount: data.TotalItems
            }

          })
          .catch(() => { throw "Data loading error"; });
      }
    });
  }
}
