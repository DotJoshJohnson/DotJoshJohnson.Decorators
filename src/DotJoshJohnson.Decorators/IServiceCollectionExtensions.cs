using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Applies the specified decorator implementation to the specified service type.
    /// This must be called AFTER the base or root service type implementation has been added to the container.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static IServiceCollection AddDecorator<TService, TDecorator>(this IServiceCollection services)
        where TDecorator : TService
    {
        return services.AddDecorator(typeof(TService), typeof(TDecorator));
    }

    /// <summary>
    /// Applies the specified decorator implementation to the specified service type.
    /// This must be called AFTER the base or root service type implementation has been added to the container.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public static IServiceCollection AddDecorator(this IServiceCollection services, Type serviceType, Type decoratorType)
    {
        if (!serviceType.IsAssignableFrom(decoratorType))
        {
            throw new InvalidOperationException($"A decorator of type {decoratorType.Name} cannot be applied to a service of type {serviceType.Name} because it is not assignable to the service type.");
        }

        var descriptor = services.FirstOrDefault(s => s.ServiceType == serviceType);

        if (descriptor is null)
        {
            throw new InvalidOperationException($"A decorator for {serviceType.Name} could not be added because a base implementation is not registered.");
        }

        var factory = ActivatorUtilities.CreateFactory(decoratorType, new[] { serviceType });

        services.Replace(ServiceDescriptor.Describe(serviceType, provider => factory(provider, new[] { _Activate(descriptor, provider) }), descriptor.Lifetime));

        return services;
    }

    private static object _Activate(ServiceDescriptor descriptor, IServiceProvider serviceProvider)
    {
        if (descriptor.ImplementationInstance is not null)
        {
            return descriptor.ImplementationInstance;
        }

        else if (descriptor.ImplementationFactory is not null)
        {
            return descriptor.ImplementationFactory(serviceProvider);
        }

        else if (descriptor.ImplementationType is not null)
        {
            return ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, descriptor.ImplementationType);
        }

        throw new InvalidOperationException($"An base implementation of {descriptor.ServiceType.Name} could not be created because the service descriptor does not have a valid implementation instance, factory, or type specified.");
    }
}