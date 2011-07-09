using OpenSpacePlanner.Contracts;
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
	}
}