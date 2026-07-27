using Microsoft.AspNetCore.SignalR;
using VendorInvoiceApi.Hubs;

namespace VendorInvoiceApi.Services
{
	public class ChangeTrackingPollingService : BackgroundService
	{

		private readonly IServiceScopeFactory _scopeFactory;
		private readonly IHubContext<VendorInvoiceHub> _hubContext;
		private readonly ILogger<ChangeTrackingPollingService> _logger;
		private readonly TimeSpan _pollInterval;

		private long _lastVersion = long.MinValue; // sentinel: "not initialized yet"

		public ChangeTrackingPollingService(
			IServiceScopeFactory scopeFactory,
			IHubContext<VendorInvoiceHub> hubContext,
			IConfiguration configuration,
			ILogger<ChangeTrackingPollingService> logger)
		{
			_scopeFactory = scopeFactory;
			_hubContext = hubContext;
			_logger = logger;

			var seconds = configuration.GetValue<int?>("ChangePolling:IntervalSeconds") ?? 2;
			_pollInterval = TimeSpan.FromSeconds(seconds);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					using var scope = _scopeFactory.CreateScope();
					var repository = scope.ServiceProvider.GetRequiredService<VendorInvoiceRepository>();

					var currentVersion = await repository.GetCurrentChangeVersionAsync(stoppingToken);

					if (_lastVersion == long.MinValue)
					{
						_lastVersion = currentVersion; // baseline on startup, don't broadcast
					}
					else if (currentVersion != _lastVersion)
					{
						_lastVersion = currentVersion;

						var data = await repository.GetAllAsync(stoppingToken);

						await _hubContext.Clients.All.SendAsync(
							"ReceiveVendorInvoiceUpdate", data, stoppingToken);

						_logger.LogInformation(
							"Vendor_Invoice_Data changed (version {Version}); pushed {Count} rows.",
							currentVersion, data.Count);
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error while polling Vendor_Invoice_Data change tracking version.");
				}

				try
				{
					await Task.Delay(_pollInterval, stoppingToken);
				}
				catch (OperationCanceledException)
				{
					// shutting down
				}
			}
		}
	}
}
