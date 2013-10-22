using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Autofac.Configuration;

namespace KORT.Database
{
    class Run
    {
        private static IContainer Container { get; set; }

        static void SelectData()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<DatabaseManager>();
            builder.RegisterModule(new ConfigurationSettingsReader("autofac"));
   
            Container = builder.Build();

            using (var scope = Container.BeginLifetimeScope())
            {
                var manager = scope.Resolve<DatabaseManager>();
                manager.Search("SELECT * FORM USER");
            }
        }        
    }
}
