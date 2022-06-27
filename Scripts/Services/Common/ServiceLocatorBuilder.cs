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
            IServiceLocatorBuilder WithService<TInterface, TImplementation>(IServiceParameters parameters = null)
                where TImplementation : class, TInterface
                where TInterface : class, IService;
        }

        private class ServiceLocatorBuilder : IServiceLocatorBuilder
        {
            private const BindingFlags startBuildflags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            private Stack<Type> initiatedServices = new Stack<Type>();
            private Dictionary<Type, Type> types = new Dictionary<Type, Type>();
            private Dictionary<Type, IServiceParameters> parameters = new Dictionary<Type, IServiceParameters>();
            private Dictionary<Type, IService> services = new Dictionary<Type, IService>();

            public IServiceLocatorBuilder WithService<TInterface, TImplementation>(IServiceParameters serviceParameters = null)
                where TInterface : class, IService
                where TImplementation : class, TInterface
            {

                var serviceInterface = typeof(TInterface);

                if (types.ContainsKey(serviceInterface))
                    throw new InvalidOperationException($"Service {serviceInterface.Name} is already registered.");

                types.Add(serviceInterface, typeof(TImplementation));
                if (serviceParameters != null) parameters.Add(typeof(TImplementation), serviceParameters);
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
                var hasParams = parameters.TryGetValue(typeof(T), out var providedParameters);

                var cParams = new List<object>();
                foreach (var paramInfo in constructor.GetParameters())
                {
                    object parameter;
                    var paramType = paramInfo.ParameterType;
                    if (IsService(paramType))
                    {
                        if (!services.TryGetValue(paramType, out var dependency))
                            throw new InvalidOperationException($"Service {paramType.Name} is not registered.");
                        parameter = dependency;
                    }
                    else
                    {
                        if (!hasParams) throw new Exception($"Parameter {paramType.Name} is not provided. Provide parameters instance with ServiceParameter attribute attached to properties");

                        var attr = paramInfo.GetCustomAttribute<FromParametersAttribute>();
                        if (attr is null) throw new Exception($"Parameter {paramType.Name} is not given a FromParameters attribute in constructor");

                        var property = providedParameters
                            .GetType()
                            .GetTypeInfo()
                            .GetProperties()
                            .FirstOrDefault(i => CheckPropertyTag(i, attr.Tag, paramInfo.Name));

                        parameter = property.GetValue(providedParameters);
                    }
                    cParams.Add(parameter);
                }

                return constructor.Invoke(cParams.ToArray()) as T;
            }

            private bool CheckPropertyTag(PropertyInfo p, string tag, string name)
            {
                var attr = p.GetCustomAttribute<ServiceParameterAttribute>();

                if (attr is null) throw new Exception($"Property {name} in provided parameters doesn't have a ServiceParameter Attribute attached");

                return String.CompareOrdinal(attr.Tag.ToLower(), tag.ToLower()) == 0;
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
