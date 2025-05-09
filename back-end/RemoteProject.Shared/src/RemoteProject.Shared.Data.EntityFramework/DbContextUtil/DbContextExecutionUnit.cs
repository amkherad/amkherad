using RemoteProject.Shared.Data.Abstractions.UnitOfWorkApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace RemoteProject.Shared.Data.EntityFramework.DbContextUtil;

public class DbContextExecutionUnit<TContext> : IExecutionUnit<TContext>
{
    private readonly TContext _context;
    private readonly IExecutionStrategy _strategy;

    public TContext Context => _context;

    public DbContextExecutionUnit(
        TContext context,
        IExecutionStrategy strategy
    )
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        ArgumentNullException.ThrowIfNull(strategy, nameof(strategy));

        _context = context;
        _strategy = strategy;
    }

    public async Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken)
    {
        return await _strategy.ExecuteAsync(
            async ct => await operation(ct),
            cancellationToken
        );
    }

    public async Task<TResult> ExecuteAsync<TResult>(
        Func<TContext, CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken
    )
    {
        return await _strategy.ExecuteAsync(
            async ct => await operation(_context, ct),
            cancellationToken
        );
    }

    public async Task<TResult> ExecuteAsync<TResult>(
        Func<TContext, CancellationToken, Task<TResult>> operation,
        Func<TContext, CancellationToken,
                Task<Abstractions.UnitOfWorkApi.ExecutionResult<TResult>>>?
            verifySucceeded,
        CancellationToken cancellationToken
    )
    {
        Func<object?, CancellationToken, Task<Microsoft.EntityFrameworkCore.Storage.ExecutionResult<TResult>>>?
            verifySucceededClb = verifySucceeded is null
                ? null
                : async (stt, ct) =>
                {
                    var r = await verifySucceeded(_context, ct);

                    return new Microsoft.EntityFrameworkCore.Storage.ExecutionResult<TResult>(r.IsSuccessful, r.Result);
                };

        return await _strategy.ExecuteAsync(
            null,
            async (stt, ct) => await operation(_context, ct),
            verifySucceededClb,
            cancellationToken
        );
    }

    public async Task<TResult> ExecuteAsync<TState, TResult>(
        TState state,
        Func<TContext, TState, CancellationToken, Task<TResult>> operation,
        Func<TContext, TState, CancellationToken,
            Task<Abstractions.UnitOfWorkApi.ExecutionResult<TResult>>>? verifySucceeded,
        CancellationToken cancellationToken
    )
    {
        Func<DbContext?, TState, CancellationToken,
                Task<Microsoft.EntityFrameworkCore.Storage.ExecutionResult<TResult>>>?
            verifySucceededClb = verifySucceeded is null
                ? null
                : async (ctx, stt, ct) =>
                {
                    var r = await verifySucceeded(_context, stt, ct);

                    return new Microsoft.EntityFrameworkCore.Storage.ExecutionResult<TResult>(r.IsSuccessful, r.Result);
                };

        return await _strategy.ExecuteAsync(
            state,
            async (ctx, stt, ct) => await operation(_context, stt, ct),
            verifySucceededClb,
            cancellationToken
        );
    }

    public async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken)
    {
        return await _strategy.ExecuteInTransactionAsync(
            async ct => await operation(ct),
            null,
            cancellationToken
        );
    }

    public async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<TContext, CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken
    )
    {
        return await _strategy.ExecuteInTransactionAsync(
            async ct => await operation(_context, ct),
            null,
            cancellationToken
        );
    }

    public async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<TContext, CancellationToken, Task<TResult>> operation,
        Func<TContext, CancellationToken, Task<Abstractions.UnitOfWorkApi.ExecutionResult<TResult>>>?
            verifySucceeded,
        CancellationToken cancellationToken
    )
    {
        Func<object?, CancellationToken, Task<bool>>?
            verifySucceededClb = verifySucceeded is null
                ? null
                : async (stt, ct) =>
                {
                    var result = await verifySucceeded(_context, ct);

                    return result.IsSuccessful;
                };

        return await _strategy.ExecuteInTransactionAsync(
            null,
            async (ctx, ct) => await operation(_context, ct),
            verifySucceededClb,
            cancellationToken
        );
    }

    public async Task<TResult> ExecuteInTransactionAsync<TState, TResult>(
        TState state,
        Func<TContext, TState, CancellationToken, Task<TResult>> operation,
        Func<TContext, TState, CancellationToken,
            Task<Abstractions.UnitOfWorkApi.ExecutionResult<TResult>>>? verifySucceeded,
        CancellationToken cancellationToken
    )
    {
        Func<TState, CancellationToken, Task<bool>>?
            verifySucceededClb = verifySucceeded is null
                ? null
                : async (stt, ct) =>
                {
                    var result = await verifySucceeded(_context, stt, ct);

                    return result.IsSuccessful;
                };

        return await _strategy.ExecuteInTransactionAsync(
            state,
            async (stt, ct) => await operation(_context, stt, ct),
            verifySucceededClb,
            cancellationToken
        );
    }
}
