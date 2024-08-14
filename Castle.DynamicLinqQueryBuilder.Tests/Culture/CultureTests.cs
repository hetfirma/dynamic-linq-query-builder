using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicLinqQueryBuilder.Tests.Rules;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Castle.DynamicLinqQueryBuilder.Tests.Culture
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class CultureTests
    {
        IQueryable<Rules.Tests.ExpressionTreeBuilderTestClass> StartingQuery;
        IQueryable<Rules.Tests.ExpressionTreeBuilderTestClass> StartingDateQuery;

        [SetUp]
        public void Setup()
        {
            StartingQuery = Rules.Tests.GetExpressionTreeData().AsQueryable();
            StartingDateQuery = Rules.Tests.GetDateExpressionTreeData().AsQueryable();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("es-ES");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-ES");
        }
        [Test]
        public void TestBetween()
        {
            //expect 3 entries to match for a double field
            var statValueFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "between",
                        Type = "double",
                        Value = new[] { "1,0", "1,12" }
                    }
                }
            };
            var statValueFilterList = StartingQuery.BuildQuery(statValueFilter, new BuildExpressionOptions()
            {
                CultureInfo = new CultureInfo("es-ES")
            }).ToList();
            Assert.That(statValueFilterList, Is.Not.Null);
            Assert.That(statValueFilterList.Count, Is.EqualTo(3));
        }

        [Test]
        public void TestDateInCulture()
        {
            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "in",
                        Type = "datetime",
                        Value = new[] { DateTime.UtcNow.Date.ToString(), DateTime.UtcNow.Date.ToString() }
                    }
                }
            };
            var lastModifiedFilterList = StartingQuery.BuildQuery(lastModifiedFilter, new BuildExpressionOptions()
            {
                CultureInfo = new CultureInfo("es-ES")
            }).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 4);
        }
    }
}
