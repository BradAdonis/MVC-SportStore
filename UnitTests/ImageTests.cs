using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Entities;
using Moq;
using Domain.Abstract;
using System.Collections.Generic;
using WebUI.Controllers;
using System.Web.Mvc;

namespace UnitTests
{
    [TestClass]
    public class ImageTests
    {
        [TestMethod]
        public void Can_Retrieve_Image_Data()
        {
            Product prod = new Product()
            {
                ProductID = 2,
                Name = "Product2",
                ImageData = new byte[] { },
                ImageMimeType = "image/png"
            };

            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>{ new Product{ProductID = 1, Name = "P1"}, prod, new Product{ProductID = 3, Name = "P3"}});

            ProductController target = new ProductController(mock.Object);
            ActionResult result = target.GetImage(2);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            Assert.AreEqual(prod.ImageMimeType, ((FileResult)result).ContentType);
        }

        [TestMethod]
        public void Cannot_RetrieveImage()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product> { new Product { ProductID = 1, Name = "P1" },new Product { ProductID = 2, Name = "P2" } });

            ProductController target = new ProductController(mock.Object);
            ActionResult result = target.GetImage(2);

            Assert.IsNull(result);
        }
    }
}
