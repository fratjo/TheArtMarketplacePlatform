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
  productReviews: Reviews;
  artisan: ArtisanProduct;
  artisanId: string;
}

export interface ProductForm {
  id?: string;
  name: string;
  description?: string;
  price: number;
  category: string;
  image?: File | string;
  imageExtension?: string;
  availability: string;
  quantityLeft: number;
}

export type Products = Product[];
export interface Category {
  id: string;
  name: string;
}

export type Categories = Category[];

export interface ArtisanProduct {
  id: string;
  username: string;
  user: {
    username: string;
  };
}

export type ArtisanProducts = ArtisanProduct[];

export interface Review {
  id: string;
  productId: string;
  rating: number;
  customerComment: string;
  artisanResponse: string;
  createdAt: Date;
  updatedAt: Date;
}

export type Reviews = Review[];
