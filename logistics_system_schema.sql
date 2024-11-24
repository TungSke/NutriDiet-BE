CREATE DATABASE LogiConnect;

USE LogiConnect;
-- 1. Bảng Roles (Vai trò người dùng)
CREATE TABLE Roles (
    RoleId INT IDENTITY PRIMARY KEY,
    RoleName NVARCHAR(50) -- Admin, LogisticsProvider, Manufacturer, EndCustomer, Guest
);

-- 2. Bảng Users (Tài khoản người dùng)
CREATE TABLE Users (
    UserId INT IDENTITY PRIMARY KEY,
    Username NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) UNIQUE,
    FullName NVARCHAR(200),
    RoleId INT,
    Status NVARCHAR(50) DEFAULT 'Active', -- Active, Inactive, Banned
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);

-- 3. Bảng Products (Sản phẩm của nhà sản xuất)
CREATE TABLE Products (
    ProductId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    ManufacturerId INT, -- FK to Users (Manufacturer)
    Category NVARCHAR(100),
    Price DECIMAL(18, 2),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME,
    FOREIGN KEY (ManufacturerId) REFERENCES Users(UserId)
);

-- 4. Bảng LogisticsProviders (Nhà cung cấp dịch vụ logistics)
CREATE TABLE LogisticsProviders (
    ProviderId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    Reputation DECIMAL(3, 2), -- Đánh giá uy tín của nhà cung cấp
    ContactInfo NVARCHAR(255),
    RegistrationDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) DEFAULT 'Active', -- Active, Inactive
    UserId INT, -- FK to Users table
    ServiceTypeId INT, -- FK to ServiceTypes
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ServiceTypeId) REFERENCES ServiceTypes(ServiceTypeId)
);

-- 5. Bảng ServiceTypes (Loại dịch vụ)
CREATE TABLE ServiceTypes (
    ServiceTypeId INT IDENTITY PRIMARY KEY,
    ServiceTypeName NVARCHAR(100), -- Ví dụ: Domestic Transport, International Transport, Warehousing
    Description NVARCHAR(MAX)
);

-- 6. Bảng Orders (Đơn hàng)
CREATE TABLE Orders (
    OrderId INT IDENTITY PRIMARY KEY,
    OrderCode NVARCHAR(50) UNIQUE,
    ManufacturerId INT, -- FK to Users (Manufacturer)
    LogisticsProviderId INT, -- FK to LogisticsProviders
    DeliveryAddress NVARCHAR(500),
    OrderDate DATETIME DEFAULT GETDATE(),
    RequiredDate DATETIME,
    Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, Shipped, Delivered, Cancelled
    TotalCost DECIMAL(18, 2),
    PaymentStatus NVARCHAR(50) DEFAULT 'Pending', -- Pending, Completed, Failed, Refunded
    FOREIGN KEY (ManufacturerId) REFERENCES Users(UserId),
    FOREIGN KEY (LogisticsProviderId) REFERENCES LogisticsProviders(ProviderId)
);

-- 7. Bảng OrderDetails (Chi tiết đơn hàng)
CREATE TABLE OrderDetails (
    OrderDetailId INT IDENTITY PRIMARY KEY,
    OrderId INT, -- FK to Orders
    ProductId INT, -- FK to Products
    Quantity INT,
    Price DECIMAL(18, 2),
    Total DECIMAL(18, 2), -- Price * Quantity
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

-- 8. Bảng Reviews (Đánh giá)
CREATE TABLE Reviews (
    ReviewId INT IDENTITY PRIMARY KEY,
    UserId INT, -- FK to Users (Manufacturer, LogisticsProvider, EndCustomer)
    ProductId INT, -- FK to Products
    OrderId INT, -- FK to Orders
    Rating INT, -- Đánh giá từ 1 đến 5
    Comments NVARCHAR(MAX),
    ReviewDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId)
);