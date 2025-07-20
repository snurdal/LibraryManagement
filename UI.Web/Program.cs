using Core.Concretes.Maps;
using Business.Middlewares;
using Microsoft.EntityFrameworkCore;
using UI.Web.Helpers;

var builder = WebApplication.CreateBuilder(args);

# region AutoMapper configuration, Core Layer - Maps - Mapping Profile
builder.Services.AddAutoMapper(opt =>
{
    opt.AddProfile<MappingProfile>();
});
#endregion

// Add services to the container.
builder.Services.AddControllersWithViews();

#region Database configuration, Bussines Layer - Middlewares - calling CustomServiceExtensions class 
builder.Services.AddDatabaseConnections(builder.Configuration);
builder.Services.AddDTOServices();
#endregion
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
