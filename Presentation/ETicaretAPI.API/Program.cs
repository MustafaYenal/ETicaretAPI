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
//buradaki kod fluent validation ý controllarda kod yazmadan devreye sokar ve gelen data validationa uymuyorsa hatayý clienta gönderir
//eðer fluent validationý controllarda kontrol etmek istersek aþaðýdaki  gibi geçerli olan filtreleri bastýtrarak controllarda kontrol ederiz
.ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Admin",options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true,//Oluþturulacak token deðerinin kimlerin hangi sitelerin kullanýcýý belirlediðimiz deðerdir ör=>www.bilmemne.com
            ValidateIssuer = true,  //Oluþturulacak token deðerinin kimin daðýttýðýný ifade edeceðimiz alandýr=>www.myapi.com
            ValidateLifetime = true, //Oluþturulan token deðerinin süresisni konrol edecek olan doðrulamadýr
            ValidateIssuerSigningKey = true, //Oluþturulan token deðerinin uygulamamýza ait bir deðer olduðunu ifade eden bir secury key deðerinin doðrulanmasýdýr

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
