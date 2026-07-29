using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VendorInvoiceLogic.Abstractions
{
	public interface IInvoicePdfParser
	{
		ParsedInvoice Parse(string pdfText);
	}

	public class ParsedInvoiceLineItem
	{
		public string Material { get; set; } = string.Empty;
		public decimal Weight { get; set; }
	}

	public class ParsedInvoice
	{
		public int VendorId { get; set; }
		public DateTime ReceivedDate { get; set; }
		public DateTime? ExpiryDate { get; set; }
		public List<ParsedInvoiceLineItem> LineItems { get; set; } = new();
	}
}
