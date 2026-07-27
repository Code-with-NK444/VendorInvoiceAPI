using Microsoft.EntityFrameworkCore;
using VendorInvoiceApi.Data;
using VendorInvoiceApi.Hubs;
using VendorInvoiceApi.Services;

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

			// Add services to the container.
			builder.Services.AddControllers();

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));

			builder.Services.AddScoped<IApplicationDbContext>(provider =>
				provider.GetRequiredService<ApplicationDbContext>());

			builder.Services.AddScoped<VendorInvoiceRepository>();

			// SignalR + the background service that watches Change Tracking
			// and pushes updates over the hub.
			builder.Services.AddSignalR();
			builder.Services.AddHostedService<ChangeTrackingPollingService>();

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("ReactClient", policy =>
					policy.WithOrigins(allowedOrigin)
						  .AllowAnyHeader()
						  .AllowAnyMethod()
						  .AllowCredentials()); // SignalR needs credentials allowed for its handshake
			});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
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