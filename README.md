# MongoNet
This package contains CRUD functions with mongodb driver functions like find that are ready for use for MongoDB.

# Step 1: Add connection class
```
public class MyMongoConnection : IMongoConnection
{ 
        public string GetConnectionString()
        {
            return "mongodb://localhost:27017/exampledb?connectTimeoutMS=60000&socketTimeoutMS=60000&maxIdleTimeMS=60000&minPoolSize=10";
        }

        public string GetDatabaseName()
        {
            return "exampledb";
        }
}
```

# Step 2: Inherit your entity from EntityBase
```
public class ExampleEntity : EntityBase 
{ 
  public string ExampleField { get; set; } 
  
  private static MongoDbRepository<ExampleEntity, MyMongoConnection> _DBContext;
  public static MongoDbRepository<ExampleEntity, MyMongoConnection> DBContext
  {
      get
      {
          if (_DBContext == null)
          {
              _DBContext = new MongoDbRepository<ExampleEntity, MyMongoConnection>();
          }
          return _DBContext;
      }
  }
}
```

# Step 3: Call any crud method from anywhere
```
ExampleEntity.DBContext.Insert(new ExampleEntity(){  });
```
