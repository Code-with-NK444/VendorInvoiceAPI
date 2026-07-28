using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VendorInvoiceApi.Models;
using VendorInvoiceApi.Services;
using VendorInvoiceLogic.Abstraction;

namespace VendorInvoiceApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VendorInvoiceController : ControllerBase
	{
		private readonly IVendorInvoiceService _repository;

		public VendorInvoiceController(IVendorInvoiceService repository)
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
