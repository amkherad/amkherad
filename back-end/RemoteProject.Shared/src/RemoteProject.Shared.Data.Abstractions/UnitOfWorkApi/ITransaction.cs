namespace RemoteProject.Shared.Data.Abstractions.UnitOfWorkApi;

public interface ITransaction : IAsyncDisposable
{
    Task CommitAsync(
        CancellationToken cancellationToken
    );

    Task RollbackAsync(
        CancellationToken cancellationToken
    );


    Task CreateSavepointAsync(
        string name,
        CancellationToken cancellationToken
    );

    Task ReleaseSavepointAsync(
        string name,
        CancellationToken cancellationToken
    );
}
