using Funq;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace SSDisposableIssue
{
    public class Global : System.Web.HttpApplication
    {
        public class TestDisposible : IDisposable
        {
            public void Dispose()
            {
            }
        }
        /// <summary>
        /// Create your ServiceStack web service application with a singleton AppHost.
        /// </summary>        
        public class HelloAppHost : AppHostBase
        {
            /// <summary>
            /// Initializes a new instance of your ServiceStack application, with the specified name and assembly containing the services.
            /// </summary>
            public HelloAppHost() : base("Hello Web Services", typeof(HelloService).Assembly) { }

            /// <summary>
            /// Configure the container with the necessary routes for your ServiceStack application.
            /// </summary>
            /// <param name="container">The built-in IoC used with ServiceStack.</param>
            public override void Configure(Container container)
            {
                //Register user-defined REST-ful urls. You can access the service at the url similar to the following.
                //http://localhost/ServiceStack.Hello/servicestack/hello or http://localhost/ServiceStack.Hello/servicestack/hello/John%20Doe
                //You can change /servicestack/ to a custom path in the web.config.
                Routes
                  .Add<Hello>("/hello")
                  .Add<Hello>("/hello/{Name}");

                container.RegisterAutoWired<TestDisposible>().ReusedWithin(ReuseScope.None);
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            (new HelloAppHost()).Init();

            new Thread(new ThreadStart(() =>
            {
                // Following test pass
                Test();
                
                var d = HelloAppHost.Instance.Container.Resolve<TestDisposible>();

                // Folloing execution fails
                Test();
            })).Start();
        }

        private void Test()
        {
            var domain = AppDomain.CreateDomain("Test");
            var list = domain.GetAssemblies();
            AppDomain.Unload(domain);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}