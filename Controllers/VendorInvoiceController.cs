using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VendorInvoiceApi.Models;
using VendorInvoiceApi.Services;

namespace VendorInvoiceApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VendorInvoiceController : ControllerBase
	{
		private readonly VendorInvoiceRepository _repository;

		public VendorInvoiceController(VendorInvoiceRepository repository)
		{
			_repository = repository;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<VendorInvoiceRecord>>> GetAll(CancellationToken ct)
		{
			var data = await _repository.GetAllAsync(ct);
			return Ok(data);
		}
	}
}
