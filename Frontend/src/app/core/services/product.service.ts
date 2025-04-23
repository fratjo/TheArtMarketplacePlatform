import { Injectable } from '@angular/core';
import { Products } from '../models/product.interface';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  products: Products = [
    {
      id: 1,
      name: 'Handmade Vase',
      price: 25.99,
      image: 'assets/images/vase.jpg',
      category: 'Decor',
      quantity: 10,
      status: 'In Stock',
      availability: 'Available',
    },
    {
      id: 2,
      name: 'Wooden Chair',
      price: 120.0,
      image: 'assets/images/chair.jpg',
      category: 'Furniture',
      quantity: 0,
      status: 'Out of Stock',
      availability: 'Available',
    },
    {
      id: 3,
      name: 'Knitted Scarf',
      price: 15.5,
      image: 'assets/images/scarf.jpg',
      category: 'Clothing',
      quantity: 5,
      status: 'In Stock',
      availability: 'Available',
    },
    {
      id: 4,
      name: 'Ceramic Plate',
      price: 12.99,
      image: 'assets/images/plate.jpg',
      category: 'Kitchenware',
      quantity: 20,
      status: 'In Stock',
      availability: 'Available',
    },
    {
      id: 5,
      name: 'Leather Wallet',
      price: 45.0,
      image: 'assets/images/wallet.jpg',
      category: 'Accessories',
      quantity: 15,
      status: 'In Stock',
      availability: 'Available',
    },
    {
      id: 6,
      name: 'Bamboo Basket',
      price: 30.0,
      image: 'assets/images/basket.jpg',
      category: 'Decor',
      quantity: 8,
      status: 'In Stock',
      availability: 'Available',
    },
    {
      id: 7,
      name: 'Wool Blanket',
      price: 60.0,
      image: 'assets/images/blanket.jpg',
      category: 'Bedding',
      quantity: 3,
      status: 'In Stock',
      availability: 'Available',
    },
    {
      id: 8,
      name: 'Glass Lamp',
      price: 80.0,
      image: 'assets/images/lamp.jpg',
      category: 'Lighting',
      quantity: 2,
      status: 'In Stock',
      availability: 'Available',
    },
    {
      id: 9,
      name: 'Cotton Towel',
      price: 10.0,
      image: 'assets/images/towel.jpg',
      category: 'Bathroom',
      quantity: 25,
      status: 'In Stock',
      availability: 'Not Available',
    },
    {
      id: 10,
      name: 'Metal Shelf',
      price: 150.0,
      image: 'assets/images/shelf.jpg',
      category: 'Furniture',
      quantity: 1,
      status: 'In Stock',
      availability: 'Available',
    },
    {
      id: 11,
      name: 'Handmade Mug',
      price: 18.0,
      image: 'assets/images/mug.jpg',
      category: 'Kitchenware',
      quantity: 12,
      status: 'In Stock',
      availability: 'Not Available',
    },
    {
      id: 12,
      name: 'Silk Pillowcase',
      price: 35.0,
      image: 'assets/images/pillowcase.jpg',
      category: 'Bedding',
      quantity: 7,
      status: 'In Stock',
      availability: 'Available',
    },
    {
      id: 13,
      name: 'Wooden Frame',
      price: 22.0,
      image: 'assets/images/frame.jpg',
      category: 'Decor',
      quantity: 18,
      status: 'In Stock',
      availability: 'Not Available',
    },
  ];

  categories$ = new BehaviorSubject<string[]>(
    Array.from(new Set(this.products.map((product) => product.category)))
  );

  getProducts$() {
    return new BehaviorSubject<Products>(this.products);
  }

  filterProducts(
    products: Products,
    filters: {
      name?: string;
      category?: string;
      priceMin?: number;
      priceMax?: number;
      status?: string;
      quantityMin?: number;
      quantityMax?: number;
      availability?: string;
    }
  ): Products {
    return products.filter((product) => {
      console.log('Filtering products with filters:', filters);

      const matchesName =
        !filters.name ||
        product.name.toLowerCase().includes(filters.name.toLowerCase());
      const matchesCategory =
        !filters.category || product.category === filters.category;
      const matchesMinPrice =
        filters.priceMin == null || product.price >= filters.priceMin;
      const matchesMaxPrice =
        filters.priceMax == null || product.price <= filters.priceMax;
      const matchesStatus =
        !filters.status || product.status === filters.status;
      const matchesQuantityMin =
        filters.quantityMin == null || product.quantity >= filters.quantityMin;
      const matchesQuantityMax =
        filters.quantityMax == null || product.quantity <= filters.quantityMax;
      const matchesAvailability =
        !filters.availability ||
        product.availability.toLowerCase() ===
          filters.availability.toLowerCase();

      return (
        matchesName &&
        matchesCategory &&
        matchesMinPrice &&
        matchesMaxPrice &&
        matchesStatus &&
        matchesQuantityMin &&
        matchesQuantityMax &&
        matchesAvailability
      );
    });
  }
}
