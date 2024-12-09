        using BTLWebAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var key = builder.Configuration["Jwt:key"];

//mã hóa key
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

//Add Authentication Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //có ki?m tra Issuer (default true)
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        //có ki?m tra Audience (default true)
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        //??m b?o ph?i có th?i gian h?t h?n trong token

        RequireExpirationTime = true,
        ValidateLifetime = true,
        //Ch? ra key s? d?ng trong token
        IssuerSigningKey = signingKey,
        RequireSignedTokens = true

    };
});

/*builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });*/
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<BTLContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Connect")));
builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowOrigin", p =>
    {
        p.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();

app.Run();

app.UseStaticFiles();