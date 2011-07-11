using System;

namespace OpenSpacePlanner.Contracts {
	public interface IAttendeeRepository {
		void Insert(IAttendee attendee);
		IAttendee Get(string tag);
		IAttendee Get(Guid id);
	}
}