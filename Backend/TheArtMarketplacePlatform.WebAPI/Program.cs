using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TheArtMarketplacePlatform.BusinessLayer.Services;
using TheArtMarketplacePlatform.BusinessLayer.Validators;
using TheArtMarketplacePlatform.Core.Interfaces.Services;
using TheArtMarketplacePlatform.Core.Interfaces.Repositories;
using TheArtMarketplacePlatform.DataAccessLayer.Repositories;
using TheArtMarketplacePlatform.WebAPI.ExceptionHandlers;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

//
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TheArtMarketplacePlatform.DataAccessLayer.TheArtMarketplacePlatformDbContext>(options =>
    options
        .UseSqlServer(connectionString)
        .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll));

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Validators
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services
    .AddValidatorsFromAssemblyContaining<RegisterArtisanValidator>()
    .AddValidatorsFromAssemblyContaining<RegisterCustomerValidator>()
    .AddValidatorsFromAssemblyContaining<RegisterDeliveryPartnerValidator>()
    .AddValidatorsFromAssemblyContaining<ArtisanInsertProductValidator>()
    .AddValidatorsFromAssemblyContaining<ArtisanUpdateProductValidator>()
    .AddValidatorsFromAssemblyContaining<CustomerInsertOrderValidators>()
    .AddValidatorsFromAssemblyContaining<ArtisanUpdateOrderStatusValidators>()
;

// Handlers
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<InvalidCredentialsExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Services
builder.Services.AddScoped<IGuestService, GuestService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IArtisanService, ArtisanService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Add CORS policy
app.UseCors(policy =>
{
    policy.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader();
});

app.UseStaticFiles();

app.MapControllers();

app.Run();