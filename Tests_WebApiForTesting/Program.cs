using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddControllers();

// swagger
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles(); // this will serve Index.html, and other default files.
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();
app.MapRazorPages();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();
// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
// specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("../swagger/v1/swagger.json", "My API V1");
});


app.Run();