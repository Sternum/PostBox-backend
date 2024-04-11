using ForumSchoolProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ProjektGContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Add Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ProjektGContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireAssertion(context =>
    {
        var userGroupId = context.User.FindFirst("UserGroupId")?.Value;
        // Check if the user group ID matches your Admin group ID
        return userGroupId == "1"; 
    }));
    options.AddPolicy("AdminOrOwner", policy =>
    policy.RequireAssertion(context =>
    {
        var user = context.User;
        var userGroupId = user.FindFirst("UserGroupId")?.Value; // Assuming you store group ID in claims
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Assuming you pass the user ID as a route parameter named "id"
        var routeData = context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext;
        var requestedUserId = routeData?.HttpContext.Request.RouteValues["id"]?.ToString();

        // Check if the user is an admin or the owner of the account
        return userGroupId == "1" || userId == requestedUserId;
    }));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
