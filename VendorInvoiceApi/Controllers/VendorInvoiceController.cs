using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VendorInvoiceLogic.Abstractions;
using VendorInvoiceRepo.Data;

namespace VendorInvoiceApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VendorInvoiceController : ControllerBase
	{
		private readonly IVendorInvoiceService _service;
		private readonly IInvoiceUploadService _uploadService;

		public VendorInvoiceController(IVendorInvoiceService service, IInvoiceUploadService uploadService)
		{
			_service = service;
			_uploadService = uploadService;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<VendorInvoiceRecord>>> GetAll(CancellationToken ct)
		{
			var data = await _service.GetAllAsync(ct);
			return Ok(data);
		}

		[HttpPost("upload")]
		[RequestSizeLimit(10_000_000)] // 10 MB cap
		public async Task<IActionResult> UploadInvoice(IFormFile file, CancellationToken ct)
		{
			if (file is null || file.Length == 0)
				return BadRequest("No file uploaded.");

			if (Path.GetExtension(file.FileName).ToLowerInvariant() != ".pdf")
				return BadRequest("Only PDF files are accepted.");

			try
			{
				using var stream = file.OpenReadStream();
				var rowsInserted = await _uploadService.UploadAsync(stream, file.FileName, ct);
				return Ok(new { rowsInserted });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message); // parsing failure - couldn't find expected fields
			}
		}
	}
}
