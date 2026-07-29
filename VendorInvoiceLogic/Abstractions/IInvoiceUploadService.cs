using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendorInvoiceLogic.Abstractions
{
	public interface IInvoiceUploadService
	{
		Task<int> UploadAsync(Stream pdfStream, string fileName, CancellationToken ct);
	}
}
