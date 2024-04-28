using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration;
string key = appSettings.GetValue<string>("JWTSecretKey");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//JWT Config
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer(cfg =>
{
    var signKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var signCred = new SigningCredentials(signKey, SecurityAlgorithms.HmacSha256);

    cfg.RequireHttpsMetadata = false;
    cfg.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = signKey,
    };
});

//RateLimiting Config
builder.Services.AddMemoryCache();

builder.Services.Configure<IpRateLimitOptions>(appSettings.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(appSettings.GetSection("IpRateLimitPolicies"));

builder.Services.AddInMemoryRateLimiting();

//builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
//builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseIpRateLimiting();

app.MapControllers().RequireAuthorization();

app.MapGet("/auth/{user}/{password}", (string user, string password) =>
{
    if(user == "testUser" && password == "testPassword")
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var keyEncoded = Encoding.UTF8.GetBytes(key);
        var tokenDes = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user),

            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyEncoded), SecurityAlgorithms.HmacSha256),
        };

        var token = tokenHandler.CreateToken(tokenDes);
        return tokenHandler.WriteToken(token);
    }
    else
    {
        return "usuario invalido.";
    }
});

app.Run();
