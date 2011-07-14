﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentNHibernate.Cfg;
using LightCore;
using OpenSpacePlanner.Contracts;
using OpenSpacePlanner.Repositories;
using OpenSpacePlanner.Repositories.Mappings;
using PDMLab.Common.NHibernate;

namespace OpenSpacePlanner.Web {
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication {
		static IContainer _container;

		public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes) {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);

			ControllerBuilder.Current.SetControllerFactory(new LightCore.Integration.Web.Mvc.ControllerFactory(_container));
		}

		protected void Application_Start() {
			AreaRegistration.RegisterAllAreas();
			setupIoc();
			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}

		void setupIoc() {
			Action<MappingConfiguration> mappingConfiguration =
				mappings => mappings.FluentMappings.AddFromAssemblyOf<AttendeeMap>();
			IContainerBuilder builder = new ContainerBuilder();
			builder.Register<INHibernateSessionProvider, SqlServerConnectionStringNHibernateSessionProvider>().WithArguments("nosplanner", mappingConfiguration);
			builder.Register<IAttendeeRepository, AttendeeRepository>();
			_container = builder.Build();
		}
	}
}