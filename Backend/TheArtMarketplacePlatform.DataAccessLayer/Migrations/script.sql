﻿IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [ProductCategories] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_ProductCategories] PRIMARY KEY ([Id])
);

CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL,
    [Username] nvarchar(100) NOT NULL,
    [Email] nvarchar(150) NOT NULL,
    [PasswordHash] nvarchar(256) NOT NULL,
    [PasswordSalt] nvarchar(256) NOT NULL,
    [Status] nvarchar(max) NOT NULL DEFAULT N'Active',
    [Role] nvarchar(max) NOT NULL,
    [IsDeleted] nvarchar(1) NOT NULL DEFAULT N'0',
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [ArtisanProfiles] (
    [UserId] uniqueidentifier NOT NULL,
    [Bio] nvarchar(500) NOT NULL,
    [City] nvarchar(100) NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_ArtisanProfiles] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_ArtisanProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [CustomerProfiles] (
    [UserId] uniqueidentifier NOT NULL,
    [ShippingAddress] nvarchar(256) NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_CustomerProfiles] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_CustomerProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [DeliveryPartnerProfiles] (
    [UserId] uniqueidentifier NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_DeliveryPartnerProfiles] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_DeliveryPartnerProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [RefreshTokens] (
    [Id] int NOT NULL IDENTITY,
    [Token] nvarchar(1000) NOT NULL,
    [ExpiryDate] datetime2 NOT NULL,
    [IsRevoked] bit NOT NULL DEFAULT CAST(0 AS bit),
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    [UserId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Products] (
    [Id] uniqueidentifier NOT NULL,
    [ArtisanId] uniqueidentifier NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [ImageUrl] nvarchar(max) NULL,
    [Price] decimal(10,2) NOT NULL,
    [QuantityLeft] int NOT NULL DEFAULT 0,
    [Rating] decimal(18,2) NULL,
    [CategoryId] uniqueidentifier NULL,
    [Status] nvarchar(max) NOT NULL,
    [Availability] nvarchar(max) NOT NULL DEFAULT N'Available',
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    [DeletedAt] datetime2 NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Product_QuantityLeft] CHECK (QuantityLeft >= 0),
    CONSTRAINT [FK_Products_ArtisanProfiles_ArtisanId] FOREIGN KEY ([ArtisanId]) REFERENCES [ArtisanProfiles] ([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Products_ProductCategories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [ProductCategories] ([Id]) ON DELETE SET NULL
);

CREATE TABLE [Orders] (
    [Id] uniqueidentifier NOT NULL,
    [DeliveryPartnerId] uniqueidentifier NULL,
    [CustomerId] uniqueidentifier NULL,
    [ArtisanId] uniqueidentifier NULL,
    [ArtisanName] nvarchar(max) NOT NULL,
    [DeliveryPartnerName] nvarchar(max) NOT NULL,
    [ShippingAddress] nvarchar(256) NOT NULL,
    [Status] nvarchar(max) NOT NULL DEFAULT N'Pending',
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    [UpdatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_CustomerProfiles_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [CustomerProfiles] ([UserId]) ON DELETE SET NULL,
    CONSTRAINT [FK_Orders_DeliveryPartnerProfiles_DeliveryPartnerId] FOREIGN KEY ([DeliveryPartnerId]) REFERENCES [DeliveryPartnerProfiles] ([UserId])
);

CREATE TABLE [ProductFavorites] (
    [CustomerId] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_ProductFavorites] PRIMARY KEY ([CustomerId], [ProductId]),
    CONSTRAINT [FK_ProductFavorites_CustomerProfiles_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [CustomerProfiles] ([UserId]),
    CONSTRAINT [FK_ProductFavorites_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ProductReviews] (
    [Id] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [CustomerId] uniqueidentifier NULL,
    [Rating] int NOT NULL,
    [CustomerComment] nvarchar(500) NOT NULL,
    [ArtisanResponse] nvarchar(500) NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    [UpdatedAt] datetime2 NULL DEFAULT (GETDATE()),
    [ArtisanProfileUserId] uniqueidentifier NULL,
    CONSTRAINT [PK_ProductReviews] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_ProductReview_Rating] CHECK (Rating >= 1 AND Rating <= 5),
    CONSTRAINT [FK_ProductReviews_ArtisanProfiles_ArtisanProfileUserId] FOREIGN KEY ([ArtisanProfileUserId]) REFERENCES [ArtisanProfiles] ([UserId]),
    CONSTRAINT [FK_ProductReviews_CustomerProfiles_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [CustomerProfiles] ([UserId]) ON DELETE SET NULL,
    CONSTRAINT [FK_ProductReviews_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id])
);

CREATE TABLE [DeliveryStatusUpdates] (
    [Id] uniqueidentifier NOT NULL,
    [OrderId] uniqueidentifier NOT NULL,
    [Status] nvarchar(max) NOT NULL DEFAULT N'Pending',
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_DeliveryStatusUpdates] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DeliveryStatusUpdates_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [OrderProducts] (
    [Id] uniqueidentifier NOT NULL,
    [OrderId] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NULL,
    [ArtisanName] nvarchar(100) NOT NULL,
    [ProductName] nvarchar(100) NOT NULL,
    [ProductDescription] nvarchar(500) NOT NULL,
    [ProductPrice] decimal(10,2) NOT NULL,
    [Quantity] int NOT NULL DEFAULT 1,
    CONSTRAINT [PK_OrderProducts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderProducts_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderProducts_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE SET NULL
);

CREATE INDEX [IX_DeliveryStatusUpdates_OrderId] ON [DeliveryStatusUpdates] ([OrderId]);

CREATE INDEX [IX_OrderProducts_OrderId] ON [OrderProducts] ([OrderId]);

CREATE INDEX [IX_OrderProducts_ProductId] ON [OrderProducts] ([ProductId]);

CREATE INDEX [IX_Orders_CustomerId] ON [Orders] ([CustomerId]);

CREATE INDEX [IX_Orders_DeliveryPartnerId] ON [Orders] ([DeliveryPartnerId]);

CREATE INDEX [IX_ProductFavorites_ProductId] ON [ProductFavorites] ([ProductId]);

CREATE INDEX [IX_ProductReviews_ArtisanProfileUserId] ON [ProductReviews] ([ArtisanProfileUserId]);

CREATE INDEX [IX_ProductReviews_CustomerId] ON [ProductReviews] ([CustomerId]);

CREATE INDEX [IX_ProductReviews_ProductId] ON [ProductReviews] ([ProductId]);

CREATE INDEX [IX_Products_ArtisanId] ON [Products] ([ArtisanId]);

CREATE INDEX [IX_Products_CategoryId] ON [Products] ([CategoryId]);

CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);

CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250607200550_InitDB', N'9.0.4');

COMMIT;
GO

