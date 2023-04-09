using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Data;

var builder = WebApplication.CreateBuilder(args);

// Add Services
builder.Services.AddAuthentication("Bearer")
        .AddJwtBearer( o => {
                o.RequireHttpsMetadata = false;

                var keyInput = "Ridwan_Abdirashid_Mohamed_Haid_Gouled_Mohamed";
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyInput));

                o.TokenValidationParameters = new TokenValidationParameters
                {
                        ValidateIssuer = true,
                        ValidIssuer = "MyAPI",
                        ValidateAudience = true,
                        ValidAudience = "MyFrontEnd",
                        ValidateLifetime = true,
                        IssuerSigningKey = key
                };
        });
builder.Services.AddControllers()
        .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppointmentsDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
);

// building CORS

builder.Services.AddCors(config => config.AddPolicy("Default", c=> c.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("Default");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
