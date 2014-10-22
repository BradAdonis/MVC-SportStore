using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Domain.Abstract;
using Domain.Entities;
using WebUI.Controllers;
using System.Collections.Generic;
using WebUI.Models;
using WebUI.HtmlHelpers;
using System.Web.Mvc;


namespace UnitTests
{
    [TestClass]
    public class ProductTests
    {
        [TestMethod]
        public void Can_Paginate()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {new Product{ ProductID = 1, Name = "P1" }, new Product { ProductID = 2, Name = "P2" }, new Product { ProductID = 3, Name = "P3" }, new Product { ProductID = 4, Name = "P4" }, new Product { ProductID = 5, Name = "P5" }});

            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            ProductListViewModel result = (ProductListViewModel)controller.List(null,2).Model;
            IList<Product> ps = new List<Product>();

            foreach (Product p in result.Products)
            {
                ps.Add(p);
            }

            Assert.IsTrue(ps.Count == 2);
            Assert.AreEqual(ps[0].Name, "P4");
            Assert.AreEqual(ps[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            HtmlHelper myHelper = null;
            PagingInfo p = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemPerPage = 10,
            };


            Func<int, string> pageUrldelegate = i => "Page" + 1;
            MvcHtmlString result = myHelper.PageLinks(p, pageUrldelegate);

            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                            + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                            + @"<a class=""btn btn-default"" href=""Page3"">3</a>",
                result.ToString());
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count() 
        {
            // Arrange
            // - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
            new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
            new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
            new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
            new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
            new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
            });
            // Arrange - create a controller and make the page size 3 items
            ProductController target = new ProductController(mock.Object);
            target.PageSize = 3;
            // Action - test the product counts for different categories
            int res1 = ((ProductListViewModel)target
            .List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductListViewModel)target
            .List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductListViewModel)target
            .List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((ProductListViewModel)target
            .List(null).Model).PagingInfo.TotalItems;

            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }
    }
}
