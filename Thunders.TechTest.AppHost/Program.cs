var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var rabbitMqPassword = builder.AddParameter("RabbitMqPassword", true);
var rabbitMq = builder.AddRabbitMQ("RabbitMq", password: rabbitMqPassword)
    .WithDataVolume()
    .WithVolume("/etc/rabbitmq")
    .WithManagementPlugin();

var sqlServerPassword = builder.AddParameter("SqlServerInstancePassword", true);
var sqlServer = builder.AddSqlServer("SqlServerInstance", sqlServerPassword)
    .WithDataVolume()
    .WithEndpoint("tcp", ep => { ep.IsProxied = false; }); 
// Line above is needed to correctly expose the container port
// [https://github.com/dotnet/aspire/issues/6532#issuecomment-2466653817]

var database = sqlServer.AddDatabase("ThundersTechTestDb", "ThundersTechTest");

var apiService = builder.AddProject<Projects.Thunders_TechTest_ApiService>("apiservice")
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();
