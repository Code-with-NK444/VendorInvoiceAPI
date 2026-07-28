using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendorInvoiceRepo.Data
{
	public class VendorInvoiceRecord
	{
		public int Id { get; set; }
		public int VendorId { get; set; }
		public string Material { get; set; } = string.Empty;
		public decimal Weight { get; set; }
		public DateTime ReceivedDate { get; set; }
		public DateTime? ExpiryDate { get; set; }
	}
}
