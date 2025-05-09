namespace RemoteProject.Shared.Data.Abstractions.Entities;

public interface IUpdateAuditableEntity
{
    DateTime? UpdatedAt { get; set; }
    Guid? UpdatedBy { get; set; }
}
