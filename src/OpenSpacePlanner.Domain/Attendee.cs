using OpenSpacePlanner.Contracts;

namespace OpenSpacePlanner.Domain {
	public class Attendee : TaggedEntity, IAttendee {
		string _tag;
		public virtual string FirstName { get; set; }
		public virtual string LastName { get; set; }
	}
}