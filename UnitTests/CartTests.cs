using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Entities;
using System.Collections.Generic;
using Moq;
using Domain.Abstract;
using WebUI.Controllers;
using System.Web.Mvc;
using WebUI.Models;

namespace UnitTests
{
    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void can_Add_New_Lines()
        {
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            Cart target = new Cart();
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            List<CartLine> results = (List<CartLine>)target.Lines;

            Assert.AreEqual(results.Count, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void can_add_Quantity_for_Existing_lines()
        {
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };

            Cart target = new Cart();
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            List<CartLine> results = (List<CartLine>)target.Lines;

            Assert.AreEqual(results.Count, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void can_remove_lines()
        {
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            Product p3 = new Product { ProductID = 3, Name = "P3" };

            Cart target = new Cart();
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);

            target.RemoveLine(p2);

            //Assert.AreEqual(target.Lines.Where(c => c.Product == p2).count(), 0);
            //Assert.AreEqual(target.Lines.Count(), 2);
        }

        [TestMethod]
        public void can_add_to_cart()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>(){
                new Product{ProductID = 1, Name = "P1", Category="Apples"},
            });

            Cart cart = new Cart();

            CartController target = new CartController(mock.Object);
            target.AddToCart(cart, 1, null);

            List<Product> p = (List<Product>)cart.Lines;

            Assert.AreEqual(p.Count, 1);
            Assert.AreEqual(p[0].ProductID, 1);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product> { new Product { ProductID = 1, Name = "P1", Category = "Apples" } });

            Cart cart = new Cart();

            CartController target = new CartController(mock.Object);

            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            Cart cart = new Cart();

            CartController target = new CartController(null);

            CartIndexViewModel result = (CartIndexViewModel)target.Index(cart, "myUrl").ViewData.Model;

            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");

        }
    }
}
