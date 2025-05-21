using Thunders.TechTest.ApiService;
using Thunders.TechTest.OutOfBox.Database;
using Thunders.TechTest.OutOfBox.Queues;
using Thunders.TechTest.ApiService.Services;
using FluentValidation;
using System.Reflection;
using Thunders.TechTest.ApiService.Database;
using Microsoft.EntityFrameworkCore;
using Thunders.TechTest.ApiService.Models;
using Thunders.TechTest.ApiService.Services.TollReportService;
using Thunders.TechTest.ApiService.Queues;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddControllers();

var features = Features.BindFromConfiguration(builder.Configuration);

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();

if (features.UseMessageBroker)
{
    builder.Services.AddBus(builder.Configuration,
        new SubscriptionBuilder()
            .Add<TollRegisterConsumer>()
            .Add<TollReportConsumer>()
    );
}

if (features.UseEntityFramework)
{
    builder.Services.AddSqlServerDbContext<ApplicationDbContext>(builder.Configuration);
}

builder.Services.AddValidatorsFromAssembly(Assembly.GetEntryAssembly());

builder.Services.AddSingleton<IMessageSender, RebusMessageSender>();
builder.Services.AddTransient<ITollRegisterService, TollRegisterService>();
builder.Services.AddTransient<ITollReportService, TollReportService>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
dbContext.Database.Migrate();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();

    if (!dbContext.TollPlazas.Any())
    {
        var plazaId1 = new Guid("eb5b2fcf-5d75-49a2-a631-1e2384377a76");
        var plazaId2 = new Guid("97a8ed74-6c40-41fd-a8c3-0f9e23f1b809");
        var plazaId3 = new Guid("ace3fa7f-313f-4c33-a69b-06d0b1c033a0");

        dbContext.TollPlazas.AddRange(
            new TollPlaza { Id = plazaId1, Name = "Praca Norte", City = "Porto Alegre", State = "RS" },
            new TollPlaza { Id = plazaId2, Name = "Praca Leste", City = "Porto Alegre", State = "RS" },
            new TollPlaza { Id = plazaId3, Name = "Praca Oeste", City = "Sao Paulo", State = "SP" }
        );

        var date = DateTime.UtcNow;

        dbContext.TollRegisters.AddRange(
            new TollRegister { TollPlazaId = plazaId1, AmountPaid = 10.00m, VehicleType = VehicleType.Car, RegisteredAt = date, CreatedAt = date, Id = Guid.NewGuid() },
            new TollRegister { TollPlazaId = plazaId1, AmountPaid = 2.00m, VehicleType = VehicleType.Motorcycle, RegisteredAt = date, CreatedAt = date, Id = Guid.NewGuid() },

            new TollRegister { TollPlazaId = plazaId2, AmountPaid = 10.00m, VehicleType = VehicleType.Car, RegisteredAt = date, CreatedAt = date, Id = Guid.NewGuid() },
            new TollRegister { TollPlazaId = plazaId2, AmountPaid = 10.00m, VehicleType = VehicleType.Car, RegisteredAt = date, CreatedAt = date, Id = Guid.NewGuid() },

            new TollRegister { TollPlazaId = plazaId3, AmountPaid = 15.00m, VehicleType = VehicleType.Truck, RegisteredAt = date, CreatedAt = date, Id = Guid.NewGuid() }
        );

        dbContext.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapDefaultEndpoints();

app.MapControllers();

app.Run();
