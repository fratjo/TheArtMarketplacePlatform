import { Component, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Order } from '../../../../core/models/order.interface';
import { CustomerService } from '../../../../core/services/customer.service';
import { ToastService } from '../../../../core/services/toast.service';
import { AsyncPipe, CurrencyPipe, DatePipe, SlicePipe } from '@angular/common';
import { OrderStatusPipe } from '../../../../core/pipes/order-status.pipe';
import { DeliveryStatusPipe } from '../../../../core/pipes/delivery-status.pipe';

@Component({
  selector: 'app-order-details',
  imports: [
    AsyncPipe,
    CurrencyPipe,
    SlicePipe,
    DatePipe,
    OrderStatusPipe,
    DeliveryStatusPipe,
  ],
  templateUrl: './order-details.component.html',
  styleUrl: './order-details.component.css',
})
export class OrderDetailsComponent implements OnInit {
  order$: BehaviorSubject<Order> = new BehaviorSubject<Order>({} as Order);
  totalPrice: number = 0;
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

    this.order$.subscribe((order) => {
      if (order && order.orderProducts) {
        this.totalPrice = order.orderProducts.reduce(
          (total, product) => total + product.productPrice * product.quantity,
          0
        );
      }
    });
  }
}
