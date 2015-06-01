using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Testing;

namespace LiteApi.Tests
{
    public class WebApiTestBase
    {
        public void PostAsJson<T>(string requestUri, T value, Action<HttpResponseMessage> assertions)
        {
            using (var server = TestServer.Create<Startup>())
            {
                using (var client = new HttpClient(server.Handler))
                {
                    using (var response = client.PostAsJsonAsync(requestUri, value).Result)
                    {
                        assertions(response);
                    }
                }
            }
        }

        public void Get(string requestUri, Action<HttpResponseMessage> assertions)
        {
            using (var server = TestServer.Create<Startup>())
            {
                using (var client = new HttpClient(server.Handler))
                {
                    using (var response = client.GetAsync(requestUri).Result)
                    {
                        assertions(response);
                    }
                }
            }
        }
    }
}
