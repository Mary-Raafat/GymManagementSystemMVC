using GymManagementSystem.DAL.Data;
using GymManagementSystem.DAL.Implementation;
using GymManagementSystem.DAL.Interceptors;
using GymManagementSystem.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using GymManagementSystem.Dbcontexts;
using GymManagementSystem.DAL;
using GymManagementSystem.BLL.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGymManagementSystemDAL(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddGymManagementSystemBLL();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); //Default



using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GymContext>();
    await context.Database.MigrateAsync();// Apply any pending migrations
    await DataSeeder.SeedDataAsync(context);

}
app.Run();
