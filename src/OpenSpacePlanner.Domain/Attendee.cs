using OpenSpacePlanner.Contracts;

namespace OpenSpacePlanner.Domain {
	public class Attendee : Entity, IAttendee {
		public virtual string FirstName { get; set; }
		public virtual string LastName { get; set; }
		public virtual string Tag { get; set; }
	}
}