using System;
using System.Collections.Generic;
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
			IContainer container = builder.Build();
			var configuration = 
				HttpHostConfiguration
					.Create()
						.SetResourceFactory((serviceType, instanceContext, request) => container.Resolve(serviceType), null);
			
			routes.MapServiceRoute<SessionsResource>("sessions", configuration);
		}

		protected void Application_Start() {
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}
	}
}