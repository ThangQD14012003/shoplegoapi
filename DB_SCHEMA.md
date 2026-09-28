# ShopLego database schema

Extracted from the live Neon database `neondb` (PostgreSQL 18.6).

## Relationships

```text
Users 1 ──< Carts 1 ──< CartItems >── 1 Products >── 1 Categories
  │                                      │
  └──< Orders >── 1 OrderStatuses        │
         │                               │
         ├──< OrderDetails >─────────────┘
         └──< EmailLogs
```

Delete behavior:

- `Users -> Carts -> CartItems`: cascade.
- `Orders -> OrderDetails` and `Orders -> EmailLogs`: cascade.
- Product/category and user/order references: restrict.

## Tables

All listed application columns are `NOT NULL`.

### Users

| Column | Type | Notes |
|---|---|---|
| Id | integer | PK, identity |
| FullName | text | |
| Email | text | |
| PasswordHash | text | |
| Phone | text | |
| Address | text | |
| Role | text | |
| CreatedAt | timestamp with time zone | |

### Categories

| Column | Type | Notes |
|---|---|---|
| Id | integer | PK, identity |
| Name | text | |
| Description | text | |

### Products

| Column | Type | Notes |
|---|---|---|
| Id | integer | PK, identity |
| CategoryId | integer | FK -> Categories.Id, restrict |
| Name | text | |
| Description | text | |
| Price | numeric | |
| StockQuantity | integer | |
| AvailableQuantity | integer | |
| ImageUrl | text | |
| CreatedAt | timestamp with time zone | |
| UpdatedAt | timestamp with time zone | |

### Carts

| Column | Type | Notes |
|---|---|---|
| Id | integer | PK, identity |
| UserId | integer | FK -> Users.Id, cascade |
| CreatedAt | timestamp with time zone | |

### CartItems

| Column | Type | Notes |
|---|---|---|
| Id | integer | PK, identity |
| CartId | integer | FK -> Carts.Id, cascade |
| ProductId | integer | FK -> Products.Id, restrict |
| Quantity | integer | |
| UnitPrice | numeric | |

### OrderStatuses

| Column | Type | Notes |
|---|---|---|
| Id | integer | PK, identity |
| Name | text | |

Seed values: `Pending`, `Confirmed`, `Shipping`, `Completed`, `Cancelled`.

### Orders

| Column | Type | Notes |
|---|---|---|
| Id | integer | PK, identity |
| UserId | integer | FK -> Users.Id, restrict |
| OrderStatusId | integer | FK -> OrderStatuses.Id, restrict |
| OrderDate | timestamp with time zone | |
| TotalAmount | numeric | |
| ShippingAddress | text | |

### OrderDetails

| Column | Type | Notes |
|---|---|---|
| Id | integer | PK, identity |
| OrderId | integer | FK -> Orders.Id, cascade |
| ProductId | integer | FK -> Products.Id, restrict |
| Quantity | integer | |
| UnitPrice | numeric | |

### EmailLogs

| Column | Type | Notes |
|---|---|---|
| Id | integer | PK, identity |
| OrderId | integer | FK -> Orders.Id, cascade |
| ReceiverEmail | text | |
| Subject | text | |
| SendTime | timestamp with time zone | |
| Status | boolean | |

### SystemSettings

| Column | Type | Notes |
|---|---|---|
| Key | text | PK |
| Value | text | |

Seed keys: `ManagerEmail`, `AccountantEmail`.

## Infrastructure tables

### __EFMigrationsHistory

| Column | Type | Notes |
|---|---|---|
| MigrationId | varchar(150) | PK |
| ProductVersion | varchar(32) | |

Applied migration: `20260914105436_InitialPostgres` (EF Core 8.0.20).

### playing_with_neon

This is Neon's sample table and is not part of the application model.

| Column | Type | Nullable | Notes |
|---|---|---|---|
| id | integer | no | PK, sequence default |
| name | text | no | |
| value | real | yes | |

## Indexes

In addition to primary-key indexes, the database has non-unique B-tree indexes on:

- `CartItems(CartId)`
- `CartItems(ProductId)`
- `Carts(UserId)`
- `EmailLogs(OrderId)`
- `OrderDetails(OrderId)`
- `OrderDetails(ProductId)`
- `Orders(OrderStatusId)`
- `Orders(UserId)`
- `Products(CategoryId)`

## Design notes

- `Users.Email` has no unique constraint or index.
- `Carts.UserId` is not unique, so a user can have multiple carts.
- `CartItems(CartId, ProductId)` is not unique, so the same product can appear multiple times in one cart.
- `OrderDetails(OrderId, ProductId)` is not unique.
- Money columns use unrestricted `numeric` rather than a fixed precision such as `numeric(18,2)`.
- Quantity and money columns have no check constraints preventing negative values.
- Text fields have no length limits at the database level.
