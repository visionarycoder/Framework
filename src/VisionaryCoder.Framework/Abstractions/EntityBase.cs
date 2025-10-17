namespace VisionaryCoder.Framework.Abstractions;

/// <summary>
/// Provides a base class for entities following Microsoft Entity Framework patterns.
/// Implements common entity functionality including optimistic concurrency control.
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// Gets or sets the row version for optimistic concurrency control.
    /// This property is automatically managed by Entity Framework.
    /// </summary>
    public byte[] RowVersion { get; set; } = [];

    /// <summary>
    /// Gets or sets the timestamp when the entity was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets or sets the timestamp when the entity was last modified.
    /// </summary>
    public DateTimeOffset? ModifiedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who last modified the entity.
    /// </summary>
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is deleted (soft delete pattern).
    /// </summary>
    public bool IsDeleted { get; set; }
}
