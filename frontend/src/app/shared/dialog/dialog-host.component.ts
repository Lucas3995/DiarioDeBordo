import { AfterViewInit, Component, ViewChild, ViewContainerRef } from '@angular/core';
import { DialogService } from './dialog.service';

@Component({
  selector: 'app-dialog-host',
  standalone: true,
  template: '<ng-container #dialogHost></ng-container>',
})
export class DialogHostComponent implements AfterViewInit {
  @ViewChild('dialogHost', { read: ViewContainerRef }) private readonly vcr!: ViewContainerRef;

  constructor(private readonly dialogService: DialogService) {}

  ngAfterViewInit(): void {
    this.dialogService.setContainer(this.vcr);
  }
}
