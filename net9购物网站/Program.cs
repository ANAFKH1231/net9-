using Microsoft.EntityFrameworkCore;
using net9购物网站.Data;
using net9购物网站.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ensure the authentication scheme is correctly configured

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/login"; 
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API 文档 V1");
    c.RoutePrefix = "swagger"; // Ensure Swagger's route prefix does not conflict with other routes
});


// Set the default page to showgoods.cshtml
app.MapGet("/", context =>
{
    context.Response.Redirect("/showgoods");
    return Task.CompletedTask;
});

app.Run();
