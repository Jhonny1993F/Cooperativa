using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Cooperativa.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin() // Permite cualquier origen
                   .AllowAnyMethod() // Permite cualquier método (GET, POST, etc.)
                   .AllowAnyHeader(); // Permite cualquier encabezado
        });
});

builder.Services.AddDbContext<CooperativaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CooperativaContext") ?? throw new InvalidOperationException("Connection string 'CooperativaContext' not found.")));

// Configuración de la autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Logins/Login";  // Página de inicio de sesión
        options.LogoutPath = "/Logins/Logout";  // Página de cierre de sesión
    });

// Agregar soporte de sesiones
builder.Services.AddDistributedMemoryCache(); // Utiliza memoria para almacenar sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Expiración de la sesión después de 30 minutos de inactividad
    options.Cookie.HttpOnly = true; // Asegura que la cookie de sesión sea solo accesible desde HTTP
    options.Cookie.IsEssential = true; // La sesión es esencial para la aplicación
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Habilitar el uso de CORS
app.UseCors("AllowAll"); // Middleware para habilitar CORS

// Habilitar el uso de sesiones
app.UseSession(); // Middleware para habilitar el uso de sesiones

app.UseAuthentication();  // Habilitar la autenticación
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
