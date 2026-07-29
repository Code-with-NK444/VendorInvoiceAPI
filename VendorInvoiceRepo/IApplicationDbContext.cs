using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendorInvoiceRepo.Data;

namespace VendorInvoiceRepo
{
	public interface IApplicationDbContext
	{
		DbSet<VendorInvoiceRecord> VendorInvoices { get; }
		DbSet<InvoiceFile> InvoiceFiles { get; }   // new
		Task<long> GetChangeTrackingVersionAsync(CancellationToken cancellationToken);
		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}
