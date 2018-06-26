using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Bson;
using MongoDB.Driver;
using MZ.Enum;

namespace MZ.MongoProvider
{
    /// <summary>
    /// 数据存储类
    /// </summary>
    public class StorageData
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public StorageType Type { get; set; }

        /// <summary>
        /// 定位关键字
        /// </summary>
        public FilterDefinition<BsonDocument> Query{ get; set; }

        /// <summary>
        /// 保存文档
        /// </summary>
        public BsonDocument Document { get; set; }
       
    }

    
}
