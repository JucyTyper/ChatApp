using ChatApp.Data;
using ChatApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}
    ).AddCookie(options =>
    {
        options.LoginPath = "/account/google-login";
    }).AddGoogle(options =>
    {
        options.ClientId = "83499501732-0rcfo4ffhe5m02cjru0rb19m50fa8sl7.apps.googleusercontent.com";
        options.ClientSecret = "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";
        options.ClaimActions.MapJsonKey("urn: google: picture", "picture", "url");
    });
builder.Services.AddDbContext<ChatAppDatabase>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ChatAppDatabaseConnectionString")));
builder.Services.AddCors(options => options.AddPolicy(name: "CorsPolicy",
    policy =>
    {
        policy.WithOrigins().AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
    }
    ));

builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();

app.UseCors("CorsPolicy");
app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
