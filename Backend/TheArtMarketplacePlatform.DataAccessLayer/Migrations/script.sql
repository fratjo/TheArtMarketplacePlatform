IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL,
    [Username] nvarchar(100) NOT NULL,
    [Email] nvarchar(150) NOT NULL,
    [PasswordHash] nvarchar(256) NOT NULL,
    [PasswordSalt] nvarchar(256) NOT NULL,
    [Role] nvarchar(max) NOT NULL DEFAULT N'Customer',
    [Status] nvarchar(max) NOT NULL DEFAULT N'Active',
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [ArtisanProfiles] (
    [UserId] uniqueidentifier NOT NULL,
    [Bio] nvarchar(500) NOT NULL,
    [City] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_ArtisanProfiles] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_ArtisanProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [CustomerProfiles] (
    [UserId] uniqueidentifier NOT NULL,
    [ShippingAddress] nvarchar(250) NOT NULL,
    CONSTRAINT [PK_CustomerProfiles] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_CustomerProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [DeliveryPartnerProfiles] (
    [UserId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_DeliveryPartnerProfiles] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_DeliveryPartnerProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Products] (
    [Id] uniqueidentifier NOT NULL,
    [ArtisanId] uniqueidentifier NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [Price] decimal(10,2) NOT NULL,
    [QuantityLeft] int NOT NULL DEFAULT 0,
    [Category] nvarchar(max) NOT NULL DEFAULT N'Art',
    [Status] nvarchar(max) NOT NULL DEFAULT N'OutOfStock',
    [IsAvailable] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Product_QuantityLeft] CHECK (QuantityLeft >= 0),
    CONSTRAINT [FK_Products_ArtisanProfiles_ArtisanId] FOREIGN KEY ([ArtisanId]) REFERENCES [ArtisanProfiles] ([UserId]) ON DELETE CASCADE
);

CREATE TABLE [Orders] (
    [Id] uniqueidentifier NOT NULL,
    [DeliveryPartnerId] uniqueidentifier NULL,
    [CustomerId] uniqueidentifier NULL,
    [Status] nvarchar(max) NOT NULL DEFAULT N'Pending',
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_CustomerProfiles_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [CustomerProfiles] ([UserId]) ON DELETE SET NULL,
    CONSTRAINT [FK_Orders_DeliveryPartnerProfiles_DeliveryPartnerId] FOREIGN KEY ([DeliveryPartnerId]) REFERENCES [DeliveryPartnerProfiles] ([UserId])
);

CREATE TABLE [ProductReviews] (
    [Id] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NULL,
    [CustomerId] uniqueidentifier NULL,
    [CustomerComment] nvarchar(500) NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT (GETDATE()),
    [Rating] int NOT NULL,
    [ArtisanResponse] nvarchar(500) NOT NULL,
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

CREATE INDEX [IX_ProductReviews_ArtisanProfileUserId] ON [ProductReviews] ([ArtisanProfileUserId]);

CREATE INDEX [IX_ProductReviews_CustomerId] ON [ProductReviews] ([CustomerId]);

CREATE INDEX [IX_ProductReviews_ProductId] ON [ProductReviews] ([ProductId]);

CREATE INDEX [IX_Products_ArtisanId] ON [Products] ([ArtisanId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250415220048_Init_DB', N'9.0.4');

COMMIT;
GO

