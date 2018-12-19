namespace Ntech.NetFramework.DecryptPluralsight {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.ComponentModel.Composition.Primitives;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Caliburn.Micro;
    using Ntech.NetFramework.DecryptPluralsight.ViewModels;

    /// <summary>
    /// Class AppBootstrapper.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.BootstrapperBase" />
    public class AppBootstrapper : BootstrapperBase
    {
        /// <summary>
        /// The container
        /// </summary>
        private CompositionContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppBootstrapper"/> class.
        /// </summary>
        public AppBootstrapper()
        {
            this.Initialize();
        }

        /// <summary>
        /// Override to configure the framework and setup your IoC container.
        /// </summary>
        protected override void Configure()
        {
            var aggassemblyCatalog = new AggregateCatalog(
                AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()
                );

            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var assemblyCatalog = new AssemblyCatalog(file);

                // Force MEF to load the plug-in and figure out if there are any exports
                // good assemblies will not throw the RTLE exception and can be added to the catalog
                if (assemblyCatalog.Parts.Any())
                {
                    aggassemblyCatalog.Catalogs.Add(assemblyCatalog);
                }
                else
                {
                    assemblyCatalog.Dispose();
                }
            }

            this.container = new CompositionContainer(aggassemblyCatalog);

            this.container.ComposeExportedValue<IWindowManager>(new WindowManager());
            this.container.ComposeExportedValue<IEventAggregator>(new EventAggregator());

            var batch = new CompositionBatch();
            batch.AddExportedValue(this.container);
            this.container.Compose(batch);
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="contract">The contract.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="Exception"></exception>
        protected override object GetInstance(Type serviceType, string contract)
        {
            var exports = this.container.GetExports(serviceType, null, contract).ToList();
            if (exports.Any())
            {
                return exports.First().Value;
            }

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            var list = new List<object>();
            var exports = this.container.GetExports(serviceType, null, string.Empty);
            list.AddRange(exports.Select(export => export.Value));
            return list;
        }

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="instance">The instance to perform injection on.</param>
        protected override void BuildUp(object instance)
        {
            this.container.SatisfyImportsOnce(instance);
        }

        /// <summary>
        /// Override this to add custom behavior to execute after the application starts.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The args.</param>
        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            this.DisplayRootViewFor<ShellViewModel>();
        }

        /// <summary>
        /// Override to tell the framework where to find assemblies to inspect for views, etc.
        /// </summary>
        /// <returns>A list of assemblies to inspect.</returns>
        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[] {
                Assembly.GetExecutingAssembly()
            };
        }
    }
}