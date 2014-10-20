using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Domain.Abstract;
using Domain.Entities;
using WebUI.Controllers;
using System.Collections.Generic;


namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {new Product{ ProductID = 1, Name = "P1" }, new Product { ProductID = 2, Name = "P2" }, new Product { ProductID = 3, Name = "P3" }, new Product { ProductID = 4, Name = "P4" }, new Product { ProductID = 5, Name = "P5" }});

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            IEnumerable<Product> result = (IEnumerable<Product>)controller.List(2).Model;
            IList<Product> ps = new List<Product>();

            foreach (Product p in result)
            {
                ps.Add(p);
            }

            Assert.IsTrue(ps.Count == 2);
            Assert.AreEqual(ps[0].Name, "P4");
            Assert.AreEqual(ps[1].Name, "P5");
        }
    }
}
