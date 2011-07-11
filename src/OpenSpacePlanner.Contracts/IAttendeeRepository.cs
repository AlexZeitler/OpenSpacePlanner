using System;
using System.Collections.Generic;

namespace OpenSpacePlanner.Contracts {
	public interface IAttendeeRepository {
		void Insert(IAttendee attendee);
		IAttendee Get(string tag);
		IAttendee Get(Guid id);
		IEnumerable<IAttendee> Get();
	}
}