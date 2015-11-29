using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;

namespace MyTeam.Pipeline
{
    public class HostConstraint : IRouteConstraint
    {
        private readonly string _hostname;

        public HostConstraint(string hostname)
        {
            _hostname = hostname;
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, IDictionary<string, object> values, RouteDirection routeDirection)
        {
            return _hostname.Equals(httpContext.Request.Host.Value, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}