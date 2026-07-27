using Microsoft.EntityFrameworkCore;
using VendorInvoiceApi.Models;

namespace VendorInvoiceApi.Data
{
	public interface IApplicationDbContext
	{
		DbSet<VendorInvoiceRecord> VendorInvoices { get; }

		Task<long> GetChangeTrackingVersionAsync(CancellationToken cancellationToken);

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}
