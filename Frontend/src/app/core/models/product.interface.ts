export interface Product {
  id: string;
  name: string;
  description?: string;
  price: number;
  imageUrl: string;
  category: Category;
  rating: number;
  quantityLeft: number;
  status: string;
  availability: string;
}

export interface ProductForm {
  id?: string;
  name: string;
  description?: string;
  price: number;
  category: string;
  image?: File | string;
  imageExtension?: string;
  status: string;
  quantityLeft: number;
}

export type Products = Product[];
export interface Category {
  id: string;
  name: string;
}

export type Categories = Category[];
