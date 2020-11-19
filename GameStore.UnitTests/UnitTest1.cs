using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using GameStore.Domain.Abstract;
using GameStore.Domain.Entities;
using GameStore.WebUI.Controllers;
using GameStore.WebUI.HtmlHelpers;
using GameStore.WebUI.Models;

namespace GameStore.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            // Organization (arrange)
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game { GameId = 1, Name = "Game1"},
                new Game { GameId = 2, Name = "Game2"},
                new Game { GameId = 3, Name = "Game3"},
                new Game { GameId = 4, Name = "Game4"}, 
                new Game { GameId = 5, Name = "Game5"}
            });
            GameController controller = new GameController(mock.Object);
            controller.pageSize = 3;

            // act
            GamesListViewModel result = (GamesListViewModel)controller.List(null, 2).Model;

            // Statement
            List<Game> games = result.Games.ToList();
            Assert.IsTrue(games.Count == 2);
            Assert.AreEqual(games[0].Name, "Game4");
            Assert.AreEqual(games[1].Name, "Game5");
        }


        [TestMethod]
        public void Can_Generate_Page_Links()
        {

            //  Organization - HTML helper definition - this is
            //  required to apply the extension method
            HtmlHelper myHelper = null;

            // Organization - creating a PagingInfo object
            PagingInfo pagingInfo = new PagingInfo
            {
                CurrentPage = 2,
                TotalItems = 28,
                ItemsPerPage = 10
            };

            // Organization - configuring a delegate using a lambda expression
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            // Act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            // Statement
            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                            + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                            + @"<a class=""btn btn-default"" href=""Page3"">3</a>",
                result.ToString());
        }


        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            // arrange
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game { GameId = 1, Name = "Game1"},
                new Game { GameId = 2, Name = "Game2"},
                new Game { GameId = 3, Name = "Game3"},
                new Game { GameId = 4, Name = "Game4"},
                new Game { GameId = 5, Name = "Game5"}
            });
            GameController controller = new GameController(mock.Object);
            controller.pageSize = 3;

            // Act
            GamesListViewModel result
                = (GamesListViewModel)controller.List(null, 2).Model;

            // Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPage, 2);
            Assert.AreEqual(pageInfo.ItemsPerPage, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Games()
        {
            // arrange
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game { GameId = 1, Name = "Game1", Category="Cat1"},
                new Game { GameId = 2, Name = "Game2", Category="Cat2"},
                new Game { GameId = 3, Name = "Game3", Category="Cat1"},
                new Game { GameId = 4, Name = "Game4", Category="Cat2"},
                new Game { GameId = 5, Name = "Game5", Category="Cat3"}
            });
            GameController controller = new GameController(mock.Object);
            controller.pageSize = 3;

            // Action
            List<Game> result = ((GamesListViewModel)controller.List("Cat2", 1).Model)
                .Games.ToList();

            // Assert
            Assert.AreEqual(result.Count(), 2);
            Assert.IsTrue(result[0].Name == "Game2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "Game4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            // Organization - creating a simulated repository
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game> {
                new Game { GameId = 1, Name = "Game1", Category="Simulator"},
                new Game { GameId = 2, Name = "Game2", Category="Simulator"},
                new Game { GameId = 3, Name = "Game3", Category="Shooter"},
                new Game { GameId = 4, Name = "Game4", Category="RPG"},
            });

            // Organization - creating a controller
            NavController target = new NavController(mock.Object);

            // Action - getting a set of categories
            List<string> results = ((IEnumerable<string>)target.Menu().Model).ToList();

            // Statement
            Assert.AreEqual(results.Count(), 3);
            Assert.AreEqual(results[0], "RPG");
            Assert.AreEqual(results[1], "Simulator");
            Assert.AreEqual(results[2], "Shooter");
        }


        [TestMethod]
        public void Indicates_Selected_Category()
        {
            // Organization - creating a simulated repository
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new Game[] {
                new Game { GameId = 1, Name = "Game1", Category="Simulator"},
                new Game { GameId = 2, Name = "Game2", Category="Shooter"}
            });

            // Organization - creating a controller
            NavController target = new NavController(mock.Object);

            // Organization - defining the selected category
            string categoryToSelect = "Shooter";

            // Act
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            // Statement
            Assert.AreEqual(categoryToSelect, result);
        }



        [TestMethod]
        public void Generate_Category_Specific_Game_Count()
        {
            /// arrange
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game { GameId = 1, Name = "Game1", Category="Cat1"},
                new Game { GameId = 2, Name = "Game2", Category="Cat2"},
                new Game { GameId = 3, Name = "Game3", Category="Cat1"},
                new Game { GameId = 4, Name = "Game4", Category="Cat2"},
                new Game { GameId = 5, Name = "Game5", Category="Cat3"}
            });
            GameController controller = new GameController(mock.Object);
            controller.pageSize = 3;

            // Action - testing product counters for various categories
            int res1 = ((GamesListViewModel)controller.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((GamesListViewModel)controller.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((GamesListViewModel)controller.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((GamesListViewModel)controller.List(null).Model).PagingInfo.TotalItems;

            // Statement
            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }


        [TestClass]
        public class CartTests
        {
            [TestMethod]
            public void Can_Add_New_Lines()
            {
                // Organization - creating multiple test games
                Game game1 = new Game { GameId = 1, Name = "Game1" };
                Game game2 = new Game { GameId = 2, Name = "Game2" };

                // Organization - creating a shopping cart
                Cart cart = new Cart();

                // Act
                cart.AddItem(game1, 1);
                cart.AddItem(game2, 1);
                List<CartLine> results = cart.Lines.ToList();

                // Statement
                Assert.AreEqual(results.Count(), 2);
                Assert.AreEqual(results[0].Game, game1);
                Assert.AreEqual(results[1].Game, game2);
            }
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            Game game1 = new Game { GameId = 1, Name = "Game1" };
            Game game2 = new Game { GameId = 2, Name = "Game2" };

            Cart cart = new Cart();

            cart.AddItem(game1, 1);
            cart.AddItem(game2, 1);
            cart.AddItem(game1, 5);
            List<CartLine> results = cart.Lines.OrderBy(c => c.Game.GameId).ToList();

            Assert.AreEqual(results.Count(), 2);
            Assert.AreEqual(results[0].Quantity, 6);    // 6 items added to cart
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            Game game1 = new Game { GameId = 1, Name = "Game1" };
            Game game2 = new Game { GameId = 2, Name = "Game2" };
            Game game3 = new Game { GameId = 3, Name = "Game3" };

            Cart cart = new Cart();

            cart.AddItem(game1, 1);
            cart.AddItem(game2, 4);
            cart.AddItem(game3, 2);
            cart.AddItem(game2, 1);

            cart.RemoveLine(game2);

            Assert.AreEqual(cart.Lines.Where(c => c.Game == game2).Count(), 0);
            Assert.AreEqual(cart.Lines.Count(), 2);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            Game game1 = new Game { GameId = 1, Name = "Game1", Price = 100 };
            Game game2 = new Game { GameId = 2, Name = "Game2", Price = 55 };

            Cart cart = new Cart();

            cart.AddItem(game1, 1);
            cart.AddItem(game2, 1);
            cart.AddItem(game1, 5);
            decimal result = cart.ComputeTotalValue();

            Assert.AreEqual(result, 655);
        }


        [TestMethod]
        public void Can_Clear_Contents()
        {
            Game game1 = new Game { GameId = 1, Name = "Game1", Price = 100 };
            Game game2 = new Game { GameId = 2, Name = "Game2", Price = 55 };

            Cart cart = new Cart();

            cart.AddItem(game1, 1);
            cart.AddItem(game2, 1);
            cart.AddItem(game1, 5);
            cart.Clear();

            Assert.AreEqual(cart.Lines.Count(), 0);
        }


        /// <summary>
        /// Checking the addition to the cart
        /// </summary>
        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            // Organization - Create a Simulated Order Processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            Cart cart = new Cart();

            
            ShippingDetails shippingDetails = new ShippingDetails();

            CartController controller = new CartController(null, mock.Object);

            // Act
            ViewResult result = controller.Checkout(cart, shippingDetails);

            // Assertion - checking that the order has not been submitted to the processor 
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()),
                Times.Never());

            // Assertion - checking that a method returned a standard view 
            Assert.AreEqual("", result.ViewName);

            // Assertion - checking that the wrong model was passed to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }
        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            Cart cart = new Cart();
            cart.AddItem(new Game(), 1);

            CartController controller = new CartController(null, mock.Object);

            controller.ModelState.AddModelError("error", "error");

            ViewResult result = controller.Checkout(cart, new ShippingDetails());

            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()),
                Times.Never());

            Assert.AreEqual("", result.ViewName);

            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }
        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            Cart cart = new Cart();
            cart.AddItem(new Game(), 1);

            CartController controller = new CartController(null, mock.Object);

            ViewResult result = controller.Checkout(cart, new ShippingDetails());

            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()),
                Times.Once());

            Assert.AreEqual("Completed", result.ViewName);

            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }


    }
}