namespace Item.Api.Data;

public abstract class BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } // soft delete
}
