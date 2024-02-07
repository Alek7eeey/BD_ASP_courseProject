using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskWave_MVC;
using TaskWave_MVC.Data;
using TaskWave_MVC.Data.Repository;
using TaskWave_MVC.Middlewares;
using TaskWave_MVC.Services;
using TaskWave_MVC.Services.Implementation.Default;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

builder.Services.Configure<AppSettings>(configuration.GetSection("ConnectionStrings"));
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<RegisterUserService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<Connection>();
builder.Services.AddScoped<SignInUserService>();
builder.Services.AddTransient<JwtService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<ProjectRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = false,
               ValidateAudience = false,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TaskWave2023_11092003TaskWave2023_11092003"))
           };
       });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseMiddleware<JwtMiddleware>();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Default}/{action=SignIn}/{id?}");

app.MapControllerRoute(
    name: "user",
    pattern: "{controller=User}/{action=MainPage}/{token?}");

app.Run();
