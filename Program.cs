using ExpenseTracker.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure application cookie settings.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login/Login";
    options.AccessDeniedPath = "/Login/AccessDenied";
});

// Retrieve JWT secret from configuration or use default.
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "your-256-bit-secret";
var key = Encoding.UTF8.GetBytes(jwtSecret);
//var jwtSecret = builder.Configuration["Jwt:Secret"];
//var key = Encoding.UTF8.GetBytes(jwtSecret);


// Add authentication and configure both cookie and JWT Bearer schemes.
builder.Services.AddAuthentication(options =>
{
    // Set the default authentication scheme to the Identity cookie scheme.
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
})
.AddCookie() // This is used by Identity.
.AddJwtBearer("JwtBearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); // Enable authentication middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
