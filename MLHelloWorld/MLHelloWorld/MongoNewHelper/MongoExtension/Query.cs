using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MZ.MongoProvider
{
    /// <summary>
    /// 用于兼容旧有方法的使用
    /// </summary>
    public class Query
    {
        private static readonly FilterDefinitionBuilder<BsonDocument> filter = Builders<BsonDocument>.Filter;
        
        //
        // 摘要:
        //     Tests that the named array element contains all of the values (see $all).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   values:
        //     The values to compare to.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> All(string name, params BsonValue[] values)
        {
            var condition = filter.All(name, values);
            return condition;
        }
        //
        // 摘要:
        //     Tests that the named array element contains all of the values (see $all).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   values:
        //     The values to compare to.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> All(string name, BsonArray values)
        {
            var condition = filter.All(name, values);
            return condition;
        }
        // 摘要:
        //     Tests that the named array element contains all of the values (see $all).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   values:
        //     The values to compare to.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> All(string name, IEnumerable<BsonValue> values)
        {
           
            var condition = filter.All(name, values);
            return condition;
        }
        //
        // 摘要:
        //     Tests that all the subqueries are true (see $and in newer versions of the server).
        //
        // 参数:
        //   clauses:
        //     A list of subqueries.
        //
        // 返回结果:
        //     A query.
        public static FilterDefinition<BsonDocument> And(params FilterDefinition<BsonDocument>[] clauses)
        {
           return filter.And(clauses);
        }
        //
        // 摘要:
        //     Tests that at least one item of the named array element matches a query (see
        //     $elemMatch).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   query:
        //     The query to match elements with.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> ElemMatch(string name, FilterDefinition<BsonDocument> query)
        {
            return filter.ElemMatch(name, query);
        }
        //
        // 摘要:
        //     Tests that the value of the named element is equal to some value.
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   value:
        //     The value to compare to.
        //
        // 返回结果:
        //     A query.
        public static FilterDefinition<BsonDocument> EQ(string name, BsonValue value)
        {
            return filter.Eq(name, value);
        }
        //
        // 摘要:
        //     Tests that an element of that name does or does not exist (see $exists).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   exists:
        //     Whether to test for the existence or absence of an element.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> Exists(string name, bool exists)
        {
            return filter.Exists(name, exists);
        }
        //
        // 摘要:
        //     Tests that the value of the named element is greater than some value (see $gt).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   value:
        //     The value to compare to.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> GT(string name, BsonValue value)
        {
            return filter.Gt(name, value);
        }
        //
        // 摘要:
        //     Tests that the value of the named element is greater than or equal to some value
        //     (see $gte).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   value:
        //     The value to compare to.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> GTE(string name, BsonValue value)
        {
            return filter.Gte(name, value);
        }
        //
        // 摘要:
        //     Tests that the value of the named element is equal to one of a list of values
        //     (see $in).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   values:
        //     The values to compare to.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> In(string name, BsonArray values)
        {
          
                return filter.In(name, values);
          
        }
        //
        // 摘要:
        //     Tests that the value of the named element is equal to one of a list of values
        //     (see $in).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   values:
        //     The values to compare to.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> In(string name, IEnumerable<BsonValue> values)
        {
             return filter.In(name, values);
        }
        //
        // 摘要:
        //     Tests that the value of the named element is equal to one of a list of values
        //     (see $in).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   values:
        //     The values to compare to.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> In(string name, params BsonValue[] values)
        {
            return filter.In(name, values);
        }
        //
        // 摘要:
        //     Tests that the value of the named element is less than some value (see $lt).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   value:
        //     The value to compare to.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> LT(string name, BsonValue value)
        {
            return filter.Lt(name, value);
        }
        //
        // 摘要:
        //     Tests that the value of the named element is less than or equal to some value
        //     (see $lte).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   value:
        //     The value to compare to.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> LTE(string name, BsonValue value)
        {
            return filter.Lte(name, value);
        }
        //
        // 摘要:
        //     Tests that the value of the named element matches a regular expression (see $regex).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   regex:
        //     The regular expression to match against.
        //
        // 返回结果:
        //     A query.
        public static FilterDefinition<BsonDocument> Matches(string name, BsonRegularExpression regex)
        {
            return filter.Regex(name, regex);
        }
        //
        // 摘要:
        //     Tests that the modulus of the value of the named element matches some value (see
        //     $mod).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   modulus:
        //     The modulus.
        //
        //   equals:
        //     The value to compare to.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> Mod(string name, int modulus, int equals)
        {
            return filter.Mod(name, modulus, equals);
        }
        //
        // 摘要:
        //     Tests that the value of the named element is not equal to some value (see $ne).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   value:
        //     The value to compare to.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> NE(string name, BsonValue value)
        {
            
                return filter.Ne(name, value);
             
        }
        //
        // 摘要:
        //     Tests that the value of the named element is near some location (see $near and
        //     $nearSphere).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   x:
        //     The x value of the origin.
        //
        //   y:
        //     The y value of the origin.
        //
        //   maxDistance:
        //     The max distance for a document to be included in the results.
        //
        //   spherical:
        //     Whether to do a spherical search.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> Near(string name, double x, double y, double maxDistance, bool spherical)
        {
            return filter.Near(name,  x,  y,  maxDistance);
        }
        //
        // 摘要:
        //     Tests that the value of the named element is near some location (see $near).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   x:
        //     The x value of the origin.
        //
        //   y:
        //     The y value of the origin.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> Near(string name, double x, double y)
        {
            return filter.Near(name, x, y );
        }
        //
        // 摘要:
        //     Tests that the value of the named element is near some location (see $near).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   x:
        //     The x value of the origin.
        //
        //   y:
        //     The y value of the origin.
        //
        //   maxDistance:
        //     The max distance for a document to be included in the results.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> Near(string name, double x, double y, double maxDistance)
        {
          
                return filter.Near(name, x, y, maxDistance);
          
        }
        ////
        //// 摘要:
        ////     Tests that none of the subqueries is true (see $nor).
        ////
        //// 参数:
        ////   queries:
        ////     The subqueries.
        ////
        //// 返回结果:
        ////     A query.
        //public static FilterDefinition<BsonDocument> Nor(params FilterDefinition<BsonDocument>[] queries)
        //{
        //    未实现
        //}
        //
        // 摘要:
        //     Tests that the value of the named element does not match any of the tests that
        //     follow (see $not).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> Not(string name)
        {
            return filter.Not(name);
        }
        //
        // 摘要:
        //     Tests that the value of the named element is not equal to any of a list of values
        //     (see $nin).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   values:
        //     The values to compare to.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> NotIn(string name, BsonArray values)
        {
            return filter.Nin(name, values);
        }
        //
        // 摘要:
        //     Tests that the value of the named element is not equal to any of a list of values
        //     (see $nin).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   values:
        //     The values to compare to.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> NotIn(string name, IEnumerable<BsonValue> values)
        {
            return filter.Nin(name, values);
        }
        //
        // 摘要:
        //     Tests that the value of the named element is not equal to any of a list of values
        //     (see $nin).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   values:
        //     The values to compare to.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> NotIn(string name, params BsonValue[] values)
        {
               return filter.Nin(name, values);
        }
        //
        // 摘要:
        //     Tests that at least one of the subqueries is true (see $or).
        //
        // 参数:
        //   queries:
        //     The subqueries.
        //
        // 返回结果:
        //     A query.
        public static FilterDefinition<BsonDocument> Or(params FilterDefinition<BsonDocument>[] queries)
        {
              return filter.Or(queries);
        }
        //
        // 摘要:
        //     Tests that the size of the named array is equal to some value (see $size).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   size:
        //     The size to compare to.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> Size(string name, int size)
        {
              return filter.Size(name,size);
        }
        //
        // 摘要:
        //     Tests that the type of the named element is equal to some type (see $type).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   type:
        //     The type to compare to.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> Type(string name, BsonType type)
        {
            return filter.Type(name, type);
        }
        //
        // 摘要:
        //     Tests that a JavaScript expression is true (see $where).
        //
        // 参数:
        //   javaScript:
        //     The where clause.
        //
        // 返回结果:
        //     A query.
        public static FilterDefinition<BsonDocument> Where(Expression<Func<BsonDocument, bool>> expression)
        {
            return filter.Where(expression);
        }
        //
        // 摘要:
        //     Tests that the value of the named element is within a circle (see $within and
        //     $center).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   centerX:
        //     The x coordinate of the origin.
        //
        //   centerY:
        //     The y coordinate of the origin.
        //
        //   radius:
        //     The radius of the circle.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> WithinCircle(string name, double centerX, double centerY, double radius)
        {
            return filter.GeoWithinCenter(name,centerX,centerY,radius);
        }
        //
        // 摘要:
        //     Tests that the value of the named element is within a circle (see $within and
        //     $center/$centerSphere).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   centerX:
        //     The x coordinate of the origin.
        //
        //   centerY:
        //     The y coordinate of the origin.
        //
        //   radius:
        //     The radius of the circle.
        //
        //   spherical:
        //     Whether to do a spherical search.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> WithinCircle(string name, double centerX, double centerY, double radius, bool spherical)
        {
            if (spherical == true)
            {
                return filter.GeoWithinCenterSphere(name, centerX, centerY, radius);
            }
            else
            {
                return WithinCircle(name, centerX, centerY, radius);
            }
        }
        //
        // 摘要:
        //     Tests that the value of the named element is within a polygon (see $within and
        //     $polygon).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   points:
        //     An array of points that defines the polygon (the second dimension must be of
        //     length 2).
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> WithinPolygon(string name, double[,] points)
        {
            return filter.GeoWithinPolygon(name, points);
        }
        //
        // 摘要:
        //     Tests that the value of the named element is within a rectangle (see $within
        //     and $box).
        //
        // 参数:
        //   name:
        //     The name of the element to test.
        //
        //   lowerLeftX:
        //     The x coordinate of the lower left corner.
        //
        //   lowerLeftY:
        //     The y coordinate of the lower left corner.
        //
        //   upperRightX:
        //     The x coordinate of the upper right corner.
        //
        //   upperRightY:
        //     The y coordinate of the upper right corner.
        //
        // 返回结果:
        //     The builder (so method calls can be chained).
        public static FilterDefinition<BsonDocument> WithinRectangle(string name, double lowerLeftX, double lowerLeftY, double upperRightX, double upperRightY)
        {
            return filter.GeoWithinBox(name,  lowerLeftX,  lowerLeftY,  upperRightX,  upperRightY);
        }
    }
}
