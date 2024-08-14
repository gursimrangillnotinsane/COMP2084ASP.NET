//using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetDrinks.Controllers;
using DotNetDrinks.Data;
using DotNetDrinks.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
 using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace DotNetDrinksTests
{
    [TestClass]
    public class ProductsControllerTest
    {
    

        [TestMethod]
        public async Task EditGetAsync()
        {

            // Set up a temp database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            // Add a test product to the temp database
            var testProduct = new Product { Id = 1, Name = "Test Product" };
            context.Products.Add(testProduct);
            context.SaveChanges();

            var controller = new ProductsController(context);

            // Get the result of the Edit method
            var result = await controller.Edit(1) as ViewResult;

            // Check if the result is a ViewResult
            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            Xunit.Assert.Equal("Edit", viewResult.ViewName);
            
            // Check if the model passed to the view is a Product
            var model = Xunit.Assert.IsType<Product>(viewResult.Model);
            Xunit.Assert.Equal(testProduct.Id, model.Id);
        }

        [TestMethod]
        public async Task DeleteConfirmed_ValidId_RemovesProductFromDatabase()
        {
            // Arrange
            var validId = 1;
            var testProduct = new Product { Id = validId, Name = "Sample Product", BrandId = 1, CategoryId = 1 };

            // Setup in-memory database with the test product
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Products.Add(testProduct);
                context.SaveChanges();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var controller = new ProductsController(context);
                await controller.DeleteConfirmed(validId);

                // Assert
                Xunit.Assert.Null(await context.Products.FindAsync(validId)); // Check that the product was removed
            }
        }

    }
}
