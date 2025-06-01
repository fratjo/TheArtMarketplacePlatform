import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Order, Orders } from '../../../core/models/order.interface';
import { CommonModule } from '@angular/common';
import { OrderStatusPipe } from '../../../core/pipes/order-status.pipe';
import { Router } from '@angular/router';

@Component({
  selector: 'app-orders-view',
  imports: [CommonModule, OrderStatusPipe],
  templateUrl: './orders-view.component.html',
  styleUrl: './orders-view.component.css',
})
export class OrdersViewComponent {
  @Input() orders: Orders | null = [];
  @Input() isArtisan: boolean = false;
  @Output() orderSelected = new EventEmitter<string>();

  constructor(private router: Router) {}

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

  goToOrderDetails(orderId: string) {
    this.orderSelected.emit(orderId);
  }
}
