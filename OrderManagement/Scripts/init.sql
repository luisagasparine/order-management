CREATE DATABASE OrderManagement;

CREATE TABLE Customers (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    Price DECIMAL(18, 2) NOT NULL,
    StockQuantity INT NOT NULL
);

CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY,
    CustomerId INT FOREIGN KEY REFERENCES Customers(Id),
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(18, 2),
    Status NVARCHAR(50) DEFAULT 'Novo'
);

CREATE TABLE OrderItems (
    Id INT PRIMARY KEY IDENTITY,
    OrderId INT FOREIGN KEY REFERENCES Orders(Id),
    ProductId INT FOREIGN KEY REFERENCES Products(Id),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18, 2) NOT NULL
);

CREATE TABLE NotificationsStatusOrder (
    Id INT PRIMARY KEY IDENTITY,
    OrderId INT FOREIGN KEY REFERENCES Orders(Id),
    StatusFrom VARCHAR(50),
    StatusTo VARCHAR(50),
    DateChanged DATETIME
);

INSERT INTO Customers (Name, Email, Phone)
VALUES 
    ('João Silva', 'joao.silva@example.com', '1234-5678'),
    ('Maria Oliveira', 'maria.oliveira@example.com', '9876-5432'),
    ('Carlos Souza', 'carlos.souza@example.com', '1122-3344');


INSERT INTO Products (Name, Description, Price, StockQuantity)
VALUES 
    ('Produto A', 'Descrição do Produto A', 100.50, 10),
    ('Produto B', 'Descrição do Produto B', 50.75, 20),
    ('Produto C', 'Descrição do Produto C', 75.00, 15);

INSERT INTO Orders (CustomerId, TotalAmount, Status)
VALUES 
    (1, 300.50, 'Novo'),
    (2, 150.75, 'Novo'), 
    (3, 225.00, 'Processando');

INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
VALUES 
    (1, 1, 2, 100.50), 
    (1, 2, 1, 50.75), 
    (2, 2, 3, 50.75),  
    (3, 1, 1, 100.50), 
    (3, 3, 2, 75.00);   