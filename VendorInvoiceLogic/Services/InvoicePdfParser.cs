using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VendorInvoiceLogic.Abstractions;

namespace VendorInvoiceLogic.Services
{
	public class InvoicePdfParser : IInvoicePdfParser
	{
		private static readonly Regex VendorIdRegex = new(@"Vendor ID:\s*(\d+)", RegexOptions.IgnoreCase);
		private static readonly Regex ReceivedDateRegex = new(@"Received Date:\s*([\d\-]+)", RegexOptions.IgnoreCase);
		private static readonly Regex ExpiryDateRegex = new(@"Expiry Date:\s*([\d\-]+)", RegexOptions.IgnoreCase);
		private static readonly Regex LineItemRegex = new(@"Material:\s*(.+?)\s*\|\s*Weight:\s*([\d.]+)", RegexOptions.IgnoreCase);

		public ParsedInvoice Parse(string pdfText)
		{
			var vendorIdMatch = VendorIdRegex.Match(pdfText);
			var receivedDateMatch = ReceivedDateRegex.Match(pdfText);
			var expiryDateMatch = ExpiryDateRegex.Match(pdfText);
			var lineItemMatches = LineItemRegex.Matches(pdfText);

			if (!vendorIdMatch.Success)
				throw new InvalidOperationException("Could not find 'Vendor ID:' in the PDF.");
			if (!receivedDateMatch.Success)
				throw new InvalidOperationException("Could not find 'Received Date:' in the PDF.");
			if (lineItemMatches.Count == 0)
				throw new InvalidOperationException("Could not find any 'Material: ... | Weight: ...' lines in the PDF.");

			var result = new ParsedInvoice
			{
				VendorId = int.Parse(vendorIdMatch.Groups[1].Value),
				ReceivedDate = DateTime.ParseExact(receivedDateMatch.Groups[1].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture),
				ExpiryDate = expiryDateMatch.Success
					? DateTime.ParseExact(expiryDateMatch.Groups[1].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture)
					: null,
			};

			foreach (Match match in lineItemMatches)
			{
				result.LineItems.Add(new ParsedInvoiceLineItem
				{
					Material = match.Groups[1].Value.Trim(),
					Weight = decimal.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
				});
			}

			return result;
		}
	}
}
