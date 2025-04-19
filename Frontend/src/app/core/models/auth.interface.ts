export interface Login {
  email: string;
  password: string;
}

export interface ArtisanRegistration {
  username: string;
  email: string;
  bio: string;
  city: string;
  password: string;
  confirmPassword: string;
}

export interface CustomerRegistration {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  shippingAddress: string;
}

export interface DeliveryPartnerRegistration {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface AuthResponse {
  token: string;
}

export interface CheckExist {
  exists: boolean;
}
