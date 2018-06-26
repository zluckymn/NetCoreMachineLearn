using System;
using MongoDB.Bson.Serialization.Attributes;

namespace MZ.MongoProvider
{
    public interface IEntity
    {
        [BsonId]
        string Id { get; set; }
    }
}
