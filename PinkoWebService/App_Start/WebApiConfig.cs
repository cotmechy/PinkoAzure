using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PinkoWebService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name:  "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                //defaults: new { controller = "PinkoFormulaDictionary", action = "Index", id = RouteParameter.Optional }
            );
        }
    }
}
