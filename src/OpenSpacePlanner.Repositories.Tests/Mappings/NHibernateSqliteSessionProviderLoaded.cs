using System.IO;
using System.Reflection;
using FluentNHibernate.Cfg.Db;
using Machine.Fakes;
using OpenSpacePlanner.Repositories.Mappings;
using PDMLab.Common.NHibernate;
using PDMLab.Common.Resources;

namespace OpenSpacePlanner.Repositories.Tests.Mappings {
	public class NHibernateSqliteSessionProviderLoaded {
		OnEstablish context
			= fakeAccessor => {
			                  	Assembly assembly = Assembly.GetAssembly(typeof(NHibernateSqliteSessionProviderLoaded));
			                  	string assemblyPath = assembly.Location;
			                  	string outputDirectory = Path.GetDirectoryName(assemblyPath);
			                  	string resourceName = "nosplanner.s3db";
			                  	string dbFilename = Path.Combine(outputDirectory, resourceName);
			                  	ResourceLoader.CreateFileFromEmbeddedResource(assembly, resourceName, dbFilename);
			                  	INHibernateSessionProvider nHibernateSessionProvider =
			                  		new NHibernateSessionProvider(
			                  			SQLiteConfiguration.Standard.ConnectionString(
			                  				connectionString => connectionString
			                  				                    	.Is(string.Format("Data Source={0};Version=3;New=False;", dbFilename))).ShowSql,

			                  			mappings => mappings.FluentMappings.AddFromAssemblyOf<NosSessionMap>(), false, false);
			                  	fakeAccessor.Configure(c => c.For<INHibernateSessionProvider>().Use(nHibernateSessionProvider));
			};
	}
}