# MongoNet
This package contains CRUD functions with mongodb driver functions like find that are ready for use for MongoDB .Net Core

### Step 1: Add connection class
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

### Step 2: Inherit your entity from EntityBase
```
public class ExampleEntity : MongoContext<ExampleEntity, MyMongoConnection> 
{ 
  public string ExampleField { get; set; }  
}
```

### Step 3: Call any create and update method from anywhere
```
ExampleEntity instance = new ExampleEntity(){  });
instance.Insert();
```

### Step 4: Call any read method from anywhere
```
ExampleEntity.Select().ToList();
ExampleEntity.Select().Where(w=> w....).ToList();
```

