using AutoMapper.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;

// This Static Method Creates a "WebApplicationBuilder" instance using default configurations (e.g., Kestrel server, appsettings, logging, DI)
/// This line sets up the environment for the web app:
/// - Prepares the Kestrel server to receive HTTP requests
/// - Loads configuration sources (like appsettings.json, environment variables, CLI args)
/// - Sets up logging infrastructure
/// - Initializes the Dependency Injection container for services
var builder = WebApplication.CreateBuilder(args);

#region Configure Services [Allow DI For Them]
// --> Here I Configured A Services Only, We Can Configure 4 Things Also Here

// Add services to the container.
            
builder.Services.AddControllers(); // Allow DI For API Services

builder.Services.AddSwaggerServices();

builder.Services.AddDbContext<StoreContext>(options => // Allow DI For DbContext and For Options
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddDbContext<AppIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
});


builder.Services.AddSingleton<IConnectionMultiplexer>(options =>
{
    var connection = builder.Configuration.GetConnectionString("Redis");
    return ConnectionMultiplexer.Connect(connection);
});

builder.Services.AddApplicationServices(); /*Extenstion Method*/

builder.Services.AddIdentityServices(builder.Configuration); /*Extenstion Method*/

// Allow DI For Cors Services
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", options =>
    {
        options.AllowAnyHeader().AllowAnyMethod().WithOrigins(builder.Configuration["FrontBaseUrl"]);
    });
});

#endregion

var app = builder.Build();

#region Apply Migration And Data Seeding

using var scope = app.Services.CreateScope(); // Create Scope Manually/Explicitly
var services = scope.ServiceProvider; // Provider For All Services That Works "Scoped"
var loggerFactory = services.GetRequiredService<ILoggerFactory>();

try
{
    var dbContext = services.GetRequiredService<StoreContext>(); // Ask CLR Explicitly For Creating Obj From DbContext [StoreContext]
    await dbContext.Database.MigrateAsync(); // Update-Database [Apply Migration]

    await StoreContextSeed.SeedAsync(dbContext);

    /*For Identity DB*/
    //var identityDbContext = services.GetRequiredService<AppIdentityDbContext>();
    //await identityDbContext.Database.MigrateAsync(); // Update-Database

    var userManager = services.GetRequiredService<UserManager<AppUser>>(); // Ask CLR To Inject Obj From UserManager Explicitly [U Must Allow DI For It => Using AddAuthentication();]
    await AppIdentityDbContextSeed.SeedUsersAsync(userManager);
}
catch (Exception ex) // Catch Thrown Exception
{
    //Console.WriteLine(ex); // Show Exception Details In Console Screen [Not Readable]
    // Better =>
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogError(ex, "An error occurred during apply the migration.");

}

#endregion

#region Configure App [Kestrel] MiddleWares

app.UseMiddleware<ExceptionMiddleware>(); // Custom Middleware



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerMiddlewares();
}


//app.UseStatusCodePagesWithRedirects("/errors/{0}");
app.UseStatusCodePagesWithReExecute("/errors/{0}");


// You Installed SSL Certificate On The Deploying Server, Result in Any Request Redirect Over Https Protocol
app.UseHttpsRedirection();

app.UseStaticFiles();


app.UseCors("MyPolicy");


app.UseAuthentication();
app.UseAuthorization();

#region Routing MiddleWare

// [In .NET 5] (OLD)
/*
    app.UseRouting();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
*/

// OR [In .NET 6] (NEW) -> Rely On Routing Per Controller
app.MapControllers();

#endregion


#endregion

app.Run();
