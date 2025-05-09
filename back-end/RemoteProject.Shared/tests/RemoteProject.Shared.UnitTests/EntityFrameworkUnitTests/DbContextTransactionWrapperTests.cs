using RemoteProject.Shared.Data.EntityFramework.DbContextUtil;
using Microsoft.EntityFrameworkCore.Storage;

namespace RemoteProject.Shared.UnitTests.EntityFrameworkUnitTests;

[Collection("UnitTests")]
public class DbContextTransactionWrapperTests
{
    private readonly IDbContextTransaction _dbContextTransaction;
    private readonly DbContextTransactionWrapper _dbContextTransactionWrapper;

    public DbContextTransactionWrapperTests()
    {
        _dbContextTransaction = Substitute.For<IDbContextTransaction>();
        _dbContextTransactionWrapper = new DbContextTransactionWrapper(_dbContextTransaction);
    }

    [Fact]
    public void TestThrowOnNullInConstructor()
    {
        // Arrange & Act
        var exp = Record.Exception(() => new DbContextTransactionWrapper(null));

        // Assert
        exp.Should().BeOfType<ArgumentNullException>();
    }

    [Fact]
    public async Task TestDisposeAsync()
    {
        // Arrange & Act
        await _dbContextTransactionWrapper.DisposeAsync();

        // Assert
        await _dbContextTransaction
            .Received(1)
            .DisposeAsync();
    }

    [Fact]
    public async Task TestCommitAsync()
    {
        // Arrange & Act
        await _dbContextTransactionWrapper.CommitAsync(CancellationToken.None);

        // Assert
        await _dbContextTransaction
            .Received(1)
            .CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task TestRollbackAsync()
    {
        // Arrange & Act
        await _dbContextTransactionWrapper.RollbackAsync(CancellationToken.None);

        // Assert
        await _dbContextTransaction
            .Received(1)
            .RollbackAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task TestCreateSavepointAsync()
    {
        // Arrange & Act
        await _dbContextTransactionWrapper.CreateSavepointAsync("test", CancellationToken.None);

        // Assert
        await _dbContextTransaction
            .Received(1)
            .CreateSavepointAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReleaseSavepointAsync()
    {
        // Arrange & Act
        await _dbContextTransactionWrapper.ReleaseSavepointAsync("test", CancellationToken.None);

        // Assert
        await _dbContextTransaction
            .Received(1)
            .ReleaseSavepointAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
