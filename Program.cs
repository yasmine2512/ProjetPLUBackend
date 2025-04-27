using Microsoft.EntityFrameworkCore;
using MyBlazorApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Database context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 34)))
);

// Add configuration
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

//  Add HttpClient (IMPORTANT for ThesisSummarizerService)
builder.Services.AddHttpClient();

// ThesisSummarizerService
builder.Services.AddScoped<ThesisSummarizerService>();

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5000") // your frontend
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Kestrel server settings
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenLocalhost(5110); // HTTP
});

// Validation behavior
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false;
});

// Other services
builder.Services.AddScoped<UserState>();

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Static files
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_pictures")),
    RequestPath = "/profile_pictures"
});

// Middlewares
app.UseCors("AllowFrontend");
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Run the app
app.Run();
