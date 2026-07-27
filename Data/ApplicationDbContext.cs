using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using VendorInvoiceApi.Models;

namespace VendorInvoiceApi.Data
{
	public class ApplicationDbContext : DbContext, IApplicationDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<VendorInvoiceRecord> VendorInvoices => Set<VendorInvoiceRecord>();

		// Wraps CHANGE_TRACKING_CURRENT_VERSION() so nothing outside this
		// class needs to know it's a raw SQL call.
		public async Task<long> GetChangeTrackingVersionAsync(CancellationToken cancellationToken)
		{
			var version = await Database.SqlQueryRaw<long>("SELECT CHANGE_TRACKING_CURRENT_VERSION() AS Value")
					.FirstOrDefaultAsync(cancellationToken);

			return version;
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<VendorInvoiceRecord>(entity =>
			{
				entity.ToTable("Vendor_Invoice_Data", "dbo");
				entity.HasKey(e => e.Id);

				entity.Property(e => e.Id).HasColumnName("Id");
				entity.Property(e => e.VendorId).HasColumnName("VendorId");
				entity.Property(e => e.Material).HasColumnName("material").HasMaxLength(200);
				entity.Property(e => e.Weight).HasColumnName("Weight").HasColumnType("decimal(10,2)");
				entity.Property(e => e.ReceivedDate).HasColumnName("recieved_date").HasColumnType("date");
				entity.Property(e => e.ExpiryDate).HasColumnName("Expiry_Date").HasColumnType("date");
			});
		}
	}
}
