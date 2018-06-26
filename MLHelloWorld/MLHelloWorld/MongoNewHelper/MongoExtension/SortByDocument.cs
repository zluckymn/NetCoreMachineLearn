using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MZ.MongoProvider
{
    //
    // 摘要:用于兼容旧有方法的使用
    //     Represents a BSON document that can be used where an IMongoSortBy is expected.
    public class SortByDocument : BsonDocument
    {
       
        //
        // 摘要:
        //     Initializes a new instance of the SortByDocument class.
        public SortByDocument()
        {
           
        }

        //
        // 摘要:
        //     Initializes a new instance of the SortByDocument class and adds one element.
        //
        // 参数:
        //   element:
        //     An element to add to the document.
        public SortByDocument(BsonElement element)
        {
            this.Add(element);
           
        }

        //
        // 摘要:
        //     Initializes a new instance of the SortByDocument class and adds new elements
        //     from a list of elements.
        //
        // 参数:
        //   elements:
        //     A list of elements to add to the document.
        public SortByDocument(IEnumerable<BsonElement> elements)
        {
            this.AddRange(elements);
        }
        //
        // 摘要:
        //     Initializes a new instance of the SortByDocument class and adds one or more elements.
        //
        // 参数:
        //   elements:
        //     One or more elements to add to the document.
        public SortByDocument(params BsonElement[] elements)
        {
            this.AddRange(elements);
        }

        //
        // 摘要:
        //     Initializes a new instance of the SortByDocument class and creates and adds a
        //     new element.
        //
        // 参数:
        //   name:
        //     The name of the element to add to the document.
        //
        //   value:
        //     The value of the element to add to the document.
        public SortByDocument(string name, BsonValue value)
        {
            this.Add(name, value);
        }
    }

}
