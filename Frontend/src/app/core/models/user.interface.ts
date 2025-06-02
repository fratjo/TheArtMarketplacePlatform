export interface DeliveryPartner {
  id: string;
  username: string;
  deliveryPartnerProfile: DeliveryPartnerProfile;
}

export enum UserRole {
  Admin = 'Admin',
  Artisan = 'Artisan',
  Customer = 'Customer',
  DeliveryPartner = 'DeliveryPartner',
}

export enum UserStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Banned = 'Banned',
}

export interface User {
  id: string;
  username: string;
  email: string;
  status: UserStatus;
  role: UserRole;
  isDeleted: boolean;
  createdAt: Date;
  updatedAt: Date;
  deletedAt?: Date | null;
}

export interface UserProfileStrategy {
  profileType: 'artisan' | 'customer' | 'deliverypartner';
}

export interface ArtisanProfile extends UserProfileStrategy {
  id: string;
  username: string;
  email: string;
  shippingAddress: string;
  profileType: 'artisan';
  bio: string;
  city: string;
}

export interface CustomerProfile extends UserProfileStrategy {
  id: string;
  username: string;
  email: string;
  shippingAddress: string;
  profileType: 'customer';
}

export interface DeliveryPartnerProfile extends UserProfileStrategy {
  id: string;
  username: string;
  email: string;
  profileType: 'deliverypartner';
}

export type AnyUserProfile =
  | ArtisanProfile
  | CustomerProfile
  | DeliveryPartnerProfile;
