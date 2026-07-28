using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VendorInvoiceRepo.Data;

namespace VendorInvoiceLogic.Abstractions
{
	public interface IVendorInvoiceService
	{
		Task<long> GetCurrentChangeVersionAsync(CancellationToken ct);
		Task<List<VendorInvoiceRecord>> GetAllAsync(CancellationToken ct);
	}
}
