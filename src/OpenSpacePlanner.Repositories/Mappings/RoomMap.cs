using FluentNHibernate.Mapping;
using OpenSpacePlanner.Domain;

namespace OpenSpacePlanner.Repositories.Mappings {
	public class RoomMap : ClassMap<Room> {
		public RoomMap() {
			Id(r => r.Id).Column("RoomId").GeneratedBy.Assigned();
			Map(r => r.Name);
			Map(r => r.Capacity);
		}
	}
}