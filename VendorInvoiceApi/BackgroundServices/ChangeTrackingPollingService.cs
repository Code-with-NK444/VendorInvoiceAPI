using Microsoft.AspNetCore.SignalR;
using VendorInvoiceApi.Hubs;
using VendorInvoiceLogic.Abstractions;

namespace VendorInvoiceApi.BackgroundServices
{
	public class ChangeTrackingPollingService : BackgroundService
	{
		private readonly IServiceScopeFactory _scopeFactory;
		private readonly IHubContext<VendorInvoiceHub> _hubContext;
		private readonly ILogger<ChangeTrackingPollingService> _logger;
		private readonly TimeSpan _pollInterval;

		private long _lastVersion = long.MinValue;

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
					var service = scope.ServiceProvider.GetRequiredService<IVendorInvoiceService>();

					var currentVersion = await service.GetCurrentChangeVersionAsync(stoppingToken);

					if (_lastVersion == long.MinValue)
					{
						_lastVersion = currentVersion;
					}
					else if (currentVersion != _lastVersion)
					{
						_lastVersion = currentVersion;
						var data = await service.GetAllAsync(stoppingToken);

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
				}
			}
		}
	}
}
