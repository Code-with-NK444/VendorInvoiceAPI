using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendorInvoiceLogic.Abstractions;
using VendorInvoiceRepo;
using VendorInvoiceRepo.Data;

namespace VendorInvoiceLogic.Services
{
	public class VendorInvoiceService : IVendorInvoiceService
	{
		private readonly IApplicationDbContext _context;

		public VendorInvoiceService(IApplicationDbContext context)
		{
			_context = context;
		}

		public Task<long> GetCurrentChangeVersionAsync(CancellationToken ct)
			=> _context.GetChangeTrackingVersionAsync(ct);

		public async Task<List<VendorInvoiceRecord>> GetAllAsync(CancellationToken ct)
		{
			return await _context.VendorInvoices
				.AsNoTracking()
				.OrderBy(v => v.Id)
				.ToListAsync(ct);
		}
	}
}
