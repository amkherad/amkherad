using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace RemoteProject.Shared.Data.EntityFramework.DbContextUtil;

public static class EntityConfigurationsApplier
{
    public static Action<ModelBuilder> CreateAutoApplier(
        Type targetContextType
    )
    {
        var configurations = targetContextType.Assembly
            .GetTypes()
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType &&
                          i.GetGenericTypeDefinition() ==
                          typeof(IEntityTypeConfiguration<>)))
            .ToArray();

        var applyConfigMethod = typeof(ModelBuilder)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(m =>
                m.Name == nameof(ModelBuilder.ApplyConfiguration));

        if (applyConfigMethod is null)
        {
            throw new InvalidOperationException();
        }

        var modelBuilderParameter = Expression.Parameter(typeof(ModelBuilder), "modelBuilder");

        var commands = new List<Expression>();

        foreach (var config in configurations)
        {
            var entityType = config.GetInterfaces()
                .Single(
                    i => i.IsGenericType &&
                         i.GetGenericTypeDefinition() ==
                         typeof(IEntityTypeConfiguration<>))
                .GetGenericArguments()
                .Single();

            var target = applyConfigMethod.MakeGenericMethod(entityType);

            var instance = Expression.New(config);

            var call = Expression.Call(modelBuilderParameter, target, instance);

            commands.Add(call);
        }

        var body = Expression.Block(commands);

        var lambda = Expression.Lambda<Action<ModelBuilder>>(body, modelBuilderParameter);

        return lambda.Compile();
    }
}
