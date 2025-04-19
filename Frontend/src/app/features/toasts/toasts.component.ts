import { Component, inject } from '@angular/core';
import { NgbToastModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-toasts',
  imports: [NgbToastModule],
  template: `
    @for (toast of toastService.toasts; track toast) {
    <ngb-toast
      [header]="toast.text"
      [textContent]="toast.text"
      [class]="toast.classname"
      [autohide]="true"
      [delay]="toast.delay || 5000"
      (click)="toastService.remove(toast)"
      style="padding: 10px 20px"
    >
    </ngb-toast>
    }
  `,
  host: {
    class: 'toast-container position-fixed bottom-0 end-0 p-3',
    style: 'z-index: 1200',
  },
})
export class ToastsComponent {
  toastService = inject(ToastService);
}
