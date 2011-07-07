namespace OpenSpacePlanner.Contracts {
	public interface IRoom : IEntity {
		string Name { get; }
		int Capacity { get; }
	}
}