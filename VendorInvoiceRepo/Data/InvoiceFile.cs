using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendorInvoiceRepo.Data
{
	public class InvoiceFile
	{
		public int Id { get; set; }
		public string FileName { get; set; } = string.Empty;
		public string FilePath { get; set; } = string.Empty;
		public DateTime UploadedDate { get; set; }
	}
}
