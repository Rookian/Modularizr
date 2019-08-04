using System;
using Shouldly;
using System.Linq;
using Modularizr.Tests.ExampleApp.ModuleA;
using Modularizr.Tests.ExampleApp.ModuleA.Internal;
using Modularizr.Tests.ExampleApp.ModuleB;
using SimpleInjector;
using Xunit;

namespace Modularizr.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Should_detect_cyclic_dependency_between_modules()
        {

        }

        [Fact]
        public void Should_determine_correct_module_for_facade()
        {
            // Arrange
            var moduleFinder = new ModuleFinder();
            
            var container = BootContainer();


            // Act
            var finderResult = moduleFinder.FindModules(GetType().Assembly, container);

            // Assert
            finderResult.ShouldNotBeNull();
            finderResult.Modules.ShouldNotBeNull();
            finderResult.Modules.Length.ShouldBe(2);
            var moduleA = finderResult.Modules.First(x => x.Name.Equals("ModuleA"));

            moduleA.Name.ShouldBe("ModuleA");
            moduleA.Namespace.ShouldBe("Modularizr.Tests.ExampleApp.ModuleA");

            moduleA.PublicEntryServices.ShouldNotBeNull();
            moduleA.PublicEntryServices.Length.ShouldBe(2);
            moduleA.PublicEntryServices[0].Name.ShouldBe("IFacadeA");
            moduleA.PublicEntryServices[1].Name.ShouldBe("IFacadeAB");
        }

        private static Container BootContainer()
        {
            var container = new Container();
            container.Register<IFacadeA, FacadeA>();
            container.Register<IFacadeB, FacadeB>();
            container.Register<IFacadeAB, FacadeAb>();
            container.Register<IServiceA, ServiceA>();
            return container;
        }
    }
}
