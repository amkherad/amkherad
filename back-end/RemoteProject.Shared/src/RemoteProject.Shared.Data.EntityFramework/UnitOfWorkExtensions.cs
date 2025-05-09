using RemoteProject.Shared.Data.Abstractions.UnitOfWorkApi;
using Microsoft.EntityFrameworkCore;

namespace RemoteProject.Shared.Data.EntityFramework;

public static class UnitOfWorkExtensions
{
    public static DbSet<T> Set<T>(this IDbContextUnitOfWork unitOfWork)
        where T : class
    {
        return unitOfWork.Query<T>() as DbSet<T> ?? throw new InvalidOperationException("Set was not found.");
    }
}
