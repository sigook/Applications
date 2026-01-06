using Covenant.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Covenant.Infrastructure.Configurations
{
    public class ProvinceTaxConfiguration : IEntityTypeConfiguration<ProvinceTax>
    {
        public void Configure(EntityTypeBuilder<ProvinceTax> builder)
        {
            builder.ToTable("ProvinceTaxes");
            builder.HasKey(pt => pt.ProvinceId);
        }
    }
}
