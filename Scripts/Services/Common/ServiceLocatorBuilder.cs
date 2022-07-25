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
            IServiceLocatorBuilder WithOption<T>(T option);
        }

        private class ServiceLocatorBuilder : IServiceLocatorBuilder
        {
            private const BindingFlags startBuildflags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            private Stack<Type> initiatedServices = new Stack<Type>();
            private Dictionary<Type, Type> types = new Dictionary<Type, Type>();
            private Dictionary<Type, IService> services = new Dictionary<Type, IService>();
            private Dictionary<Type, object> options = new Dictionary<Type, object>();

            public IServiceLocatorBuilder WithService<TInterface, TImplementation>()
                where TInterface : class, IService
                where TImplementation : class, TInterface =>
                AddTo(types, typeof(TInterface), typeof(TImplementation));

            public IServiceLocatorBuilder WithOption<T>(T option) =>
                AddTo(options, typeof(T), option);

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
                services[initiatedServices.Pop()] = service;
            }

            private T BuildService<T>() where T : class, IService
            {
                var constructor = GetConstructor<T>();

                var cParams = constructor
                    .GetParameters()
                    .Select(paramInfo => GetParameterByKey(paramInfo.ParameterType))
                    .ToArray();

                return constructor.Invoke(cParams.ToArray()) as T;
            }

            private bool IsService(Type type) => 
                type.GetInterface("IService") != null;

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

            private IServiceLocatorBuilder AddTo<T>(IDictionary<Type, T> dict, Type type, T value)
            {
                var serviceOrOption = type is IService ? "Service" : "Option";
                if (dict.ContainsKey(type))
                    throw new InvalidOperationException($"{serviceOrOption} {type.Name} is already registered.");

                dict.Add(type, value);
                return this;
            }

            private object GetParameterByKey(Type key){
                if (IsService(key))
                    return GetParameterByKeyFrom(services, key);
                else
                    return GetParameterByKeyFrom(options, key);
            }

            private object GetParameterByKeyFrom<T>(IDictionary<Type, T> dict, Type key)
            {
                var serviceOrOption = key is IService ? "Service" : "Option";
                if (!dict.TryGetValue(key, out var dependency))
                    throw new InvalidOperationException($"{serviceOrOption} {key.Name} is not registered.");
                return dependency;
            }
        }
    }
}
