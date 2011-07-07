using OpenSpacePlanner.Contracts;

namespace OpenSpacePlanner.Domain {
	public class Attendee : Entity, IAttendee {
		public virtual string FirstName { get; private set; }
		public virtual string LastName { get; private set; }
		public virtual string Tag { get; private set; }
	}
}