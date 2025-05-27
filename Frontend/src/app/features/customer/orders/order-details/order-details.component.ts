import { Component, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Order } from '../../../../core/models/order.interface';
import { CustomerService } from '../../../../core/services/customer.service';
import { ToastService } from '../../../../core/services/toast.service';
import { AsyncPipe, CurrencyPipe, SlicePipe } from '@angular/common';

@Component({
  selector: 'app-order-details',
  imports: [AsyncPipe, CurrencyPipe, SlicePipe],
  templateUrl: './order-details.component.html',
  styleUrl: './order-details.component.css',
})
export class OrderDetailsComponent implements OnInit {
  order$: BehaviorSubject<Order> = new BehaviorSubject<Order>({} as Order);
  orderId: string | undefined = undefined;

  constructor(
    private customerService: CustomerService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.orderId = window.location.pathname.split('/').pop();

    this.customerService.getOrderById(this.orderId!).subscribe({
      next: (order) => {
        this.order$.next(order);
      },
      error: (error) => {
        console.log('Error fetching order:', error);

        this.toastService.show({
          text: `Error fetching order: ${error.error.title}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });
  }

  // Ajoute d'autres méthodes si nécessaire
}
