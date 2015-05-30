using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Owin;

namespace StrongApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            app.UseWebApi(config);
        }
    }
}
