using System;
using System.Collections.Generic;

namespace CaravanSecrets.Core.Services
{
    public sealed class ServiceRegistry
    {
        private readonly Dictionary<Type, object> _services = new();
        public void Register<T>(T service) where T : class => _services[typeof(T)] = service ?? throw new ArgumentNullException(nameof(service));
        public T Resolve<T>() where T : class => _services.TryGetValue(typeof(T), out var value)
            ? (T)value
            : throw new InvalidOperationException($"Service {typeof(T).Name} is not registered.");
    }
}
