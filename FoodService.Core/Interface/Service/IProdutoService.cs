﻿using FoodService.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodService.Core.Interface.Service
{
    /// <summary>
    /// Interface for the product service.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Retrieves all products asynchronously.
        /// </summary>
        Task<List<Product>> GetAllProductsAsync();

        /// <summary>
        /// Retrieves a product by its ID asynchronously.
        /// </summary>
        Task<Product> GetProductByIdAsync(int id);

        /// <summary>
        /// Creates a new product asynchronously.
        /// </summary>
        Task<Product> CreateProductAsync(Product product);

        /// <summary>
        /// Updates a product asynchronously.
        /// </summary>
        Task<Product?> UpdateProductAsync(Product product);

        /// <summary>
        /// Deletes a product by its ID asynchronously.
        /// </summary>
        Task<bool> DeleteProductAsync(int id);
    }
}
