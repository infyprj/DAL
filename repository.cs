using Shop3DDataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

        #region GetAllUsers
        public List<Users> GetAllUsers()
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            List<Users> users = new List<Users>();
            try
            {
                users = (from user in Context.Users select user).ToList();
            }
            catch (Exception ex)
            {
                users = new List<Users>();
                Console.WriteLine(ex.Message);
            }
            return users;
        }
        #endregion

        #region GetUserByEmail
        public Users GetUserByEmail(string email)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            Users user = null;
            try
            {
                user = (from u in Context.Users where u.Email == email select u).FirstOrDefault();
            }
            catch (Exception ex)
            {
                user = null;
                Console.WriteLine(ex.Message);
            }
            return user;
        }
        #endregion

        #region GetUserById
        public Users GetUserById(int userId)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            Users user = null;
            try
            {
                user = Context.Users.Find(userId);
            }
            catch (Exception ex)
            {
                user = null;
                Console.WriteLine(ex.Message);
            }
            return user;
        }
        #endregion

        #region AddUser
        public bool AddUser(Users user)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            try
            {
                Context.Users.Add(user);
                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion

        #region UpdateUser
        public bool UpdateUser(Users user)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            try
            {
                var existingUser = Context.Users.Find(user.UserID);
                if (existingUser != null)
                {
                    existingUser.Email = user.Email;
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.PhoneNumber = user.PhoneNumber;
                    existingUser.Address = user.Address;
                    existingUser.City = user.City;
                    existingUser.State = user.State;
                    existingUser.PostalCode = user.PostalCode;
                    existingUser.Country = user.Country;
                    
                    Context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion

        #region GetUserRole
        public string GetUserRole(int userId)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            string roleName = string.Empty;
            try
            {
                roleName = (from user in Context.Users
                           join role in Context.Roles on user.RoleID equals role.RoleID
                           where user.UserID == userId
                           select role.RoleName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                roleName = string.Empty;
                Console.WriteLine(ex.Message);
            }
            return roleName;
        }
        #endregion

        #region ValidateUserCredentials
        public Users ValidateUserCredentials(string email, string passwordHash)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            Users user = null;
            try
            {
                user = (from u in Context.Users 
                       where u.Email == email && u.PasswordHash == passwordHash
                       select u).FirstOrDefault();
            }
            catch (Exception ex)
            {
                user = null;
                Console.WriteLine(ex.Message);
            }
            return user;
        }
        #endregion

        #endregion

        #region Product Management

        #region GetAllProducts
        public List<Products> GetAllProducts()
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            List<Products> products = new List<Products>();
            try
            {
                products = (from product in Context.Products select product).ToList();
            }
            catch (Exception ex)
            {
                products = new List<Products>();
                Console.WriteLine(ex.Message);
            }
            return products;
        }
        #endregion

        #region GetProductById
        public Products GetProductById(int productId)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            Products product = null;
            try
            {
                product = Context.Products.Find(productId);
            }
            catch (Exception ex)
            {
                product = null;
                Console.WriteLine(ex.Message);
            }
            return product;
        }
        #endregion

        #region GetProductsByCategory
        public List<Products> GetProductsByCategory(int categoryId)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            List<Products> products = new List<Products>();
            try
            {
                products = (from product in Context.Products
                           where product.CategoryID == categoryId
                           select product).ToList();
            }
            catch (Exception ex)
            {
                products = new List<Products>();
                Console.WriteLine(ex.Message);
            }
            return products;
        }
        #endregion

        #region GetProductsByPriceRange
        public List<Products> GetProductsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            List<Products> products = new List<Products>();
            try
            {
                products = (from product in Context.Products
                           where product.Price >= minPrice && product.Price <= maxPrice
                           select product).ToList();
            }
            catch (Exception ex)
            {
                products = new List<Products>();
                Console.WriteLine(ex.Message);
            }
            return products;
        }
        #endregion

        #region SearchProducts
        public List<Products> SearchProducts(string searchTerm)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            List<Products> products = new List<Products>();
            try
            {
                products = (from product in Context.Products
                           where product.Name.Contains(searchTerm) || 
                                 product.Description.Contains(searchTerm)
                           select product).ToList();
            }
            catch (Exception ex)
            {
                products = new List<Products>();
                Console.WriteLine(ex.Message);
            }
            return products;
        }
        #endregion

        #region AddProduct
        public bool AddProduct(Products product)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            try
            {
                Context.Products.Add(product);
                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion

        #region UpdateProduct
        public bool UpdateProduct(Products product)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            try
            {
                var existingProduct = Context.Products.Find(product.ProductID);
                if (existingProduct != null)
                {
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.CategoryID = product.CategoryID;
                    existingProduct.ModelURL = product.ModelURL;
                    existingProduct.ThumbnailURL = product.ThumbnailURL;
                    existingProduct.Quantity = product.Quantity;
                    
                    Context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion

        #region DeleteProduct
        public bool DeleteProduct(int productId)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            try
            {
                var product = Context.Products.Find(productId);
                if (product != null)
                {
                    Context.Products.Remove(product);
                    Context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion

        #region GetProductImages
        public List<ProductImages> GetProductImages(int productId)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            List<ProductImages> images = new List<ProductImages>();
            try
            {
                images = (from image in Context.ProductImages
                         where image.ProductID == productId
                         select image).ToList();
            }
            catch (Exception ex)
            {
                images = new List<ProductImages>();
                Console.WriteLine(ex.Message);
            }
            return images;
        }
        #endregion

        #endregion

        #region Category Management

        #region GetAllCategories
        public List<Categories> GetAllCategories()
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            List<Categories> categories = new List<Categories>();
            try
            {
                categories = (from category in Context.Categories select category).ToList();
            }
            catch (Exception ex)
            {
                categories = new List<Categories>();
                Console.WriteLine(ex.Message);
            }
            return categories;
        }
        #endregion

        #region GetCategoryById
        public Categories GetCategoryById(int categoryId)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            Categories category = null;
            try
            {
                category = Context.Categories.Find(categoryId);
            }
            catch (Exception ex)
            {
                category = null;
                Console.WriteLine(ex.Message);
            }
            return category;
        }
        #endregion

        #endregion

        #region SavedProducts Management

        #region GetSavedProductsByUser
        public List<Products> GetSavedProductsByUser(int userId)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            List<Products> savedProducts = new List<Products>();
            try
            {
                savedProducts = (from saved in Context.SavedProducts
                                join product in Context.Products on saved.ProductID equals product.ProductID
                                where saved.UserID == userId
                                select product).ToList();
            }
            catch (Exception ex)
            {
                savedProducts = new List<Products>();
                Console.WriteLine(ex.Message);
            }
            return savedProducts;
        }
        #endregion

        #region SaveProduct
        public bool SaveProduct(int userId, int productId)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            try
            {
                // Check if already saved
                var existingSaved = (from saved in Context.SavedProducts
                                    where saved.UserID == userId && saved.ProductID == productId
                                    select saved).FirstOrDefault();
                
                if (existingSaved == null)
                {
                    SavedProducts savedProduct = new SavedProducts
                    {
                        UserID = userId,
                        ProductID = productId
                    };
                    
                    Context.SavedProducts.Add(savedProduct);
                    Context.SaveChanges();
                    return true;
                }
                return false; // Already saved
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion

        #region RemoveSavedProduct
        public bool RemoveSavedProduct(int userId, int productId)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            try
            {
                var savedProduct = (from saved in Context.SavedProducts
                                  where saved.UserID == userId && saved.ProductID == productId
                                  select saved).FirstOrDefault();
                
                if (savedProduct != null)
                {
                    Context.SavedProducts.Remove(savedProduct);
                    Context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion

        #endregion

        #region Order Management

        #region GetOrdersByUser
        public List<Orders> GetOrdersByUser(int userId)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            List<Orders> orders = new List<Orders>();
            try
            {
                orders = (from order in Context.Orders
                         where order.UserID == userId
                         select order).ToList();
            }
            catch (Exception ex)
            {
                orders = new List<Orders>();
                Console.WriteLine(ex.Message);
            }
            return orders;
        }
        #endregion

        #region GetOrderById
        public Orders GetOrderById(int orderId)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            Orders order = null;
            try
            {
                order = Context.Orders.Find(orderId);
            }
            catch (Exception ex)
            {
                order = null;
                Console.WriteLine(ex.Message);
            }
            return order;
        }
        #endregion

        #region GetOrderItems
        public List<OrderItems> GetOrderItems(int orderId)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            List<OrderItems> orderItems = new List<OrderItems>();
            try
            {
                orderItems = (from item in Context.OrderItems
                             where item.OrderID == orderId
                             select item).ToList();
            }
            catch (Exception ex)
            {
                orderItems = new List<OrderItems>();
                Console.WriteLine(ex.Message);
            }
            return orderItems;
        }
        #endregion

        #region CreateOrder
        public int CreateOrder(Orders order)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            try
            {
                Context.Orders.Add(order);
                Context.SaveChanges();
                return order.OrderID; // Return the new order ID
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        #endregion

        #region AddOrderItem
        public bool AddOrderItem(OrderItems orderItem)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            try
            {
                Context.OrderItems.Add(orderItem);
                Context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion

        #endregion

        #region Stored Procedures

        #region GetFilteredProducts
        public List<Products> GetFilteredProducts(int? categoryId, decimal? minPrice, decimal? maxPrice, string searchTerm)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            List<Products> products = new List<Products>();
            try
            {
                var query = from product in Context.Products select product;
                
                if (categoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryID == categoryId.Value);
                }
                
                if (minPrice.HasValue)
                {
                    query = query.Where(p => p.Price >= minPrice.Value);
                }
                
                if (maxPrice.HasValue)
                {
                    query = query.Where(p => p.Price <= maxPrice.Value);
                }
                
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
                }
                
                products = query.ToList();
            }
            catch (Exception ex)
            {
                products = new List<Products>();
                Console.WriteLine(ex.Message);
            }
            return products;
        }
        #endregion

        #region GetProductWithRelatedData
        public Products GetProductWithRelatedData(int productId)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            Products product = null;
            try
            {
                product = Context.Products.Find(productId);
                if (product != null)
                {
                    // Explicitly load related data
                    var category = Context.Categories.Find(product.CategoryID);
                    var images = (from img in Context.ProductImages
                                 where img.ProductID == productId
                                 select img).ToList();
                    
                    // You might need to manually assign these to navigation properties
                    // depending on your entity configuration
                }
            }
            catch (Exception ex)
            {
                product = null;
                Console.WriteLine(ex.Message);
            }
            return product;
        }
        #endregion

        #region GetNumberOfSavedProductsByUser
        public int GetNumberOfSavedProductsByUser(int userId)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            int count = 0;
            try
            {
                count = (from saved in Context.SavedProducts
                        where saved.UserID == userId
                        select saved).Count();
            }
            catch (Exception ex)
            {
                count = 0;
                Console.WriteLine(ex.Message);
            }
            return count;
        }
        #endregion

        #region UpdateProductQuantity
        public bool UpdateProductQuantity(int productId, int newQuantity)
        {
            //To Do: Implement appropriate logic and change the return statement as per your logic
            try
            {
                var product = Context.Products.Find(productId);
                if (product != null)
                {
                    product.Quantity = newQuantity;
                    Context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        #endregion

        #endregion
    }
}
