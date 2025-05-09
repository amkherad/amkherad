namespace RemoteProject.Shared.Data.Abstractions.Entities;

public interface ICreateAuditableEntity
{
    DateTime CreatedAt { get; set; }
    Guid CreatedBy { get; set; }
}
