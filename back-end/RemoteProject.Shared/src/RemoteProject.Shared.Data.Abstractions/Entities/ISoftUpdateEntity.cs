namespace RemoteProject.Shared.Data.Abstractions.Entities;

public interface ISoftUpdateEntity : IEntity, ISoftDeletableEntity
{
    public Guid? BaseId { get; set; }
}
