using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RS.Core.Security.Model;

namespace RS.Core.Security.EntityFrameworkCore;

public class KeyMaterialMap : IEntityTypeConfiguration<KeyMaterial>
{
	public void Configure(EntityTypeBuilder<KeyMaterial> builder)
	{
		builder.HasKey(c => c.Id);

		builder.Property(c => c.Parameters)
			.HasMaxLength(8000)
			.IsRequired();
	}
}
