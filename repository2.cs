using Shop3DDataAccessLayer.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Shop3DDataAccessLayer
{
    public class Shop3DRepository
    {
        public Shop3DDbContext Context { get; set; }

        #region Shop3DRepository - Constructor
        public Shop3DRepository(Shop3DDbContext context)
        {
            Context = context;
        }
        #endregion

        #region User Management

        #region RegisterUser - Registers a new user
        public User RegisterUser(string email, string passwordHash, string firstName, string lastName, string phoneNumber, 
                                string address, string city, string state, string postalCode, string country, int roleId)
        {
            User newUser = null;
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Email", email),
                    new SqlParameter("@PasswordHash", passwordHash),
                    new SqlParameter("@FirstName", firstName),
                    new SqlParameter("@LastName", lastName),
                    new SqlParameter("@PhoneNumber", phoneNumber),
                    new SqlParameter("@Address", address),
                    new SqlParameter("@City", city),
                    new SqlParameter("@State", state),
                    new SqlParameter("@PostalCode", postalCode),
                    new SqlParameter("@Country", country),
                    new SqlParameter("@RoleID", roleId)
                };

                var userId = Context.Database
                    .ExecuteSqlRaw("EXEC sp_RegisterUser @Email, @PasswordHash, @FirstName, @LastName, @PhoneNumber, " +
                                  "@Address, @City, @State, @PostalCode, @Country, @RoleID", parameters);

                if (userId > 0)
                {
                    newUser = Context.Users.FirstOrDefault(u => u.Email == email);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return newUser;
        }
        #endregion

        #region AuthenticateUser - Authenticates user with email and password
        public User AuthenticateUser(string email, string passwordHash)
        {
            User user = null;
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Email", email),
                    new SqlParameter("@PasswordHash", passwordHash)
                };

                var users = Context.Users
                    .FromSqlRaw("EXEC sp_AuthenticateUser @Email, @PasswordHash", parameters)
                    .ToList();

                user = users.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return user;
        }
        #endregion

        #region UpdateUserDetails - Updates user profile details
        public bool UpdateUserDetails(User userObj)
        {
            bool status = false;
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserID", userObj.UserId),
                    new SqlParameter("@FirstName", userObj.FirstName),
                    new SqlParameter("@LastName", userObj.LastName),
                    new SqlParameter("@PhoneNumber", userObj.PhoneNumber),
                    new SqlParameter("@Address", userObj.Address),
                    new SqlParameter("@City", userObj.City),
                    new SqlParameter("@State", userObj.State),
                    new SqlParameter("@PostalCode", userObj.PostalCode),
                    new SqlParameter("@Country", userObj.Country)
                };

                var result = Context.Database
                    .ExecuteSqlRaw("EXEC sp_UpdateUserDetails @UserID, @FirstName, @LastName, @PhoneNumber, " +
                                  "@Address, @City, @State, @PostalCode, @Country", parameters);

                status = result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = false;
            }
            return status;
        }
        #endregion

        #endregion

        #region Product Management

        #region GetAllProducts - Returns all products
        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            try
            {
                products = Context.Products
                    .FromSqlRaw("EXEC sp_GetAllProducts")
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                products = new List<Product>();
            }
            return products;
        }
        #endregion

        #region GetProductById - Returns a product by ID
        public Product GetProductById(int productId)
        {
            Product product = null;
            try
            {
                var parameter = new SqlParameter("@ProductID", productId);
                
                var products = Context.Products
                    .FromSqlRaw("EXEC sp_GetProductById @ProductID", parameter)
                    .ToList();

                product = products.FirstOrDefault();

                if (product != null)
                {
                    // Get product images
                    product.ProductImages = Context.ProductImages
                        .Where(img => img.ProductId == productId)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return product;
        }
        #endregion

        #region SearchProducts - Searches products by term
        public List<Product> SearchProducts(string searchTerm)
        {
            List<Product> products = new List<Product>();
            try
            {
                var parameter = new SqlParameter("@SearchTerm", searchTerm);
                
                products = Context.Products
                    .FromSqlRaw("EXEC sp_SearchProducts @SearchTerm", parameter)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                products = new List<Product>();
            }
            return products;
        }
        #endregion

        #region FilterProductsByCategory - Filters products by category
        public List<Product> FilterProductsByCategory(int categoryId)
        {
            List<Product> products = new List<Product>();
            try
            {
                var parameter = new SqlParameter("@CategoryID", categoryId);
                
                products = Context.Products
                    .FromSqlRaw("EXEC sp_FilterProductsByCategory @CategoryID", parameter)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                products = new List<Product>();
            }
            return products;
        }
        #endregion

        #region FilterProductsByPriceRange - Filters products by price range
        public List<Product> FilterProductsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            List<Product> products = new List<Product>();
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@MinPrice", minPrice),
                    new SqlParameter("@MaxPrice", maxPrice)
                };
                
                products = Context.Products
                    .FromSqlRaw("EXEC sp_FilterProductsByPriceRange @MinPrice, @MaxPrice", parameters)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                products = new List<Product>();
            }
            return products;
        }
        #endregion

        #region AddProduct - Adds a new product
        public int AddProduct(Product productObj)
        {
            int productId = 0;
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@Name", productObj.Name),
                    new SqlParameter("@Description", productObj.Description ?? (object)DBNull.Value),
                    new SqlParameter("@Price", productObj.Price),
                    new SqlParameter("@CategoryID", productObj.CategoryId),
                    new SqlParameter("@ModelURL", productObj.ModelUrl),
                    new SqlParameter("@ThumbnailURL", productObj.ThumbnailUrl ?? (object)DBNull.Value),
                    new SqlParameter("@Quantity", productObj.Quantity)
                };

                var result = Context.Database
                    .ExecuteSqlRaw("EXEC sp_AddProduct @Name, @Description, @Price, @CategoryID, @ModelURL, @ThumbnailURL, @Quantity", 
                                  parameters);

                if (result > 0)
                {
                    // Get the last inserted product ID
                    productId = Context.Products.Max(p => p.ProductId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                productId = 0;
            }
            return productId;
        }
        #endregion

        #region UpdateProduct - Updates a product
        public bool UpdateProduct(Product productObj)
        {
            bool status = false;
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@ProductID", productObj.ProductId),
                    new SqlParameter("@Name", productObj.Name),
                    new SqlParameter("@Description", productObj.Description ?? (object)DBNull.Value),
                    new SqlParameter("@Price", productObj.Price),
                    new SqlParameter("@CategoryID", productObj.CategoryId),
                    new SqlParameter("@ModelURL", productObj.ModelUrl),
                    new SqlParameter("@ThumbnailURL", productObj.ThumbnailUrl ?? (object)DBNull.Value),
                    new SqlParameter("@Quantity", productObj.Quantity)
                };

                var result = Context.Database
                    .ExecuteSqlRaw("EXEC sp_UpdateProduct @ProductID, @Name, @Description, @Price, @CategoryID, @ModelURL, @ThumbnailURL, @Quantity", 
                                  parameters);

                status = result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = false;
            }
            return status;
        }
        #endregion

        #region DeleteProduct - Deletes a product
        public bool DeleteProduct(int productId)
        {
            bool status = false;
            try
            {
                var parameter = new SqlParameter("@ProductID", productId);
                
                var result = Context.Database
                    .ExecuteSqlRaw("EXEC sp_DeleteProduct @ProductID", parameter);

                status = result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = false;
            }
            return status;
        }
        #endregion

        #endregion

        #region Saved Products Management

        #region SaveProduct - Saves a product for a user
        public bool SaveProduct(int userId, int productId)
        {
            bool status = false;
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@ProductID", productId)
                };
                
                var result = Context.Database
                    .ExecuteSqlRaw("EXEC sp_SaveProduct @UserID, @ProductID", parameters);

                status = result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = false;
            }
            return status;
        }
        #endregion

        #region RemoveSavedProduct - Removes a saved product for a user
        public bool RemoveSavedProduct(int userId, int productId)
        {
            bool status = false;
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@ProductID", productId)
                };
                
                var result = Context.Database
                    .ExecuteSqlRaw("EXEC sp_RemoveSavedProduct @UserID, @ProductID", parameters);

                status = result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = false;
            }
            return status;
        }
        #endregion

        #region GetSavedProducts - Gets all saved products for a user
        public List<Product> GetSavedProducts(int userId)
        {
            List<Product> products = new List<Product>();
            try
            {
                var parameter = new SqlParameter("@UserID", userId);
                
                products = Context.Products
                    .FromSqlRaw("EXEC sp_GetSavedProducts @UserID", parameter)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                products = new List<Product>();
            }
            return products;
        }
        #endregion

        #endregion

        #region Order Management

        #region CreateOrder - Creates a new order
        public int CreateOrder(Order orderObj)
        {
            int orderId = 0;
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@UserID", orderObj.UserId),
                    new SqlParameter("@TotalAmount", orderObj.TotalAmount),
                    new SqlParameter("@ShippingAddress", orderObj.ShippingAddress),
                    new SqlParameter("@ShippingCity", orderObj.ShippingCity),
                    new SqlParameter("@ShippingState", orderObj.ShippingState),
                    new SqlParameter("@ShippingPostalCode", orderObj.ShippingPostalCode),
                    new SqlParameter("@ShippingCountry", orderObj.ShippingCountry)
                };

                var result = Context.Database
                    .ExecuteSqlRaw("EXEC sp_CreateOrder @UserID, @TotalAmount, @ShippingAddress, @ShippingCity, " +
                                  "@ShippingState, @ShippingPostalCode, @ShippingCountry", parameters);

                if (result > 0)
                {
                    // Get the last inserted order ID
                    orderId = Context.Orders.Max(o => o.OrderId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                orderId = 0;
            }
            return orderId;
        }
        #endregion

        #region AddOrderItem - Adds an item to an order
        public bool AddOrderItem(OrderItem orderItemObj)
        {
            bool status = false;
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@OrderID", orderItemObj.OrderId),
                    new SqlParameter("@ProductID", orderItemObj.ProductId),
                    new SqlParameter("@Quantity", orderItemObj.Quantity),
                    new SqlParameter("@UnitPrice", orderItemObj.UnitPrice),
                    new SqlParameter("@CustomizationDetails", 
                        orderItemObj.CustomizationDetails ?? (object)DBNull.Value)
                };

                var result = Context.Database
                    .ExecuteSqlRaw("EXEC sp_AddOrderItem @OrderID, @ProductID, @Quantity, @UnitPrice, @CustomizationDetails", 
                                  parameters);

                status = result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = false;
            }
            return status;
        }
        #endregion

        #region GetOrderHistory - Gets order history for a user
        public List<Order> GetOrderHistory(int userId)
        {
            List<Order> orders = new List<Order>();
            try
            {
                var parameter = new SqlParameter("@UserID", userId);
                
                // Get orders
                orders = Context.Orders
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                // Get order items for each order
                foreach (var order in orders)
                {
                    order.OrderItems = Context.OrderItems
                        .Where(oi => oi.OrderId == order.OrderId)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                orders = new List<Order>();
            }
            return orders;
        }
        #endregion

        #endregion

        #region Category Management

        #region GetAllCategories - Gets all categories
        public List<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();
            try
            {
                categories = Context.Categories
                    .FromSqlRaw("EXEC sp_GetAllCategories")
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                categories = new List<Category>();
            }
            return categories;
        }
        #endregion

        #endregion

        #region Helper Methods

        #region GetProductsCount - Gets count of products based on filters
        public int GetProductsCount(int? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            int count = 0;
            try
            {
                string sql = "SELECT dbo.fn_GetProductsCount(@CategoryID, @MinPrice, @MaxPrice)";
                
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@CategoryID", categoryId.HasValue ? (object)categoryId.Value : DBNull.Value),
                    new SqlParameter("@MinPrice", minPrice.HasValue ? (object)minPrice.Value : DBNull.Value),
                    new SqlParameter("@MaxPrice", maxPrice.HasValue ? (object)maxPrice.Value : DBNull.Value)
                };

                var result = Context.Database.SqlQuery<int>(sql, parameters).FirstOrDefault();
                count = result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                count = 0;
            }
            return count;
        }
        #endregion

        #region AddProductImage - Adds an image to a product
        public bool AddProductImage(int productId, string imageUrl)
        {
            bool status = false;
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@ProductID", productId),
                    new SqlParameter("@ImageURL", imageUrl)
                };
                
                var result = Context.Database
                    .ExecuteSqlRaw("EXEC sp_AddProductImage @ProductID, @ImageURL", parameters);

                status = result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                status = false;
            }
            return status;
        }
        #endregion

        #region GetProductImages - Gets all images for a product
        public List<ProductImage> GetProductImages(int productId)
        {
            List<ProductImage> images = new List<ProductImage>();
            try
            {
                var parameter = new SqlParameter("@ProductID", productId);
                
                images = Context.ProductImages
                    .FromSqlRaw("EXEC sp_GetProductImages @ProductID", parameter)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                images = new List<ProductImage>();
            }
            return images;
        }
        #endregion

        #endregion
    }
}
