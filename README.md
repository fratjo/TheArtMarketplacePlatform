# TheArtMarketplacePlatform

## Technologies

-   [x] Angular

-   [x] Bootstrap

-   [x] Asp.Net core

-   [x] Entity Framework core

-   [x] .Net 9.0

-   [x] JWT Based Authentication

-   [x] N-tier Architecture (WepApi, BusinessLayer, Core/Domain, DataAccessLayer)

## Features

### Common

-   [x] Register, Login, Logout

-   [x] Hashing & Salting password

-   [x] Change Password (Current, New, NewRepeat)

-   [x] Manage Profile infos as Username, Email, ...

-   [x] Browse marketplace and see products details

-   [x] Add products to ShoppingCart

-   [x] Filters data depending the page. Different types of filters (select, multi-select, search, ...)

-   [x] Sorting data depending the page. Different types of sorting (select, table-title, ...)

### Admin

-   [x] Manage users : activate/deactivate or soft-delete/undelete

-   [x] Manage products : soft-delete/undelete

### Artisan

-   [x] Create, Edit, Delete products

-   [x] Process and Ship orders

-   [x] Respond to customer reviews

-   [x] Get summary through Dashboard

### Customer

-   [x] Browse marketplace and see products details

-   [x] Leave reviews and rating on bought products

-   [x] Track own orders

-   [x] Mark/Unmark products as favorites

-   [x] View favorites

-   [x] Add products to ShoppingCart

-   [x] Proceed to ShoppingCart checkout

-   [x] Select delivery partner

-   [x] Receive order(s) confirmation (if multiples artisans involved in the order, one order is create for each artisan involved)

-   [x] => No dashboard provided but a navbar with all access needed

### Delivery Partner

-   [x] Track deliveries

-   [x] Update delivery status

-   [x] Get summary through Dashboard

## Extra-features

-   [x] Refresh Opaque Token (JWT short living + opaque token long living)

-   [x] FluentValidation

-   [x] DTOs (Request/Respone)

-   [x] Automatic Seeding & Migrating

-   [x] Live Password Validation

-   [x] Live Email/Username Checking

-   [x] Some work with RxJs

-   [x] JWT information for validate route access (JWT UserId must be the same as the route UserId)

-   [x] Role based Guard for limite page and childrens access

## Architecture

As said is an N-Tier Architecture, with veticality on roles.

Frontend Role service -> Api Role Controller -> Role Service -> Entity Repository

## Test Users

| Role             | Email                           | Password   |
| ---------------- | ------------------------------- | ---------- |
| Admin            | admin@marketplace.com           | Password1! |
| Artisan          | artisan@marketplace.com         | Password1! |
| Customer         | customer@marketplace.com        | Password1! |
| Delivery Partner | deliverypartner@marketplace.com | Password1! |

## Start App

Download last "Examen Release"

**! Don't forget to provide your own credentials for the database!**  
Database connection settings are located in:  
`Backend/TheArtMarketplacePlatform.WebAPI/appsettings.json`

Frontend run :

-   npm install
-   ng serve

Backend run :

-   dotnet run --projet ./TheArtMarketplacePlatform.WebAPI

> ⚠️ Database migration and seeding should run automatically on first start.  
> If you encounter issues, check your connection string in `Backend/TheArtMarketplacePlatform.WebAPI/appsettings.json`.
> If the database is not created or updated, try running a manual migration.
