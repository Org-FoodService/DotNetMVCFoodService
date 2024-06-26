﻿using FoodService.Data.Context;
using FoodService.Core.Interface.Repository;
using FoodService.Data.Model;
using FoodService.Core.Repository.Generic;
using Microsoft.EntityFrameworkCore;

namespace FoodService.Core.Repository
{
    /// <summary>
    /// Repository implementation for products.
    /// </summary>
    public class ProductRepository : GenericRepository<Product, int>, IProductRepository
    {
        /// <summary>
        /// Initializes a new instance of the ProductRepository class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public ProductRepository(AppDbContext context) : base(context)
        {
        }
    }
}
