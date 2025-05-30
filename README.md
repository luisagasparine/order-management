# Order Management System

A simplified order management system developed for a small store. The system allows staff to register customers and products, as well as create and track orders efficiently.

## Tech Stack

- **Backend:** C# with ASP.NET Core MVC  
- **Frontend:** HTML5, CSS3, Bootstrap, jQuery  
- **Database:** SQL Server  
- **ORM:** Dapper.NET

## Features

### 1. Customers
- Full CRUD (Create, Read, Update, Delete)
- Filters by name or email

### 2. Products
- Full CRUD
- Filter by name

### 3. Orders
- Create new order:
  - Select a customer
  - Add one or more products with quantity
  - Check product stock availability
  - Calculate total order amount
  - Save the order with its items
- View order list:
  - Display customer, date, total amount, and status
  - Filter by customer or status
- View order details
- Update order status (e.g., `New` → `Processing`)
- Log status update history

## Project Structure

The project is organized into layers for better maintainability and scalability:
/OrderManagement
- Controllers # Presentation layer (ASP.NET MVC)
- Models # Domain models
- Services # Business logic
- Repositories # Data access using Dapper.NET
- ViewModels # View models (input/output)
- Views # Razor pages (.cshtml)
- wwwroot # Static files (JS, CSS)
- Scripts # SQL script to create and seed the database

## Tests

- Basic unit tests have been implemented for the service layer using xUnit and Moq.
- Tests cover scenarios such as order creation, stock validation, and status updates.
