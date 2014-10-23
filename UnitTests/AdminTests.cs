using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Domain.Abstract;
using Domain.Entities;
using System.Collections.Generic;
using WebUI.Controllers;
using System.Web.Mvc;

namespace UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void IndexContains_All_Products()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>() { new Product { ProductID = 1, Name = "P1" }, new Product { ProductID = 2, Name = "P2" }, new Product { ProductID = 3, Name = "P3" } });

            AdminController target = new AdminController(mock.Object);

            IList<Product> result = ((IList<Product>)target.Index().ViewData.Model);

            Assert.AreEqual(result.Count, 3);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }

        [TestMethod]
        public void Can_Edit_Product()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>() { new Product { ProductID = 1, Name = "P1" }, new Product { ProductID = 2, Name = "P2" }, new Product { ProductID = 3, Name = "P3" } });

            AdminController target = new AdminController(mock.Object);

            Product p1 = target.Edit(1).ViewData.Model as Product;
            Product p2 = target.Edit(2).ViewData.Model as Product;
            Product p3 = target.Edit(3).ViewData.Model as Product;

            Assert.AreEqual(p1.ProductID, 1);
            Assert.AreEqual(p2.ProductID, 2);
            Assert.AreEqual(p3.ProductID, 3);
        }

        [TestMethod]
        public void Cannot_Edit_NonExistant(){
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>() { new Product { ProductID = 1, Name = "P1" }, new Product { ProductID = 2, Name = "P2" }, new Product { ProductID = 3, Name = "P3" } });

            AdminController target = new AdminController(mock.Object);

            Product result = (Product)target.Edit(4).ViewData.Model;

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            AdminController target = new AdminController(mock.Object);
            Product product = new Product { Name = "Test" };

            ActionResult result = target.Edit(product.ProductID);

            mock.Verify(m => m.SaveProduct(product));

            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            AdminController target = new AdminController(mock.Object);

            Product product = new Product { Name = "Test" };
            target.ModelState.AddModelError("error", "error");
            ActionResult result = target.Edit(product.ProductID);

            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());

            Assert.IsInstanceOfType(result, typeof(ViewResult));

        }

        [TestMethod]
        public void Can_Delete_Product()
        {
            Product prod = new Product { ProductID = 2, Name = "Test" };

            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product> { new Product{ProductID = 1, Name = "P1"}, new Product{ProductID = 2, Name = "P3"}, new Product{ProductID = 3, Name = "P3"}});

            AdminController target = new AdminController(mock.Object);

            target.Delete(prod.ProductID);

            mock.Verify(m => m.DeleteProduct(prod.ProductID));
        }
    }
}
