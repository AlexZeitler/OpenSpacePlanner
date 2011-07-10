using OpenSpacePlanner.Contracts;

namespace OpenSpacePlanner.Domain {
	public class Room : Entity, IRoom {
		public virtual string Name { get; set; }
		public virtual int Capacity { get; set; }
	}
}