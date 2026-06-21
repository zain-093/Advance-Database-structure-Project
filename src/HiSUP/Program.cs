using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using HiSUP.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register HttpContextAccessor to read claims inside the database connection interceptor
builder.Services.AddHttpContextAccessor();

// Connect to SQL Server and attach the RLS interceptor, passing the HttpContextAccessor
builder.Services.AddDbContext<HiSUPContext>((serviceProvider, options) =>
{
    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
    var connectionString = builder.Configuration.GetConnectionString("HiSUP_DB");
    options.UseSqlServer(connectionString)
           .AddInterceptors(new SessionConnectionInterceptor(httpContextAccessor));
});

// Configure Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Design-time DB Context factory pointing to HITECUNI_DB
public class HiSUPContextFactory : Microsoft.EntityFrameworkCore.Design.IDesignTimeDbContextFactory<HiSUP.Data.HiSUPContext>
{
    public HiSUP.Data.HiSUPContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HiSUP.Data.HiSUPContext>();
        optionsBuilder.UseSqlServer("Server=.;Database=HiSUP_DB;Trusted_Connection=True;TrustServerCertificate=True;");
        return new HiSUP.Data.HiSUPContext(optionsBuilder.Options);
    }
}