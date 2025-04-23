export interface Product {
  id: number;
  name: string;
  description?: string;
  price: number;
  image: string;
  category: string;
  rating: number;
  quantity: number;
  status: string;
  availability: string;
}

export type Products = Product[];
