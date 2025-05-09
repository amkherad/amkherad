namespace RemoteProject.Shared.Data.Abstractions.Entities;

public interface IEntity : ICreateAuditableEntity, IUpdateAuditableEntity//, ISoftDeletableEntity
{
    Guid Id { get; set; }
}
