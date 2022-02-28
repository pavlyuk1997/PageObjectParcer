using Atata;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParcerTry
{
    [SetUpFixture]
    public class SetUpFixture
    {
        [OneTimeSetUp]
        public void GlobalSetUp()
        {
            AtataContext.GlobalConfiguration
                .UseChrome()
                    .WithArguments("start-maximized")
                .UseBaseUrl("https://atata.io")
                .UseCulture("en-US")
                .UseAllNUnitFeatures();

            AtataContext.GlobalConfiguration.AutoSetUpDriverToUse();
        }
    }
}
