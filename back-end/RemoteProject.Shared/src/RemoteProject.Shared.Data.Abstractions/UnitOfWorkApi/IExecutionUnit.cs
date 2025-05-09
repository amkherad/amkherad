namespace RemoteProject.Shared.Data.Abstractions.UnitOfWorkApi;

public interface IExecutionUnit<TContext>
{
    TContext Context { get; }

    Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken
    );

    Task<TResult> ExecuteAsync<TResult>(
        Func<TContext, CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken
    );

    Task<TResult> ExecuteAsync<TResult>(
        Func<TContext, CancellationToken, Task<TResult>> operation,
        Func<TContext, CancellationToken, Task<ExecutionResult<TResult>>>? verifySucceeded,
        CancellationToken cancellationToken
    );

    Task<TResult> ExecuteAsync<TState, TResult>(
        TState state,
        Func<TContext, TState, CancellationToken, Task<TResult>> operation,
        Func<TContext, TState, CancellationToken, Task<ExecutionResult<TResult>>>? verifySucceeded,
        CancellationToken cancellationToken
    );


    Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken
    );

    Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<TContext, CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken
    );

    Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<TContext, CancellationToken, Task<TResult>> operation,
        Func<TContext, CancellationToken, Task<ExecutionResult<TResult>>>? verifySucceeded,
        CancellationToken cancellationToken
    );

    Task<TResult> ExecuteInTransactionAsync<TState, TResult>(
        TState state,
        Func<TContext, TState, CancellationToken, Task<TResult>> operation,
        Func<TContext, TState, CancellationToken, Task<ExecutionResult<TResult>>>? verifySucceeded,
        CancellationToken cancellationToken
    );
}
