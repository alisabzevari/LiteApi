using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace StrongApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "http://localhost:18080";
            using (WebApp.Start<Startup>(url))
            {
                Process.Start(url); // Launch the browser.
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
        }
    }
}
