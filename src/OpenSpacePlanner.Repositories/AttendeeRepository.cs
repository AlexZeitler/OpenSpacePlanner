using System;
using System.Linq;
using NHibernate.Linq;
using OpenSpacePlanner.Contracts;
using OpenSpacePlanner.Domain;
using PDMLab.Common.NHibernate;

namespace OpenSpacePlanner.Repositories {
	public class AttendeeRepository : IAttendeeRepository {
		readonly INHibernateSessionProvider _nHibernateSessionProvider;

		public AttendeeRepository(INHibernateSessionProvider nHibernateSessionProvider) {
			_nHibernateSessionProvider = nHibernateSessionProvider;
		}

		public void Insert(IAttendee attendee) {
			using(var session = _nHibernateSessionProvider.GetSession()) {
				session.Save(attendee);
				session.Flush();
			}
		}

		public IAttendee Get(string tag) {
			using(var session = _nHibernateSessionProvider.GetSession()) {
				return session.Query<Attendee>().Where(a => a.Tag == tag).FirstOrDefault();
			}
		}

		public IAttendee Get(Guid id) {
			using(var session = _nHibernateSessionProvider.GetSession()) {
				return session.Get<Attendee>(id);
			}
		}
	}
}