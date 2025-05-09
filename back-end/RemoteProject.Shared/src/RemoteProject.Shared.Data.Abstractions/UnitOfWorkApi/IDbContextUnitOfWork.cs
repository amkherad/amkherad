namespace RemoteProject.Shared.Data.Abstractions.UnitOfWorkApi;

public interface IDbContextUnitOfWork
{
    IQueryable<T> Query<T>()
        where T : class;

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken
    );


    IExecutionUnit<IDbContextUnitOfWork> GetExecutionUnit();

    Task<ITransaction> BeginTransactionAsync(
        System.Data.IsolationLevel isolationLevel,
        CancellationToken cancellationToken
    );

    Task<ITransaction> BeginTransactionAsync(
        CancellationToken cancellationToken
    );
}

public interface IDbContextUnitOfWork<TActualContext> : IDbContextUnitOfWork
    where TActualContext : class, IDbContextUnitOfWork
{
    new IExecutionUnit<TActualContext> GetExecutionUnit();
}
