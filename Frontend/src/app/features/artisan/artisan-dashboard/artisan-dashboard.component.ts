import { Component, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Chart, ChartConfiguration, registerables } from 'chart.js';
import { ArtisanService } from '../../../core/services/artisan.service';

Chart.register(...registerables);

@Component({
  selector: 'app-artisan-dashboard',
  imports: [ReactiveFormsModule, FormsModule],
  templateUrl: './artisan-dashboard.component.html',
  styleUrl: './artisan-dashboard.component.css',
})
export class ArtisanDashboardComponent implements OnInit {
  orders: any[] = [];
  years: number[] = [];
  selectedYear: number = new Date().getFullYear();

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
          label: 'Monthly Sales',
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
          text: 'Monthly Sales Overview',
        },
      },
    },
  };
  chart1: any;

  config2: ChartConfiguration<'bar'> = {
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
          label: 'Monthly Revenue',
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
          text: 'Monthly Revenue Overview',
        },
      },
    },
  };
  chart2: any;

  constructor(private artisanService: ArtisanService) {}

  ngOnInit(): void {
    this.years.push(new Date().getFullYear());
    for (let i = 1; i <= 5; i++) {
      this.years.push(new Date().getFullYear() - i);
    }

    this.chart1 = new Chart('sales', this.config1);
    this.chart2 = new Chart('revenue', this.config2);
    this.updateChart();
  }

  updateChart() {
    this.artisanService.getOrders().subscribe({
      next: (orders) => {
        // chart 1
        this.chart1.data.datasets[0].data = Array(12).fill(0);
        orders.forEach((order: any) => {
          const orderDate = new Date(order.createdAt);

          if (orderDate.getFullYear() == this.selectedYear) {
            const month = orderDate.getMonth();
            this.chart1.data.datasets[0].data[month] += 1;
          }
        });

        this.chart1.update();

        // chart 2
        this.chart2.data.datasets[0].data = Array(12).fill(0);
        orders.forEach((order: any) => {
          const orderDate = new Date(order.createdAt);

          if (orderDate.getFullYear() == this.selectedYear) {
            console.log(order);

            const month = orderDate.getMonth();
            const totalPrice = order.orderProducts.reduce(
              (sum: number, item: any) =>
                sum + item.productPrice * item.quantity,
              0
            );
            this.chart2.data.datasets[0].data[month] += totalPrice;
          }
        });

        this.chart2.update();
      },
      error: (err) => {
        console.error('Error fetching orders:', err);
      },
    });
  }
}
