import { Component, OnInit } from '@angular/core';
import { ArtisanService } from '../../../../core/services/artisan.service';
import { BehaviorSubject } from 'rxjs';
import { Order } from '../../../../core/models/order.interface';
import { ToastService } from '../../../../core/services/toast.service';
import { AsyncPipe, CurrencyPipe, DatePipe, SlicePipe } from '@angular/common';
import { OrderStatusPipe } from '../../../../core/pipes/order-status.pipe';
import { DeliveryStatusPipe } from '../../../../core/pipes/delivery-status.pipe';

@Component({
  selector: 'app-order-details',
  imports: [
    AsyncPipe,
    CurrencyPipe,
    DatePipe,
    OrderStatusPipe,
    DeliveryStatusPipe,
  ],
  templateUrl: './order-details.component.html',
  styleUrl: './order-details.component.css',
})
export class OrderDetailsComponent implements OnInit {
  order$: BehaviorSubject<Order> = new BehaviorSubject<Order>({} as Order);
  orderId: string | undefined = undefined;
  totalPrice: number = 0;

  constructor(
    private artisanService: ArtisanService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.orderId = window.location.pathname.split('/').pop();

    this.artisanService.getOrderById(this.orderId!).subscribe({
      next: (order) => {
        this.order$.next(order);

        console.log('Order fetched successfully:', order);
      },
      error: (error) => {
        this.toastService.show({
          text: `Error fetching order: ${error.error.detail}`,
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

  getNextOrderStatus(): string {
    const currentStatus = this.order$.getValue().status;

    switch (currentStatus) {
      case 0: // Pending
        return 'Processing';
      case 1: // Processing
        return 'Shipped';
      default:
        return ''; // Rien à faire pour les autres statuts
    }
  }

  setOrderStatus(status: string): void {
    const orderId = this.order$.getValue().id;

    this.artisanService.updateOrderStatus(orderId, status).subscribe({
      next: (updatedOrder) => {
        this.order$.next(updatedOrder);

        console.log('Order status updated successfully:', updatedOrder);
        // Afficher un toast de succès

        this.toastService.show({
          text: 'Order status updated successfully',
          classname: 'bg-success text-light',
          delay: 3000,
        });
      },
      error: (error) => {
        console.log('Error updating order status:', error);
        this.toastService.show({
          text: `Error updating order status: ${error.error.detail}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });
  }
}
