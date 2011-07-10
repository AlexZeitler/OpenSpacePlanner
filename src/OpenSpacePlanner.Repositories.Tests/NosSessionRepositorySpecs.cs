using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Fakes;
using Machine.Specifications;
using OpenSpacePlanner.Contracts;
using OpenSpacePlanner.Domain;
using OpenSpacePlanner.Repositories.Tests.Mappings;
using PDMLab.Common.NHibernate;

namespace OpenSpacePlanner.Repositories.Tests
{
	public class Given_a_nos_session_repository_when_requesting_to_insert_a_nos_sessionNos : WithSubject<NosSessionRepository>
	{
		static INosSession _expectedNosSession;
		static INosSession _actualNosSession;

		Establish context 
			= () =>
			  	{
			  		With<NHibernateSqliteSessionProviderLoaded>();
					_expectedNosSession = new NosSession()
					                      	{
					                      		CreatedOn = DateTime.Now,
												Title = "NHibernate in a NutShell",
												Description = "Something on NHibernate",
												Start = DateTime.Now + new TimeSpan(1,0,0),
												End = DateTime.Now + new TimeSpan(2,0,0),
												Owner = "Alexander Zeitler",
												OwnerTag = "2arc",
												Room = "3A",
												Tag = "2seb"
					                      	};
			  	};

		Because of 
			= () =>
			  	{
			  		Subject.Insert(_expectedNosSession);
					using(var session = The<INHibernateSessionProvider>().GetSession()) {
						_actualNosSession= session.Get<NosSession>(_expectedNosSession.Id);
					}
			  	};

		It should_insert_nos_session = () => { _actualNosSession.ShouldEqual(_expectedNosSession); };
	}

	public class Given_a_nos_session_repository_when_requesting_all_sessions : WithSubject<NosSessionRepository> {
		static IEnumerable<INosSession> _actualSessions;
		static INosSession _expectedNosSession1;
		static INosSession _expectedNosSession2;
		static INosSession _expectedNosSession3;

		Establish context 
			= () =>
			  	{
			  		With<NHibernateSqliteSessionProviderLoaded>();
					_expectedNosSession1 = new NosSession() {
						CreatedOn = DateTime.Now,
						Title = "NHibernate in a NutShell",
						Description = "Something on NHibernate",
						Start = DateTime.Now + new TimeSpan(1, 0, 0),
						End = DateTime.Now + new TimeSpan(2, 0, 0),
						Owner = "Alexander Zeitler",
						OwnerTag = "2arc",
						Room = "3A",
						Tag = "2seb"
					};
					_expectedNosSession2 = new NosSession() {
						CreatedOn = DateTime.Now,
						Title = "NHibernate in a NutShell",
						Description = "Something on NHibernate",
						Start = DateTime.Now + new TimeSpan(1, 0, 0),
						End = DateTime.Now + new TimeSpan(2, 0, 0),
						Owner = "Alexander Zeitler",
						OwnerTag = "2arc",
						Room = "3A",
						Tag = "2seb"
					};
					_expectedNosSession3 = new NosSession() {
						CreatedOn = DateTime.Now,
						Title = "NHibernate in a NutShell",
						Description = "Something on NHibernate",
						Start = DateTime.Now + new TimeSpan(1, 0, 0),
						End = DateTime.Now + new TimeSpan(2, 0, 0),
						Owner = "Alexander Zeitler",
						OwnerTag = "2arc",
						Room = "3A",
						Tag = "2seb"
					};
					using(var session = The<INHibernateSessionProvider>().GetSession()) {
						session.Save(_expectedNosSession1);
						session.Save(_expectedNosSession2);
						session.Save(_expectedNosSession3);
						session.Flush();
					}
			  	};

		Because of = () =>
		             	{
		             		_actualSessions = Subject.Get();
		             	};

		It should_yield_all_sessions = () => { _actualSessions.Count().ShouldEqual(3); };
	}

	public class NosSessionRepository : INosSessionRepository {
		readonly INHibernateSessionProvider _nHibernateSessionProvider;

		public NosSessionRepository(INHibernateSessionProvider nHibernateSessionProvider) {
			_nHibernateSessionProvider = nHibernateSessionProvider;
		}

		public void Insert(INosSession nosSession) {
			using(var session = _nHibernateSessionProvider.GetSession()) {
				session.Save(nosSession);
				session.Flush();
			}
		}

		public IEnumerable<INosSession> Get() {
			using(var session = _nHibernateSessionProvider.GetSession()) {
				return session.CreateCriteria<NosSession>().List<NosSession>();
			}
		}
	}

	public interface INosSessionRepository {
	}
}