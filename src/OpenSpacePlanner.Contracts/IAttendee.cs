namespace OpenSpacePlanner.Contracts {
	public interface IAttendee : IEntity {
		string FirstName { get; }
		string LastName { get; }
		string Tag { get; }
	}
}