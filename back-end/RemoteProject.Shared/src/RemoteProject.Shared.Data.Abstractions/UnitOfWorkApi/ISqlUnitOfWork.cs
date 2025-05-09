using System.Data.Common;

namespace RemoteProject.Shared.Data.Abstractions.UnitOfWorkApi;

public interface ISqlUnitOfWork
{
    DbConnection Connection { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken
    );

    IExecutionUnit<ISqlUnitOfWork> GetExecutionUnit();
}
