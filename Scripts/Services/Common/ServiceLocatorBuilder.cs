using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Grate.Services
{
    public static partial class Services
    {
        public interface IServiceLocatorBuilder
        {
            IServiceLocatorBuilder WithService<TInterface, TImplementation>()
                where TImplementation : class, TInterface
                where TInterface : class, IService;
            IServiceLocatorBuilder WithParameter<T>(T parameter);
        }

        private class ServiceLocatorBuilder : IServiceLocatorBuilder
        {
            private const BindingFlags startBuildflags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            private Stack<Type> initiatedServices = new Stack<Type>();
            private Dictionary<Type, Type> types = new Dictionary<Type, Type>();
            private Dictionary<Type, object> options = new Dictionary<Type, object>();
            private Dictionary<Type, IService> services = new Dictionary<Type, IService>();

            public IServiceLocatorBuilder WithService<TInterface, TImplementation>()
                where TInterface : class, IService
                where TImplementation : class, TInterface
            {

                var serviceInterface = typeof(TInterface);

                if (types.ContainsKey(serviceInterface))
                    throw new InvalidOperationException($"Service {serviceInterface.Name} is already registered.");

                types.Add(serviceInterface, typeof(TImplementation));
                return this;
            }

            public IServiceLocatorBuilder WithParameter<T>(T parameter)
            {
                var type = typeof(T);
                if (options.ContainsKey(type))
                    throw new InvalidOperationException($"Option {type.Name} is already registered. Consider wrapping same parameter types.");

                options.Add(type, parameter);
                return this;
            }

            public ServiceLocator Build()
            {
                foreach ((var iType, var sType) in types.Select(s => (s.Key, s.Value)))
                {
                    GetType()
                        .GetMethod(nameof(StartBuildService), startBuildflags)
                        .MakeGenericMethod(iType, sType)
                        .Invoke(this, null);
                }
                return new ServiceLocator(services);
            }

            private void StartBuildService<TI, TS>()
                where TI : class, IService
                where TS : class, TI
            {
                var type = typeof(TI);
                if (services.ContainsKey(type)) return;

                if (initiatedServices.Contains(type))
                    throw new InvalidOperationException("Cyclic dependency");

                initiatedServices.Push(type);
                Godot.GD.Print($"put {type} on stack");

                foreach (var dependency in GetDependencies<TS>())
                {
                    if (!IsService(dependency)) continue;

                    if (!types.TryGetValue(dependency, out Type serviceType))
                        throw new InvalidOperationException($"{dependency.Name} is not registered");
                    GetType()
                        .GetMethod(nameof(StartBuildService), startBuildflags)
                        .MakeGenericMethod(dependency, serviceType)
                        .Invoke(this, null);
                }

                var service = BuildService<TS>();
                Godot.GD.Print($"pop {type} from stack");
                services[initiatedServices.Pop()] = service;
            }

            private T BuildService<T>() where T : class, IService
            {
                var constructor = GetConstructor<T>();

                var cParams = new List<object>();
                foreach (var paramType in constructor.GetParameters().Select(pInfo => pInfo.ParameterType))
                    cParams.Add(GetParameter(paramType));

                return constructor.Invoke(cParams.ToArray()) as T;
            }

            private object GetParameter(Type paramType)
            {
                if (IsService(paramType))
                {
                    if (!services.TryGetValue(paramType, out var dependency))
                        throw new InvalidOperationException($"Service {paramType.Name} is not registered.");
                    return dependency;
                }
                else
                {
                    if (!options.TryGetValue(paramType, out var paramDependency))
                        throw new InvalidOperationException($"Parameter {paramType.Name} is not registered.");
                    return paramDependency;
                }
            }

            private bool IsService(Type type)
            {
                return type.GetInterface("IService") != null;
            }

            private List<Type> GetDependencies<T>() where T : class
            {
                return GetConstructor<T>()
                    .GetParameters()
                    .Select(p => p.ParameterType)
                    .ToList();
            }

            private ConstructorInfo GetConstructor<T>() where T : class
            {
                var constructors = typeof(T).GetConstructors();
                if (constructors.Length > 1)
                    throw new InvalidOperationException($"Service {typeof(T).Name} has more than one constructors");
                return constructors[0];
            }
        }
    }
}
