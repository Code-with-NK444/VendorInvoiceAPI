using Microsoft.EntityFrameworkCore;
using VendorInvoiceApi.BackgroundServices;
using VendorInvoiceApi.Hubs;
using VendorInvoiceLogic.Abstractions;
using VendorInvoiceLogic.Services;
using VendorInvoiceRepo;

namespace VendorInvoiceApi
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
				?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured in appsettings.json.");

			var allowedOrigin = builder.Configuration["Cors:AllowedOrigin"] ?? "http://localhost:5173";

			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));

			builder.Services.AddScoped<IApplicationDbContext>(provider =>
				provider.GetRequiredService<ApplicationDbContext>());

			builder.Services.AddScoped<IVendorInvoiceService, VendorInvoiceService>();

			builder.Services.AddSignalR();
			builder.Services.AddHostedService<ChangeTrackingPollingService>();

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("ReactClient", policy =>
					policy.WithOrigins(allowedOrigin)
						  .AllowAnyHeader()
						  .AllowAnyMethod()
						  .AllowCredentials());
			});

			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseCors("ReactClient");
			app.UseAuthorization();

			app.MapControllers();
			app.MapHub<VendorInvoiceHub>("/hubs/vendor-invoice");

			app.Run();
		}
	}
}