using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MZ.MongoProvider
{
    [BsonIgnoreExtraElements(Inherited = true)]
    public abstract class Entity : IEntity
    { 
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
