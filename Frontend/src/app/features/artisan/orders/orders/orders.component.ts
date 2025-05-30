import { Component, OnInit } from '@angular/core';
import { ArtisanService } from '../../../../core/services/artisan.service';
import { ToastService } from '../../../../core/services/toast.service';
import { Order, Orders } from '../../../../core/models/order.interface';
import { BehaviorSubject } from 'rxjs';
import { AsyncPipe, CommonModule, DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { OrderStatusPipe } from '../../../../core/pipes/order-status.pipe';

@Component({
  selector: 'app-orders',
  imports: [AsyncPipe, DatePipe, OrderStatusPipe, CommonModule],
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.css',
})
export class OrdersComponent implements OnInit {
  orders$: BehaviorSubject<Orders> = new BehaviorSubject<Orders>([]);

  constructor(
    private artisanService: ArtisanService,
    private toastService: ToastService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Initialization logic can go here
    this.artisanService.getOrders().subscribe({
      next: (orders) => {
        // Handle successful retrieval of orders
        this.orders$.next(orders);
      },
      error: (error) => {
        // Handle error in retrieving orders
        console.error('Error fetching orders:', error);
        this.toastService.show({
          text: `Error fetching orders: ${error.error.title}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });
  }

  goToOrderDetails(orderId: string) {
    this.router.navigate(['/artisan/orders', orderId]);
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
}
