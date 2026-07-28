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

		public VendorInvoiceController(IVendorInvoiceService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<VendorInvoiceRecord>>> GetAll(CancellationToken ct)
		{
			var data = await _service.GetAllAsync(ct);
			return Ok(data);
		}
	}
}
