using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

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
                new ean { article_id = 2, ean1 = "1111111111"}
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

            Assert.AreEqual(1, aid);
        }
    }
}
