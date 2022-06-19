using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using RolebaseAccess.Configuration;
using RolebaseAccess.Data;

var builder = WebApplication.CreateBuilder(args);

OpenApiSecurityScheme  openApiSecurityScheme = new OpenApiSecurityScheme {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = OpenApiSecurityApiKeyLocation.Header
        };
        
// Add services to the container.
List<string> token = new List<string>() ;
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

builder.Services.AddDbContext<ApiDbContext>(options =>
options.UseSqlite(
    builder.Configuration.GetConnectionString("DefaultConnection")
));

var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);
var tokenValidationParameters = new TokenValidationParameters{
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        RequireExpirationTime = false
    };
builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt => {
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = tokenValidationParameters;
});

builder.Services.AddAuthorization(options => {
    options.AddPolicy("DepartmentPolicy",
        policy => policy.RequireClaim("department"));
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
.AddEntityFrameworkStores<ApiDbContext>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerDocument(Options=>{
    Options.Title = "Role based access system";
    Options.Description = "Developer: Stephen Kaguri Kanyago";
    Options.Version = "V1";
    Options.AddSecurity("Bearer",token, openApiSecurityScheme);
});

var app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUi3();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
