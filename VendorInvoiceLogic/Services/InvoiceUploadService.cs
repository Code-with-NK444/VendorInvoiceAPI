using VendorInvoiceLogic.Abstractions;
using VendorInvoiceRepo;
using VendorInvoiceRepo.Data;

namespace VendorInvoiceLogic.Services
{
	public class InvoiceUploadService : IInvoiceUploadService
	{
		private readonly IApplicationDbContext _context;
		private readonly IInvoicePdfParser _parser;
		private readonly string _storagePath;

		public InvoiceUploadService(IApplicationDbContext context, IInvoicePdfParser parser, string storagePath)
		{
			_context = context;
			_parser = parser;
			_storagePath = storagePath;
		}

		public async Task<int> UploadAsync(Stream pdfStream, string fileName, CancellationToken ct)
		{
			// Read the PDF into memory once - needed both for text extraction and for saving to disk.
			using var memoryStream = new MemoryStream();
			await pdfStream.CopyToAsync(memoryStream, ct);
			var pdfBytes = memoryStream.ToArray();

			var text = ExtractText(pdfBytes);
			var parsed = _parser.Parse(text);

			Directory.CreateDirectory(_storagePath);
			var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
			var fullPath = Path.Combine(_storagePath, uniqueFileName);
			await File.WriteAllBytesAsync(fullPath, pdfBytes, ct);

			var invoiceFile = new InvoiceFile
			{
				FileName = fileName,
				FilePath = fullPath,
				UploadedDate = DateTime.UtcNow,
			};
			_context.InvoiceFiles.Add(invoiceFile);
			await _context.SaveChangesAsync(ct); // so invoiceFile.Id is generated before we use it below

			foreach (var line in parsed.LineItems)
			{
				_context.VendorInvoices.Add(new VendorInvoiceRecord
				{
					VendorId = parsed.VendorId,
					Material = line.Material,
					Weight = line.Weight,
					ReceivedDate = parsed.ReceivedDate,
					ExpiryDate = parsed.ExpiryDate,
					InvoiceFileId = invoiceFile.Id,
				});
			}

			await _context.SaveChangesAsync(ct);

			return parsed.LineItems.Count;
		}

		private static string ExtractText(byte[] pdfBytes)
		{
			using var document = UglyToad.PdfPig.PdfDocument.Open(pdfBytes);
			var text = new System.Text.StringBuilder();
			foreach (var page in document.GetPages())
			{
				text.AppendLine(page.Text);
			}
			return text.ToString();
		}
	}
}
