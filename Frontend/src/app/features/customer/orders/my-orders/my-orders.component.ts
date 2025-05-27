import { Component, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Order, Orders } from '../../../../core/models/order.interface';
import { CustomerService } from '../../../../core/services/customer.service';
import { ToastService } from '../../../../core/services/toast.service';
import {
  AsyncPipe,
  CommonModule,
  CurrencyPipe,
  DatePipe,
} from '@angular/common';
import { OrderStatusPipe } from '../../../../core/pipes/order-status.pipe';
import { Router } from '@angular/router';

@Component({
  selector: 'app-my-orders',
  imports: [AsyncPipe, DatePipe, CurrencyPipe, OrderStatusPipe, CommonModule],
  templateUrl: './my-orders.component.html',
  styleUrl: './my-orders.component.css',
})
export class MyOrdersComponent implements OnInit {
  orders$: BehaviorSubject<Orders> = new BehaviorSubject<Orders>([]);

  constructor(
    private customerService: CustomerService,
    private toastService: ToastService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.customerService.getOrders().subscribe({
      next: (orders) => {
        this.orders$.next(
          orders.sort((a, b) => {
            return (
              new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
            );
          })
        );
      },
      error: (error) => {
        console.log('Error fetching orders:', error);

        this.toastService.show({
          text: `Error fetching orders: ${error.error.title}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });
  }

  getTotalQuantity(order: Order): number {
    if (!Array.isArray(order.orderProducts)) return 0;
    return order.orderProducts.reduce(
      (sum, p) => sum + Number(p.quantity || 0),
      0
    );
  }

  getTotalPrice(order: Order): number {
    if (!Array.isArray(order.orderProducts)) return 0;
    return order.orderProducts.reduce(
      (sum, p) => sum + Number(p.productPrice || 0) * Number(p.quantity || 0),
      0
    );
  }

  goToOrderDetails(orderId: string) {
    this.router.navigate(['/customer/orders', orderId]);
  }

  getOrderStatusClass(status: number): string {
    switch (status) {
      case 0:
        return 'text-warning';
      case 1:
        return 'text-primary';
      case 2:
        return 'text-info';
      case 3:
        return 'text-success';
      case 4:
        return 'text-danger';
      default:
        return '';
    }
  }
}
