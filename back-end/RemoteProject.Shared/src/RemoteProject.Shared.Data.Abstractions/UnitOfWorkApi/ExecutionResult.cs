namespace RemoteProject.Shared.Data.Abstractions.UnitOfWorkApi;

public class ExecutionResult<TResult>
{
    /// <summary>
    /// Indicates whether the operation succeeded.
    /// </summary>
    public virtual bool IsSuccessful { get; }

    /// <summary>
    /// The result of the operation if successful.
    /// </summary>
    public virtual TResult Result { get; }

    /// <summary>
    /// Creates a new instance of Microsoft.EntityFrameworkCore.Storage.ExecutionResult`1.
    /// </summary>
    /// <param name="successful">true if the operation succeeded.</param>
    /// <param name="result">The result of the operation if successful.</param>
    public ExecutionResult(bool successful, TResult result)
    {
        IsSuccessful = successful;
        Result = result;
    }
}
