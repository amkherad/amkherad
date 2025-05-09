using System.Data;
using RemoteProject.Shared.Data.Abstractions.UnitOfWorkApi;
using Microsoft.EntityFrameworkCore;

namespace RemoteProject.Shared.Data.EntityFramework.DbContextUtil;

public abstract class DbContextBase<TContext> : DbContext, IDbContextUnitOfWork<TContext>, IDbContextUnitOfWork
    where TContext : DbContextBase<TContext>
{
    private static readonly Lazy<Action<ModelBuilder>> EntityConfigurationApplierInstance = new(
        () => EntityConfigurationsApplier.CreateAutoApplier(typeof(TContext))
    );

    protected DbContextBase(DbContextOptions<TContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        EntityConfigurationApplierInstance.Value.Invoke(builder);
    }

    public IQueryable<T> Query<T>() where T : class => Set<T>();


    public IExecutionUnit<TContext> GetExecutionUnit()
    {
        var strategy = Database.CreateExecutionStrategy();
        return new DbContextExecutionUnit<TContext>((TContext)this, strategy);
    }

    IExecutionUnit<IDbContextUnitOfWork> IDbContextUnitOfWork.GetExecutionUnit()
    {
        var strategy = Database.CreateExecutionStrategy();
        return new DbContextExecutionUnit<IDbContextUnitOfWork>(this, strategy);
    }

    public Task<ITransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken
    )
    {
        throw new NotSupportedException("The overload with isolationLevel was removed??");
        // var tr = await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        // return new DbContextTransactionWrapperTransactionWrapper(tr);
    }

    public async Task<ITransaction> BeginTransactionAsync(
        CancellationToken cancellationToken
    )
    {
        var tr = await Database.BeginTransactionAsync(cancellationToken);
        return new DbContextTransactionWrapper(tr);
    }
}
