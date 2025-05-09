using RemoteProject.Shared.Data.Abstractions.UnitOfWorkApi;
using RemoteProject.Shared.Data.EntityFramework.DbContextUtil;
using Microsoft.EntityFrameworkCore.Storage;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;

namespace RemoteProject.Shared.UnitTests.EntityFrameworkUnitTests;

[Collection("UnitTests")]
public class DbContextExecutionUnitTests
{
    private readonly IExecutionStrategy _strategy;
    private readonly IDbContextUnitOfWork _unitOfWork;
    private readonly DbContextExecutionUnit<IDbContextUnitOfWork> _executionUnit;

    public DbContextExecutionUnitTests()
    {
        _strategy = Substitute.For<IExecutionStrategy>();
        _unitOfWork = Substitute.For<IDbContextUnitOfWork>();
        _executionUnit = new DbContextExecutionUnit<IDbContextUnitOfWork>(_unitOfWork, _strategy);
    }

    [Fact]
    public void TestThrowOnNullInConstructor()
    {
        // Arrange & Act
        var exp1 = Record.Exception(() => new DbContextExecutionUnit<IDbContextUnitOfWork>(null, _strategy));
        var exp2 = Record.Exception(() => new DbContextExecutionUnit<IDbContextUnitOfWork>(_unitOfWork, null));

        // Assert
        exp1.Should().BeOfType<ArgumentNullException>();
        exp2.Should().BeOfType<ArgumentNullException>();
    }

    [Fact]
    public async Task TestExecuteAsyncWith2ParametersNoUnitOfWorkParam()
    {
        // Arrange & Act
        await _executionUnit.ExecuteAsync(
            ct => Task.FromResult<object>(0),
            CancellationToken.None
        );

        // Assert
        await _strategy
            .Received(1)
            .ExecuteAsync(
                Arg.Any<Func<CancellationToken, Task<object>>>(),
                Arg.Any<Func<DbContext, Func<CancellationToken, Task<object>>, CancellationToken, Task<object>>>(),
                Arg.Any<Func<DbContext, Func<CancellationToken, Task<object>>, CancellationToken,
                    Task<Microsoft.EntityFrameworkCore.Storage.ExecutionResult<object>>>?>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task TestExecuteAsyncWith2Parameters()
    {
        // Arrange & Act
        await _executionUnit.ExecuteAsync(
            (uow, ct) => Task.FromResult<object>(0),
            CancellationToken.None
        );

        // Assert
        await _strategy
            .Received(1)
            .ExecuteAsync(
                Arg.Any<Func<CancellationToken, Task<object>>>(),
                Arg.Any<Func<DbContext, Func<CancellationToken, Task<object>>, CancellationToken, Task<object>>>(),
                Arg.Any<Func<DbContext, Func<CancellationToken, Task<object>>, CancellationToken,
                    Task<Microsoft.EntityFrameworkCore.Storage.ExecutionResult<object>>>?>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task TestExecuteAsyncWith3Parameters()
    {
        // Arrange & Act
        await _executionUnit.ExecuteAsync(
            (uow, ct) => Task.FromResult<object>(0),
            (work, arg3) => Task.FromResult(new RemoteProject.Shared.Data.Abstractions.UnitOfWorkApi.ExecutionResult<object>(true, 10)),
            CancellationToken.None
        );

        // Assert
        await _strategy
            .Received(1)
            .ExecuteAsync(
                Arg.Any<object>(),
                Arg.Any<Func<DbContext, object, CancellationToken, Task<object>>>(),
                Arg.Any<Func<DbContext, object, CancellationToken,
                    Task<Microsoft.EntityFrameworkCore.Storage.ExecutionResult<object>>>?>(),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task TestExecuteAsyncWith4Parameters()
    {
        // Arrange & Act
        await _executionUnit.ExecuteAsync<object, object>(
            10,
            (uow, stt, ct) => Task.FromResult<object>(0),
            (work, i, arg3) => Task.FromResult(new RemoteProject.Shared.Data.Abstractions.UnitOfWorkApi.ExecutionResult<object>(true, 10)),
            CancellationToken.None
        );

        // Assert
        await _strategy
            .Received(1)
            .ExecuteAsync(
                Arg.Any<object>(),
                Arg.Any<Func<DbContext, object, CancellationToken, Task<object>>>(),
                Arg.Any<Func<DbContext, object, CancellationToken,
                    Task<Microsoft.EntityFrameworkCore.Storage.ExecutionResult<object>>>?>(),
                Arg.Any<CancellationToken>()
            );
    }
}
