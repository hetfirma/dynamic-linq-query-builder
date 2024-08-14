using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using Castle.DynamicLinqQueryBuilder.SystemTextJson;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using ExceptionAssert = Castle.DynamicLinqQueryBuilder.Tests.Helpers.ExceptionAssert;

namespace Castle.DynamicLinqQueryBuilder.Tests.Rules
{
    [ExcludeFromCodeCoverage]
    [TestFixture]

    class SystemTextJsonTests
    {
        IQueryable<Rules.Tests.ExpressionTreeBuilderTestClass> StartingQuery;
        IQueryable<Rules.Tests.ExpressionTreeBuilderTestClass> StartingDateQuery;

        [SetUp]
        public void Setup()
        {
            StartingQuery = Rules.Tests.GetExpressionTreeData().AsQueryable();
            StartingDateQuery = Rules.Tests.GetDateExpressionTreeData().AsQueryable();
        }

        #region Wrapper
        /// <summary>
        /// Some libraries, such as Newtonsoft.Json, will deserialize the elements of an array (that should be placed in the <see cref="IFilterRule.Value"/>) into a wrapper object
        /// </summary>
        private class Wrapper
        {
            public object Value { get; }

            public Wrapper(object value)
            {
                Value = value;
            }

            public override string ToString() => Value?.ToString();
        }
        #endregion

        #region JsonSerializer
        private SystemTextJsonFilterRule passThroughSerializer(SystemTextJsonFilterRule rule)
        {
            return JsonSerializer.Deserialize<SystemTextJsonFilterRule>(JsonSerializer.Serialize(rule));
        }

        [Test]
        public void SystemTextJsonIntegerHandling()
        {
            QueryBuilder.ParseDatesAsUtc = true;
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = JsonSerializer.SerializeToElement(new[] { 1, 2 })
                    }
                }
            };

            var queryable = StartingDateQuery.BuildQuery<Rules.Tests.ExpressionTreeBuilderTestClass>(contentIdFilter);
            var contentIdFilteredList = queryable.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 1);

            var contentIdFilter2 = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = JsonSerializer.SerializeToElement(new[] { "1", "2" })
                    }
                }
            };

            var queryable2 = StartingDateQuery.BuildQuery<Rules.Tests.ExpressionTreeBuilderTestClass>(contentIdFilter2);
            var contentIdFilteredList2 = queryable2.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList2 != null);
            ClassicAssert.IsTrue(contentIdFilteredList2.Count == 1);


            QueryBuilder.ParseDatesAsUtc = false;
        }

        [Test]
        public void SystemTextJsonDoubleHandling()
        {
            QueryBuilder.ParseDatesAsUtc = true;
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "DoubleList",
                        Id = "DoubleList",
                        Input = "NA",
                        Operator = "in",
                        Type = "double",
                        Value = JsonSerializer.SerializeToElement(new[] { 1.84 })
                    }
                }
            };

            var queryable = StartingQuery.BuildQuery<Rules.Tests.ExpressionTreeBuilderTestClass>(contentIdFilter);
            var contentIdFilteredList = queryable.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 3);

            var contentIdFilter2 = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "DoubleList",
                        Id = "DoubleList",
                        Input = "NA",
                        Operator = "in",
                        Type = "double",
                        Value = JsonSerializer.SerializeToElement(new[] { "1.84" })
                    }
                }
            };

            var queryable2 = StartingQuery.BuildQuery<Rules.Tests.ExpressionTreeBuilderTestClass>(contentIdFilter2);
            var contentIdFilteredList2 = queryable2.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList2 != null);
            ClassicAssert.IsTrue(contentIdFilteredList2.Count == 3);


            QueryBuilder.ParseDatesAsUtc = false;
        }

        [Test]
        public void SystemTextJsonStringHandling()
        {
            QueryBuilder.ParseDatesAsUtc = true;
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeName",
                        Id = "ContentTypeName",
                        Input = "NA",
                        Operator = "in",
                        Type = "string",
                        Value = JsonSerializer.SerializeToElement(new[] { "Multiple-Choice" })
                    }
                }
            };

            var queryable = StartingQuery.BuildQuery<Rules.Tests.ExpressionTreeBuilderTestClass>(contentIdFilter);
            var contentIdFilteredList = queryable.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 2);

            var contentIdFilter2 = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeName",
                        Id = "ContentTypeName",
                        Input = "NA",
                        Operator = "in",
                        Type = "string",
                        Value = JsonSerializer.SerializeToElement("Multiple-Choice")
                    }
                }
            };

            var queryable2 = StartingQuery.BuildQuery<Rules.Tests.ExpressionTreeBuilderTestClass>(contentIdFilter2);
            var contentIdFilteredList2 = queryable2.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList2 != null);
            ClassicAssert.IsTrue(contentIdFilteredList2.Count == 2);

            QueryBuilder.ParseDatesAsUtc = false;
        }

        [Test]
        public void SystemTextJsonBoolHandling()
        {
            QueryBuilder.ParseDatesAsUtc = true;
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "IsPossiblyNotSetBool",
                        Id = "IsPossiblyNotSetBool",
                        Input = "NA",
                        Operator = "in",
                        Type = "boolean",
                        Value = JsonSerializer.SerializeToElement(new[] { false })
                    }
                }
            };

            var queryable = StartingQuery.BuildQuery<Rules.Tests.ExpressionTreeBuilderTestClass>(contentIdFilter);
            var contentIdFilteredList = queryable.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 1);

            var contentIdFilter2 = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "IsPossiblyNotSetBool",
                        Id = "IsPossiblyNotSetBool",
                        Input = "NA",
                        Operator = "in",
                        Type = "boolean",
                        Value = JsonSerializer.SerializeToElement("true")
                    }
                }
            };

            var queryable2 = StartingQuery.BuildQuery<Rules.Tests.ExpressionTreeBuilderTestClass>(contentIdFilter2);
            var contentIdFilteredList2 = queryable2.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList2 != null);
            ClassicAssert.IsTrue(contentIdFilteredList2.Count == 2);

            QueryBuilder.ParseDatesAsUtc = false;
        }

        [Test]
        public void SystemTextJsonGuidHandling()
        {
            QueryBuilder.ParseDatesAsUtc = true;
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeGuid",
                        Id = "ContentTypeGuid",
                        Input = "NA",
                        Operator = "in",
                        Type = "guid",
                        Value = JsonSerializer.SerializeToElement(new[] { StartingQuery.Last().ContentTypeGuid })
                    }
                }
            };

            var queryable = StartingQuery.BuildQuery<Rules.Tests.ExpressionTreeBuilderTestClass>(contentIdFilter);
            var contentIdFilteredList = queryable.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 1);


            QueryBuilder.ParseDatesAsUtc = false;
        }

        [Test]
        public void SystemTextJsonErrorHandling()
        {
            QueryBuilder.ParseDatesAsUtc = true;
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeGuid",
                        Id = "ContentTypeGuid",
                        Input = "NA",
                        Operator = "in",
                        Type = "non-type",
                        Value = JsonSerializer.SerializeToElement(new[] { StartingQuery.Last().ContentTypeGuid })
                    }
                }
            };

            ExceptionAssert.Throws<Exception>(() =>
            {
                var shouldThrow = contentIdFilter.Rules.First().Value;
            });

            var contentIdFilter2 = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeGuid",
                        Id = "ContentTypeGuid",
                        Input = "NA",
                        Operator = "in",
                        Type = "guid",
                        Value = JsonSerializer.SerializeToElement(new[] { "totally invalid guid" })
                    }
                }
            };

            ExceptionAssert.Throws<InvalidCastException>(() =>
            {
                var shouldThrow = contentIdFilter2.Rules.First().Value;
            });

            QueryBuilder.ParseDatesAsUtc = false;
        }

        [Test]
        public void SystemTextJsonDateHandling()
        {
            var options = new BuildExpressionOptions() { CultureInfo = CultureInfo.CurrentCulture };

            QueryBuilder.ParseDatesAsUtc = true;
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "in",
                        Type = "date",
                        Value = JsonSerializer.SerializeToElement(new[] { DateTime.Parse("2/23/2016", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal) })
                    }
                }
            };

            var queryable = StartingDateQuery.BuildQuery(contentIdFilter, options);
            var contentIdFilteredList = queryable.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 1);

            var contentIdFilter2 = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "in",
                        Type = "date",
                        Value = JsonSerializer.SerializeToElement("2/23/2016")
                    }
                }
            };

            var queryable2 = StartingDateQuery.BuildQuery(contentIdFilter2, options);
            var contentIdFilteredList2 = queryable2.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList2 != null);
            ClassicAssert.IsTrue(contentIdFilteredList2.Count == 1);

            var contentIdFilter3 = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "in",
                        Type = "date",
                        Value = JsonSerializer.SerializeToElement(new[] { DateTime.Parse("2/23/2016", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal) })
                    }
                }
            };

            var queryable3 = StartingDateQuery.BuildQuery(contentIdFilter3, options);
            var contentIdFilteredList3 = queryable3.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList3 != null);
            ClassicAssert.IsTrue(contentIdFilteredList3.Count == 1);

            var contentIdFilter4 = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "in",
                        Type = "date",
                        Value = JsonSerializer.SerializeToElement("2/23/2016")
                    }
                }
            };

            var queryable4 = StartingDateQuery.BuildQuery(contentIdFilter4, options);
            var contentIdFilteredList4 = queryable4.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList4 != null);
            ClassicAssert.IsTrue(contentIdFilteredList4.Count == 1);

            QueryBuilder.ParseDatesAsUtc = false;
        }

        #endregion

        #region Expression Tree Builder
        [Test]
        public void DateHandling()
        {
            QueryBuilder.ParseDatesAsUtc = true;
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "equal",
                        Type = "date",
                        Value = "2/23/2016"
                    }
                }
            };
            var queryable = StartingDateQuery.BuildQuery<Rules.Tests.ExpressionTreeBuilderTestClass>(contentIdFilter);
            var contentIdFilteredList = queryable.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 1);

            contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "equal",
                        Type = "date",
                        Value = DateTime.UtcNow.ToString("M/dd/yyyy")
                    }
                }
            };
            queryable = StartingDateQuery.BuildQuery(contentIdFilter);
            contentIdFilteredList = queryable.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 1);

            contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "NullableLastModified",
                        Id = "NullableLastModified",
                        Input = "NA",
                        Operator = "equal",
                        Type = "date",
                        Value = "2/23/2016"
                    }
                }
            };
            queryable = StartingDateQuery.BuildQuery(contentIdFilter);
            contentIdFilteredList = queryable.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 0);

            contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "equal",
                        Type = "date",
                        Value = ""
                    }
                }
            };
            ExceptionAssert.Throws<Exception>(() =>
            {
                var contentIdFilteredListNull1 = StartingQuery.BuildQuery(contentIdFilter).ToList();
            });


            contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "DateList",
                        Id = "DateList",
                        Input = "NA",
                        Operator = "in",
                        Type = "date",
                        Value = "2/23/2016"
                    }
                }
            };
            queryable = StartingDateQuery.BuildQuery(contentIdFilter);
            var contentIdFilteredList2 = queryable.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList2 != null);
            ClassicAssert.IsTrue(contentIdFilteredList2.Count == 1);

            contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "DateList",
                        Id = "DateList",
                        Input = "NA",
                        Operator = "between",
                        Type = "date",
                        Value = ""
                    }
                }
            };
            ExceptionAssert.Throws<Exception>(() =>
            {
                var contentIdFilteredListNull2 = StartingDateQuery.BuildQuery(contentIdFilter).ToList();

            });

            QueryBuilder.ParseDatesAsUtc = false;
        }

        [Test]
        public void InClause()
        {
            //expect two entries to match for an integer comparison
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = new[] { 1, 2 }
                    }
                }
            };
            var contentIdFilteredList = StartingQuery.BuildQuery(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 3);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => (new List<int>() { 1, 2 }).Contains(p.ContentTypeId)));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(contentIdFilter).ToList();

            });

            //single value test
            contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = new[] { 1 }
                    }
                }
            };
            contentIdFilteredList = StartingQuery.BuildQuery(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => (new List<int>() { 1 }).Contains(p.ContentTypeId)));

            //expect two entries to match for an integer comparison
            var nullableContentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = new[] { 1, 2 }
                    }
                }
            };
            var nullableContentIdFilteredList =
                StartingQuery.BuildQuery(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(
                nullableContentIdFilteredList.All(p => (new List<int>() { 1, 2 }).Contains(p.NullableContentTypeId.Value)));




            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "in",
                        Type = "string",
                        Value = new[] { "there is something interesting about this text", "there is something interesting about this text2" }
                    }
                }
            };
            var longerTextToFilterList = StartingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 3);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p == "there is something interesting about this text"));

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilterCaps = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "in",
                        Type = "string",
                        Value = new[] { "THERE is something interesting about this text", "there is something interesting about this text2" }
                    }
                }
            };
            var longerTextToFilterListCaps = StartingQuery.BuildQuery(longerTextToFilterFilterCaps).ToList();
            ClassicAssert.IsTrue(longerTextToFilterListCaps != null);
            ClassicAssert.IsTrue(longerTextToFilterListCaps.Count == 3);
            ClassicAssert.IsTrue(
                longerTextToFilterListCaps.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p == "there is something interesting about this text"));


            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "in",
                        Type = "datetime",
                        Value = new[] { DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture), DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture) }
                    }
                }
            };
            var lastModifiedFilterList = StartingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 4);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => p == DateTime.UtcNow.Date));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "in",
                        Type = "datetime",
                        Value = new[] { DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture), DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture) }
                    }
                }
            };
            var nullableLastModifiedFilterList = StartingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => p == DateTime.UtcNow.Date));


            //expect 2 entries to match for a double field
            var statValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "in",
                        Type = "double",
                        Value = new[] { 1.11d, 1.12d }
                    }
                }
            };
            var statValueFilterList = StartingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 3);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (new List<double>() { 1.11, 1.12 }).Contains(p)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable double field
            var nullableStatValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "in",
                        Type = "double",
                        Value = new[] {1.112D, 1.113D }
                    }
                }
            };
            var nullableStatFilterList = StartingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p == 1.112));

            //expect 2 entries to match for a List<DateTime> field
            var dateListFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "DateList",
                        Id = "DateList",
                        Input = "NA",
                        Operator = "in",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var dateListFilterList = StartingQuery.ToList().BuildQuery(dateListFilter).ToList();
            ClassicAssert.IsTrue(dateListFilterList != null);
            ClassicAssert.IsTrue(dateListFilterList.Count == 3);
            ClassicAssert.IsTrue(dateListFilterList.All(p => p.DateList.Contains(DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                dateListFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(dateListFilter).ToList();

            });

            //expect 2 entries to match for a List<string> field
            var strListFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "StrList",
                        Id = "StrList",
                        Input = "NA",
                        Operator = "in",
                        Type = "string",
                        Value = "Str2"
                    }
                }
            };
            var strListFilterList = StartingQuery.AsEnumerable().BuildQuery(strListFilter).ToList();
            ClassicAssert.IsTrue(strListFilterList != null);
            ClassicAssert.IsTrue(strListFilterList.Count == 3);
            ClassicAssert.IsTrue(strListFilterList.All(p => p.StrList.Contains("Str2")));

            //expect 2 entries to match for a List<int> field
            var intListFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "IntList",
                        Id = "IntList",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = new[] {"1", "3"}
                    }
                }
            };
            var intListFilterList = StartingQuery.BuildQuery(intListFilter).ToList();
            ClassicAssert.IsTrue(intListFilterList != null);
            ClassicAssert.IsTrue(intListFilterList.Count == 3);
            ClassicAssert.IsTrue(intListFilterList.All(p => p.IntList.Contains(1) || p.IntList.Contains(3)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                intListFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(intListFilter).ToList();

            });

            //expect 2 entries to match for a nullable nullable int field
            var nullableIntListFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "NullableIntList",
                        Id = "NullableIntList",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = 5
                    }
                }
            };
            var nullableIntListList = StartingQuery.BuildQuery(nullableIntListFilter).ToList();
            ClassicAssert.IsTrue(nullableIntListList != null);
            ClassicAssert.IsTrue(nullableIntListList.Count == 3);
            ClassicAssert.IsTrue(nullableIntListList.All(p => p.NullableIntList.Contains(5)));

            var multipleWithBlankRule = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "StrList",
                        Id = "StrList",
                        Input = "NA",
                        Operator = "in",
                        Type = "string",
                        Value = new[] {"", "Str2" }
                    }
                }
            };
            var multipleWithBlankList = StartingQuery.BuildQuery(multipleWithBlankRule).ToList();
            ClassicAssert.IsTrue(multipleWithBlankList != null);
            ClassicAssert.IsTrue(multipleWithBlankList.Count == 4);
            ClassicAssert.IsTrue(multipleWithBlankList.All(p => p.StrList.Contains("") || p.StrList.Contains("Str2")));

            //expect 2 entries to match for a nullable double field
            var nullableWrappedStatValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "in",
                        Type = "double",
                        Value = new[] {new Wrapper(1.112D), new Wrapper(1.113D) }
                    }
                }
            };
            var nullableWrappedStatFilterList = StartingQuery.BuildQuery(nullableWrappedStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableWrappedStatFilterList != null);
            ClassicAssert.IsTrue(nullableWrappedStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableWrappedStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p == 1.112));
        }

        [Test]
        public void NotInClause()
        {
            //expect two entries to match for an integer comparison
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "integer",
                        Value = new[] { 1,2 }
                    }
                }
            };
            var contentIdFilteredList = StartingQuery.BuildQuery(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 1);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => (new List<int>() { 3 }).Contains(p.ContentTypeId)));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(contentIdFilter).ToList();

            });

            //expect two entries to match for an integer comparison
            var nullableContentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "integer",
                        Value = new[] { 1, 2 }
                    }
                }
            };
            var nullableContentIdFilteredList =
                StartingQuery.BuildQuery(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(
                nullableContentIdFilteredList.All(p => !(new List<int>() { 1, 2 }).Contains(p.NullableContentTypeId.GetValueOrDefault())));




            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "string",
                        Value = "there is something interesting about this text"
                    }
                }
            };
            var longerTextToFilterList = StartingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 1);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p == null || p != "there is something interesting about this text"));


            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "datetime",
                        Value = new[] { DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture), DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture) }
                    }
                }
            };
            var lastModifiedFilterList = StartingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 0);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => p == DateTime.UtcNow.Date));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "datetime",
                        Value = new[] { DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture), DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture) }
                    }
                }
            };
            var nullableLastModifiedFilterList = StartingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 1);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => p != DateTime.UtcNow.Date));


            //expect 2 entries to match for a double field
            var statValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "double",
                        Value = new[] { 1.11D, 1.12D }
                    }
                }
            };
            var statValueFilterList = StartingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 1);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => !(new List<double>() { 1.11, 1.12 }).Contains(p)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable double field
            var nullableStatValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "double",
                        Value = new object[] { 1.112D, 1.113D }
                    }
                }
            };
            var nullableStatFilterList = StartingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p != 1.112));

            //expect 2 entries to match for a List<DateTime> field
            var dateListFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "DateList",
                        Id = "DateList",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var dateListFilterList = StartingQuery.BuildQuery(dateListFilter).ToList();
            ClassicAssert.IsTrue(dateListFilterList != null);
            ClassicAssert.IsTrue(dateListFilterList.Count == 1);
            ClassicAssert.IsTrue(dateListFilterList.All(p => !p.DateList.Contains(DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                dateListFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(dateListFilter).ToList();

            });

            //expect 2 entries to match for a List<string> field
            var strListFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "StrList",
                        Id = "StrList",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "string",
                        Value = "Str2"
                    }
                }
            };
            var strListFilterList = StartingQuery.BuildQuery(strListFilter).ToList();
            ClassicAssert.IsTrue(strListFilterList != null);
            ClassicAssert.IsTrue(strListFilterList.Count == 1);
            ClassicAssert.IsTrue(strListFilterList.All(p => !p.StrList.Contains("Str2")));
            
            //expect 2 entries to match for a List<int> field
            var intListFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "IntList",
                        Id = "IntList",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "integer",
                        Value = new[] { 1 ,3 }
                    }
                }
            };
            var intListFilterList = StartingQuery.BuildQuery(intListFilter).ToList();
            ClassicAssert.IsTrue(intListFilterList != null);
            ClassicAssert.IsTrue(intListFilterList.Count == 1);
            ClassicAssert.IsTrue(intListFilterList.All(p => !p.IntList.Contains(1) && !p.IntList.Contains(3)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                intListFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(intListFilter).ToList();

            });

            //expect 2 entries to match for a nullable int field
            var nullableIntListFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "NullableIntList",
                        Id = "NullableIntList",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "integer",
                        Value = "5"
                    }
                }
            };
            var nullableIntListList = StartingQuery.BuildQuery(nullableIntListFilter).ToList();
            ClassicAssert.IsTrue(nullableIntListList != null);
            ClassicAssert.IsTrue(nullableIntListList.Count == 1);
            ClassicAssert.IsTrue(
                nullableIntListList.All(p => !p.NullableIntList.Contains(5)));

            //expect 2 entries to match for a nullable double field
            var nullableWrappedStatValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "double",
                        Value = new[] {new Wrapper(1.112D), new Wrapper(1.113D) }
                    }
                }
            };
            var nullableWrappedStatFilterList = StartingQuery.BuildQuery(nullableWrappedStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableWrappedStatFilterList != null);
            ClassicAssert.IsTrue(nullableWrappedStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableWrappedStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p != 1.112));

            //expect 3 entries to match for a List<long> field
            var longListFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LongList",
                        Id = "LongList",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "long",
                        Value = new[] { 7, 20 }
                    }
                }
            };
            var longListFilterList = StartingQuery.BuildQuery(longListFilter).ToList();
            ClassicAssert.IsTrue(longListFilterList != null);
            ClassicAssert.IsTrue(longListFilterList.Count == 3);
            ClassicAssert.IsTrue(longListFilterList.All(p => !p.LongList.Contains(7) && !p.LongList.Contains(20)));
            
            //expect 3 entries to match for a nullable List<long> field
            var nullableLongListFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "NullableLongList",
                        Id = "NullableLongList",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "long",
                        Value = "5"
                    }
                }
            };
            var nullableLongListList = StartingQuery.BuildQuery(nullableLongListFilter).ToList();
            ClassicAssert.IsTrue(nullableLongListList != null);
            ClassicAssert.IsTrue(nullableLongListList.Count == 2);
            ClassicAssert.IsTrue(nullableLongListList.All(p => !p.NullableLongList.Contains(5)));
        }

        [Test]
        public void IsNullClause()
        {
            //expect 1 entries to match for a case-insensitive string comparison (nullable type)
            var longerTextToFilterFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "is_null",
                        Type = "string",
                        Value = ""
                    }
                }
            };
            var longerTextToFilterList = StartingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 1);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p == null));


            //expect 0 entries to match for a non-nullable type
            var contentTypeIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "is_null",
                        Type = "integer",
                        Value = ""
                    }
                }
            };
            var contentTypeIdFilterList = StartingQuery.BuildQuery(contentTypeIdFilter).ToList();
            ClassicAssert.IsTrue(contentTypeIdFilterList != null);
            ClassicAssert.IsTrue(contentTypeIdFilterList.Count == 0);
            ClassicAssert.IsTrue(
                contentTypeIdFilterList.Select(p => p.ContentTypeId)
                    .All(p => p == 0));

        }

        [Test]
        public void IsNotNullClause()
        {
            //expect 3 entries to match for a case-insensitive string comparison (nullable type)
            var longerTextToFilterFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "is_not_null",
                        Type = "string",
                        Value = ""
                    }
                }
            };
            var longerTextToFilterList = StartingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 3);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p != null));


            //expect 0 entries to match for a non-nullable type
            var contentTypeIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "is_not_null",
                        Type = "integer",
                        Value = ""
                    }
                }
            };
            var contentTypeIdFilterList = StartingQuery.BuildQuery(contentTypeIdFilter).ToList();
            ClassicAssert.IsTrue(contentTypeIdFilterList != null);
            ClassicAssert.IsTrue(contentTypeIdFilterList.Count == 4);
            ClassicAssert.IsTrue(
                contentTypeIdFilterList.Select(p => p.ContentTypeId)
                    .All(p => p != 0));

        }

        [Test]
        public void IsEmptyClause()
        {
            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "is_empty",
                        Type = "string",
                        Value = ""
                    }
                }
            };
            var longerTextToFilterList = StartingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 0);


            //expect 2 entries to match for a List<DateTime> field
            var dateListFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "DateList",
                        Id = "DateList",
                        Input = "NA",
                        Operator = "is_empty",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var dateListFilterList = StartingQuery.BuildQuery(dateListFilter).ToList();
            ClassicAssert.IsTrue(dateListFilterList != null);
            ClassicAssert.IsTrue(dateListFilterList.Count == 0);
            //ClassicAssert.IsTrue(dateListFilterList.All(p => !p.DateList.Contains(DateTime.UtcNow.Date.AddDays(-2))));



            //expect 2 entries to match for a List<string> field
            var strListFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "StrList",
                        Id = "StrList",
                        Input = "NA",
                        Operator = "is_empty",
                        Type = "string",
                        Value = "Str2"
                    }
                }
            };
            var strListFilterList = StartingQuery.BuildQuery(strListFilter).ToList();
            ClassicAssert.IsTrue(strListFilterList != null);
            ClassicAssert.IsTrue(strListFilterList.Count == 0);
            //ClassicAssert.IsTrue(strListFilterList.All(p => !p.StrList.Contains("Str2")));


            //expect 2 entries to match for a List<int> field
            var intListFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "IntList",
                        Id = "IntList",
                        Input = "NA",
                        Operator = "is_empty",
                        Type = "integer",
                        Value = new[] { 1, 3 }
                    }
                }
            };
            var intListFilterList = StartingQuery.BuildQuery(intListFilter).ToList();
            ClassicAssert.IsTrue(intListFilterList != null);
            ClassicAssert.IsTrue(intListFilterList.Count == 0);



        }

        [Test]
        public void IsNotEmptyClause()
        {
            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "is_not_empty",
                        Type = "string",
                        Value = ""
                    }
                }
            };
            var longerTextToFilterList = StartingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 4);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p == null || p.Length > 0));


            //expect 2 entries to match for a List<DateTime> field
            var dateListFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "DateList",
                        Id = "DateList",
                        Input = "NA",
                        Operator = "is_not_empty",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var dateListFilterList = StartingQuery.BuildQuery(dateListFilter).ToList();
            ClassicAssert.IsTrue(dateListFilterList != null);
            ClassicAssert.IsTrue(dateListFilterList.Count == 4);
            //ClassicAssert.IsTrue(dateListFilterList.All(p => !p.DateList.Contains(DateTime.UtcNow.Date.AddDays(-2))));



            //expect 2 entries to match for a List<string> field
            var strListFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "StrList",
                        Id = "StrList",
                        Input = "NA",
                        Operator = "is_not_empty",
                        Type = "string",
                        Value = "Str2"
                    }
                }
            };
            var strListFilterList = StartingQuery.BuildQuery(strListFilter).ToList();
            ClassicAssert.IsTrue(strListFilterList != null);
            ClassicAssert.IsTrue(strListFilterList.Count == 4);
            //ClassicAssert.IsTrue(strListFilterList.All(p => !p.StrList.Contains("Str2")));


            //expect 2 entries to match for a List<int> field
            var intListFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "IntList",
                        Id = "IntList",
                        Input = "NA",
                        Operator = "is_not_empty",
                        Type = "integer",
                        Value = new[] {1, 3 }
                    }
                }
            };
            var intListFilterList = StartingQuery.BuildQuery(intListFilter).ToList();
            ClassicAssert.IsTrue(intListFilterList != null);
            ClassicAssert.IsTrue(intListFilterList.Count == 4);

        }

        [Test]
        public void ContainsClause()
        {
            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "contains",
                        Type = "string",
                        Value = "something interesting"
                    }
                }
            };
            var longerTextToFilterList = StartingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 3);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p.Contains("something interesting")));

        }

        [Test]
        public void NotContainsClause()
        {
            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "not_contains",
                        Type = "string",
                        Value = "something interesting"
                    }
                }
            };
            var longerTextToFilterList = StartingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 1);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p == null));

        }

        [Test]
        public void NotEndsWithClause()
        {
            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "not_ends_with",
                        Type = "string",
                        Value = "about this text"
                    }
                }
            };
            var longerTextToFilterList = StartingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 1);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p == null));

        }

        [Test]
        public void EndsWithClause()
        {
            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "ends_with",
                        Type = "string",
                        Value = "about this text"
                    }
                }
            };
            var longerTextToFilterList = StartingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 3);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p.EndsWith("about this text")));

        }

        [Test]
        public void NotBeginsWithClause()
        {
            //expect 1 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "not_begins_with",
                        Type = "string",
                        Value = "there is something"
                    }
                }
            };
            var longerTextToFilterList = StartingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 1);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p == null));

        }

        [Test]
        public void BeginsWithClause()
        {
            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "begins_with",
                        Type = "string",
                        Value = "there is something"
                    }
                }
            };
            var longerTextToFilterList = StartingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 3);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p.StartsWith("there is something")));

        }

        [Test]
        public void EqualsClause()
        {

            //expect two entries to match for an integer comparison
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "equal",
                        Type = "integer",
                        Value = "1"
                    }
                }
            };
            var contentIdFilteredList = StartingQuery.BuildQuery(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId == 1));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(contentIdFilter).ToList();

            });

            //expect two entries to match for an integer comparison
            var nullableContentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "equal",
                        Type = "integer",
                        Value = "1"
                    }
                }
            };
            var nullableContentIdFilteredList =
                StartingQuery.BuildQuery(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 1);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId == 1));




            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "equal",
                        Type = "string",
                        Value = "there is something interesting about this text"
                    }
                }
            };
            var longerTextToFilterList = StartingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 3);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p == "there is something interesting about this text"));


            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "equal",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = StartingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 4);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => p == DateTime.UtcNow.Date));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "equal",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = StartingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => p == DateTime.UtcNow.Date));







            //expect 3 entries to match for a boolean field
            var isSelectedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "IsSelected",
                        Id = "IsSelected",
                        Input = "NA",
                        Operator = "equal",
                        Type = "boolean",
                        Value = true
                    }
                }
            };
            var isSelectedFilterList = StartingQuery.BuildQuery(isSelectedFilter).ToList();
            ClassicAssert.IsTrue(isSelectedFilterList != null);
            ClassicAssert.IsTrue(isSelectedFilterList.Count == 3);
            ClassicAssert.IsTrue(
                isSelectedFilterList.Select(p => p.IsSelected)
                    .All(p => p == true));

            //expect failure when an invalid bool is encountered in bool comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                isSelectedFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(isSelectedFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableIsSelectedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "IsPossiblyNotSetBool",
                        Id = "IsPossiblyNotSetBool",
                        Input = "NA",
                        Operator = "equal",
                        Type = "boolean",
                        Value = true
                    }
                }
            };
            var nullableIsSelectedFilterList = StartingQuery.BuildQuery(nullableIsSelectedFilter).ToList();
            ClassicAssert.IsTrue(nullableIsSelectedFilterList != null);
            ClassicAssert.IsTrue(nullableIsSelectedFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableIsSelectedFilterList.Select(p => p.IsPossiblyNotSetBool)
                    .All(p => p == true));


            //expect 2 entries to match for a double field
            var statValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "equal",
                        Type = "double",
                        Value = 1.11D
                    }
                }
            };
            var statValueFilterList = StartingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 2);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => p == 1.11));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "equal",
                        Type = "double",
                        Value = 1.112D
                    }
                }
            };
            var nullableStatFilterList = StartingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p == 1.112));



        }

        [Test]
        public void NotEqualsClause()
        {
            //expect two entries to match for an integer comparison
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "integer",
                        Value = 1
                    }
                }
            };
            var contentIdFilteredList = StartingQuery.BuildQuery(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId != 1));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(contentIdFilter).ToList();

            });

            //expect 3 entries to match for an integer comparison
            var nullableContentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "integer",
                        Value = 1
                    }
                }
            };
            var nullableContentIdFilteredList =
                StartingQuery.BuildQuery(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 3);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId != 1));

            //expect 1 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "string",
                        Value = "there is something interesting about this text"
                    }
                }
            };
            var longerTextToFilterList = StartingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 1);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => (p == null) || (p.ToLower() != "there is something interesting about this text")));


            //expect 0 entries to match for a Date comparison
            var lastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = StartingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 0);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => p == DateTime.UtcNow.Date));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(lastModifiedFilter).ToList();

            });

            //expect 1 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = StartingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 1);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => p != DateTime.UtcNow.Date));


            //expect 1 entries to match for a boolean field
            var isSelectedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "IsSelected",
                        Id = "IsSelected",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "boolean",
                        Value = true
                    }
                }
            };
            var isSelectedFilterList = StartingQuery.BuildQuery(isSelectedFilter).ToList();
            ClassicAssert.IsTrue(isSelectedFilterList != null);
            ClassicAssert.IsTrue(isSelectedFilterList.Count == 1);
            ClassicAssert.IsTrue(
                isSelectedFilterList.Select(p => p.IsSelected)
                    .All(p => p != true));

            //expect failure when an invalid bool is encountered in bool comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                isSelectedFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(isSelectedFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableIsSelectedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "IsPossiblyNotSetBool",
                        Id = "IsPossiblyNotSetBool",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "boolean",
                        Value = true
                    }
                }
            };
            var nullableIsSelectedFilterList = StartingQuery.BuildQuery(nullableIsSelectedFilter).ToList();
            ClassicAssert.IsTrue(nullableIsSelectedFilterList != null);
            ClassicAssert.IsTrue(nullableIsSelectedFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableIsSelectedFilterList.Select(p => p.IsPossiblyNotSetBool)
                    .All(p => p != true));


            //expect 2 entries to match for a double field
            var statValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "double",
                        Value = 1.11D
                    }
                }
            };
            var statValueFilterList = StartingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 2);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => p != 1.11));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "double",
                        Value = 1.112D
                    }
                }
            };
            var nullableStatFilterList = StartingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p != 1.112));

        }

        [Test]
        public void BetweenClause()
        {
            //expect 3 entries to match for an integer comparison
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "between",
                        Type = "integer",
                        Value = new[] { 1, 2 }
                    }
                }
            };
            var contentIdFilteredList = StartingQuery.BuildQuery(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 3);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId < 3));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(contentIdFilter).ToList();

            });

            //expect 2 entries to match for an integer comparison
            var nullableContentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "between",
                        Type = "integer",
                        Value = new[] { 1, 2 }
                    }
                }
            };
            var nullableContentIdFilteredList =
                StartingQuery.BuildQuery(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId < 3));





            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "between",
                        Type = "datetime",
                        Value = new[] { DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture), DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture) }
                    }
                }
            };
            var lastModifiedFilterList = StartingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 4);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => (p >= DateTime.UtcNow.Date.AddDays(-2)) && (p <= DateTime.UtcNow.Date)));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "between",
                        Type = "datetime",
                        Value = new[] { DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture), DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture) }
                    }
                }
            };
            var nullableLastModifiedFilterList = StartingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p >= DateTime.UtcNow.Date.AddDays(-2)) && (p <= DateTime.UtcNow.Date)));


            //expect 3 entries to match for a double field
            var statValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "between",
                        Type = "double",
                        Value = new[] { 1.0D, 1.12D }
                    }
                }
            };
            var statValueFilterList = StartingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 3);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (p >= 1.0) && (p <= 1.12)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "between",
                        Type = "double",
                        Value = new[] { 1.112D, 1.112D }
                    }
                }
            };
            var nullableStatFilterList = StartingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p == 1.112));

        }

        [Test]
        public void NotBetweenClause()
        {
            //expect 1 entry to match for an integer comparison
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "integer",
                        Value = new[] { 1, 2 }
                    }
                }
            };
            var contentIdFilteredList = StartingQuery.BuildQuery(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 1);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId < 1 || p.ContentTypeId > 2));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(contentIdFilter).ToList();

            });

            //expect 2 entries to match for an integer comparison
            var nullableContentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "integer",
                        Value = new[] { 1, 2 }
                    }
                }
            };
            var nullableContentIdFilteredList =
                StartingQuery.BuildQuery(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId < 1 || p.NullableContentTypeId > 2 || p.NullableContentTypeId == null));





            //expect 0 entries to match for a Date comparison
            var lastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "datetime",
                        Value = new[] {DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture), DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture) }
                    }
                }
            };
            var lastModifiedFilterList = StartingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 0);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(-2)) && (p >= DateTime.UtcNow.Date)));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(lastModifiedFilter).ToList();

            });

            //expect 1 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "datetime",
                        Value = new[] { DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture), DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture) }
                    }
                }
            };
            var nullableLastModifiedFilterList = StartingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 1);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(-2) && p >= DateTime.UtcNow.Date) || p == null));


            //expect 3 entries to match for a double field
            var statValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "double",
                        Value = new[] { 1.0D, 1.12D }
                    }
                }
            };
            var statValueFilterList = StartingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 1);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (p <= 1.0) || (p >= 1.12)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "double",
                        Value = "1.112,1.112"
                    }
                }
            };
            var nullableStatFilterList = StartingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p != 1.112));

        }

        [Test]
        public void GreaterOrEqualClause()
        {
            //expect 1 entries to match for an integer comparison
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "greater_or_equal",
                        Type = "integer",
                        Value = 2
                    }
                }
            };
            var contentIdFilteredList = StartingQuery.BuildQuery(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId >= 2));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(contentIdFilter).ToList();

            });

            //expect 1 entries to match for an integer comparison
            var nullableContentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "greater_or_equal",
                        Type = "integer",
                        Value = 2
                    }
                }
            };
            var nullableContentIdFilteredList =
                StartingQuery.BuildQuery(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId >= 2));


            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "greater_or_equal",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = StartingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 4);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => (p >= DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(lastModifiedFilter).ToList();

            });

            //expect 0 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "greater_or_equal",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = StartingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 0);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p >= DateTime.UtcNow.Date.AddDays(1))));


            //expect 4 entries to match for a double field
            var statValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "greater_or_equal",
                        Type = "double",
                        Value = 1D
                    }
                }
            };
            var statValueFilterList = StartingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 4);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (p >= 1)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "greater_or_equal",
                        Type = "double",
                        Value = 1.112D
                    }
                }
            };
            var nullableStatFilterList = StartingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p >= 1.112));

        }

        [Test]
        public void GreaterClause()
        {
            //expect 1 entries to match for an integer comparison
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "greater",
                        Type = "integer",
                        Value = 2
                    }
                }
            };
            var contentIdFilteredList = StartingQuery.BuildQuery(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 1);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId > 2));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(contentIdFilter).ToList();

            });

            //expect 1 entries to match for an integer comparison
            var nullableContentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "greater",
                        Type = "integer",
                        Value = 2
                    }
                }
            };
            var nullableContentIdFilteredList =
                StartingQuery.BuildQuery(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 1);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId > 2));


            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "greater",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = StartingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 4);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => (p > DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(lastModifiedFilter).ToList();

            });

            //expect 0 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "greater",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = StartingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 0);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p > DateTime.UtcNow.Date.AddDays(1))));


            //expect 4 entries to match for a double field
            var statValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "greater",
                        Type = "double",
                        Value = 1D
                    }
                }
            };
            var statValueFilterList = StartingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 4);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (p > 1)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "greater",
                        Type = "double",
                        Value = 1.112D
                    }
                }
            };
            var nullableStatFilterList = StartingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 0);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p > 1.112));

        }

        [Test]
        public void LessClause()
        {
            //expect 2 entries to match for an integer comparison
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "less",
                        Type = "integer",
                        Value = 2
                    }
                }
            };
            var contentIdFilteredList = StartingQuery.BuildQuery(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId < 2));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(contentIdFilter).ToList();

            });

            //expect 1 entries to match for an integer comparison
            var nullableContentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "less",
                        Type = "integer",
                        Value = 2
                    }
                }
            };
            var nullableContentIdFilteredList =
                StartingQuery.BuildQuery(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 1);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId < 2));


            //expect 0 entries to match for a Date comparison
            var lastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "less",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = StartingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 0);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "less",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = StartingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(1))));


            //expect 3 entries to match for a double field
            var statValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "less",
                        Type = "double",
                        Value = 1.13D
                    }
                }
            };
            var statValueFilterList = StartingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 3);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (p <= 1.12)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "less",
                        Type = "double",
                        Value = "1.113"
                    }
                }
            };
            var nullableStatFilterList = StartingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p < 1.113));

        }

        [Test]
        public void LessOrEqualClause()
        {
            //expect 3 entries to match for an integer comparison
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "less_or_equal",
                        Type = "integer",
                        Value = "2"
                    }
                }
            };
            var contentIdFilteredList = StartingQuery.BuildQuery(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 3);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId <= 2));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(contentIdFilter).ToList();

            });

            //expect 2 entries to match for an integer comparison
            var nullableContentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "less_or_equal",
                        Type = "integer",
                        Value = "2"
                    }
                }
            };
            var nullableContentIdFilteredList =
                StartingQuery.BuildQuery(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId <= 2));


            //expect 0 entries to match for a Date comparison
            var lastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "less_or_equal",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = StartingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 0);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "less_or_equal",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = StartingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(1))));


            //expect 3 entries to match for a double field
            var statValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "less_or_equal",
                        Type = "double",
                        Value = "1.13"
                    }
                }
            };
            var statValueFilterList = StartingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 4);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (p <= 1.13)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                StartingQuery.BuildQuery(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable double field
            var nullableStatValueFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "less_or_equal",
                        Type = "double",
                        Value = "1.113"
                    }
                }
            };
            var nullableStatFilterList = StartingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p <= 1.113));

        }

        [Test]
        public void FilterWithInvalidParameters()
        {
            //expect 3 entries to match for an integer comparison
            var contentIdFilter = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Rules = new List<SystemTextJsonFilterRule>
                {
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "less_or_equal",
                        Type = "integer",
                        Value = 2
                    },
                    new SystemTextJsonFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "less_or_equal",
                        Type = "integer",
                        Value = 2
                    }
                }
            };

            StartingQuery.BuildQuery(contentIdFilter).ToList();

            contentIdFilter.Condition = "or";

            StartingQuery.BuildQuery(contentIdFilter).ToList();

            StartingQuery.BuildQuery(null).ToList();

            StartingQuery.BuildQuery(new SystemTextJsonFilterRule()).ToList();

            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Type = "NOT_A_TYPE";
                StartingQuery.BuildQuery(contentIdFilter).ToList();

            });

            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Type = "integer";
                contentIdFilter.Rules.First().Operator = "NOT_AN_OPERATOR";
                StartingQuery.BuildQuery(contentIdFilter).ToList();
            });
        }

        private class IndexedClass
        {
            int this[string someIndex]
            {
                get
                {
                    return 2;
                }
            }
        }

        [Test]
        public void IndexedExpression_Test()
        {
            var rule = new SystemTextJsonFilterRule
            {
                Condition = "and",
                Field = "ContentTypeId",
                Id = "ContentTypeId",
                Input = "NA",
                Operator = "equal",
                Type = "integer",
                Value = 2
            };

            var result = new List<IndexedClass> { new IndexedClass() }.AsQueryable().BuildQuery(rule,
                new BuildExpressionOptions() { UseIndexedProperty = true, IndexedPropertyName = "Item" });
            ClassicAssert.IsTrue(result.Any());

            rule.Value = 3;
            result = new[] { new IndexedClass() }.BuildQuery(rule, true, "Item");
            ClassicAssert.IsFalse(result.Any());
        }
        #endregion
    }
}
