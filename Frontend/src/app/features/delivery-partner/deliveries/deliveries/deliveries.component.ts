import { Component, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Deliveries, Delivery } from '../../../../core/models/order.interface';
import { DeliveryPartnerService } from '../../../../core/services/delivery-partner.service';
import { ToastService } from '../../../../core/services/toast.service';
import { Router } from '@angular/router';
import { AsyncPipe, CommonModule, DatePipe } from '@angular/common';
import { DeliveryStatusPipe } from '../../../../core/pipes/delivery-status.pipe';
import { OrderStatusPipe } from '../../../../core/pipes/order-status.pipe';

@Component({
  selector: 'app-deliveries',
  imports: [AsyncPipe, DeliveryStatusPipe, DatePipe, CommonModule],
  templateUrl: './deliveries.component.html',
  styleUrl: './deliveries.component.css',
})
export class DeliveriesComponent implements OnInit {
  deliveries$: BehaviorSubject<Deliveries> = new BehaviorSubject<Deliveries>(
    []
  );

  constructor(
    private deliveryPartnerService: DeliveryPartnerService,
    private toastService: ToastService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadDeliveries();
  }

  setAsDelivered(deliveryId: string): void {
    this.deliveryPartnerService.setDeliveryAsDelivered(deliveryId).subscribe({
      next: () => {
        this.toastService.show({
          text: 'Delivery marked as delivered successfully.',
          classname: 'bg-success text-light',
          delay: 5000,
        });
        this.loadDeliveries(); // Refresh the deliveries list after marking as delivered
      },
      error: (error) => {
        console.error('Error marking delivery as delivered:', error);
        this.toastService.show({
          text: `Error marking delivery as delivered: ${error.error.detail}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });
  }

  loadDeliveries(): void {
    this.deliveryPartnerService
      .getDeliveries({
        sortBy: 'createdAt',
        sortOrder: 'desc',
      })
      .subscribe({
        next: (deliveries) => {
          this.deliveries$.next(deliveries);
        },
        error: (error) => {
          console.error('Error fetching deliveries:', error);
          this.toastService.show({
            text: `Error fetching deliveries: ${error.error.detail}`,
            classname: 'bg-danger text-light',
            delay: 5000,
          });
        },
      });
  }

  getDeliveryStatusClass(delivery: Delivery): string {
    switch (
      delivery.deliveryStatusUpdates[delivery.deliveryStatusUpdates.length - 1]
        .status
    ) {
      case 0:
        return 'text-warning'; // Pending
      case 1:
        return 'text-primary'; // In Progress
      case 2:
        return 'text-success'; // Delivered
      default:
        return '';
    }
  }
  onStatusChange($event: Event) {
    let filters = {
      status: ($event.target as HTMLSelectElement).value,
      sortBy: 'createdAt',
      sortOrder: 'desc' as 'asc' | 'desc',
    };
    this.deliveryPartnerService.getDeliveries(filters).subscribe({
      next: (deliveries) => {
        this.deliveries$.next(deliveries);
      },
      error: (error) => {
        console.error('Error fetching deliveries:', error);
        this.toastService.show({
          text: `Error fetching deliveries: ${error.error.detail}`,
          classname: 'bg-danger text-light',
          delay: 5000,
        });
      },
    });
  }
}
