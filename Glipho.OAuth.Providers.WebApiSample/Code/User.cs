using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Glipho.OAuth.Providers.WebApiSample.Code
{
    public class User
    {
        [BsonId]
        public BsonObjectId Id { get; set; }

        [BsonElement("username"), BsonRequired]
        public string Username { get; set; }
    }
}