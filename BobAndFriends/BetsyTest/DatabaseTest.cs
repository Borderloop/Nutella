using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using BorderSource.Common;

namespace BetsyTest
{
    [TestClass]
    public class DatabaseTest
    {
        [TestMethod]
        public void GetArticleNumberFromEan_ShouldReturn1()
        {
            var data = new List<ean>{
                new ean { article_id = 1, ean1 = "0000000000"},
            }.AsQueryable();

            var mockSet = new Mock<DbSet<ean>>();
            mockSet.As<IQueryable<ean>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<ean>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<ean>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<ean>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<BetsyEntities>();
            mockContext.Setup(c => c.ean).Returns(mockSet.Object);

            var service = new BetsyService(mockContext.Object);
            var aid = service.GetArticleNumber("ean", "ean", "0000000000");
            var wrongEan = service.GetArticleNumber("ean", "ean", "1111111111");
            var wrongTable = service.GetArticleNumber("NonExistingTable", "NonExistingColumn", "SomeValue");

            Assert.AreEqual(1, aid);
            Assert.AreEqual(-1, wrongEan);
            Assert.AreEqual(-1, wrongTable);
        }

        [TestMethod]
        public void GetArticleNumberFromSku_ShouldReturn1()
        {
            var data = new List<sku>{
                new sku { article_id = 1, sku1 = "0000000000"},
            }.AsQueryable();

            var mockSet = new Mock<DbSet<sku>>();
            mockSet.As<IQueryable<sku>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<sku>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<sku>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<sku>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<BetsyEntities>();
            mockContext.Setup(c => c.sku).Returns(mockSet.Object);

            var service = new BetsyService(mockContext.Object);
            var aid = service.GetArticleNumber("sku", "sku", "0000000000");
            var wrongSku = service.GetArticleNumber("sku", "sku", "1111111111");

            Assert.AreEqual(1, aid);
            Assert.AreEqual(-1, wrongSku);
        }

        [TestMethod]
        public void GetArticleNumberFromTitle_ShouldReturn1()
        {
            var data = new List<title>{
                new title { article_id = 1, title1 = "Hello"},
            }.AsQueryable();

            var mockSet = new Mock<DbSet<title>>();
            mockSet.As<IQueryable<title>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<title>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<title>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<title>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<BetsyEntities>();
            mockContext.Setup(c => c.title).Returns(mockSet.Object);

            var service = new BetsyService(mockContext.Object);
            var aid = service.GetArticleNumber("title", "title", "Hello");
            var wrongTitle = service.GetArticleNumber("title", "title", "Bye");

            Assert.AreEqual(1, aid);
            Assert.AreEqual(-1, wrongTitle);
        }

        [TestMethod]
        public void CheckIfBrandExists_ShouldReturnTrue()
        {
            var data = new List<article>{
                new article{ brand = "DefinitelyNotApple", id = 1, description = "This thing is anything but Applish", image_loc = "somewhere.jpg" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<article>>();
            mockSet.As<IQueryable<article>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<article>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<article>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<article>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<BetsyEntities>();
            mockContext.Setup(c => c.article).Returns(mockSet.Object);

            var service = new BetsyService(mockContext.Object);
            var checkTrue = service.CheckIfBrandExists("DefinitelyNotApple");
            var checkFalse = service.CheckIfBrandExists("Apple");

            Assert.AreEqual(true, checkTrue);
            Assert.AreEqual(false, checkFalse);
        }

        [TestMethod]
        public void CheckIfUACExists_ShouldReturn1()
        {
            var data = new List<product>{
               new product{ article_id = 1, webshop_url = "www.borderloop.nl", affiliate_unique_id = "UAC"}
           }.AsQueryable();

            var mockSet = new Mock<DbSet<product>>();
            mockSet.As<IQueryable<product>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<product>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<product>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<product>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<BetsyEntities>();
            mockContext.Setup(c => c.product).Returns(mockSet.Object);

            var service = new BetsyService(mockContext.Object);
            var correctArticleId = service.GetAIDFromUAC(new Product { Webshop = "www.borderloop.nl", AffiliateProdID = "UAC" });
            var incorrectArticleId = service.GetAIDFromUAC(new Product { Webshop = "www.borderloop.nl", AffiliateProdID = "NotUAC" });

            Assert.AreEqual(1, correctArticleId);
            Assert.AreEqual(-1, incorrectArticleId);           
        }

        [TestMethod]
        public void SaveMatchTest()
        {
            var articleData = new List<article>{
                new article{
                    id = 1,
                    title = new List<title>{ 
                        new title{
                            title1 = "Samsung Galaxy S5",
                            id = 1,
                            title_synonym = new List<title_synonym>{
                                new title_synonym{
                                    title = "Galaxy S5",
                                    title_id = 1,
                                    occurrences = 5
                                }
                            },
                            country = new country{
                                id = 1,
                                name = "Nederland",
                                extension = "nl"
                            },
                            article_id = 1,
                            country_id = 1
                        }
                    },
                    brand = "Samsung",
                    ean = new List<ean> {
                        new ean {
                            ean1 = "0000000000"
                        },
                        new ean { 
                            ean1 = "0000000001"
                        }
                    },
                    sku = new List<sku> {
                        new sku {
                            sku1 = "1111111111"
                        },
                        new sku {
                            sku1 = "1111111110"
                        }
                    }
                }
            }.AsQueryable();

            var titleData = new List<title>
            {
                new title
                {
                            title1 = "Samsung Galaxy S5",
                            id = 1,
                            title_synonym = new List<title_synonym>{
                                new title_synonym{
                                    title = "Galaxy S5",
                                    title_id = 1,
                                    occurrences = 1,                                   
                                }
                            },
                            country = new country{
                                id = 1,
                                name = "Nederland",
                                extension = "nl"
                            },
                            article_id = 1

                }
            }.AsQueryable();

            var titleSynonymData = new List<title_synonym> 
            {
                new title_synonym
                {
                                    title = "Galaxy S5",
                                    title_id = 1,
                                    occurrences = 1
                }
            }.AsQueryable();

            var skuData = new List<sku>{}.AsQueryable();
            
            var articleMockSet = new Mock<DbSet<article>>();
            articleMockSet.As<IQueryable<article>>().Setup(m => m.Provider).Returns(articleData.Provider);
            articleMockSet.As<IQueryable<article>>().Setup(m => m.Expression).Returns(articleData.Expression);
            articleMockSet.As<IQueryable<article>>().Setup(m => m.ElementType).Returns(articleData.ElementType);
            articleMockSet.As<IQueryable<article>>().Setup(m => m.GetEnumerator()).Returns(articleData.GetEnumerator());

            var titleMockSet = new Mock<DbSet<title>>();
            titleMockSet.As<IQueryable<title>>().Setup(m => m.Provider).Returns(titleData.Provider);
            titleMockSet.As<IQueryable<title>>().Setup(m => m.Expression).Returns(titleData.Expression);
            titleMockSet.As<IQueryable<title>>().Setup(m => m.ElementType).Returns(titleData.ElementType);
            titleMockSet.As<IQueryable<title>>().Setup(m => m.GetEnumerator()).Returns(titleData.GetEnumerator());

            var titleSynMockSet = new Mock<DbSet<title_synonym>>();
            titleSynMockSet.As<IQueryable<title_synonym>>().Setup(m => m.Provider).Returns(titleSynonymData.Provider);
            titleSynMockSet.As<IQueryable<title_synonym>>().Setup(m => m.Expression).Returns(titleSynonymData.Expression);
            titleSynMockSet.As<IQueryable<title_synonym>>().Setup(m => m.ElementType).Returns(titleSynonymData.ElementType);
            titleSynMockSet.As<IQueryable<title_synonym>>().Setup(m => m.GetEnumerator()).Returns(titleSynonymData.GetEnumerator());

            var skuMockSet = new Mock<DbSet<sku>>();
            skuMockSet.As<IQueryable<sku>>().Setup(m => m.Provider).Returns(skuData.Provider);
            skuMockSet.As<IQueryable<sku>>().Setup(m => m.Expression).Returns(skuData.Expression);
            skuMockSet.As<IQueryable<sku>>().Setup(m => m.ElementType).Returns(skuData.ElementType);
            skuMockSet.As<IQueryable<sku>>().Setup(m => m.GetEnumerator()).Returns(skuData.GetEnumerator());

            var mockContext = new Mock<BetsyEntities>();
            mockContext.Setup(c => c.article).Returns(articleMockSet.Object);
            mockContext.Setup(c => c.title).Returns(titleMockSet.Object);
            mockContext.Setup(c => c.title_synonym).Returns(titleSynMockSet.Object);
            mockContext.Setup(c => c.sku).Returns(skuMockSet.Object);


            var service = new BetsyService(mockContext.Object);
            service.SaveMatch(new Product { Title = "Galaxy S5", EAN = "0000000000", SKU = "1010101010" }, 1, 1);

            mockContext.Verify(m => m.SaveChanges(), Times.AtMost(2));
        }  
    }
}
