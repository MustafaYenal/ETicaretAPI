using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infrastructure;
using ETicaretAPI.Infrastructure.Filters;
using ETicaretAPI.Persistence;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using ETicaretAPI.Application;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ETicaretAPI.Infrastructure.Services.Storage.Local;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddPersistenceServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddStorage<LocalStorage>();
builder.Services.AddApplicationServices();

//builder.Services.AddStorage<LocalStorage>();

//builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
//policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
//));

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
policy.WithOrigins("http://localhost:4200/", "https://localhost:4200/").AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
));

builder.Services.AddControllers(options=>options.Filters.Add<ValidationFilters>())
    .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidator>())
//buradaki kod fluent validation � controllarda kod yazmadan devreye sokar ve gelen data validationa uymuyorsa hatay� clienta g�nderir
//e�er fluent validation� controllarda kontrol etmek istersek a�a��daki  gibi ge�erli olan filtreleri bast�trarak controllarda kontrol ederiz
.ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Admin",options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true,//Olu�turulacak token de�erinin kimlerin hangi sitelerin kullan�c�� belirledi�imiz de�erdir �r=>www.bilmemne.com
            ValidateIssuer = true,  //Olu�turulacak token de�erinin kimin da��tt���n� ifade edece�imiz aland�r=>www.myapi.com
            ValidateLifetime = true, //Olu�turulan token de�erinin s�resisni konrol edecek olan do�rulamad�r
            ValidateIssuerSigningKey = true, //Olu�turulan token de�erinin uygulamam�za ait bir de�er oldu�unu ifade eden bir secury key de�erinin do�rulanmas�d�r

            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])) 
        };
    });



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
