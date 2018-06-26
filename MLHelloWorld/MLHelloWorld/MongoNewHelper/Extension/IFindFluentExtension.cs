using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.IO;
using System.Linq.Expressions;

///<summary>
///MongoDB基础及其重载
///</summary>
namespace MZ.MongoProvider
{
    /// <summary>
    /// BsonDoc方法重载
    /// </summary>
    public static class IFindFluentExtension
    {

        private static ProjectionDefinition<BsonDocument> SetFields(params string[] fields)
        {
            ProjectionDefinition<BsonDocument> project = null;
            foreach (string columnName in fields)
            {
                if (project == null)
                {
                    project = Builders<BsonDocument>.Projection.Include(columnName);
                }
                else
                {
                    project = project.Include(columnName);
                }
            }
            return project;
        }
        private static SortDefinition<BsonDocument> OrderBy(Expression<Func<BsonDocument, object>> sortExpress)
        {
            var sort = new SortDefinitionBuilder<BsonDocument>();
            return sort.Ascending(sortExpress); ;

        }

        private static SortDefinition<BsonDocument> OrderByDesc(Expression<Func<BsonDocument, object>> sortExpress)
        {
            var sort = new SortDefinitionBuilder<BsonDocument>();
            return sort.Descending(sortExpress); ;

        }
     
        /// <summary>
        /// 获取字段对应的主表记录
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="foreignFieldName"> 外键字段名称</param>
        /// <returns></returns>
        public static IFindFluent<BsonDocument, BsonDocument> SetFields(this IFindFluent<BsonDocument, BsonDocument> fluent, params string[] fields)
        {
            return fluent.Project(SetFields(fields));
        }
        /// <summary>
        /// 获取字段对应的主表记录
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="foreignFieldName"> 外键字段名称</param>
        /// <returns></returns>
        public static IFindFluent<BsonDocument, BsonDocument> OrderBy(this IFindFluent<BsonDocument, BsonDocument> fluent, Expression<Func<BsonDocument, object>> sort)
        {
            return fluent.Sort(OrderBy(sort));
        }
        /// <summary>
        /// 获取字段对应的主表记录
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="foreignFieldName"> 外键字段名称</param>
        /// <returns></returns>
        public static IFindFluent<BsonDocument, BsonDocument> OrderByDesc(this IFindFluent<BsonDocument, BsonDocument> fluent, Expression<Func<BsonDocument, object>> sort)
        {
            return fluent.Sort(OrderByDesc(sort));
        }

        /// <summary>
        /// 获取字段对应的主表记录
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="foreignFieldName"> 外键字段名称</param>
        /// <returns></returns>
        public static IFindFluent<BsonDocument, BsonDocument> SetSortOrder(this IFindFluent<BsonDocument, BsonDocument> fluent, SortByDocument sort)
        {
            foreach(var elem in sort.Elements)
            {
                var name = elem.Name;
                var value = elem.Value;
                if (value == 1)
                {
                    return fluent.OrderBy(d=>d[name]);
                }
                else
                {
                    return fluent.OrderByDesc(d => d[name]);
                }
            }
            return fluent;
        }

        
    }
}
