using BCryptPasswordEncryptor;
using ForumSchoolProject.Authorization;
using ForumSchoolProject.Models;
using ForumSchoolProject.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Dependency injection
builder.Services.AddScoped<IPasswordEncryptor>(serviceProvider =>
{
	// Create an instance of BCryptPasswordEncryptor using the factory method
	return BCryptPasswordEncryptor.Factory.CreateEncryptor();
});
builder.Services.AddScoped<IAuthorizationHelperService, AuthorizationHelperService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<ProjektGContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
			ValidateIssuer = false,
			ValidateAudience = false
		};
	});

// Add CORS services
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAllOrigins",
		builder => builder
			.AllowAnyOrigin() // Allow any origin
			.AllowAnyHeader()
			.AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

	// Define the Bearer Authentication scheme
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});

	// Ensure that the authorization requirement is applied globally
	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				},
				Scheme = "oauth2",
				Name = "Bearer",
				In = ParameterLocation.Header
			},
			new List<string>()
		}
	});
});

var app = builder.Build();

// Use CORS middleware
app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
	c.RoutePrefix = string.Empty; // Set the Swagger UI at the app's root (optional)
});

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
