using ChatApp.Data;
using ChatApp.Hubs;
using ChatApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); builder.Services.AddSwaggerGen(options =>
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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme
    ).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
            (config.GetSection("jwt:Key").Value)),
            ValidateLifetime = true,
            ValidateAudience = false,
            ValidateIssuer = false,
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
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors= true;
});
builder.Services.AddDbContext<ChatAppDatabase>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ChatAppDatabaseConnectionString")));
builder.Services.AddCors(options => options.AddPolicy(name: "CorsPolicy",
    policy =>
    {
        policy.WithOrigins().AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
    }
    ));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IFileService, FileService>();


var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();

try
{
    string path = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
    if (!Directory.Exists(path))
    {
        Directory.CreateDirectory(path);
    }
    app.UseStaticFiles(new StaticFileOptions
    {
        //File assests = new File()
        FileProvider = new PhysicalFileProvider(
               Path.Combine(builder.Environment.ContentRootPath, "Assets")),
        RequestPath = "/Assets"
    });
}
catch(Exception ex)
{
    Console.WriteLine(ex.ToString()); 
}

app.UseCors("CorsPolicy");
app.UseAuthentication();

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    app.MapControllers();
    endpoints.MapHub<ChatHub>("/chatHub");
});
app.MapControllers();

app.Run();
