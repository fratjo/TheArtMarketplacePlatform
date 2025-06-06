import { Component, OnInit } from '@angular/core';
import { Chart, ChartConfiguration, registerables } from 'chart.js';
import { ArtisanService } from '../../../core/services/artisan.service';
import { DeliveryPartnerService } from '../../../core/services/delivery-partner.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Delivery } from '../../../core/models/order.interface';
import { DeliveryStatusPipe } from '../../../core/pipes/delivery-status.pipe';
import { ToastService } from '../../../core/services/toast.service';

Chart.register(...registerables);
@Component({
  selector: 'app-delivery-partner-dashboard',
  imports: [CommonModule, FormsModule],
  templateUrl: './delivery-partner-dashboard.component.html',
  styleUrl: './delivery-partner-dashboard.component.css',
})
export class DeliveryPartnerDashboardComponent implements OnInit {
  orders: any[] = [];
  years: number[] = [];
  selectedYear: number = new Date().getFullYear();
  totalYearlyDeliveriesInTransit: number = 0;
  totalYearlyDeliveriesClosed: number = 0;

  config1: ChartConfiguration<'bar'> = {
    type: 'bar',
    data: {
      labels: [
        'January',
        'February',
        'March',
        'April',
        'May',
        'June',
        'July',
        'August',
        'September',
        'October',
        'November',
        'December',
      ],
      datasets: [
        {
          label: 'Monthly Deliveries',
          data: Array(12).fill(0), // Initialize with zeros
          backgroundColor: 'rgba(192, 132, 75, 0.2)',
          borderColor: 'rgb(192, 141, 75)',
          borderWidth: 1,
        },
      ],
    },
    options: {
      responsive: true,
      plugins: {
        legend: {
          position: 'top',
        },
        title: {
          display: true,
          text: 'Monthly Deliveries Overview',
        },
      },
    },
  };
  chart1: any;

  constructor(
    private deliveryPartnerService: DeliveryPartnerService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.years.push(new Date().getFullYear());
    for (let i = 1; i <= 5; i++) {
      this.years.push(new Date().getFullYear() - i);
    }

    this.chart1 = new Chart('deliveries', this.config1);
    this.updateChart();
  }

  updateChart() {
    this.deliveryPartnerService
      .getDeliveries({
        year: this.selectedYear,
      })
      .subscribe({
        next: (orders) => {
          // chart 1
          this.chart1.data.datasets[0].data = Array(12).fill(0);
          this.totalYearlyDeliveriesInTransit = 0;
          this.totalYearlyDeliveriesClosed = 0;

          orders.forEach((order: Delivery) => {
            console.log('Order:', order);

            const orderDate = new Date(
              order.deliveryStatusUpdates[1].createdAt
            );

            if (orderDate.getFullYear() == this.selectedYear) {
              if (order.deliveryStatusUpdates.length <= 2) {
                this.totalYearlyDeliveriesInTransit += 1;
              } else if (order.deliveryStatusUpdates.length > 2) {
                this.totalYearlyDeliveriesClosed += 1;
              }

              const month = orderDate.getMonth();
              this.chart1.data.datasets[0].data[month] += 1;
            }
          });

          this.chart1.update();
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
