using HealthCare.Data.Context;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Services;
using HealthCare.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//Repositories
builder.Services.AddTransient<ICredentialsRepository, CredentialsRepository>();

//Domain
builder.Services.AddTransient<ICredentialsService, CredentialsService>();


var connectionString = builder.Configuration.GetConnectionString("HealthCareConnection");
builder.Services.AddDbContext<HealthCareContext>(x => x.UseSqlServer(connectionString));

builder.Services.AddCors(options => {
    options.AddPolicy("CorsPolicy", 
        corsBuilder => corsBuilder.WithOrigins("http://localhost:43022").AllowAnyMethod()
           .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.`
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapControllers());

//app.MapRazorPages();

app.Run();
