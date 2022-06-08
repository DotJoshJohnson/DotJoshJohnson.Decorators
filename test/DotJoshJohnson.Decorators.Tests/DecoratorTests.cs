using Microsoft.Extensions.DependencyInjection;

namespace DotJoshJohnson.Decorators.Tests
{
    public class DecoratorTests
    {
        private readonly IServiceCollection _services = new ServiceCollection();

        [Fact]
        public void AddDecorator_ThrowsIfBaseImplementationIsMissing()
        {
            Assert.Throws<InvalidOperationException>(() => _services.AddDecorator<IMyServiceType, MyUppercaseDecorator>());
        }

        [Fact]
        public void AddDecorator_AppliesSpecifiedDecorator()
        {
            _services
                .AddTransient<IMyServiceType, MyBaseImplementation>()
                .AddDecorator<IMyServiceType, MyUppercaseDecorator>();

            var provider = _services.BuildServiceProvider();

            var myService = provider.GetRequiredService<IMyServiceType>();

            Assert.Equal("TEST", myService.Echo("test"));
        }

        [Fact]
        public void AddDecorator_AppliesMultipleDecoratorsInOrder()
        {
            _services
                .AddTransient<IMyServiceType, MyBaseImplementation>()
                .AddDecorator<IMyServiceType, MyExclamationDecorator>()
                .AddDecorator<IMyServiceType, MyUppercaseDecorator>();

            var provider = _services.BuildServiceProvider();

            var myService = provider.GetRequiredService<IMyServiceType>();

            Assert.Equal("TEST!!!", myService.Echo("test"));
        }
    }

    public interface IMyServiceType
    {
        string Echo(string input);
    }

    public class MyBaseImplementation : IMyServiceType
    {
        public string Echo(string input)
        {
            return input;
        }
    }

    public class MyUppercaseDecorator : IMyServiceType
    {
        private readonly IMyServiceType _baseImplementation;

        public MyUppercaseDecorator(IMyServiceType baseImplementation)
        {
            _baseImplementation = baseImplementation;
        }

        public string Echo(string input)
        {
            return _baseImplementation.Echo(input.ToUpper());
        }
    }

    public class MyExclamationDecorator : IMyServiceType
    {
        private readonly IMyServiceType _baseImplementation;

        public MyExclamationDecorator(IMyServiceType baseImplementation)
        {
            _baseImplementation = baseImplementation;
        }

        public string Echo(string input)
        {
            return _baseImplementation.Echo(string.Concat(input, "!!!"));
        }
    }
}