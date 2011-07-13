using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentNHibernate.Cfg;
using LightCore;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;
using Microsoft.ApplicationServer.Http.Dispatcher;
using OpenSpacePlanner.Contracts;
using OpenSpacePlanner.Repositories;
using OpenSpacePlanner.Repositories.Mappings;
using PDMLab.Common.NHibernate;

namespace OpenSpacePlanner.WebApi {
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication {
		public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes) {
			ContainerBuilder builder = new ContainerBuilder();
			Action<MappingConfiguration> mappingsConfig = mappings=>mappings.FluentMappings.AddFromAssemblyOf<NosSessionMap>();
			builder.Register<INHibernateSessionProvider, SqlServerConnectionStringNHibernateSessionProvider>().WithArguments(
				"nosplanner", mappingsConfig);
			builder.Register<INosSessionRepository, NosSessionRepository>();
			builder.Register<IAttendeeRepository, AttendeeRepository>();
			IContainer container = builder.Build();
			var configuration = 
				HttpHostConfiguration
					.Create()
						.SetResourceFactory((serviceType, instanceContext, request) => container.Resolve(serviceType), null)
						.SetErrorHandler<ContactManagerErrorHandler>();
			
			routes.MapServiceRoute<SessionsResource>("sessions", configuration);
			routes.MapServiceRoute<SessionResource>("session", configuration);
			routes.MapServiceRoute<AttendeesResource>("attendees",configuration);
		}

		protected void Application_Start() {
			//AreaRegistration.RegisterAllAreas();

			//RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}
	}

	public class ContactManagerErrorHandler : HttpErrorHandler {
		protected override bool OnHandleError(Exception error) {
			Trace.Listeners.Add(new TextWriterTraceListener(@"C:\Webs\NOSSued\OpenSpacePlanner\trace.log"));
			Trace.WriteLine(DateTime.Now);
			Trace.WriteLine(error.ToString());
			Trace.Flush();
			return false;
		}

		protected override System.Net.Http.HttpResponseMessage OnProvideResponse(Exception error) {
			Trace.Listeners.Add(new TextWriterTraceListener(@"C:\Webs\NOSSued\OpenSpacePlanner\trace.log"));
			Trace.WriteLine(DateTime.Now);
			Trace.WriteLine(error.ToString());
			Trace.Flush();
			var exception = (HttpResponseException)error;
			var response = exception.Response;
			response.ReasonPhrase = "[Handled]" + response.ReasonPhrase;
			return response;
		}
	}
}