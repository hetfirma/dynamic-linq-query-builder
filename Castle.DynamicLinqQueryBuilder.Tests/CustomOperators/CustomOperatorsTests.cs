﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Castle.DynamicLinqQueryBuilder.Tests.Helpers;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Castle.DynamicLinqQueryBuilder.Tests.CustomOperators
{
    public class MyRecord { 
        public string UserHostAddress { get; set; }
    }

    public class InIpRangeOperator : IFilterOperator
    {
        public string Operator =>  "in_ip_range";

        public Expression GetExpression(Type type, IFilterRule rule, Expression propertyExp, BuildExpressionOptions options)
        {
            
            return Expression.Call(this.GetType().GetMethod("ContainsIP"), new[] { propertyExp, Expression.Constant(rule.Value) });
        
        }
        public static bool ContainsIP(string ip, string[] ranges) {
            return true; //TODO: implement custom ip range validation
        }
    }

    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class CustomOperatorsTests
    {
        [Test]
        public void CustomInOperator_Test() {
            var rec = new MyRecord();

            rec.UserHostAddress = "8.10.8.13";

            var records = new List<MyRecord>() { rec };

            var myFilter = new QueryBuilderFilterRule()
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>()
                {
                    new QueryBuilderFilterRule()
                    {
                        Condition = "and",
                        Field = "UserHostAddress",
                        Operator = "in_ip_range",
                        Type = "string",
                        Value = new [] { "10.10.10.*" }
                    }
                }
            };
            var options = new  BuildExpressionOptions();
            options.Operators = new List<IFilterOperator>() { new InIpRangeOperator()};
            var result = records.AsQueryable().BuildQuery<MyRecord>(myFilter, options).ToList();
            var len = result.Count;
            ClassicAssert.AreEqual(1, len);
        }

        [Test]
        public void UnknownOperator_Test1()
        {
            var rec = new MyRecord();

            rec.UserHostAddress = "8.10.8.13";

            var records = new List<MyRecord>() { rec };

            var myFilter = new QueryBuilderFilterRule()
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>()
                {
                    new QueryBuilderFilterRule()
                    {
                        Condition = "and",
                        Field = "UserHostAddress",
                        Operator = "in_ip_range",
                        Type = "string",
                        Value = new [] { "10.10.10.*" }
                    }
                }
            };
            
            ExceptionAssert.Throws<Exception>(() =>
            {
                var result = records.AsQueryable().BuildQuery<MyRecord>(myFilter).ToList();
            });
        }

        [Test]
        public void UnknownOperator_Test2()
        {
            var rec = new MyRecord();

            rec.UserHostAddress = "8.10.8.13";

            var records = new List<MyRecord>() { rec };

            var myFilter = new QueryBuilderFilterRule()
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>()
                {
                    new QueryBuilderFilterRule()
                    {
                        Condition = "and",
                        Field = "UserHostAddress",
                        Operator = "in_ip_range_spelledwrong",
                        Type = "string",
                        Value = new [] { "10.10.10.*" }
                    }
                }
            };
            var options = new BuildExpressionOptions();
            options.Operators = new List<IFilterOperator>() { new InIpRangeOperator() };
            ExceptionAssert.Throws<Exception>(() =>
            {
                var result = records.AsQueryable().BuildQuery<MyRecord>(myFilter, options).ToList();
            });
        }
    }
}
