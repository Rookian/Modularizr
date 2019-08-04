using Modularizr.Tests.ExampleApp.ModuleB;

namespace Modularizr.Tests.ExampleApp.ModuleA
{
    public interface IFacadeA
    {
        
    }

    public class FacadeA : IFacadeA
    {
        private readonly IFacadeB _facadeB;

        public FacadeA(IFacadeB facadeB)
        {
            _facadeB = facadeB;
        }
    }
}