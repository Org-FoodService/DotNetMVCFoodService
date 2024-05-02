using FoodService.Config.Ioc;
using FoodService.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add connection to Database
string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.ConfigureDatabase(mySqlConnection);
builder.Services.UpdateMigrationDatabase();

builder.Services.ConfigureAuthentication(builder);

// Add IOC
builder.Services.ConfigureRepositoryIoc();
builder.Services.ConfigureServiceIoc();
builder.Services.ConfigureCommandIoc();

builder.Services.AddControllersWithViews(options =>
{
    // Add the global custom exception filter
    //options.Filters.Add(new CustomExceptionFilterAttribute());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Middleware for error handling
    app.UseStatusCodePagesWithRedirects("/Home/Index"); // Middleware for redirecting not found pages
    app.UseHsts();
}

app.UseHttpsRedirection(); // Redirects all HTTP requests to HTTPS
app.UseStaticFiles();

app.UseRouting();

using (var scope = app.Services.CreateScope())
{
    await scope.AddAdminRole();
    await scope.AddEmployeeRole();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();