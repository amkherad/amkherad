using RemoteProject.Shared.Data.Abstractions.Entities;
using RemoteProject.Shared.Data.Abstractions.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RemoteProject.Shared.Data.EntityFramework;

public static class EntityConfigurationExtensions
{
    public const string SqlServerProvider = "sql-server";
    public const string PostgresqlProvider = "postgres";

    public static string Provider { get; set; } = SqlServerProvider;

    public static void HasStatusFilter<TEntity>(
        this IndexBuilder<TEntity> indexBuilder,
        SoftDeleteStatusCodes value = SoftDeleteStatusCodes.Active
    )
    {
        switch (Provider)
        {
            case SqlServerProvider:
                indexBuilder.HasFilter($"'{nameof(ISoftDeletableEntity.Status)}' = {(int)value}");
                break;
            case PostgresqlProvider:
                indexBuilder.HasFilter($"\"{nameof(ISoftDeletableEntity.Status)}\" = {(int)value}");
                break;
            default:
                throw new InvalidOperationException("UnknownProvider");
        }
    }
}
