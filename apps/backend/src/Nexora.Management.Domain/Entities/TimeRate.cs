using Nexora.Management.Domain.Common;

namespace Nexora.Management.Domain.Entities;

public class TimeRate : BaseEntity
{
    public Guid? UserId { get; set; }
    public Guid? ProjectId { get; set; }
    public decimal HourlyRate { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public Project? Project { get; set; }
}
