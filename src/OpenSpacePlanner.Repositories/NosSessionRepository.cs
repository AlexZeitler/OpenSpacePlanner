using System;
using System.Collections.Generic;
using OpenSpacePlanner.Contracts;
using OpenSpacePlanner.Domain;
using PDMLab.Common.NHibernate;

namespace OpenSpacePlanner.Repositories {
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

		public INosSession Get(Guid id) {
			using(var session = _nHibernateSessionProvider.GetSession()) {
				return session.Get<NosSession>(id);
			}
		}

		public void Update(INosSession nosSession) {
			using(var session = _nHibernateSessionProvider.GetSession()) {
				session.Update(nosSession);
			}
		}
	}
}