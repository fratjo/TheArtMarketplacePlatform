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
  artisanId: string;
  artisanName: string;
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
  userId: string;
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

export interface ProductFavorite {
  id: string;
  name: string;
  description?: string;
  price: number;
  category: Category;
  availability: string;
  rating: number;
  imageUrl: string;
}

export type ProductFavorites = ProductFavorite[];
