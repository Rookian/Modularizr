using SimpleInjector;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Modularizr
{
    public class ModuleFinder
    {
        public ModuleFinderResult FindModules(Assembly assembly, Container container)
        {
            var registrations = container.GetCurrentRegistrations();

            var modules = assembly.GetTypes()
                .Where(x => x.IsPublic && x.IsInterface)
                .GroupBy(x => x.Namespace)
                .Select(x =>
                {
                    var publicServices = x.ToArray();

                    var serviceDependencies = registrations
                        .Where(r => r.ServiceType.Namespace?.Contains(x.Key) ?? false)
                        .Select(t => t.ServiceType)
                        .Except(publicServices)
                        .ToArray();

                    return new Module
                    {
                        Namespace = x.Key,
                        PublicEntryServices = publicServices,
                        Name = x.Key?.Split(".").Last(),
                        ServiceDependencies = serviceDependencies,
                    };
                })
                .OrderBy(x => x.Name)
                .ToImmutableArray();

            foreach (var module in modules)
            {

                var dependentModuleServices = registrations
                    .Where(r => r.ServiceType.IsPublic && ((!r.ServiceType.Namespace?.Contains(module.Namespace) ?? false)))
                    .Select(t => t.ServiceType)
                    .ToArray();

                foreach (var dependentModuleService in dependentModuleServices)
                {
                    var dependentModule = modules.SingleOrDefault(x => dependentModuleService.Namespace?.StartsWith(x.Namespace) ?? false);
                    module.Modules = module.Modules.Add(dependentModule);
                }
            }

            return new ModuleFinderResult { Modules = modules };
        }
    }

    public class ModuleFinderResult
    {
        public ImmutableArray<Module> Modules { get; set; }
    }

    public class Module
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public Type[] PublicEntryServices { get; set; }
        public Type[] ServiceDependencies { get; set; }
        public IImmutableSet<Module> Modules { get; set; } = ImmutableHashSet.Create<Module>();


        public override string ToString()
        {
            return $"Name:{Name} Namespace:{Namespace}";
        }
    }
}