using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Castle.DynamicLinqQueryBuilder.Tests.Helpers;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Castle.DynamicLinqQueryBuilder.Tests.Rules
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class Tests
    {
        #region Expression Tree Builder

        public class ExpressionTreeBuilderTestClass
        {
            public int ContentTypeId { get; set; }
            public int? NullableContentTypeId { get; set; }
            public Guid ContentTypeGuid { get; set; }
            public Guid? NullableContentTypeGuid { get; set; }
            public List<int> Enemies { get; set; }
            public List<string> Flags { get; set; }
            public string ContentTypeName { get; set; }
            public string LongerTextToFilter { get; set; }
            public bool IsSelected { get; set; }
            public bool? IsPossiblyNotSetBool { get; set; }
            public DateTime LastModified { get; set; }
            public DateTime? LastModifiedIfPresent { get; set; }
            public DateTime? NullableDateTime { get; set; }
            public DateTime? NullableLastModified { get { return null; } }
            public double StatValue { get; set; }
            public double? PossiblyEmptyStatValue { get; set; }
            public List<int> IntList { get; set; }
            public List<int?> NullableIntList { get; set; }
            public List<long> LongList { get; set; }
            public List<long?> NullableLongList { get; set; }
            public List<DateTime> DateList { get; set; }
            public List<DateTime?> NullDateList { get { return new List<DateTime?> { null }; } }
            public List<double> DoubleList { get; set; }
            public List<string> StrList { get; set; }
            public List<ChildClass> ChildClasses { get; set; }
            public List<ChildClass> NestedNullObjectChildClasses { get; set; }

            public Dictionary<string, object> Dictionary { get; set; } = new Dictionary<string, object>();
        }

        public class ChildClass
        {
            public string ClassName { get; set; }
            public ChildSubClass ChildSubClass { get; set; }
        }

        public class  ChildSubClass
        {
            public string ClassName { get; set; }
        }

        public static List<ExpressionTreeBuilderTestClass> GetDateExpressionTreeData()
        {
            var tData = new List<ExpressionTreeBuilderTestClass>();

            var entry1 = new ExpressionTreeBuilderTestClass()
            {
                ContentTypeId = 1,
                ContentTypeGuid = Guid.NewGuid(),
                ContentTypeName = "Multiple-Choice",
                Enemies = new List<int>(),
                Flags = new List<string>(),
                IsPossiblyNotSetBool = true,
                IsSelected = true,
                LastModified = DateTime.Parse("2/23/2016", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal).AddHours(3),
                LastModifiedIfPresent = DateTime.UtcNow.Date,
                LongerTextToFilter = "There is something interesting about this text",
                NullableContentTypeId = 1,
                PossiblyEmptyStatValue = null,
                StatValue = 1.11,
                IntList = new List<int>() { 1, 3, 5, 7 },
                StrList = new List<string>() { "Str1", "Str2" },
                DateList = new List<DateTime>() { DateTime.Parse("2/23/2016", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal).AddHours(2), DateTime.UtcNow.AddDays(-2) },
                DoubleList = new List<double>() { 1.48, 1.84, 1.33 },
                NullableIntList = new List<int?>() { 3, 4, 5, null }
            };
            tData.Add(entry1);

            return tData;
        }

        public static List<ExpressionTreeBuilderTestClass> GetExpressionTreeData()
        {
            var tData = new List<ExpressionTreeBuilderTestClass>();

            var entry1 = new ExpressionTreeBuilderTestClass()
            {
                ContentTypeId = 1,
                ContentTypeGuid = Guid.Empty,
                NullableContentTypeGuid = Guid.Empty,
                ContentTypeName = "Multiple-Choice",
                Enemies = null,
                Flags = new List<string>(),
                IsPossiblyNotSetBool = true,
                IsSelected = true,
                LastModified = DateTime.UtcNow.Date,
                LastModifiedIfPresent = DateTime.UtcNow.Date,
                LongerTextToFilter = "There is something interesting about this text",
                NullableContentTypeId = 1,
                PossiblyEmptyStatValue = null,
                StatValue = 1.11,
                IntList = new List<int>() { 1, 3, 5, 7 },
                LongList = new List<long>(),
                StrList = new List<string>() { "Str1", "Str2" },
                DateList = new List<DateTime>() { DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(-2) },
                DoubleList = new List<double>() { 1.48, 1.84, 1.33 },
                NullableIntList = new List<int?>() { 3, 4, 5, null },
                NullableLongList = new List<long?>() { null },
                NestedNullObjectChildClasses = new List<ChildClass>()
                {
                    new()
                    {
                        ChildSubClass = new ChildSubClass()
                        {
                            ClassName = "ChildSubClass"
                        }
                    }
                },
                Dictionary = new Dictionary<string, object> {
                    ["first_name"] = "Emma",
                    ["last_name"] = "Watson"
                }
            };
            tData.Add(entry1);

            var entry2 = new ExpressionTreeBuilderTestClass()
            {
                ContentTypeId = 2,
                ContentTypeGuid = Guid.NewGuid(),
                NullableContentTypeGuid = Guid.NewGuid(),
                ContentTypeName = "Multiple-Select",
                Enemies = null,
                Flags = null,
                IsPossiblyNotSetBool = false,
                IsSelected = false,
                LastModified = DateTime.UtcNow.Date,
                LastModifiedIfPresent = DateTime.UtcNow.Date,
                LongerTextToFilter = null,
                NullableContentTypeId = 2,
                PossiblyEmptyStatValue = 1.112,
                StatValue = 1.12,
                IntList = new List<int>() { 5, 7 },
                LongList = new List<long>() { 12, 14},
                StrList = new List<string>() { "Str1", "Str2" },
                DateList = new List<DateTime>() { DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(-2) },
                DoubleList = new List<double>() { 1.48, 1.84, 1.33 },
                NullableIntList = new List<int?>() { 3, 4, 5, null },
                NullableLongList = new List<long?>() { 5, 7, 9 },
                NestedNullObjectChildClasses = new List<ChildClass>()
                {
                    new(),
                    new()
                    {
                        ChildSubClass = new ChildSubClass()
                        {
                            ClassName = "className"
                        }
                    }
                }
            };
            tData.Add(entry2);

            var entry3 = new ExpressionTreeBuilderTestClass()
            {
                ContentTypeId = 3,
                ContentTypeGuid = Guid.NewGuid(),
                ContentTypeName = "Drag-and-Drop Item",
                Enemies = new List<int>() { 3391, 3985 },
                Flags = new List<string>() { "this is a flag" },
                IsPossiblyNotSetBool = null,
                IsSelected = true,
                LastModified = DateTime.UtcNow.Date,
                NullableDateTime = DateTime.UtcNow.AddDays(-1),
                LastModifiedIfPresent = null,
                LongerTextToFilter = "There is something interesting about this text",
                NullableContentTypeId = 3,
                PossiblyEmptyStatValue = null,
                StatValue = 1.13,
                IntList = new List<int>() { 1, 3, 5, 7 },
                LongList = new List<long>() { 8, 9, 10, 11 },
                StrList = new List<string>() { "Str1", "" },
                DateList = new List<DateTime>() { DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(-2) },
                DoubleList = new List<double>() { 1.48, 1.84, 1.33 },
                NullableIntList = new List<int?>() { 3, 4, 5, null },
                NullableLongList = new List<long?>() { 3, null, 5 },
                Dictionary = new Dictionary<string, object> {
                    ["first_name"] = "Madonna",
                }
            };
            tData.Add(entry3);

            var entry4 = new ExpressionTreeBuilderTestClass()
            {
                ContentTypeId = 1,
                ContentTypeGuid = Guid.NewGuid(),
                ContentTypeName = "Multiple-Choice",
                Enemies = new List<int>(),
                Flags = new List<string>() { "THIS IS A FLAG" },
                IsPossiblyNotSetBool = true,
                IsSelected = true,
                LastModified = DateTime.UtcNow.Date,
                NullableDateTime = DateTime.UtcNow,
                LastModifiedIfPresent = DateTime.UtcNow.Date,
                LongerTextToFilter = "THERE IS SOMETHING INTERESTING ABOUT THIS TEXT",
                NullableContentTypeId = null,
                PossiblyEmptyStatValue = 1.112,
                StatValue = 1.11,
                IntList = new List<int>() { 1, 3, 5, 7 },
                LongList = new List<long>() { 1, 3, 5, 7 },
                StrList = new List<string>() { "Str1", "Str2" },
                DateList = new List<DateTime>() { DateTime.UtcNow.Date },
                DoubleList = new List<double>() { 1.48 },
                NullableIntList = new List<int?>() { 3, 4, null, null },
                NullableLongList = new List<long?>() { 1, 3, null },
                Dictionary = new Dictionary<string, object> {
                    ["first_name"] = "Emma",
                    ["last_name"] = "Stone"
                }
            };
            tData.Add(entry4);


            return tData;
        }

        [Test]
        public void DateHandling()
        {
            QueryBuilder.ParseDatesAsUtc = true;
            var startingQuery = GetDateExpressionTreeData().AsQueryable();
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var queryable = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter);
            var contentIdFilteredList = queryable.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 1);

            contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
                var contentIdFilteredListNull1 = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            });

            contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "equal",
                        Type = "date",
                        Value = "23/02/2016"
                    }
                }
            };
            ExceptionAssert.Throws<Exception>(() =>
            {
                var contentIdFilteredListNull1 = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter, new BuildExpressionOptions()
                {
                    CultureInfo = CultureInfo.InvariantCulture,
                    ParseDatesAsUtc = true,
                    Operators = new List<IFilterOperator>()
                }).ToList();
            });


            contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            queryable = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter);
            var contentIdFilteredList2 = queryable.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList2 != null);
            ClassicAssert.IsTrue(contentIdFilteredList2.Count == 1);

            contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
                var contentIdFilteredListNull2 = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            QueryBuilder.ParseDatesAsUtc = false;
        }

        [Test]
        public void IsNullSubCollection()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect no entries to match
            var contentNullFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "ChildClasses.ClassName",
                        Id = "ChildClasses.ClassName",
                        Input = "NA",
                        Operator = "equal",
                        Type = "string",
                        Value = new[] { "books" }
                    }
                }
            };
            var contentNullFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentNullFilter, new BuildExpressionOptions()
            {
                NullCheckNestedCLRObjects = true
            }).ToList();
            ClassicAssert.IsTrue(contentNullFilteredList != null);
            ClassicAssert.IsTrue(contentNullFilteredList.Count == 0);
        }

        [Test]
        public void IsNullNestedClassInSubCollection()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect no entries to match
            var contentNullFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "NestedNullObjectChildClasses.ChildSubClass.ClassName",
                        Id = "NestedNullObjectChildClasses.ChildSubClass.ClassName",
                        Input = "NA",
                        Operator = "equal",
                        Type = "string",
                        Value = new[] { "books" }
                    }
                }
            };
            var contentNullFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentNullFilter, new BuildExpressionOptions()
            {
                NullCheckNestedCLRObjects = true
            }).ToList();
            ClassicAssert.IsTrue(contentNullFilteredList != null);
            ClassicAssert.IsTrue(contentNullFilteredList.Count == 0);
        }

        [Test]
        public void InClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            var firstGuid = startingQuery.First().ContentTypeGuid.ToString();
            //expect one entry to match for a Guid Comparison
            var contentGuidFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeGuid",
                        Id = "ContentTypeGuid",
                        Input = "NA",
                        Operator = "in",
                        Type = "guid",
                        Value = new[] { firstGuid, firstGuid }
                    }
                }
            };
            var contentGuidFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentGuidFilter).ToList();
            ClassicAssert.IsTrue(contentGuidFilteredList != null);
            ClassicAssert.IsTrue(contentGuidFilteredList.Count == 1);

            //expect no entry to match for a Guid Comparison against a null nullable Id
            var nullableContentGuidFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeGuid",
                        Id = "NullableContentTypeGuid",
                        Input = "NA",
                        Operator = "in",
                        Type = "guid",
                        Value = new[] { firstGuid }
                    }
                }
            };
            var nullableContentGuidFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentGuidFilter).ToList();
            ClassicAssert.IsTrue(nullableContentGuidFilteredList != null);
            ClassicAssert.IsTrue(contentGuidFilteredList.Count == 1);

            //expect two entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = "[1,2]"
                    }
                }
            };
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 3);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => (new List<int>() { 1, 2 }).Contains(p.ContentTypeId)));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //single value test
            contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = "[1]"
                    }
                }
            };
            contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => (new List<int>() { 1 }).Contains(p.ContentTypeId)));

            //expect two entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = "[1,2]"
                    }
                }
            };
            var nullableContentIdFilteredList =
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(
                nullableContentIdFilteredList.All(p => (new List<int>() { 1, 2 }).Contains(p.NullableContentTypeId.Value)));

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "in",
                        Type = "string",
                        Value = "there is something interesting about this text,there is something interesting about this text2"
                    }
                }
            };
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 3);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p == "there is something interesting about this text"));

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilterCaps = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LongerTextToFilter",
                        Id = "LongerTextToFilter",
                        Input = "NA",
                        Operator = "in",
                        Type = "string",
                        Value = "THERE is something interesting about this text,there is something interesting about this text2"
                    }
                }
            };
            var longerTextToFilterListCaps = startingQuery.BuildQuery(longerTextToFilterFilterCaps).ToList();
            ClassicAssert.IsTrue(longerTextToFilterListCaps != null);
            ClassicAssert.IsTrue(longerTextToFilterListCaps.Count == 3);
            ClassicAssert.IsTrue(
                longerTextToFilterListCaps.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p == "there is something interesting about this text"));


            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "in",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 4);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => p == DateTime.UtcNow.Date));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "in",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => p == DateTime.UtcNow.Date));


            //expect 2 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "in",
                        Type = "double",
                        Value = "1.11,1.12"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 3);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (new List<double>() { 1.11, 1.12 }).Contains(p)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable double field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "in",
                        Type = "double",
                        Value = "1.112, 1.113"
                    }
                }
            };
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p == 1.112));

            //expect 2 entries to match for a List<DateTime> field
            var dateListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var dateListFilterList = startingQuery.ToList().BuildQuery(dateListFilter).ToList();
            ClassicAssert.IsTrue(dateListFilterList != null);
            ClassicAssert.IsTrue(dateListFilterList.Count == 3);
            ClassicAssert.IsTrue(dateListFilterList.All(p => p.DateList.Contains(DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                dateListFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(dateListFilter).ToList();

            });

            //expect 2 entries to match for a List<string> field
            var strListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var strListFilterList = startingQuery.AsEnumerable().BuildQuery(strListFilter).ToList();
            ClassicAssert.IsTrue(strListFilterList != null);
            ClassicAssert.IsTrue(strListFilterList.Count == 3);
            ClassicAssert.IsTrue(strListFilterList.All(p => p.StrList.Contains("Str2")));
            
            //expect 2 entries to match for a List<int> field
            var intListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "IntList",
                        Id = "IntList",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = "1,3"
                    }
                }
            };
            var intListFilterList = startingQuery.BuildQuery(intListFilter).ToList();
            ClassicAssert.IsTrue(intListFilterList != null);
            ClassicAssert.IsTrue(intListFilterList.Count == 3);
            ClassicAssert.IsTrue(intListFilterList.All(p => p.IntList.Contains(1) || p.IntList.Contains(3)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                intListFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(intListFilter).ToList();

            });

            //expect 2 entries to match for a nullable nullable int field
            var nullableIntListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableIntList",
                        Id = "NullableIntList",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = "5"
                    }
                }
            };
            var nullableIntListList = startingQuery.BuildQuery(nullableIntListFilter).ToList();
            ClassicAssert.IsTrue(nullableIntListList != null);
            ClassicAssert.IsTrue(nullableIntListList.Count == 3);
            ClassicAssert.IsTrue(
                nullableIntListList.All(p => p.NullableIntList.Contains(5)));


            startingQuery = GetExpressionTreeData().AsQueryable();
            var multipleWithBlankRule = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StrList",
                        Id = "StrList",
                        Input = "NA",
                        Operator = "in",
                        Type = "string",
                        Value = "[,Str2]"
                    }
                }
            };
            var multipleWithBlankList = startingQuery.BuildQuery(multipleWithBlankRule).ToList();
            ClassicAssert.IsTrue(multipleWithBlankList != null);
            ClassicAssert.IsTrue(multipleWithBlankList.Count == 4);
            ClassicAssert.IsTrue(
                multipleWithBlankList.All(p => p.StrList.Contains("") || p.StrList.Contains("Str2")));
        }

        [Test]
        public void NotInClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            var firstGuid = startingQuery.First().ContentTypeGuid.ToString();
            var contentGuidFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeGuid",
                        Id = "ContentTypeGuid",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "guid",
                        Value = new[] { firstGuid, firstGuid }
                    }
                }
            };
            var contentGuidFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentGuidFilter).ToList();
            ClassicAssert.IsNotNull(contentGuidFilteredList);
            CollectionAssert.IsNotEmpty(contentGuidFilteredList);
            CollectionAssert.DoesNotContain(contentGuidFilteredList.Select(x => x.ContentTypeGuid), firstGuid);

            //expect two entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "integer",
                        Value = "[1,2]"
                    }
                }
            };
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 1);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => (new List<int>() { 3 }).Contains(p.ContentTypeId)));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //expect two entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "integer",
                        Value = "[1,2]"
                    }
                }
            };
            var nullableContentIdFilteredList =
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(
                nullableContentIdFilteredList.All(p => !(new List<int>() { 1, 2 }).Contains(p.NullableContentTypeId.GetValueOrDefault())));




            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 1);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p == null || p != "there is something interesting about this text"));


            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 0);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => p == DateTime.UtcNow.Date));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.AddDays(1).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 1);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => p != DateTime.UtcNow.Date));


            //expect 2 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "double",
                        Value = "1.11,1.12"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 1);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => !(new List<double>() { 1.11, 1.12 }).Contains(p)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable double field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "double",
                        Value = "1.112, 1.113"
                    }
                }
            };
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p != 1.112));

            //expect 2 entries to match for a List<DateTime> field
            var dateListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var dateListFilterList = startingQuery.BuildQuery(dateListFilter).ToList();
            ClassicAssert.IsTrue(dateListFilterList != null);
            ClassicAssert.IsTrue(dateListFilterList.Count == 1);
            ClassicAssert.IsTrue(dateListFilterList.All(p => !p.DateList.Contains(DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                dateListFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(dateListFilter).ToList();

            });

            //expect 2 entries to match for a List<string> field
            var strListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var strListFilterList = startingQuery.BuildQuery(strListFilter).ToList();
            ClassicAssert.IsTrue(strListFilterList != null);
            ClassicAssert.IsTrue(strListFilterList.Count == 1);
            ClassicAssert.IsTrue(strListFilterList.All(p => !p.StrList.Contains("Str2")));
            
            //expect 2 entries to match for a List<int> field
            var intListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "IntList",
                        Id = "IntList",
                        Input = "NA",
                        Operator = "not_in",
                        Type = "integer",
                        Value = "1,3"
                    }
                }
            };
            var intListFilterList = startingQuery.BuildQuery(intListFilter).ToList();
            ClassicAssert.IsTrue(intListFilterList != null);
            ClassicAssert.IsTrue(intListFilterList.Count == 1);
            ClassicAssert.IsTrue(intListFilterList.All(p => !p.IntList.Contains(1) && !p.IntList.Contains(3)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                intListFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(intListFilter).ToList();

            });

            //expect 2 entries to match for a nullable nullable int field
            var nullableIntListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var nullableIntListList = startingQuery.BuildQuery(nullableIntListFilter).ToList();
            ClassicAssert.IsTrue(nullableIntListList != null);
            ClassicAssert.IsTrue(nullableIntListList.Count == 1);
            ClassicAssert.IsTrue(
                nullableIntListList.All(p => !p.NullableIntList.Contains(5)));
        }

        [Test]
        public void IsNullClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            var contentGuidFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeGuid",
                        Id = "ContentTypeGuid",
                        Input = "NA",
                        Operator = "is_null",
                        Type = "guid",
                        Value = ""
                    }
                }
            };
            var contentGuidFilteredList = startingQuery.BuildQuery(contentGuidFilter).ToList();
            ClassicAssert.IsNotNull(contentGuidFilteredList);
            CollectionAssert.IsEmpty(contentGuidFilteredList);
            ClassicAssert.IsTrue(
                contentGuidFilteredList.Select(p => p.ContentTypeGuid)
                    .All(p => p == null));

            var nullableContentGuidFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeGuid",
                        Id = "NullableContentTypeGuid",
                        Input = "NA",
                        Operator = "is_null",
                        Type = "guid",
                        Value = ""
                    }
                }
            };
            var nullableContentGuidFilteredList = startingQuery.BuildQuery(nullableContentGuidFilter).ToList();
            ClassicAssert.IsNotNull(nullableContentGuidFilteredList);
            CollectionAssert.IsNotEmpty(nullableContentGuidFilteredList);
            ClassicAssert.AreEqual(2, nullableContentGuidFilteredList.Count);
            ClassicAssert.IsTrue(
                nullableContentGuidFilteredList.Select(p => p.NullableContentTypeGuid)
                    .All(p => p == null));


            //expect 1 entries to match for a case-insensitive string comparison (nullable type)
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 1);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p == null));


            //expect 0 entries to match for a non-nullable type
            var contentTypeIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var contentTypeIdFilterList = startingQuery.BuildQuery(contentTypeIdFilter).ToList();
            ClassicAssert.IsTrue(contentTypeIdFilterList != null);
            ClassicAssert.IsTrue(contentTypeIdFilterList.Count == 0);
            ClassicAssert.IsTrue(
                contentTypeIdFilterList.Select(p => p.ContentTypeId)
                    .All(p => p == 0));

        }

        [Test]
        public void IsNotNullClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            var contentGuidFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeGuid",
                        Id = "ContentTypeGuid",
                        Input = "NA",
                        Operator = "is_not_null",
                        Type = "guid",
                        Value = ""
                    }
                }
            };
            var contentGuidFilterList = startingQuery.BuildQuery(contentGuidFilter).ToList();
            ClassicAssert.IsNotNull(contentGuidFilterList);
            ClassicAssert.AreEqual(4, contentGuidFilterList.Count);
            CollectionAssert.AllItemsAreNotNull(contentGuidFilterList.Select(x => x.ContentTypeGuid));


            var nullableContentGuidFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeGuid",
                        Id = "NullableContentTypeGuid",
                        Input = "NA",
                        Operator = "is_not_null",
                        Type = "guid",
                        Value = ""
                    }
                }
            };
            var nullableContentGuidFilteredList = startingQuery.BuildQuery(nullableContentGuidFilter).ToList();
            ClassicAssert.IsNotNull(nullableContentGuidFilteredList);
            ClassicAssert.AreEqual(2, nullableContentGuidFilteredList.Count);
            CollectionAssert.AllItemsAreNotNull(nullableContentGuidFilteredList.Select(x => x.NullableContentTypeGuid));


            //expect 3 entries to match for a case-insensitive string comparison (nullable type)
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 3);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p != null));


            //expect 0 entries to match for a non-nullable type
            var contentTypeIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var contentTypeIdFilterList = startingQuery.BuildQuery(contentTypeIdFilter).ToList();
            ClassicAssert.IsTrue(contentTypeIdFilterList != null);
            ClassicAssert.IsTrue(contentTypeIdFilterList.Count == 4);
            ClassicAssert.IsTrue(
                contentTypeIdFilterList.Select(p => p.ContentTypeId)
                    .All(p => p != 0));

        }

        [Test]
        public void IsEmptyClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            var contentGuidFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeGuid",
                        Id = "ContentTypeGuid",
                        Input = "NA",
                        Operator = "is_empty",
                        Type = "guid",
                        Value = ""
                    }
                }
            };
            var contentGuidFilterList = startingQuery.BuildQuery(contentGuidFilter).ToList();
            ClassicAssert.IsNotNull(contentGuidFilterList);
            CollectionAssert.IsNotEmpty(contentGuidFilterList);
            ClassicAssert.AreEqual(1, contentGuidFilterList.Count);
            ClassicAssert.IsTrue(
                contentGuidFilterList.Select(p => p.NullableContentTypeGuid)
                    .All(p => p == Guid.Empty));


            var nullContentGuidFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeGuid",
                        Id = "NullableContentTypeGuid",
                        Input = "NA",
                        Operator = "is_empty",
                        Type = "guid",
                        Value = ""
                    }
                }
            };
            var nullContentGuidFilterList = startingQuery.BuildQuery(nullContentGuidFilter).ToList();
            ClassicAssert.IsNotNull(nullContentGuidFilterList);
            CollectionAssert.IsNotEmpty(nullContentGuidFilterList);
            ClassicAssert.AreEqual(1, nullContentGuidFilterList.Count);
            ClassicAssert.IsTrue(
                nullContentGuidFilterList.Select(p => p.NullableContentTypeGuid)
                    .All(p => p == Guid.Empty));


            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 0);


            //expect 2 entries to match for a List<DateTime> field
            var dateListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {                        Condition = "and",
                        Field = "DateList",
                        Id = "DateList",
                        Input = "NA",
                        Operator = "is_empty",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var dateListFilterList = startingQuery.BuildQuery(dateListFilter).ToList();
            ClassicAssert.IsTrue(dateListFilterList != null);
            ClassicAssert.IsTrue(dateListFilterList.Count == 0);
            //ClassicAssert.IsTrue(dateListFilterList.All(p => !p.DateList.Contains(DateTime.UtcNow.Date.AddDays(-2))));


            //expect 2 entries to match for a List<string> field
            var strListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var strListFilterList = startingQuery.BuildQuery(strListFilter).ToList();
            ClassicAssert.IsTrue(strListFilterList != null);
            ClassicAssert.IsTrue(strListFilterList.Count == 0);
            //ClassicAssert.IsTrue(strListFilterList.All(p => !p.StrList.Contains("Str2")));


            //expect 2 entries to match for a List<int> field
            var intListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "IntList",
                        Id = "IntList",
                        Input = "NA",
                        Operator = "is_empty",
                        Type = "integer",
                        Value = "1,3"
                    }
                }
            };
            var intListFilterList = startingQuery.BuildQuery(intListFilter).ToList();
            ClassicAssert.IsTrue(intListFilterList != null);
            ClassicAssert.IsTrue(intListFilterList.Count == 0);
        }

        [Test]
        public void IsNotEmptyClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            var contentGuidFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeGuid",
                        Id = "ContentTypeGuid",
                        Input = "NA",
                        Operator = "is_not_empty",
                        Type = "guid",
                        Value = ""
                    }
                }
            };
            var contentGuidFilterList = startingQuery.BuildQuery(contentGuidFilter).ToList();
            ClassicAssert.IsNotNull(contentGuidFilterList);
            CollectionAssert.IsNotEmpty(contentGuidFilterList);
            ClassicAssert.AreEqual(3, contentGuidFilterList.Count);
            ClassicAssert.IsTrue(
                            contentGuidFilterList.Select(p => p.ContentTypeGuid)
                                .All(p => p != Guid.Empty));

            var nullContentGuidFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeGuid",
                        Id = "NullableContentTypeGuid",
                        Input = "NA",
                        Operator = "is_not_empty",
                        Type = "guid",
                        Value = ""
                    }
                }
            };
            var nullContentGuidFilterList = startingQuery.BuildQuery(nullContentGuidFilter).ToList();
            ClassicAssert.IsNotNull(nullContentGuidFilterList);
            CollectionAssert.IsNotEmpty(nullContentGuidFilterList);
            ClassicAssert.AreEqual(1, nullContentGuidFilterList.Count);
            CollectionAssert.AllItemsAreNotNull(nullContentGuidFilterList.Select(z => z.NullableContentTypeGuid));
            ClassicAssert.IsTrue(
                nullContentGuidFilterList.Select(p => p.NullableContentTypeGuid)
                    .All(p => p != null && p != Guid.Empty));


            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 4);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p == null || p.Length > 0));


            //expect 2 entries to match for a List<DateTime> field
            var dateListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var dateListFilterList = startingQuery.BuildQuery(dateListFilter).ToList();
            ClassicAssert.IsTrue(dateListFilterList != null);
            ClassicAssert.IsTrue(dateListFilterList.Count == 4);
            //ClassicAssert.IsTrue(dateListFilterList.All(p => !p.DateList.Contains(DateTime.UtcNow.Date.AddDays(-2))));



            //expect 2 entries to match for a List<string> field
            var strListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var strListFilterList = startingQuery.BuildQuery(strListFilter).ToList();
            ClassicAssert.IsTrue(strListFilterList != null);
            ClassicAssert.IsTrue(strListFilterList.Count == 4);
            //ClassicAssert.IsTrue(strListFilterList.All(p => !p.StrList.Contains("Str2")));


            //expect 2 entries to match for a List<int> field
            var intListFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "IntList",
                        Id = "IntList",
                        Input = "NA",
                        Operator = "is_not_empty",
                        Type = "integer",
                        Value = "1,3"
                    }
                }
            };
            var intListFilterList = startingQuery.BuildQuery(intListFilter).ToList();
            ClassicAssert.IsTrue(intListFilterList != null);
            ClassicAssert.IsTrue(intListFilterList.Count == 4);

        }

        [Test]
        public void ContainsClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            //expect at least one entry to match for a Guid Comparison
            var contentGuidFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeGuid",
                        Id = "ContentTypeGuid",
                        Input = "NA",
                        Operator = "contains",
                        Type = "guid",
                        Value = new[] { startingQuery.First().ContentTypeGuid.ToString().Substring(0,5) }
                    }
                }
            };
            var contentGuidFilteredList = startingQuery.BuildQuery(contentGuidFilter).ToList();
            ClassicAssert.IsTrue(contentGuidFilteredList != null);
            ClassicAssert.IsTrue(contentGuidFilteredList.Count >= 1);


            //expect no entry to match for a Guid Comparison against a null nullable Id
            var nullableContentGuidFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeGuid",
                        Id = "NullableContentTypeGuid",
                        Input = "NA",
                        Operator = "contains",
                        Type = "guid",
                        Value = new[] { startingQuery.First().ContentTypeGuid.ToString().Substring(0, 5) }
                    }
                }
            };
            var nullableContentGuidFilteredList = startingQuery.BuildQuery(nullableContentGuidFilter).ToList();
            ClassicAssert.IsTrue(nullableContentGuidFilteredList != null);
            ClassicAssert.IsTrue(nullableContentGuidFilteredList.Count == 1);

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 3);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p.Contains("something interesting")));

        }

        [Test]
        public void NotContainsClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 1);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p == null));

        }

        [Test]
        public void NotEndsWithClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 1);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p == null));

        }

        [Test]
        public void EndsWithClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            var fristGuid = startingQuery.First().ContentTypeGuid.ToString();
            //expect at least one entry to match for a Guid Comparison
            var contentGuidFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeGuid",
                        Id = "ContentTypeGuid",
                        Input = "NA",
                        Operator = "ends_with",
                        Type = "guid",
                        Value = new[] { fristGuid.Substring(fristGuid.Length - 5) }
                    }
                }
            };
            var contentGuidFilteredList = startingQuery.BuildQuery(contentGuidFilter).ToList();
            ClassicAssert.IsTrue(contentGuidFilteredList != null);
            ClassicAssert.IsTrue(contentGuidFilteredList.Count >= 1);

            //expect no entry to match for a Guid Comparison against a null nullable Id
            var nullableContentGuidFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeGuid",
                        Id = "NullableContentTypeGuid",
                        Input = "NA",
                        Operator = "ends_with",
                        Type = "guid",
                        Value = new[] { fristGuid.Substring(fristGuid.Length - 5) }
                    }
                }
            };
            var nullableContentGuidFilteredList = startingQuery.BuildQuery(nullableContentGuidFilter).ToList();
            ClassicAssert.IsTrue(nullableContentGuidFilteredList != null);
            ClassicAssert.IsTrue(nullableContentGuidFilteredList.Count == 1);

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 3);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p.EndsWith("about this text")));

        }

        [Test]
        public void NotBeginsWithClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            //expect 1 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 1);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => p == null));

        }

        [Test]
        public void BeginsWithClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            var firstGuid = startingQuery.First().ContentTypeGuid.ToString();
            //expect at least one entry to match for a Guid Comparison
            var contentGuidFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeGuid",
                        Id = "ContentTypeGuid",
                        Input = "NA",
                        Operator = "begins_with",
                        Type = "guid",
                        Value = new[] { firstGuid.Substring(0,5) }
                    }
                }
            };
            var contentGuidFilteredList = startingQuery.BuildQuery(contentGuidFilter).ToList();
            ClassicAssert.IsTrue(contentGuidFilteredList != null);
            ClassicAssert.IsTrue(contentGuidFilteredList.Count >= 1);

            //expect no entry to match for a Guid Comparison against a null nullable Id
            var nullableContentGuidFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeGuid",
                        Id = "NullableContentTypeGuid",
                        Input = "NA",
                        Operator = "begins_with",
                        Type = "guid",
                        Value = new[] { firstGuid.Substring(0,5) }
                    }
                }
            };
            var nullableContentGuidFilteredList = startingQuery.BuildQuery(nullableContentGuidFilter).ToList();
            ClassicAssert.IsTrue(nullableContentGuidFilteredList != null);
            ClassicAssert.IsTrue(nullableContentGuidFilteredList.Count == 1);

            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 3);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p.StartsWith("there is something")));

        }

        [Test]
        public void EqualsClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();

            //expect one entry to match for a Guid Comparison
            var contentGuidFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "ContentTypeGuid",
                        Id = "ContentTypeGuid",
                        Input = "NA",
                        Operator = "equal",
                        Type = "string",
                        Value = new[] { startingQuery.First().ContentTypeGuid.ToString() }
                    }
                }
            };
            var contentGuidFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentGuidFilter).ToList();
            ClassicAssert.IsTrue(contentGuidFilteredList != null);
            ClassicAssert.IsTrue(contentGuidFilteredList.Count == 1);

            //expect no entry to match for a Guid Comparison against a null nullable Id
            var nullableContentGuidFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "NullableContentTypeGuid",
                        Id = "NullableContentTypeGuid",
                        Input = "NA",
                        Operator = "equal",
                        Type = "string",
                        Value = new[] { startingQuery.First().ContentTypeGuid.ToString() }
                    }
                }
            };
            var nullableContentGuidFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentGuidFilter).ToList();
            ClassicAssert.IsTrue(nullableContentGuidFilteredList != null);
            ClassicAssert.IsTrue(nullableContentGuidFilteredList.Count == 1);


            //expect two entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId == 1));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //expect two entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 1);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId == 1));




            //expect 3 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 3);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter.ToLower())
                    .All(p => p == "there is something interesting about this text"));


            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 4);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => p == DateTime.UtcNow.Date));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => p == DateTime.UtcNow.Date));







            //expect 3 entries to match for a boolean field
            var isSelectedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "IsSelected",
                        Id = "IsSelected",
                        Input = "NA",
                        Operator = "equal",
                        Type = "boolean",
                        Value = "true"
                    }
                }
            };
            var isSelectedFilterList = startingQuery.BuildQuery(isSelectedFilter).ToList();
            ClassicAssert.IsTrue(isSelectedFilterList != null);
            ClassicAssert.IsTrue(isSelectedFilterList.Count == 3);
            ClassicAssert.IsTrue(
                isSelectedFilterList.Select(p => p.IsSelected)
                    .All(p => p == true));

            //expect failure when an invalid bool is encountered in bool comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                isSelectedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(isSelectedFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableIsSelectedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "IsPossiblyNotSetBool",
                        Id = "IsPossiblyNotSetBool",
                        Input = "NA",
                        Operator = "equal",
                        Type = "boolean",
                        Value = "true"
                    }
                }
            };
            var nullableIsSelectedFilterList = startingQuery.BuildQuery(nullableIsSelectedFilter).ToList();
            ClassicAssert.IsTrue(nullableIsSelectedFilterList != null);
            ClassicAssert.IsTrue(nullableIsSelectedFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableIsSelectedFilterList.Select(p => p.IsPossiblyNotSetBool)
                    .All(p => p == true));


            //expect 2 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "equal",
                        Type = "double",
                        Value = "1.11"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 2);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => p == 1.11));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "equal",
                        Type = "double",
                        Value = "1.112"
                    }
                }
            };
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p == 1.112));



        }

        [Test]
        public void NotEqualsClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect two entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "integer",
                        Value = "1"
                    }
                }
            };
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId != 1));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //expect 3 entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "integer",
                        Value = "1"
                    }
                }
            };
            var nullableContentIdFilteredList =
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 3);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId != 1));




            //expect 1 entries to match for a case-insensitive string comparison
            var longerTextToFilterFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var longerTextToFilterList = startingQuery.BuildQuery(longerTextToFilterFilter).ToList();
            ClassicAssert.IsTrue(longerTextToFilterList != null);
            ClassicAssert.IsTrue(longerTextToFilterList.Count == 1);
            ClassicAssert.IsTrue(
                longerTextToFilterList.Select(p => p.LongerTextToFilter)
                    .All(p => (p == null) || (p.ToLower() != "there is something interesting about this text")));


            //expect 0 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 0);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => p == DateTime.UtcNow.Date));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 1 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 1);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => p != DateTime.UtcNow.Date));


            //expect 1 entries to match for a boolean field
            var isSelectedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "IsSelected",
                        Id = "IsSelected",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "boolean",
                        Value = "true"
                    }
                }
            };
            var isSelectedFilterList = startingQuery.BuildQuery(isSelectedFilter).ToList();
            ClassicAssert.IsTrue(isSelectedFilterList != null);
            ClassicAssert.IsTrue(isSelectedFilterList.Count == 1);
            ClassicAssert.IsTrue(
                isSelectedFilterList.Select(p => p.IsSelected)
                    .All(p => p != true));

            //expect failure when an invalid bool is encountered in bool comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                isSelectedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(isSelectedFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableIsSelectedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "IsPossiblyNotSetBool",
                        Id = "IsPossiblyNotSetBool",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "boolean",
                        Value = "true"
                    }
                }
            };
            var nullableIsSelectedFilterList = startingQuery.BuildQuery(nullableIsSelectedFilter).ToList();
            ClassicAssert.IsTrue(nullableIsSelectedFilterList != null);
            ClassicAssert.IsTrue(nullableIsSelectedFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableIsSelectedFilterList.Select(p => p.IsPossiblyNotSetBool)
                    .All(p => p != true));


            //expect 2 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "double",
                        Value = "1.11"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 2);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => p != 1.11));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "not_equal",
                        Type = "double",
                        Value = "1.112"
                    }
                }
            };
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p != 1.112));

        }

        [Test]
        public void BetweenDatesCultureClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            
            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new JsonNetFilterRule()
            {
                Condition = "and",
                Rules = new List<JsonNetFilterRule>()
                {
                    new JsonNetFilterRule()
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "in",
                        Type = "date",
                        Value = DateTime.UtcNow.Date.AddDays(-2)
                    }
                }
            };
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter, new BuildExpressionOptions()
            {
                CultureInfo = CultureInfo.CurrentCulture,
                ParseDatesAsUtc = true
            }).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 0);
            

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter, new BuildExpressionOptions()
                {
                    CultureInfo = CultureInfo.InvariantCulture
                }).ToList();

            });
            
        }



        [Test]
        public void BetweenClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect 3 entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "between",
                        Type = "integer",
                        Value = "1,2"
                    }
                }
            };
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 3);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId < 3));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //expect 2 entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "between",
                        Type = "integer",
                        Value = "1,2"
                    }
                }
            };
            var nullableContentIdFilteredList =
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId < 3));





            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "between",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 4);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => (p >= DateTime.UtcNow.Date.AddDays(-2)) && (p <= DateTime.UtcNow.Date)));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "between",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p >= DateTime.UtcNow.Date.AddDays(-2)) && (p <= DateTime.UtcNow.Date)));


            //expect 3 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "between",
                        Type = "double",
                        Value = "1.0,1.12"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 3);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (p >= 1.0) && (p <= 1.12)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "between",
                        Type = "double",
                        Value = "1.112,1.112"
                    }
                }
            };
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p == 1.112));

        }

        [Test]
        public void NotBetweenClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect 1 entry to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "integer",
                        Value = "1,2"
                    }
                }
            };
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 1);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId < 1 || p.ContentTypeId > 2));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //expect 2 entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "integer",
                        Value = "1,2"
                    }
                }
            };
            var nullableContentIdFilteredList =
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId < 1 || p.NullableContentTypeId > 2 || p.NullableContentTypeId == null));





            //expect 0 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 0);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(-2)) && (p >= DateTime.UtcNow.Date)));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 1 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModifiedIfPresent",
                        Id = "LastModifiedIfPresent",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "datetime",
                        Value = DateTime.UtcNow.Date.AddDays(-2).ToString("d", CultureInfo.InvariantCulture) + "," + DateTime.UtcNow.Date.ToString("d", CultureInfo.InvariantCulture)
                    }
                }
            };
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 1);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(-2) && p >= DateTime.UtcNow.Date) || p == null));


            //expect 3 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "not_between",
                        Type = "double",
                        Value = "1.0,1.12"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 1);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (p <= 1.0) || (p >= 1.12)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p != 1.112));

        }

        [Test]
        public void GreaterOrEqualClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect 1 entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "greater_or_equal",
                        Type = "integer",
                        Value = "2"
                    }
                }
            };
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId >= 2));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //expect 1 entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "greater_or_equal",
                        Type = "integer",
                        Value = "2"
                    }
                }
            };
            var nullableContentIdFilteredList =
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId >= 2));


            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 4);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => (p >= DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 0 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 0);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p >= DateTime.UtcNow.Date.AddDays(1))));


            //expect 4 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "greater_or_equal",
                        Type = "double",
                        Value = "1"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 4);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (p >= 1)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "greater_or_equal",
                        Type = "double",
                        Value = "1.112"
                    }
                }
            };
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p >= 1.112));

        }

        [Test]
        public void GreaterClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect 1 entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "greater",
                        Type = "integer",
                        Value = "2"
                    }
                }
            };
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 1);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId > 2));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //expect 1 entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "greater",
                        Type = "integer",
                        Value = "2"
                    }
                }
            };
            var nullableContentIdFilteredList =
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 1);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId > 2));


            //expect 4 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 4);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => (p > DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 0 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 0);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p > DateTime.UtcNow.Date.AddDays(1))));


            //expect 4 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "greater",
                        Type = "double",
                        Value = "1"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 4);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (p > 1)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "PossiblyEmptyStatValue",
                        Id = "PossiblyEmptyStatValue",
                        Input = "NA",
                        Operator = "greater",
                        Type = "double",
                        Value = "1.112"
                    }
                }
            };
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 0);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p > 1.112));

        }

        [Test]
        public void LessClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect 2 entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "less",
                        Type = "integer",
                        Value = "2"
                    }
                }
            };
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId < 2));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //expect 1 entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "NullableContentTypeId",
                        Id = "NullableContentTypeId",
                        Input = "NA",
                        Operator = "less",
                        Type = "integer",
                        Value = "2"
                    }
                }
            };
            var nullableContentIdFilteredList =
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 1);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId < 2));


            //expect 0 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 0);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(1))));


            //expect 3 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "StatValue",
                        Id = "StatValue",
                        Input = "NA",
                        Operator = "less",
                        Type = "double",
                        Value = "1.13"
                    }
                }
            };
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 3);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (p <= 1.12)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable boolean field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p < 1.113));

        }

        [Test]
        public void LessOrEqualClause()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect 3 entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var contentIdFilteredList = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 3);
            ClassicAssert.IsTrue(contentIdFilteredList.All(p => p.ContentTypeId <= 2));

            //expect failure when non-numeric value is encountered in integer comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            //expect 2 entries to match for an integer comparison
            var nullableContentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(nullableContentIdFilter).ToList();
            ClassicAssert.IsTrue(nullableContentIdFilteredList != null);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.Count == 2);
            ClassicAssert.IsTrue(nullableContentIdFilteredList.All(p => p.NullableContentTypeId <= 2));


            //expect 0 entries to match for a Date comparison
            var lastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var lastModifiedFilterList = startingQuery.BuildQuery(lastModifiedFilter).ToList();
            ClassicAssert.IsTrue(lastModifiedFilterList != null);
            ClassicAssert.IsTrue(lastModifiedFilterList.Count == 0);
            ClassicAssert.IsTrue(
                lastModifiedFilterList.Select(p => p.LastModified)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(-2))));

            //expect failure when an invalid date is encountered in date comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                lastModifiedFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(lastModifiedFilter).ToList();

            });

            //expect 3 entries to match for a possibly empty Date comparison
            var nullableLastModifiedFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var nullableLastModifiedFilterList = startingQuery.BuildQuery(nullableLastModifiedFilter).ToList();
            ClassicAssert.IsTrue(nullableLastModifiedFilterList != null);
            ClassicAssert.IsTrue(nullableLastModifiedFilterList.Count == 3);
            ClassicAssert.IsTrue(
                nullableLastModifiedFilterList.Select(p => p.LastModifiedIfPresent)
                    .All(p => (p <= DateTime.UtcNow.Date.AddDays(1))));


            //expect 3 entries to match for a double field
            var statValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var statValueFilterList = startingQuery.BuildQuery(statValueFilter).ToList();
            ClassicAssert.IsTrue(statValueFilterList != null);
            ClassicAssert.IsTrue(statValueFilterList.Count == 4);
            ClassicAssert.IsTrue(
                statValueFilterList.Select(p => p.StatValue)
                    .All(p => (p <= 1.13)));

            //expect failure when an invalid double is encountered in double comparison
            ExceptionAssert.Throws<Exception>(() =>
            {
                statValueFilter.Rules.First().Value = "hello";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(statValueFilter).ToList();

            });

            //expect 2 entries to match for a nullable double field
            var nullableStatValueFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
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
            var nullableStatFilterList = startingQuery.BuildQuery(nullableStatValueFilter).ToList();
            ClassicAssert.IsTrue(nullableStatFilterList != null);
            ClassicAssert.IsTrue(nullableStatFilterList.Count == 2);
            ClassicAssert.IsTrue(
                nullableStatFilterList.Select(p => p.PossiblyEmptyStatValue)
                    .All(p => p <= 1.113));

        }

        [Test]
        public void FilterWithInvalidParameters()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect 3 entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "less_or_equal",
                        Type = "integer",
                        Value = "2"
                    },
                    new FilterRule()
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

            startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            contentIdFilter.Condition = "or";

            startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(null).ToList();

            startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(new FilterRule()).ToList();

            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Type = "NOT_A_TYPE";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();

            });

            ExceptionAssert.Throws<Exception>(() =>
            {
                contentIdFilter.Rules.First().Type = "integer";
                contentIdFilter.Rules.First().Operator = "NOT_AN_OPERATOR";
                startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
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
            var rule = new FilterRule
            {
                Condition = "and",
                Field = "ContentTypeId",
                Id = "ContentTypeId",
                Input = "NA",
                Operator = "equal",
                Type = "integer",
                Value = "2"
            };

            var result = new List<IndexedClass> { new IndexedClass() }.AsQueryable().BuildQuery(rule,
                new BuildExpressionOptions() { UseIndexedProperty = true, IndexedPropertyName = "Item" });
            ClassicAssert.IsTrue(result.Any());

            rule.Value = "3";
            result = new[] { new IndexedClass() }.BuildQuery(rule, true, "Item");
            ClassicAssert.IsFalse(result.Any());
        }
        #endregion

        #region Predicate
        [Test]
        public void Predicate_Test()
        {
            var rule = new FilterRule
            {
                Condition = "and",
                Field = "ContentTypeId",
                Id = "ContentTypeId",
                Input = "NA",
                Operator = "equal",
                Type = "integer",
                Value = "2",
            };


            var predicate = rule.BuildPredicate<IndexedClass>(new BuildExpressionOptions { IndexedPropertyName = "Item", UseIndexedProperty = true });

            var result = new[] { new IndexedClass() }.Where(predicate);
            ClassicAssert.IsTrue(result.Any());

            rule.Value = "3";
            result = new[] { new IndexedClass() }.BuildQuery(rule, true, "Item");
            ClassicAssert.IsFalse(result.Any());
        }
        [Test]
        public void Build_Predicate_Null_Test()
        {
            FilterRule rule = null;
            var predicate = rule.BuildPredicate<ExpressionTreeBuilderTestClass>(new BuildExpressionOptions() { ParseDatesAsUtc = true },
                out _);

            var resData = GetExpressionTreeData();

            var res = resData.Where(predicate).ToList();

            ClassicAssert.IsTrue(res.Count == 4);

        }

        #endregion

        #region NestedObjects

        public class StudentDTO
        {
            public int Id { get; set; }
            public List<SubjectDTO> Subjects { get; set; }
        }
        public class SubjectDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class NestedClass
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public NestedAddress Address { get; set; }
            public List<NestedClass> Children { get; set; }
        }

        public class NestedAddress
        {
            public string Address { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }
            public LatLonPair Location { get; set; }

        }

        public class LatLonPair
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }

        public List<StudentDTO> GetStudents()
        {
            List<StudentDTO> list = new List<StudentDTO>();
            list.Add(new StudentDTO
            {
                Id = 1,
                Subjects = new List<SubjectDTO>
                    {new SubjectDTO {Id = 1, Name = "Math"}, new SubjectDTO {Id = 1, Name = "Science"}}
            });
            list.Add(new StudentDTO
            {
                Id = 2,
                Subjects = new List<SubjectDTO>
                    {new SubjectDTO {Id = 1, Name = "Math"}, new SubjectDTO {Id = 1, Name = "Science"}}
            });
            list.Add(new StudentDTO
            {
                Id = 3,
                Subjects = new List<SubjectDTO>
                    {new SubjectDTO {Id = 1, Name = "History"}, new SubjectDTO {Id = 1, Name = "Geography"}}
            });
            return list;
        }

        public List<NestedClass> GetNestedClassTest()
        {
            var list = new List<NestedClass>()
            {
                new NestedClass()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Children = new List<NestedClass>()
                    {
                        new NestedClass()
                        {
                            FirstName = "John Jr.",
                            LastName = "Doe",
                        }
                    },
                    Address = new NestedAddress()
                    {
                        Address = "1234 Downing St",
                        City = "London",
                        State = "UK",
                        Zip = "029375",
                        Location = new LatLonPair()
                        {
                            Latitude = 38,
                            Longitude = -78
                        }
                    }
                },
                new NestedClass()
                {
                    FirstName = "Jane",
                    LastName = "Doe",
                    Children = new List<NestedClass>(),
                    Address = new NestedAddress()
                    {
                        Address = "1235 Downing St",
                        City = "London",
                        State = "UK",
                        Zip = "029375",
                        Location = new LatLonPair()
                        {
                            Latitude = 39,
                            Longitude = -78
                        }
                    }
                }
            };

            return list;
        }

        [Test]
        public void TestNestedProperties()
        {
            var rule = new FilterRule
            {
                Condition = "and",
                Field = "Address.Location.Latitude",
                Id = "Address.Location.Latitude",
                Input = "NA",
                Operator = "equal",
                Type = "double",
                Value = "38",
            };


            var list = GetNestedClassTest();

            var res = list.BuildQuery(rule).ToList();

            ClassicAssert.IsTrue(res.Count == 1);

        }

        [Test]
        public void TestNestedCollection()
        {
            var searchFilterTest = new QueryBuilderFilterRule()
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>()
                {
                    new QueryBuilderFilterRule()
                    {
                        Condition = "and",
                        Field = "Subjects.Name",
                        Id = "Name",
                        Operator = "not_equal", //not_equal
                        Type = "string",
                        Value = new [] { "Geography" }
                    }
                }
            };

            var data = GetStudents().AsQueryable().BuildQuery(searchFilterTest).ToList();



            var rule = new FilterRule
            {
                Condition = "and",
                Field = "Children.FirstName",
                Id = "Children.FirstName",
                Input = "NA",
                Operator = "equal",
                Type = "string",
                Value = "John Jr.",
            };


            var list = GetNestedClassTest();

            var res = list.BuildQuery(rule).ToList();

            ClassicAssert.IsTrue(res.Count == 1);
        }



        #endregion

        #region Misc
        [Test]
        public void Build_Query_Null_Test()
        {
            FilterRule rule = null;


            var data = GetExpressionTreeData();

            var res = data.AsQueryable().BuildQuery(rule, new BuildExpressionOptions() { ParseDatesAsUtc = true })
                .ToList();

            ClassicAssert.IsTrue(res.Count == 4);


        }

        [Test]
        public void AttemptDateCultureTest()
        {
            var startingQuery = GetDateExpressionTreeData().AsQueryable();
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "LastModified",
                        Id = "LastModified",
                        Input = "NA",
                        Operator = "equal",
                        Type = "date",
                        Value = "23/02/2016"
                    }
                }
            };
            var queryable = startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter,
                new BuildExpressionOptions() { CultureInfo = new CultureInfo("en-GB", true) });
            var contentIdFilteredList = queryable.ToList();
            ClassicAssert.IsTrue(contentIdFilteredList != null);
            ClassicAssert.IsTrue(contentIdFilteredList.Count == 1);
        }

        [Test]
        public void ComparePerformanceOfMethods_Test()
        {
            var startingQuery = GetExpressionTreeData().AsQueryable();


            //expect two entries to match for an integer comparison
            var contentIdFilter = new FilterRule()
            {
                Condition = "and",
                Rules = new List<FilterRule>()
                {
                    new FilterRule()
                    {
                        Condition = "and",
                        Field = "ContentTypeId",
                        Id = "ContentTypeId",
                        Input = "NA",
                        Operator = "in",
                        Type = "integer",
                        Value = "[1,2]"
                    }
                }
            };
            var sw1 = new Stopwatch();
            sw1.Start();
            for (var x = 0; x < 1000; x++)
            {
                var contentIdFilteredList =
                    startingQuery.BuildQuery<ExpressionTreeBuilderTestClass>(contentIdFilter).ToList();
            }
            sw1.Stop();

            var sw2 = new Stopwatch();
            sw2.Start();
            var predicate =
                contentIdFilter.BuildPredicate<ExpressionTreeBuilderTestClass>(new BuildExpressionOptions()
                { ParseDatesAsUtc = true });
            for (var x = 0; x < 1000; x++)
            {
                var contentIdFilteredList =
                    startingQuery.Where(predicate).ToList();
            }
            sw2.Stop();
        }
        #endregion

        #region Column Definition Builder

        public class ColumnBuilderTestClass
        {
            public int Age { get; set; }
            public int? FavoriteNumber { get; set; }
            public string Name { get; set; }
            public DateTime Birthday { get; set; }
            public DateTime? FavoriteBirthday { get; set; }
            public double DollarsInWallet { get; set; }
            public double? DesiredDollarsInWallet { get; set; }
#pragma warning disable IDE1006 // Naming Styles
            public string camelCaseField { get; set; }
#pragma warning restore IDE1006 // Naming Styles
            [IgnoreDataMember]
            public int IgnoreField { get; set; }
            public bool IsOfAge { get; set; }

            public bool? IsOfAge2 { get; set; }
        }

        [Test]
        public void ColumnBuilderTest()
        {
            var result = typeof(ColumnBuilderTestClass).GetDefaultColumnDefinitionsForType();

            ClassicAssert.IsTrue(result.Count == 10);

            result = typeof(ColumnBuilderTestClass).GetDefaultColumnDefinitionsForType(true);

            ClassicAssert.IsTrue(result.Count == 10);

        }

        [Test]
        public void TestColumnDefinition()
        {
            var cDef = new ColumnDefinition();
            var res = cDef.PrettyOutputTransformer.Invoke("Test");
            ClassicAssert.IsTrue(res.ToString() == "Test");
        }

        #endregion
    }
}
