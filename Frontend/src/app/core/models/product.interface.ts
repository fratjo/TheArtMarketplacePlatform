export interface Product {
  id: number;
  name: string;
  price: number;
  image: string;
  category: string;
  quantity: number;
  status: string;
  availability: string;
}

export type Products = Product[];
