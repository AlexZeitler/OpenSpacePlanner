using OpenSpacePlanner.Contracts;

namespace OpenSpacePlanner.Domain {
	public class Room : Entity, IRoom {
		public string Name { get; private set; }
		public int Capacity { get; private set; }
	}
}