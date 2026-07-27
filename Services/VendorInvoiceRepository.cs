using Microsoft.EntityFrameworkCore;
using VendorInvoiceApi.Data;
using VendorInvoiceApi.Models;

namespace VendorInvoiceApi.Services
{
	public class VendorInvoiceRepository
	{
		private readonly IApplicationDbContext _context;

		public VendorInvoiceRepository(IApplicationDbContext context)
		{
			_context = context;
		}

		// Cheap "did anything change" check — used later by the polling service.
		public Task<long> GetCurrentChangeVersionAsync(CancellationToken ct)
			=> _context.GetChangeTrackingVersionAsync(ct);

		// Full reload of the table.
		public async Task<List<VendorInvoiceRecord>> GetAllAsync(CancellationToken ct)
		{
			return await _context.VendorInvoices
				.AsNoTracking()
				.OrderBy(v => v.Id)
				.ToListAsync(ct);
		}
	}
}
