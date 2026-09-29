using BibliotecaData.Configuracion;
using BibliotecaData.Contrato;
using BibliotecaData.Implementacion;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// ======================
// SERVICES
// ======================
builder.Services.AddControllersWithViews();

// Configuración de cadena de conexión
builder.Services.Configure<ConnectionStrings>(
    builder.Configuration.GetSection("ConnectionStrings"));

// Repositorios
builder.Services.AddScoped<IEstudianteRepositorio, EstudianteRepositorio>();
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<ILibroRepositorio, LibroRepositorio>();
builder.Services.AddScoped<IPrestamoRepositorio, PrestamoRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IDashboardRepositorio, DashboardRepositorio>();

// Autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Acceso/Login";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        option.AccessDeniedPath = "/Acceso/Denegado";
    });
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://localhost:5291");
}
else
{
    builder.WebHost.UseUrls("http://0.0.0.0:5290");
}
var app = builder.Build();

// ======================
// PIPELINE
// ======================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// IMPORTANTE: orden correcto
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ======================
// RUTA INICIAL
// ======================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}");

// ======================
// AUTO ABRIR NAVEGADOR(EXE tipo app real)
// ======================
if (!app.Environment.IsDevelopment())
{
    _ = Task.Run(async () =>
    {
        await Task.Delay(1500);

        Process.Start(new ProcessStartInfo
        {
            FileName = "http://localhost:5290",
            UseShellExecute = true
        });
    });
}

app.Run();