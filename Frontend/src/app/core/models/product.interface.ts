export interface Product {
  id: string;
  name: string;
  description?: string;
  price: number;
  image: string;
  category: Category;
  rating: number;
  quantityLeft: number;
  status: string;
  availability: string;
}

export type Products = Product[];

export interface Category {
  id: string;
  name: string;
}
