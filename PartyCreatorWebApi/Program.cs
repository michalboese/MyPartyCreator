global using PartyCreatorWebApi.Data;
global using Microsoft.EntityFrameworkCore;
global using Azure.Storage.Blobs;
using PartyCreatorWebApi.HubConfig;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using PartyCreatorWebApi.Repositories.Contracts;
using PartyCreatorWebApi.Repositories;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
        builder
        .WithOrigins("http://localhost:4200", "https://partycreatorfrontend.azurewebsites.net", "https://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

if (builder.Environment.IsProduction())
{
    builder.Services.AddSignalR().AddAzureSignalR(options =>
    {
        options.ConnectionString = "Endpoint=https://partycreatorsignalr.service.signalr.net;AccessKey=e9CUqGZI3z1NnA0d17Dk2JrUPZQb6jCd52zc23Ygjmg=;Version=1.0;";
    });
}

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSignalR();
}

builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration.GetConnectionString("AzureBlobConnection")));

builder.Services.AddControllers();

if (builder.Environment.IsProduction())
{
    builder.Services.AddDbContext<DataContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("PubConnection"));
    });
}

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<DataContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

}

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IShoppingListRepository, ShoppingListRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IBlobStorageRepository, BlobStorageRepository>();
builder.Services.AddScoped<IReceiptItemRepository, ReceiptItemRepository>();
builder.Services.AddScoped<ISpotifyRepository, SpotifyRepository>();
builder.Services.AddScoped<ISurveyRepository, SurveyRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (string.IsNullOrEmpty(accessToken) == false)
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });



var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat");

app.Run();
