using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicLinqQueryBuilder.Tests.Database;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Castle.DynamicLinqQueryBuilder.Tests.ORM {
    [ExcludeFromCodeCoverage]
    #if LOCALTEST
    [TestFixture]
    #endif
    public class ORMTestsMongo : IDisposable {
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<Restaurant> _collection;
        private const string CollectionName = "restaurant";

        public ORMTestsMongo() {
            var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
            if (connectionString == null) {
                Console.WriteLine(
                    "You must set your 'MONGODB_URI' environmental variable. See\n\t https://www.mongodb.com/docs/drivers/go/current/usage-examples/#environment-variable");
                Environment.Exit(0);
            }

            var client = new MongoClient(connectionString);
            SetEnumConvention();
            _db = client.GetDatabase("DynamicLinqQueryBuilderTests");
            _db.DropCollection(CollectionName);
            _db.CreateCollection(CollectionName);
            _collection = _db.GetCollection<Restaurant>(CollectionName);


            var restaurants = new List<Restaurant>() {
                new Restaurant {
                    Name = "My restaurant",
                    RestaurantId = "Id",
                    Status = Status.Open,
                    Details = new Dictionary<string, object> {
                        ["items_count"] = 30,
                        ["date_started"] = DateTime.Now - TimeSpan.FromDays(365),
                        ["address"] = "street23"
                    }
                },
                new Restaurant {
                    Name = "My restaurant2",
                    RestaurantId = "Id2",
                    Status = Status.Closed,
                    Details = new Dictionary<string, object> {
                        ["items_count"] = 40,
                        ["date_started"] = DateTime.Now - TimeSpan.FromDays(20),
                        ["address"] = "street33"
                    }
                },
                new Restaurant {
                    Name = "My restaurant3",
                    RestaurantId = "Id3",
                    Status = Status.Closed,
                    Details = new Dictionary<string, object> {
                        ["not_any_other_field"] = "value"
                    }
                }
            };

            _collection.InsertMany(restaurants);
        }

        private void SetEnumConvention() {
            var pack = new ConventionPack {
                new EnumRepresentationConvention(BsonType.String)
            };

            ConventionRegistry.Register("EnumStringConvention", pack, t => true);
        }

        public void Dispose() => _db.DropCollection(CollectionName);

        #if LOCALTEST
        [Test]
        #endif
        public async Task StringEndsWithTest() {
            var filter = new JsonNetFilterRule() {
                Condition = "and",
                Rules = new List<JsonNetFilterRule> {
                    new JsonNetFilterRule {
                        Field = "Name",
                        Operator = "ends_with",
                        Type = "string",
                        Value = "2"
                    }
                }
            };
            var expression = filter.BuildExpressionLambda<Restaurant>(new BuildExpressionOptions(), out var _);
            var result = await _collection.Find(expression).ToListAsync();
            ClassicAssert.AreEqual(result.Count, 1);
            ClassicAssert.AreEqual(result.First().RestaurantId, "Id2");
        }
        #if LOCALTEST
        [Test]
        #endif
        public async Task EnumTest() {
            var filter = new JsonNetFilterRule() {
                Condition = "and",
                Rules = new List<JsonNetFilterRule> {
                    new JsonNetFilterRule {
                        Field = "Status",
                        Operator = "equal",
                        Type = "string",
                        Value = "open"
                    }
                }
            };
            var expression = filter.BuildExpressionLambda<Restaurant>(new BuildExpressionOptions(), out var _);
            var result = await _collection.Find(expression).ToListAsync();
            ClassicAssert.AreEqual(result.Count, 1);
            ClassicAssert.AreEqual(result.First().RestaurantId, "Id");

            filter = new JsonNetFilterRule() {
                Condition = "and",
                Rules = new List<JsonNetFilterRule> {
                    new JsonNetFilterRule {
                        Field = "Status",
                        Operator = "equal",
                        Type = "string",
                        Value = "open"
                    }
                }
            };

            expression =
                filter.BuildExpressionLambda<Restaurant>(
                    new BuildExpressionOptions { StringCaseSensitiveComparison = true }, out var _);
            result = await _collection.Find(expression).ToListAsync();
            ClassicAssert.IsTrue(result.Count == 0);
        }


        #if LOCALTEST
        [Test]
        #endif
        public async Task DictionaryIntegerGreaterThenTest() {
            var filter = new JsonNetFilterRule() {
                Condition = "and",
                Rules = new List<JsonNetFilterRule> {
                    new JsonNetFilterRule {
                        Field = "Details.items_count",
                        Operator = "greater",
                        Type = "integer",
                        Value = "35"
                    }
                }
            };
            var expression = filter.BuildExpressionLambda<Restaurant>(new BuildExpressionOptions(), out var _);
            var result = await _collection.Find(expression).ToListAsync();
            ClassicAssert.IsTrue(result.Count == 1);
            ClassicAssert.AreEqual(result.First().RestaurantId, "Id2");
        }


        #if LOCALTEST
        [Test]
        #endif
        public async Task DictionaryDateTimeGreaterThenTest() {
            var filter = new JsonNetFilterRule() {
                Condition = "and",
                Rules = new List<JsonNetFilterRule> {
                    new JsonNetFilterRule {
                        Field = "Details.date_started",
                        Operator = "greater",
                        Type = "datetime",
                        Value = DateTime.Now - TimeSpan.FromDays(40)
                    }
                }
            };
            var expression =
                filter.BuildExpressionLambda<Restaurant>(
                    new BuildExpressionOptions { CultureInfo = CultureInfo.CurrentCulture }, out var _);
            var result = await _collection.Find(expression).ToListAsync();
            ClassicAssert.IsTrue(result.Count == 1);
            ClassicAssert.AreEqual(result.First().RestaurantId, "Id2");
        }

        #if LOCALTEST
        [Test]
        #endif
        public async Task DictionaryStringEndsWithTest() {
            var filter = new JsonNetFilterRule() {
                Condition = "and",
                Rules = new List<JsonNetFilterRule> {
                    new JsonNetFilterRule {
                        Field = "Details.address",
                        Operator = "ends_with",
                        Type = "string",
                        Value = "23"
                    }
                }
            };
            var expression =
                filter.BuildExpressionLambda<Restaurant>(
                    new BuildExpressionOptions { CultureInfo = CultureInfo.CurrentCulture }, out var _);
            var result = await _collection.Find(expression).ToListAsync();
            ClassicAssert.IsTrue(result.Count == 1);
            ClassicAssert.AreEqual(result.First().RestaurantId, "Id");
        }


        #if LOCALTEST
        [Test]
        #endif
        public async Task DictionaryIsNullTest() {
            var filter = new JsonNetFilterRule() {
                Condition = "and",
                Rules = new List<JsonNetFilterRule> {
                    new JsonNetFilterRule {
                        Field = "Details.address",
                        Operator = "is_null",
                        Type = "string",
                    }
                }
            };
            var expression =
                filter.BuildExpressionLambda<Restaurant>(
                    new BuildExpressionOptions { CultureInfo = CultureInfo.CurrentCulture }, out var _);
            var result = await _collection.Find(expression).ToListAsync();
            ClassicAssert.IsTrue(result.Count == 1);
            ClassicAssert.AreEqual(result.First().RestaurantId, "Id3");
        }


        #if LOCALTEST
        [Test]
        #endif
        public async Task DictionaryNotEqualTest() {
            var filter = new JsonNetFilterRule() {
                Condition = "and",
                Rules = new List<JsonNetFilterRule> {
                    // mongo supports not_equal even if the the item does not exist in the dictionary
                    // as oppose to inMemory filtering where it will throw KeyNotFound exception
                    new JsonNetFilterRule {
                        Field = "Details.address",
                        Operator = "not_equal",
                        Type = "string",
                        Value = "a"
                    }
                }
            };
            var expression =
                filter.BuildExpressionLambda<Restaurant>(
                    new BuildExpressionOptions
                        { CultureInfo = CultureInfo.CurrentCulture, StringCaseSensitiveComparison = true }, out var _);
            var a = Builders<Restaurant>.Filter.Where(expression);
            var b = a.Render(_collection.DocumentSerializer, _collection.Settings.SerializerRegistry).ToString();
            var result = await _collection.Find(expression).ToListAsync();
            ClassicAssert.IsTrue(result.Count == 3);
            
            // Verify that the expression is using ToString() on the value
            var filterDefinition = Builders<Restaurant>.Filter.Where(expression);
            var mongoQuery = filterDefinition
                .Render(_collection.DocumentSerializer, _collection.Settings.SerializerRegistry).ToString();
            ClassicAssert.IsTrue(mongoQuery == @"{ ""$nor"" : [{ ""$and"" : [{ ""Details.address"" : { ""$ne"" : null } }, { ""$expr"" : { ""$eq"" : [{ ""$toString"" : ""$Details.address"" }, ""a""] } }] }] }");
        }

        #if LOCALTEST
        [Test]
        #endif
        public async Task DictionaryNotEqualTestAndRequireExplicitToStringConversionFlagFalse() {
            var filter = new JsonNetFilterRule() {
                Condition = "and",
                Rules = new List<JsonNetFilterRule> {
                    // mongo supports not_equal even if the the item does not exist in the dictionary
                    // as oppose to inMemory filtering where it will throw KeyNotFound exception
                    new JsonNetFilterRule {
                        Field = "Details.address",
                        Operator = "not_equal",
                        Type = "string",
                        Value = "a"
                    }
                }
            };
            var expression =
                filter.BuildExpressionLambda<Restaurant>(
                    new BuildExpressionOptions {
                        CultureInfo = CultureInfo.CurrentCulture,
                        StringCaseSensitiveComparison = true,
                        RequireExplicitToStringConversion = false
                    }, out var _);

            var result = await _collection.Find(expression).ToListAsync();
            ClassicAssert.IsTrue(result.Count == 3);

            // Verify that the expression is not using ToString() on the value
            var filterDefinition = Builders<Restaurant>.Filter.Where(expression);
            var mongoQuery = filterDefinition
                .Render(_collection.DocumentSerializer, _collection.Settings.SerializerRegistry).ToString();
            ClassicAssert.IsTrue(mongoQuery == @"{ ""$nor"" : [{ ""$and"" : [{ ""Details.address"" : { ""$ne"" : null } }, { ""Details.address"" : ""a"" }] }] }");
        }

        #if LOCALTEST
        [Test]
        #endif
        public async Task DictionaryNotContainsTest() {
            var filter = new JsonNetFilterRule() {
                Condition = "and",
                Rules = new List<JsonNetFilterRule> {
                    // mongo supports not_equal even if the the item does not exist in the dictionary
                    // as oppose to inMemory filtering where it will throw KeyNotFound exception
                    new JsonNetFilterRule {
                        Field = "Details",
                        Operator = "not_contains",
                        Type = "string",
                        Value = "address"
                    }
                }
            };
            var expression =
                filter.BuildExpressionLambda<Restaurant>(
                    new BuildExpressionOptions { CultureInfo = CultureInfo.CurrentCulture }, out var _);
            var result = await _collection.Find(expression).ToListAsync();
            ClassicAssert.IsTrue(result.Count == 1);
        }
    }
}
