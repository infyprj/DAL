USE Shop3D;
GO

-- User Management Stored Procedures
CREATE OR ALTER PROCEDURE sp_RegisterUser
    @Email NVARCHAR(255),
    @PasswordHash NVARCHAR(255),
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @PhoneNumber NVARCHAR(20),
    @Address NVARCHAR(255),
    @City NVARCHAR(100),
    @State NVARCHAR(100),
    @PostalCode NVARCHAR(20),
    @Country NVARCHAR(100),
    @RoleID INT
AS
BEGIN
    INSERT INTO Users (Email, PasswordHash, FirstName, LastName, PhoneNumber, Address, City, State, PostalCode, Country, RoleID)
    VALUES (@Email, @PasswordHash, @FirstName, @LastName, @PhoneNumber, @Address, @City, @State, @PostalCode, @Country, @RoleID);
    
    SELECT SCOPE_IDENTITY() AS UserID;
END
GO

CREATE OR ALTER PROCEDURE sp_AuthenticateUser
    @Email NVARCHAR(255),
    @PasswordHash NVARCHAR(255)
AS
BEGIN
    SELECT u.*, r.RoleName
    FROM Users u
    JOIN Roles r ON u.RoleID = r.RoleID
    WHERE u.Email = @Email AND u.PasswordHash = @PasswordHash;
END
GO

CREATE OR ALTER PROCEDURE sp_UpdateUserDetails
    @UserID INT,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @PhoneNumber NVARCHAR(20),
    @Address NVARCHAR(255),
    @City NVARCHAR(100),
    @State NVARCHAR(100),
    @PostalCode NVARCHAR(20),
    @Country NVARCHAR(100)
AS
BEGIN
    UPDATE Users
    SET FirstName = @FirstName,
        LastName = @LastName,
        PhoneNumber = @PhoneNumber,
        Address = @Address,
        City = @City,
        State = @State,
        PostalCode = @PostalCode,
        Country = @Country
    WHERE UserID = @UserID;
    
    SELECT @@ROWCOUNT AS RecordsUpdated;
END
GO

-- Product Management Stored Procedures
CREATE OR ALTER PROCEDURE sp_GetAllProducts
AS
BEGIN
    SELECT p.*, c.Name AS CategoryName
    FROM Products p
    JOIN Categories c ON p.CategoryID = c.CategoryID;
END
GO

CREATE OR ALTER PROCEDURE sp_GetProductById
    @ProductID INT
AS
BEGIN
    SELECT p.*, c.Name AS CategoryName
    FROM Products p
    JOIN Categories c ON p.CategoryID = c.CategoryID
    WHERE p.ProductID = @ProductID;
    
    -- Get product images
    SELECT * FROM ProductImages
    WHERE ProductID = @ProductID;
END
GO

CREATE OR ALTER PROCEDURE sp_SearchProducts
    @SearchTerm NVARCHAR(255)
AS
BEGIN
    SELECT p.*, c.Name AS CategoryName
    FROM Products p
    JOIN Categories c ON p.CategoryID = c.CategoryID
    WHERE p.Name LIKE '%' + @SearchTerm + '%' 
       OR p.Description LIKE '%' + @SearchTerm + '%'
       OR c.Name LIKE '%' + @SearchTerm + '%';
END
GO

CREATE OR ALTER PROCEDURE sp_FilterProductsByCategory
    @CategoryID INT
AS
BEGIN
    SELECT p.*, c.Name AS CategoryName
    FROM Products p
    JOIN Categories c ON p.CategoryID = c.CategoryID
    WHERE p.CategoryID = @CategoryID;
END
GO

CREATE OR ALTER PROCEDURE sp_FilterProductsByPriceRange
    @MinPrice DECIMAL(18, 2),
    @MaxPrice DECIMAL(18, 2)
AS
BEGIN
    SELECT p.*, c.Name AS CategoryName
    FROM Products p
    JOIN Categories c ON p.CategoryID = c.CategoryID
    WHERE p.Price BETWEEN @MinPrice AND @MaxPrice;
END
GO

CREATE OR ALTER PROCEDURE sp_AddProduct
    @Name NVARCHAR(255),
    @Description NVARCHAR(MAX),
    @Price DECIMAL(18, 2),
    @CategoryID INT,
    @ModelURL NVARCHAR(255),
    @ThumbnailURL NVARCHAR(255),
    @Quantity INT
AS
BEGIN
    INSERT INTO Products (Name, Description, Price, CategoryID, ModelURL, ThumbnailURL, Quantity)
    VALUES (@Name, @Description, @Price, @CategoryID, @ModelURL, @ThumbnailURL, @Quantity);
    
    SELECT SCOPE_IDENTITY() AS ProductID;
END
GO

CREATE OR ALTER PROCEDURE sp_UpdateProduct
    @ProductID INT,
    @Name NVARCHAR(255),
    @Description NVARCHAR(MAX),
    @Price DECIMAL(18, 2),
    @CategoryID INT,
    @ModelURL NVARCHAR(255),
    @ThumbnailURL NVARCHAR(255),
    @Quantity INT
AS
BEGIN
    UPDATE Products
    SET Name = @Name,
        Description = @Description,
        Price = @Price,
        CategoryID = @CategoryID,
        ModelURL = @ModelURL,
        ThumbnailURL = @ThumbnailURL,
        Quantity = @Quantity
    WHERE ProductID = @ProductID;
    
    SELECT @@ROWCOUNT AS RecordsUpdated;
END
GO

CREATE OR ALTER PROCEDURE sp_DeleteProduct
    @ProductID INT
AS
BEGIN
    DELETE FROM ProductImages WHERE ProductID = @ProductID;
    DELETE FROM Products WHERE ProductID = @ProductID;
    
    SELECT @@ROWCOUNT AS RecordsDeleted;
END
GO

-- Saved Products Stored Procedures
CREATE OR ALTER PROCEDURE sp_SaveProduct
    @UserID INT,
    @ProductID INT
AS
BEGIN
    -- Check if product is already saved
    IF NOT EXISTS (SELECT 1 FROM SavedProducts WHERE UserID = @UserID AND ProductID = @ProductID)
    BEGIN
        INSERT INTO SavedProducts (UserID, ProductID)
        VALUES (@UserID, @ProductID);
        
        SELECT SCOPE_IDENTITY() AS SavedProductID;
    END
    ELSE
    BEGIN
        SELECT 0 AS SavedProductID; -- Already saved
    END
END
GO

CREATE OR ALTER PROCEDURE sp_RemoveSavedProduct
    @UserID INT,
    @ProductID INT
AS
BEGIN
    DELETE FROM SavedProducts
    WHERE UserID = @UserID AND ProductID = @ProductID;
    
    SELECT @@ROWCOUNT AS RecordsDeleted;
END
GO

CREATE OR ALTER PROCEDURE sp_GetSavedProducts
    @UserID INT
AS
BEGIN
    SELECT p.*, c.Name AS CategoryName
    FROM SavedProducts sp
    JOIN Products p ON sp.ProductID = p.ProductID
    JOIN Categories c ON p.CategoryID = c.CategoryID
    WHERE sp.UserID = @UserID;
END
GO

-- Order Management Stored Procedures
CREATE OR ALTER PROCEDURE sp_CreateOrder
    @UserID INT,
    @TotalAmount DECIMAL(18, 2),
    @ShippingAddress NVARCHAR(255),
    @ShippingCity NVARCHAR(100),
    @ShippingState NVARCHAR(100),
    @ShippingPostalCode NVARCHAR(20),
    @ShippingCountry NVARCHAR(100)
AS
BEGIN
    INSERT INTO Orders (UserID, TotalAmount, ShippingAddress, ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry)
    VALUES (@UserID, @TotalAmount, @ShippingAddress, @ShippingCity, @ShippingState, @ShippingPostalCode, @ShippingCountry);
    
    SELECT SCOPE_IDENTITY() AS OrderID;
END
GO

CREATE OR ALTER PROCEDURE sp_AddOrderItem
    @OrderID INT,
    @ProductID INT,
    @Quantity INT,
    @UnitPrice DECIMAL(18, 2),
    @CustomizationDetails NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO OrderItems (OrderID, ProductID, Quantity, UnitPrice, CustomizationDetails)
    VALUES (@OrderID, @ProductID, @Quantity, @UnitPrice, @CustomizationDetails);
    
    -- Update product quantity
    UPDATE Products
    SET Quantity = Quantity - @Quantity
    WHERE ProductID = @ProductID;
    
    SELECT SCOPE_IDENTITY() AS OrderItemID;
END
GO

CREATE OR ALTER PROCEDURE sp_GetOrderHistory
    @UserID INT
AS
BEGIN
    SELECT o.*
    FROM Orders o
    WHERE o.UserID = @UserID
    ORDER BY o.OrderDate DESC;
    
    -- Get order items
    SELECT oi.*, p.Name, p.ThumbnailURL
    FROM Orders o
    JOIN OrderItems oi ON o.OrderID = oi.OrderID
    JOIN Products p ON oi.ProductID = p.ProductID
    WHERE o.UserID = @UserID
    ORDER BY o.OrderDate DESC;
END
GO

-- Category Management Stored Procedures
CREATE OR ALTER PROCEDURE sp_GetAllCategories
AS
BEGIN
    SELECT * FROM Categories;
END
GO

-- Helper Functions
CREATE OR ALTER FUNCTION fn_GetProductsCount
(
    @CategoryID INT = NULL,
    @MinPrice DECIMAL(18, 2) = NULL,
    @MaxPrice DECIMAL(18, 2) = NULL
)
RETURNS INT
AS
BEGIN
    DECLARE @Count INT;
    
    IF @CategoryID IS NOT NULL AND @MinPrice IS NOT NULL AND @MaxPrice IS NOT NULL
        SELECT @Count = COUNT(*) FROM Products WHERE CategoryID = @CategoryID AND Price BETWEEN @MinPrice AND @MaxPrice;
    ELSE IF @CategoryID IS NOT NULL
        SELECT @Count = COUNT(*) FROM Products WHERE CategoryID = @CategoryID;
    ELSE IF @MinPrice IS NOT NULL AND @MaxPrice IS NOT NULL
        SELECT @Count = COUNT(*) FROM Products WHERE Price BETWEEN @MinPrice AND @MaxPrice;
    ELSE
        SELECT @Count = COUNT(*) FROM Products;
        
    RETURN @Count;
END
GO

-- Image Management Stored Procedures
CREATE OR ALTER PROCEDURE sp_AddProductImage
    @ProductID INT,
    @ImageURL NVARCHAR(255)
AS
BEGIN
    INSERT INTO ProductImages (ProductID, ImageURL)
    VALUES (@ProductID, @ImageURL);
    
    SELECT SCOPE_IDENTITY() AS ImageID;
END
GO

CREATE OR ALTER PROCEDURE sp_GetProductImages
    @ProductID INT
AS
BEGIN
    SELECT * FROM ProductImages
    WHERE ProductID = @ProductID;
END
GO
