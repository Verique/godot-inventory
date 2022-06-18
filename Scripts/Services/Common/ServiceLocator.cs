using System;
using System.Collections.Generic;

namespace Grate.Services
{
    public interface IService { }

    public interface IServiceLocator
    {
        TInterface Get<TInterface>() where TInterface : class, IService;
    }

    public static partial class Services
    {
        public static IServiceLocator CreateLocator(Action<IServiceLocatorBuilder> registerAction)
        {
            var builder = new ServiceLocatorBuilder();
            registerAction(builder);
            return builder.Build();
        }

        private class ServiceLocator : IServiceLocator
        {
            private Dictionary<Type, IService> _services;

            public ServiceLocator(Dictionary<Type, IService> services)
            {
                _services = services;
            }

            public TInterface Get<TInterface>() where TInterface : class, IService
            {
                var type = typeof(TInterface);
                var success = _services.TryGetValue(type, out var service);
                if (!success) throw new InvalidOperationException($"Service {type.Name} isn't registered");
                return service as TInterface;
            }
        }
    }
}
