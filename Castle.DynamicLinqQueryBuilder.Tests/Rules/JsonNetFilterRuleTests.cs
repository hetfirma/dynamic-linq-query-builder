using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Castle.DynamicLinqQueryBuilder.Tests.CustomOperators;
using Castle.DynamicLinqQueryBuilder.Tests.Helpers;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Castle.DynamicLinqQueryBuilder.Tests.Rules
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class JsonNetFilterRuleTests
    {
        IQueryable<Tests.ExpressionTreeBuilderTestClass> StartingQuery;
        IQueryable<Tests.ExpressionTreeBuilderTestClass> StartingDateQuery;
        BuildExpressionOptions dtOptionCurrentCulture;

        [SetUp]
        public void Setup()
        {
            StartingQuery = Tests.GetExpressionTreeData().AsQueryable();
            StartingDateQuery = Tests.GetDateExpressionTreeData().AsQueryable();
            dtOptionCurrentCulture = new BuildExpressionOptions() { CultureInfo = CultureInfo.CurrentCulture };
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

        #region Expression Tree Builder        

        [Test]
        public void DateHandling()
        {
            QueryBuilder.ParseDatesAsUtc = true;
            var contentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var queryable = StartingDateQuery.BuildQuery<Tests.ExpressionTreeBuilderTestClass>(contentIdFilter);
            var contentIdFilteredList = queryable.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 1);

            contentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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


            contentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            contentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var contentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            contentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableContentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var longerTextToFilterFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var longerTextToFilterFilterCaps = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var lastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var lastModifiedFilterList = StartingQuery.BuildQuery(lastModifiedFilter, dtOptionCurrentCulture).ToList();
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
            var nullableLastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "in",
                        Type = "datetime",
                        Value = new[] { DateTime.UtcNow.Date.ToString(), DateTime.UtcNow.Date.AddDays(1).ToString() }
                    }
                }
            };
            var nullableLastModifiedFilterList = StartingQuery.BuildQuery(nullableLastModifiedFilter, dtOptionCurrentCulture).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => p == DateTime.UtcNow.Date));


            //expect 2 entries to match for a double field
            var statValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableStatValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var dateListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var strListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var intListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableIntListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            //expect 2 entries to match for a List<int> field
            var longListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "LongList",
                        Id = "LongList",
                        Input = "NA",
                        Operator = "in",
                        Type = "long",
                        Value = new[] {"1", "3"}
                    }
                }
            };
            var longListFilterList = StartingQuery.BuildQuery(longListFilter).ToList();
            ClassicAssert.IsTrue(longListFilterList != null);
            ClassicAssert.IsTrue(longListFilterList.Count == 1);
            ClassicAssert.IsTrue(longListFilterList.All(p => p.LongList.Contains(1) || p.LongList.Contains(3)));

            //expect 2 entries to match for a nullable nullable int field
            var nullableLongListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "NullableLongList",
                        Id = "NullableLongList",
                        Input = "NA",
                        Operator = "in",
                        Type = "long",
                        Value = 5
                    }
                }
            };
            var nullableLongListList = StartingQuery.BuildQuery(nullableLongListFilter).ToList();
            ClassicAssert.IsTrue(nullableLongListList != null);
            ClassicAssert.IsTrue(nullableLongListList.Count == 2);
            ClassicAssert.IsTrue(nullableLongListList.All(p => p.NullableLongList.Contains(5)));
            
            var multipleWithBlankRule = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableWrappedStatValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            var firstGuid = StartingQuery.First().ContentTypeGuid.ToString();
            //expect one entry to match for a Guid Comparison
            var contentGuidFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeGuid",
                        Id = "ContentTypeGuid",
                        Input = "NA",
                        Operator = "in",
                        Type = "string",
                        Value = new[] { firstGuid, firstGuid }
                    }
                }
            };
            var contentGuidFilteredList = StartingQuery.BuildQuery(contentGuidFilter).ToList();
            ClassicAssert.IsTrue(contentGuidFilteredList != null);
            ClassicAssert.IsTrue(contentGuidFilteredList.Count == 1);

            //expect one entry to match for a Guid Comparison against a null nullable Id
            var nullableContentGuidFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeGuid",
                        Id = "NullableContentTypeGuid",
                        Input = "NA",
                        Operator = "in",
                        Type = "string",
                        Value = new[] { firstGuid }
                    }
                }
            };
            var nullableContentGuidFilteredList = StartingQuery.BuildQuery(nullableContentGuidFilter).ToList();
            ClassicAssert.IsTrue(nullableContentGuidFilteredList != null);
            ClassicAssert.IsTrue(nullableContentGuidFilteredList.Count == 1);
        }

        [Test]
        public void NotInClause()
        {
            //expect two entries to match for an integer comparison
            var contentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableContentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var longerTextToFilterFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var lastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "datetime",
                        Value = new[] { DateTime.UtcNow.Date.ToString(), DateTime.UtcNow.Date.ToString() }
                    }
                }
            };
            var lastModifiedFilterList = StartingQuery.BuildQuery(lastModifiedFilter, dtOptionCurrentCulture).ToList();
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
            var nullableLastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "datetime",
                        Value = new[] { DateTime.UtcNow.Date.ToString(), DateTime.UtcNow.Date.AddDays(1).ToString() }
                    }
                }
            };
            var nullableLastModifiedFilterList = StartingQuery.BuildQuery(nullableLastModifiedFilter, dtOptionCurrentCulture).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 1);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => p != DateTime.UtcNow.Date));


            //expect 2 entries to match for a double field
            var statValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableStatValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var dateListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var strListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var intListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            //expect 2 entries to match for a nullable nullable int field
            var nullableIntListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableWrappedStatValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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



        }

        [Test]
        public void IsNullClause()
        {
            //expect 1 entries to match for a case-insensitive string comparison (nullable type)
            var longerTextToFilterFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var contentTypeIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var longerTextToFilterFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var contentTypeIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var longerTextToFilterFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var dateListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var strListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var intListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var longerTextToFilterFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var dateListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var strListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var intListFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var longerTextToFilterFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            //expect at least one entry to match for a Guid Comparison
            var contentGuidFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeGuid",
                        Id = "ContentTypeGuid",
                        Input = "NA",
                        Operator = "contains",
                        Type = "string",
                        Value = StartingQuery.First().ContentTypeGuid.ToString().Substring(0,5)
                    }
                }
            };
            var contentGuidFilteredList = StartingQuery.BuildQuery(contentGuidFilter).ToList();
            ClassicAssert.IsTrue(contentGuidFilteredList != null);
            ClassicAssert.IsTrue(contentGuidFilteredList.Count >= 1);

            //expect one match for a Guid Comparison against a null nullable Id
            var nullableContentGuidFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeGuid",
                        Id = "NullableContentTypeGuid",
                        Input = "NA",
                        Operator = "contains",
                        Type = "string",
                        Value = StartingQuery.First().ContentTypeGuid.ToString().Substring(0, 5)
                    }
                }
            };
            var nullableContentGuidFilteredList = StartingQuery.BuildQuery(nullableContentGuidFilter).ToList();
            ClassicAssert.IsTrue(nullableContentGuidFilteredList != null);
            ClassicAssert.IsTrue(nullableContentGuidFilteredList.Count == 1);
        }

        [Test]
        public void NotContainsClause()
        {
            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var longerTextToFilterFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

        public class HashClass
        {
            public HashSet<string> Hobbies { get; set; }
        }
        [Test]
        public void HashSetTest()
        {
            var hashSet = new List<HashClass>()
            {
                new HashClass() {Hobbies = new HashSet<string> {"Baseball"}},
                new HashClass() {Hobbies = new HashSet<string> {"Football"}}
            };
            var longerTextToFilterFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "Hobbies",
                        Id = "Hobbies",
                        Input = "NA",
                        Operator = "in",
                        Type = "string",
                        Value = "Baseball"
                    }
                }
            };
            var longerTextToFilterList = hashSet.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 1);
        }

        [Test]
        public void EndsWithClause()
        {
            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            var fristGuid = StartingQuery.First().ContentTypeGuid.ToString();
            //expect at least one entry to match for a Guid Comparison
            var contentGuidFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeGuid",
                        Id = "ContentTypeGuid",
                        Input = "NA",
                        Operator = "ends_with",
                        Type = "string",
                        Value = fristGuid.Substring(fristGuid.Length - 5)
                    }
                }
            };
            var contentGuidFilteredList = StartingQuery.BuildQuery(contentGuidFilter).ToList();
            ClassicAssert.IsTrue(contentGuidFilteredList != null);
            ClassicAssert.IsTrue(contentGuidFilteredList.Count >= 1);

            //expect one match for a Guid Comparison against a null nullable Id
            var nullableContentGuidFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeGuid",
                        Id = "NullableContentTypeGuid",
                        Input = "NA",
                        Operator = "ends_with",
                        Type = "string",
                        Value = fristGuid.Substring(fristGuid.Length - 5)
                    }
                }
            };
            var nullableContentGuidFilteredList = StartingQuery.BuildQuery(nullableContentGuidFilter).ToList();
            ClassicAssert.IsTrue(nullableContentGuidFilteredList != null);
            ClassicAssert.IsTrue(nullableContentGuidFilteredList.Count == 1);
        }

        [Test]
        public void NotBeginsWithClause()
        {
            //expect 1 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var longerTextToFilterFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            var firstGuid = StartingQuery.First().ContentTypeGuid.ToString();
            //expect at least one entry to match for a Guid Comparison
            var contentGuidFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeGuid",
                        Id = "ContentTypeGuid",
                        Input = "NA",
                        Operator = "begins_with",
                        Type = "string",
                        Value = firstGuid.Substring(0,5)
                    }
                }
            };
            var contentGuidFilteredList = StartingQuery.BuildQuery(contentGuidFilter).ToList();
            ClassicAssert.IsTrue(contentGuidFilteredList != null);
            ClassicAssert.IsTrue(contentGuidFilteredList.Count >= 1);

            //expect one match for a Guid Comparison against a null nullable Id
            var nullableContentGuidFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeGuid",
                        Id = "NullableContentTypeGuid",
                        Input = "NA",
                        Operator = "begins_with",
                        Type = "string",
                        Value = firstGuid.Substring(0,5)
                    }
                }
            };
            var nullableContentGuidFilteredList = StartingQuery.BuildQuery(nullableContentGuidFilter).ToList();
            ClassicAssert.IsTrue(nullableContentGuidFilteredList != null);
            ClassicAssert.IsTrue(nullableContentGuidFilteredList.Count == 1);
        }

        [Test]
        public void EqualsClause()
        {
            
            //expect two entries to match for an integer comparison
            var contentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableContentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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

            //expect one entry to match for a Guid Comparison
            var contentGuidFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeGuid",
                        Id = "ContentTypeGuid",
                        Input = "NA",
                        Operator = "equal",
                        Type = "string",
                        Value = StartingQuery.First().ContentTypeGuid.ToString()
                    }
                }
            };
            var contentGuidFilteredList = StartingQuery.BuildQuery(contentGuidFilter).ToList();
            ClassicAssert.IsTrue(contentGuidFilteredList != null);
            ClassicAssert.IsTrue(contentGuidFilteredList.Count == 1);

            //expect one match for a Guid Comparison against a null nullable Id
            var nullableContentGuidFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeGuid",
                        Id = "NullableContentTypeGuid",
                        Input = "NA",
                        Operator = "equal",
                        Type = "string",
                        Value = StartingQuery.First().ContentTypeGuid.ToString()
                    }
                }
            };
            var nullableContentGuidFilteredList = StartingQuery.BuildQuery(nullableContentGuidFilter).ToList();
            ClassicAssert.IsTrue(nullableContentGuidFilteredList != null);
            ClassicAssert.IsTrue(nullableContentGuidFilteredList.Count == 1);

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var lastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableLastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var isSelectedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableIsSelectedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var statValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableStatValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var contentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableContentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var longerTextToFilterFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var lastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableLastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var isSelectedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableIsSelectedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var statValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableStatValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var contentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableContentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var lastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "between",
                        Type = "datetime",
                        Value = new[] { DateTime.UtcNow.Date.AddDays(-2).ToString(), DateTime.UtcNow.Date.ToString() }
                    }
                }
            };
            var lastModifiedFilterList = StartingQuery.BuildQuery(lastModifiedFilter, dtOptionCurrentCulture).ToList();
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
            var nullableLastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "between",
                        Type = "datetime",
                        Value = new[] { DateTime.UtcNow.Date.AddDays(-2).ToString(), DateTime.UtcNow.Date.ToString() }
                    }
                }
            };
            var nullableLastModifiedFilterList = StartingQuery.BuildQuery(nullableLastModifiedFilter, dtOptionCurrentCulture).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p >= DateTime.UtcNow.Date.AddDays(-2)) && (p <= DateTime.UtcNow.Date)));


            //expect 3 entries to match for a double field
            var statValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableStatValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var contentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableContentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var lastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "datetime",
                        Value = new[] {DateTime.UtcNow.Date.AddDays(-2).ToString(), DateTime.UtcNow.Date.ToString() }
                    }
                }
            };
            var lastModifiedFilterList = StartingQuery.BuildQuery(lastModifiedFilter, dtOptionCurrentCulture).ToList();
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
            var nullableLastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "datetime",
                        Value = new[] { DateTime.UtcNow.Date.AddDays(-2).ToString(), DateTime.UtcNow.Date.ToString() }
                    }
                }
            };
            var nullableLastModifiedFilterList = StartingQuery.BuildQuery(nullableLastModifiedFilter, dtOptionCurrentCulture).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 1);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(-2) && p >= DateTime.UtcNow.Date) || p == null));


            //expect 3 entries to match for a double field
            var statValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableStatValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var contentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableContentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var lastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableLastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var statValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableStatValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var contentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableContentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var lastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableLastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var statValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableStatValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var contentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableContentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var lastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableLastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var statValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableStatValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var contentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableContentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var lastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableLastModifiedFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var statValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var nullableStatValueFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
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
            var contentIdFilter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "less_or_equal",
                        Type = "integer",
                        Value = 2
                    },
                    new JsonNetFilterRule
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

            StartingQuery.BuildQuery(new JsonNetFilterRule()).ToList();

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
            var rule = new JsonNetFilterRule
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
        
           
        private class DictionaryClass
        {
            public DictionaryClass() { DynamicData = new Dictionary<string, object>(); }

            public Dictionary<string, object> DynamicData { get; set; }

        }

        private class DictionaryClassFail
        {
            public DictionaryClassFail() { DynamicData = new Dictionary<int, object>(); }

            public Dictionary<int, object> DynamicData { get; set; }

        }

        private class DictionaryClassConcrete
        {
            public DictionaryClassConcrete() { DynamicData = new Dictionary<string, DataValue>(); }

            public Dictionary<string, DataValue> DynamicData { get; set; }

        }
        private class DataValue
        { 
            public string StringValue { get; set; }
        }

        
        [Test]
        public void DictionaryConcreteExpression_Test()
        {
            var rule = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "DynamicData.test.StringValue",
                        Id = "Id",
                        Operator = "equal",
                        Type = "string",
                        Value = "test"
                    }
                }
            };
            var dataValue = new DataValue { StringValue = "test" };
            var d1 = new DictionaryClassConcrete();
            d1.DynamicData.Add("test", dataValue);
            var d2 = new DictionaryClassConcrete();
            d2.DynamicData.Add("test", new DataValue { StringValue = "NotTest"} );

            var result = new List<DictionaryClassConcrete> { d1, d2 }.AsQueryable().BuildQuery(rule,
                new BuildExpressionOptions());

            ClassicAssert.IsTrue(result.Any());
            ClassicAssert.AreEqual(result.Count(), 1);

            var resultList = result.ToList();
            ClassicAssert.AreEqual(resultList[0].DynamicData["test"], dataValue);
        }
        
        [Test]
        public void DictionaryExpression_Test()
        {
            var rule = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "DynamicData.test",
                        Id = "Id",
                        Operator = "equal",
                        Type = "string",
                        Value = "test"
                    }
                }
            };
            var d1 = new DictionaryClass();
            d1.DynamicData.Add("test", "test");
            var d2 = new DictionaryClass();
            d2.DynamicData.Add("test", "NotTest" );

            var result = new List<DictionaryClass> { d1, d2 }.AsQueryable().BuildQuery(rule,
                new BuildExpressionOptions());

            ClassicAssert.IsTrue(result.Any());
            ClassicAssert.AreEqual(result.Count(), 1);

            var resultList = result.ToList();
            ClassicAssert.AreEqual(resultList[0].DynamicData["test"], "test");
        }
        
        [Test]
        public void DictionaryContainsExpression_Test()
        {
            var rule = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "DynamicData",
                        Id = "Id",
                        Operator = "contains",
                        Type = "string",
                        Value = "existing"
                    }
                }
            };
            var d1 = new DictionaryClass();
            d1.DynamicData.Add("existing", "test");
            var d2 = new DictionaryClass();
            d2.DynamicData.Add("existing", "NotTest" );

            var result = new List<DictionaryClass> { d1, d2 }.AsQueryable().BuildQuery(rule,
                new BuildExpressionOptions());

            ClassicAssert.IsTrue(result.Any());
            ClassicAssert.AreEqual(result.Count(), 2);
        }
        
        [Test]
        public void DictionaryNotContainsExpression_Test()
        {
            var rule = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "DynamicData",
                        Id = "Id",
                        Operator = "not_contains",
                        Type = "string",
                        Value = "not_existing"
                    }
                }
            };
            var d1 = new DictionaryClass();
            d1.DynamicData.Add("test", "test");
            var d2 = new DictionaryClass();
            d2.DynamicData.Add("test", "NotTest" );

            var result = new List<DictionaryClass> { d1, d2 }.AsQueryable().BuildQuery(rule,
                new BuildExpressionOptions());

            ClassicAssert.IsTrue(result.Any());
            ClassicAssert.AreEqual(result.Count(), 2);
        }

        [Test]
        public void DictionaryEqualWithRequireExplicitToStringConversionFlagTrue() {
            var filter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "Dictionary",
                        Operator = "contains",
                        Type = "string",
                        Value = "first_name"
                    },
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "Dictionary.first_name",
                        Operator = "equal",
                        Type = "string",
                        Value = "Emma"
                    }
                }
            };

            var query = StartingQuery.BuildQuery(filter, new BuildExpressionOptions {
                RequireExplicitToStringConversion = true,
                StringCaseSensitiveComparison = true
            });
            
            var results = query.ToList();
            ClassicAssert.AreEqual(results.Count, 2);
            
            // Verify that the expression is using ToString() on the value
            ClassicAssert.IsTrue(query.Expression.ToString().Contains("ToString()"));
        }
        
        [Test]
        public void DictionaryEqualWithRequireExplicitToStringConversionFlagFalse() {
            var filter = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "Dictionary",
                        Operator = "contains",
                        Type = "string",
                        Value = "first_name"
                    },
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "Dictionary.first_name",
                        Operator = "equal",
                        Type = "string",
                        Value = "Emma"
                    }
                }
            };
            
            var query = StartingQuery.BuildQuery(filter, new BuildExpressionOptions {
                RequireExplicitToStringConversion = false,
                StringCaseSensitiveComparison = true
            });
            
            var results = query.ToList();
            ClassicAssert.AreEqual(results.Count, 2);

            // Verify that the expression is not using ToString() on the value
            ClassicAssert.IsFalse(query.Expression.ToString().Contains("ToString()"));
        }
             
        [Test]
        public void DictionaryNotEqualWithoutContainsCheckExpression_Test()
        {
            var rule = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "DynamicData.not_existing",
                        Id = "Id",
                        Operator = "not_equal",
                        Type = "string",
                        Value = "test"
                    }
                }
            };
            var d1 = new DictionaryClass();
            d1.DynamicData.Add("test", "test");
            var d2 = new DictionaryClass();
            d2.DynamicData.Add("test", "NotTest" );

            ExceptionAssert.Throws<KeyNotFoundException>(() =>
            {
                var result = new List<DictionaryClass> { d1, d2 }.AsQueryable().BuildQuery(rule,
                    new BuildExpressionOptions());
                result.Any();
            });
        }
        
        [Test]
        public void DictionaryNotEqualWithContainsCheckExpression_Test()
        {
            var rule = new JsonNetFilterRule
            {
                Condition = "or",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Field = "DynamicData",
                        Id = "Id",
                        Operator = "not_contains",
                        Type = "string",
                        Value = "not_existing"
                    },
                    new JsonNetFilterRule
                    {
                        Field = "DynamicData.not_existing",
                        Id = "Id",
                        Operator = "not_equal",
                        Type = "string",
                        Value = "test"
                    },
                   
                }
            };
            var d1 = new DictionaryClass();
            d1.DynamicData.Add("test", "test");
            var d2 = new DictionaryClass();
            d2.DynamicData.Add("test", "NotTest" );

            var result = new List<DictionaryClass> { d1, d2 }.AsQueryable().BuildQuery(rule,
                new BuildExpressionOptions());
            ClassicAssert.IsTrue(result.Any());
            ClassicAssert.AreEqual(result.Count(), 2);
        }
        
        [Test]
        public void DictionaryExpression_Test_Fail()
        {
            var rule = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "DynamicData.test",
                        Id = "Id",
                        Operator = "equal",
                        Type = "string",
                        Value = "test"
                    }
                }
            };
            var d1 = new DictionaryClassFail();
            d1.DynamicData.Add(1, "test");
            var d2 = new DictionaryClassFail();
            d2.DynamicData.Add(1, "NotTest");

            

            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                var result = new List<DictionaryClassFail> { d1, d2 }.AsQueryable().BuildQuery(rule,
                    new BuildExpressionOptions());
            });
            
        }
        
        [Test]
        public void ObjectInExpression_Test()
        {
            var rule = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "DynamicData.test",
                        Id = "Id",
                        Operator = "in",
                        Type = "string",
                        Value = new List<string> {"tEst", "TEST2", "test3"}
                    }
                }
            };
            var d1 = new DictionaryClass();
            d1.DynamicData.Add("test", "test");
            var d2 = new DictionaryClass();
            d2.DynamicData.Add("test", "test2");
            var d3 = new DictionaryClass();
            d3.DynamicData.Add("test", "test4");

            var result = new List<DictionaryClass> { d1, d2, d3 }.AsQueryable().BuildQuery(rule,
                new BuildExpressionOptions());

            ClassicAssert.IsTrue(result.Any());
            ClassicAssert.AreEqual(result.Count(), 2);
        }
        
        [Test]
        public void CaseSensitiveExpression_Test()
        {
            var rule = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "DynamicData.test",
                        Id = "Id",
                        Operator = "in",
                        Type = "string",
                        Value = new List<string> {"tEst", "TEST2", "test3", "test4","test13"}
                    }
                }
            };
            var items = Enumerable.Range(0, 10).Select(counter => new DictionaryClass
            {
                DynamicData = new Dictionary<string, object> { ["test"] = $"test{counter}" }
            }).ToList();
            
            var result = items.AsQueryable().BuildQuery(rule,
                new BuildExpressionOptions {StringCaseSensitiveComparison = true});

            ClassicAssert.IsTrue(result.Any());
            ClassicAssert.AreEqual(result.Count(), 2);
        }
        
        private enum MyEnum
        {
            Test,
            Test2,
            Test3
        }
        private class ClassWithEnum
        {
            public MyEnum EnumProperty { get; set; }
        }
        
        [Test]
        public void EnumExpression_Test()
        {
            var rule = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "EnumProperty",
                        Id = "Id",
                        Operator = "in",
                        Type = "string",
                        Value = new List<string>{"Test2", "Test3"}
                    }
                }
            };

            var items = new List<ClassWithEnum>
            {
                new ClassWithEnum
                {
                    EnumProperty = MyEnum.Test
                },
                new ClassWithEnum
                {
                    EnumProperty = MyEnum.Test2
                }
            }; 
            
            var result = items.AsQueryable().BuildQuery(rule,
                new BuildExpressionOptions {StringCaseSensitiveComparison = true});

            ClassicAssert.IsTrue(result.Any());
            ClassicAssert.AreEqual(result.Count(), 1);
            
            var resultList = result.ToList();
            ClassicAssert.AreEqual(resultList.First().EnumProperty, MyEnum.Test2);
        }
        
        [Test]
        public void EnumExpressionCastedToInteger_Test()
        {
            var rule = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Condition = "and",
                        Field = "EnumProperty",
                        Id = "Id",
                        Operator = "equal",
                        Type = "integer",
                        Value = "1"
                    }
                }
            };

            var items = new List<ClassWithEnum>
            {
                new ClassWithEnum
                {
                    EnumProperty = MyEnum.Test
                },
                new ClassWithEnum
                {
                    EnumProperty = MyEnum.Test2
                }
            }; 
            
            var result = items.AsQueryable().BuildQuery(rule,
                new BuildExpressionOptions {StringCaseSensitiveComparison = true});

            ClassicAssert.IsTrue(result.Any());
            ClassicAssert.AreEqual(result.Count(), 1);
            
            var resultList = result.ToList();
            ClassicAssert.AreEqual(resultList.First().EnumProperty, MyEnum.Test2);
        }

        class ObjectsClass
        {
            public List<object> ListTest { get; set; }
            public object Test { get; set; }
        }
        [Test]
        public void ListObjectsInString_Test()
        {
            var rule = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Field = "ListTest",
                        Operator = "in",
                        Type = "string",
                        Value = new List<string> {"1","2","3","4"}
                    },
                    
                }
            };
            var items = Enumerable.Range(0, 10).Select(counter => new ObjectsClass
            {
                ListTest = new List<object> {$"{counter}"},
            }).ToList();
            
            var result = items.AsQueryable().BuildQuery(rule, new BuildExpressionOptions ());

            ClassicAssert.IsTrue(result.Any());
            ClassicAssert.AreEqual(result.Count(), 4);
        }
        
        [Test]
        public void ListObjectsInInteger_Test()
        {
            var rule = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Field = "ListTest",
                        Operator = "in",
                        Type = "integer",
                        Value = new List<int> {1,2,3,4}
                    },
                    
                }
            };
            var items = Enumerable.Range(0, 10).Select(counter => new ObjectsClass
            {
                ListTest = new List<object> {counter},
            }).ToList();
            
            var result = items.AsQueryable().BuildQuery(rule, new BuildExpressionOptions());

            ClassicAssert.IsTrue(result.Any());
            ClassicAssert.AreEqual(result.Count(), 4);
        }
        
        [Test]
        public void ObjectEqualInteger_Test()
        {
            var rule = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Field = "Test",
                        Operator = "equal",
                        Type = "integer",
                        Value = 5
                    },
                    
                }
            };
            var items = Enumerable.Range(0, 10).Select(counter => new ObjectsClass
            {
                Test = counter,
            }).ToList();
            
            var result = items.AsQueryable().BuildQuery(rule, new BuildExpressionOptions());

            ClassicAssert.IsTrue(result.Any());
            ClassicAssert.AreEqual(result.Count(), 1);
        }
        
        [Test]
        public void ObjectGreaterInteger_Test()
        {
            var rule = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Field = "Test",
                        Operator = "greater",
                        Type = "integer",
                        Value = 5
                    },
                    
                }
            };
            var items = Enumerable.Range(0, 10).Select(counter => new ObjectsClass
            {
                Test = counter,
            }).ToList();
            
            var result = items.AsQueryable().BuildQuery(rule, new BuildExpressionOptions());

            ClassicAssert.IsTrue(result.Any());
            ClassicAssert.AreEqual(result.Count(), 4);
        }
        
        [Test]
        public void ObjectEqualString_Test()
        {
            var rule = new JsonNetFilterRule
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>
                {
                    new JsonNetFilterRule
                    {
                        Field = "Test",
                        Operator = "equal",
                        Type = "string",
                        Value = "4"
                    },
                    
                }
            };
            var items = Enumerable.Range(0, 10).Select(counter => new ObjectsClass
            {
                Test = $"{counter}",
            }).ToList();
            
            var result = items.AsQueryable().BuildQuery(rule, new BuildExpressionOptions());

            ClassicAssert.IsTrue(result.Any());
            ClassicAssert.AreEqual(result.Count(), 1);
        }
     

       
        #endregion
    }
}
