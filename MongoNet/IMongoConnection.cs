using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDB.MongoNet
{
    public interface  IMongoConnection
    {
          string GetConnectionString();
          string GetDatabaseName();
    }
}
