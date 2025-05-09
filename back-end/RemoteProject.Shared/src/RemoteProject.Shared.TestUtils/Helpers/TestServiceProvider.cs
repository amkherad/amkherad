// namespace RemoteProject.Shared.TestUtils.Helpers;
//
// public class TestServiceProvider : IServiceProvider
// {
//     private readonly Dictionary<Type, object> _instances;
//
//     public TestServiceProvider()
//     {
//         
//     }
//
//     private IServiceProvider GetServiceProvider(Dictionary<Type, object> instances = null)
//     {
//         instances ??= new Dictionary<Type, object>();
//         var serviceProvider = Substitute.For<IServiceProvider>();
//     
//         var serviceScope = Substitute.For<IServiceScope>();
//         serviceScope.ServiceProvider.Returns(serviceProvider);
//     
//         var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
//         serviceScopeFactory.CreateScope().Returns(serviceScope);
//     
//         var allInstances = instances.ToDictionary(k => k.Key, v => v.Value);
//         allInstances.Add(typeof(IServiceScopeFactory), serviceScopeFactory);
//     
//         serviceProvider.GetService(Arg.Any<Type>())
//             .Returns(args => allInstances[(Type)args[0]]);
//     
//         return serviceProvider;
//     }
//
//     public object? GetService(Type serviceType)
//     {
//         _instances.TryGetValue(serviceType, out var result);
//
//         return result;
//     }
// }