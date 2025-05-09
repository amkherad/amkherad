using RemoteProject.Shared.Data.Abstractions.Enums;

namespace RemoteProject.Shared.Data.Abstractions.Entities;

public interface ISoftDeletableEntity
{
    SoftDeleteStatusCodes Status { get; set; }
}
