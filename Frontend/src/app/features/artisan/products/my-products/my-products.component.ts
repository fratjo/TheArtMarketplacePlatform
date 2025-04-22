import { AsyncPipe } from '@angular/common';
import { Component } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-my-products',
  imports: [AsyncPipe],
  templateUrl: './my-products.component.html',
  styleUrl: './my-products.component.css',
})
export class MyProductsComponent {
  products = [
    {
      id: 1,
      name: 'Handmade Vase',
      price: 25.99,
      image: 'assets/images/vase.jpg',
      category: 'Decor',
      status: 'Available',
    },
    {
      id: 2,
      name: 'Wooden Chair',
      price: 120.0,
      image: 'assets/images/chair.jpg',
      category: 'Furniture',
      status: 'Out of Stock',
    },
    {
      id: 3,
      name: 'Knitted Scarf',
      price: 15.5,
      image: 'assets/images/scarf.jpg',
      category: 'Clothing',
      status: 'Available',
    },
  ];

  products$ = new BehaviorSubject<any[]>(this.products);

  sorting: any = {
    name: 'asc',
    price: 'asc',
    category: 'asc',
    status: 'asc',
  };

  sortBy(property: keyof (typeof this.products)[0]) {
    const direction = this.sorting[property] === 'asc' ? 1 : -1;
    this.products$.next(
      this.products.sort((a, b) => {
        if (a[property] < b[property]) {
          return -1 * direction;
        }
        if (a[property] > b[property]) {
          return 1 * direction;
        }
        return 0;
      })
    );
    this.sorting[property] = this.sorting[property] === 'asc' ? 'desc' : 'asc';
  }

  search(event: Event) {
    const searchTerm = (event.target as HTMLInputElement).value.toLowerCase();
    const filteredProducts = this.products.filter(
      (product) =>
        product.name.toLowerCase().includes(searchTerm) ||
        product.category.toLowerCase().includes(searchTerm) ||
        product.status.toLowerCase().includes(searchTerm)
    );
    this.products$.next(filteredProducts);
    console.log('Filtered products:', filteredProducts);
  }
}
