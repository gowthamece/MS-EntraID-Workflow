var builder = DistributedApplication.CreateBuilder(args);
var connectionString= builder.AddConnectionString("sql");

var cache = builder.AddRedis("cache");

//var sql= builder.AddSqlServer("sql")
//    .WithExternalHttpEndpoints()
//    .WithReference(cache)
//    .WaitFor(cache);
//var db = sql.AddDatabase("sqldata");


var apiService = builder.AddProject<Projects.EntraID_Workflow_ApiService>("apiservice").WithReference(connectionString);

builder.AddProject<Projects.EntraID_Workflow_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
