// Purpose: Data Platform Multipe Prefix Direct Route Provider.
// This direct route provider is gateway of capturing the multiple order RoutePrefix in the controller
// and routing multiple routePrefix to the same controller actions

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace MutlipleRouterPrefix
{
    public class CustomMultiplePrefixDirectRouteProvider: DefaultDirectRouteProvider
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CustomMultiplePrefixDirectRouteProvider()
        {

        }
        #endregion

        #region Overriden Method
        /// <summary>
        /// <see cref="DefaultDirectRouteProvider.GetActionDirectRoutes(HttpActionDescriptor, IReadOnlyList{IDirectRouteFactory}, IInlineConstraintResolver)"/>
        /// </summary>
        /// <returns>ActionDescriptors</returns>
        protected override IReadOnlyList<RouteEntry> GetActionDirectRoutes(HttpActionDescriptor actionDescriptor, IReadOnlyList<IDirectRouteFactory> factories, IInlineConstraintResolver constraintResolver)
        {
            List<String> routePrefixes = GetRoutePrefixes(actionDescriptor.ControllerDescriptor);
            return CreateRouteEntries(routePrefixes, factories, new[] { actionDescriptor }, constraintResolver, true);
        }
        /// <summary>
        /// <see cref="DefaultDirectRouteProvider.GetControllerDirectRoutes(HttpControllerDescriptor, IReadOnlyList{HttpActionDescriptor}, IReadOnlyList{IDirectRouteFactory}, IInlineConstraintResolver)"/>
        /// </summary>
        /// <returns></returns>
        protected override IReadOnlyList<RouteEntry> GetControllerDirectRoutes(HttpControllerDescriptor controllerDescriptor, IReadOnlyList<HttpActionDescriptor> actionDescriptors, IReadOnlyList<IDirectRouteFactory> factories, IInlineConstraintResolver constraintResolver)
        {
            List<String> routePrefixes = GetRoutePrefixes(controllerDescriptor);
            return CreateRouteEntries(GetRoutePrefixes(controllerDescriptor), factories, actionDescriptors, constraintResolver, false);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Helps to get the RoutePrefixes provided in the given controllerDescriptor
        /// </summary>
        /// <param name="controllerDescriptor">ControllerDescriptor</param>
        /// <returns>Route Prefix in the current Controller</returns>
        private List<String> GetRoutePrefixes(HttpControllerDescriptor controllerDescriptor)
        {
            Collection<IRoutePrefix> attributes = controllerDescriptor.GetCustomAttributes<IRoutePrefix>(false);
            if (attributes == null) { return new List<String> { null }; }
            List<String> prefixes = new List<String>();
            foreach (IRoutePrefix attribute in attributes)
            {
                if (attribute == null) { continue; }
                String prefix = attribute.Prefix;
                HandleRoutePrefixErrors(prefix, controllerDescriptor);
                prefixes.Add(prefix);
            }
            if (prefixes.Count == 0) { prefixes.Add(null); }
            return prefixes.ToList();
        }
        /// <summary>
        /// Helps to create Route entries from the given DirectRouteFactories
        /// </summary>
        /// <param name="prefixes">Prefixes</param>
        /// <param name="factories">DirectRouteFactories</param>
        /// <param name="actions">Action descriptors</param>
        /// <param name="constraintResolver">Inline constraint resolver</param>
        /// <param name="targetIsAction">TargetIsAction</param>
        /// <returns>RouteEntries</returns>
        private IReadOnlyList<RouteEntry> CreateRouteEntries(List<String> prefixes, IReadOnlyCollection<IDirectRouteFactory> factories,
            IReadOnlyCollection<HttpActionDescriptor> actions, IInlineConstraintResolver constraintResolver, Boolean targetIsAction)
        {
            List<RouteEntry> entries = new List<RouteEntry>();
            foreach (String prefix in prefixes)
            {
                foreach (IDirectRouteFactory factory in factories)
                {
                    RouteEntry entry = CreateRouteEntry(prefix, factory, actions, constraintResolver, targetIsAction);
                    entries.Add(entry);
                }
            }
            return entries;
        }
        /// <summary>
        /// Helps to create route entry
        /// </summary>
        /// <param name="prefix">Prefix</param>
        /// <param name="factory">Direct Route Factory</param>
        /// <param name="actions">Action Descriptors</param>
        /// <param name="constraintResolver">Inline Constraint Resolver</param>
        /// <param name="targetIsAction">TargetIsAction</param>
        /// <returns>RouteEntry</returns>
        private RouteEntry CreateRouteEntry(String prefix, IDirectRouteFactory factory, IReadOnlyCollection<HttpActionDescriptor> actions,
            IInlineConstraintResolver constraintResolver, Boolean targetIsAction)
        {
            DirectRouteFactoryContext context = new DirectRouteFactoryContext(prefix, actions, constraintResolver, targetIsAction);
            RouteEntry entry = factory.CreateRoute(context);
            ValidateRouteEntry(entry);
            return entry;
        }
        #endregion

        #region Private Exception Handling methods
        /// <summary>
        /// Helps to handle the RoutePrefix Errors
        /// </summary>
        /// <param name="prefix">Current route prefix</param>
        /// <param name="controllerDescriptor">Controller Descriptor</param>
        private void HandleRoutePrefixErrors(String prefix, HttpControllerDescriptor controllerDescriptor)
        {
            if (prefix == null)
            {
                throw new InvalidOperationException("Prefix can not be null. Controller: " + controllerDescriptor.ControllerType.FullName);
            }
            if (prefix.EndsWith("/", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Invalid prefix" + prefix + " in " + controllerDescriptor.ControllerName);
            }
        }
        /// <summary>
        /// Helps to validate the route entry
        /// </summary>
        /// <param name="routeEntry">RouteEntry</param>
        private static void ValidateRouteEntry(RouteEntry routeEntry)
        {
            if (routeEntry == null)
            {
                throw new ArgumentNullException("routeEntry");
            }
            IHttpRoute route = routeEntry.Route;
            if (route.Handler != null)
            {
                throw new InvalidOperationException("Direct route handler is not supported");
            }
        }
        #endregion
    }
}