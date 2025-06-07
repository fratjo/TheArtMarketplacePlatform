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
// => Normaly dont need to specify the validators
builder.Services
    .AddValidatorsFromAssemblyContaining<RegisterArtisanValidator>()
    .AddValidatorsFromAssemblyContaining<RegisterCustomerValidator>()
    .AddValidatorsFromAssemblyContaining<RegisterDeliveryPartnerValidator>()
    .AddValidatorsFromAssemblyContaining<ArtisanInsertProductValidator>()
    .AddValidatorsFromAssemblyContaining<ArtisanUpdateProductValidator>()
    .AddValidatorsFromAssemblyContaining<CustomerInsertOrderValidators>()
    .AddValidatorsFromAssemblyContaining<ArtisanUpdateOrderStatusValidators>()
    .AddValidatorsFromAssemblyContaining<CustomerUpdateProfileRequestValidator>()
    .AddValidatorsFromAssemblyContaining<ArtisanUpdateProfileRequestValidator>()
    .AddValidatorsFromAssemblyContaining<DeliveryPartnerUpdateProfileRequestValidator>()
;

// Handlers
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ArgumentExceptionHandler>();
builder.Services.AddExceptionHandler<ArgumentOutOfRangeExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<UnauthorizedAccessExceptionHandler>();
builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<InvalidOperationExceptionHandler>();
builder.Services.AddExceptionHandler<InvalidCredentialsExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Services
builder.Services.AddScoped<IGuestService, GuestService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IArtisanService, ArtisanService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IDeliveryPartnerService, DeliveryPartnerService>();

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Seed the database with initial data
        TheArtMarketplacePlatform.DataAccessLayer.Seed.SeedData.Initialize(services);
        Console.WriteLine("Database updated & seeded successfully.");
    }
    catch (Exception ex)
    {
        // Handle exceptions during seeding
        Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
    }
}

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