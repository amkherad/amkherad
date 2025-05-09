using RemoteProject.Shared.Data.Abstractions.UnitOfWorkApi;
using Microsoft.EntityFrameworkCore.Storage;

namespace RemoteProject.Shared.Data.EntityFramework.DbContextUtil;

public class DbContextTransactionWrapper : ITransaction
{
    public IDbContextTransaction Transaction { get; private set; }

    public DbContextTransactionWrapper(
        IDbContextTransaction transaction
    )
    {
        ArgumentNullException.ThrowIfNull(transaction, nameof(transaction));

        Transaction = transaction;
    }

    public async ValueTask DisposeAsync()
    {
        await Transaction.DisposeAsync();
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await Transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        await Transaction.RollbackAsync(cancellationToken);
    }

    public async Task CreateSavepointAsync(string name, CancellationToken cancellationToken)
    {
        await Transaction.CreateSavepointAsync(name, cancellationToken);
    }

    public async Task ReleaseSavepointAsync(string name, CancellationToken cancellationToken)
    {
        await Transaction.ReleaseSavepointAsync(name, cancellationToken);
    }
}
