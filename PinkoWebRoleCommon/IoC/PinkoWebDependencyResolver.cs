using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Http.Dependencies;
using Microsoft.Practices.Unity;

namespace PinkoWebRoleCommon.IoC
{
    public class PinkoWebDependencyResolver : IDependencyResolver, IDependencyScope
    {
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            // nothing to do
        }

        /// <summary>
        /// Retrieves a service from the scope.
        /// </summary>
        /// <returns>
        /// The retrieved service.
        /// </returns>
        /// <param name="serviceType">The service to be retrieved.</param>
        public object GetService(Type serviceType)
        {
            object obj = null;

            if (PinkoContainer.IsRegistered(serviceType))
                obj = PinkoContainer.Resolve(serviceType);

            return obj;
        }

        /// <summary>
        /// Retrieves a collection of services from the scope.
        /// </summary>
        /// <returns>
        /// The retrieved collection of services.
        /// </returns>
        /// <param name="serviceType">The collection of services to be retrieved.</param>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return PinkoContainer.ResolveAll(serviceType);
        }

        /// <summary>
        /// Starts a resolution scope. 
        /// </summary>
        /// <returns>
        /// The dependency scope.
        /// </returns>
        public IDependencyScope BeginScope()
        {
            return this;
        }

        /// <summary>
        /// PinkoContainer
        /// </summary>
        [Dependency]
        public IUnityContainer PinkoContainer { get; set; }
    }
}
