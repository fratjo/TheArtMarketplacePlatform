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
  reviews: Reviews;
  artisan: ArtisanProduct;
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
  user: string;
  rating: number;
  comment: string;
  authorComment: string;
  createdAt: Date;
}

export type Reviews = Review[];
