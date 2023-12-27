using System;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApi.Controllers;
using WebApi.Models;
using WebApi.Repository;
using Xunit;

namespace TestWebApi.ControllerTest
{
    public class ProductControllerTest
    {
        [Fact]
        public void GetProductById_ReturnsProductAnd200StatusCode()
        {
            // Arrange
            var productId = 1;
            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(repo => repo.GetProductById(productId)).Returns(new Product { ProductId = productId });
            var controller = new ProductController(mockRepository.Object);

            // Act
            var result = controller.Get(productId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<Product>(result.Value);
        }

        [Fact]
        public void AddProduct_ReturnsCreatedResponseAnd201StatusCode()
        {
            // Arrange
            var product = new Product { ProductId = 1 };
            var mockRepository = new Mock<IProductRepository>();
            var controller = new ProductController(mockRepository.Object);

            // Act
            var result = controller.Post(product) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal("Get", result.ActionName);
            Assert.Equal(1, result.RouteValues["id"]);
            Assert.IsType<Product>(result.Value);
        }

        [Fact]
        public void UpdateProduct_ReturnsNoContentAnd200StatusCode()
        {
            // Arrange
            var productId = 1;
            var product = new Product { ProductId = productId };
            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(repo => repo.UpdateProduct(product));
            var controller = new ProductController(mockRepository.Object);

            // Act
            var result = controller.Put(productId, product) as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.True(result.StatusCode == 200 || result.StatusCode == 204);
        }

        [Fact]
        public void DeleteProduct_ReturnsNoContentAnd200StatusCode()
        {
            // Arrange
            var productId = 1;
            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(repo => repo.DeleteProduct(productId));
            var controller = new ProductController(mockRepository.Object);

            // Act
            var result = controller.Delete(productId) as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);
        }

        [Fact]
        public void GetProductById_ReturnsNotFoundAnd404StatusCode()
        {
            // Arrange
            var productId = 1;
            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(repo => repo.GetProductById(productId)).Returns((Product)null);
            var controller = new ProductController(mockRepository.Object);

            // Act
            var result = controller.Get(productId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public void AddProduct_ReturnsBadRequestWhenFailedToAdd()
        {
            // Arrange
            var product = new Product { ProductId = 1 };
            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(repo => repo.AddProduct(product)).Throws(new Exception("Failed to add product."));
            var controller = new ProductController(mockRepository.Object);

            // Act
            var result = controller.Post(product) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Failed to add product. Failed to add product.", result.Value); // Update this assertion based on the actual exception message
        }

        [Fact]
        public void UpdateProduct_ReturnsNotFoundAnd404StatusCodeWhenProductNotFound()
        {
            // Arrange
            var productId = 1;
            var product = new Product { ProductId = productId };
            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(repo => repo.UpdateProduct(product)).Throws(new KeyNotFoundException());
            var controller = new ProductController(mockRepository.Object);

            // Act
            var result = controller.Put(productId, product) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public void DeleteProduct_ReturnsNotFoundAnd404StatusCodeWhenProductNotFound()
        {
            // Arrange
            var productId = 1;
            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(repo => repo.DeleteProduct(productId)).Throws(new KeyNotFoundException());
            var controller = new ProductController(mockRepository.Object);

            // Act
            var result = controller.Delete(productId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }
    }
}
