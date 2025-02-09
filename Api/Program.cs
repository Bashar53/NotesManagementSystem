using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ 1️⃣ Load JWT settings from `appsettings.json`
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings.GetValue<string>("SecretKey") ?? throw new ArgumentNullException("JwtSettings:SecretKey is missing!");
var issuer = jwtSettings.GetValue<string>("Issuer") ?? throw new ArgumentNullException("JwtSettings:Issuer is missing!");
var audience = jwtSettings.GetValue<string>("Audience") ?? throw new ArgumentNullException("JwtSettings:Audience is missing!");

// Validate that the secret key is strong enough
if (secretKey.Length < 32)
{
    throw new InvalidOperationException("JWT SecretKey must be at least 32 characters (256 bits) long!");
}

// Setup JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)) // Ensure the secret is the same as in token generation
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"error\": \"Unauthorized - Invalid Token\"}");
            }
        };


    });


// ✅ 2️⃣ Configure CORS (Allow frontend requests)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ✅ 3️⃣ Add JWT Authentication


// ✅ 4️⃣ Add Authorization
builder.Services.AddAuthorization();

// ✅ 5️⃣ Add API Controllers
builder.Services.AddControllers();

// ✅ 6️⃣ Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // 🔥 Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}' to authenticate",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// ✅ 7️⃣ Build Application
var app = builder.Build();

// ✅ Enable CORS (Placed BEFORE Authentication)
app.UseCors("AllowAll");

// ✅ Enable HTTPS
app.UseHttpsRedirection();

// ✅ Enable Authentication & Authorization
app.UseAuthentication(); // Only once
app.UseAuthorization();  // Only once

// ✅ Enable Swagger (Only in Development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Map API Controllers
app.MapControllers();

// ✅ 9️⃣ Run Application
app.Run();
