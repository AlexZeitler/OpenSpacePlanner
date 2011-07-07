using FluentNHibernate.Mapping;
using OpenSpacePlanner.Domain;

namespace OpenSpacePlanner.Repositories.Mappings {
	public class AttendeeMap : ClassMap<Attendee> {
		public AttendeeMap() {
			Id(a => a.Id).Column("AttendeeId").GeneratedBy.Assigned();
			Map(a => a.FirstName);
			Map(a => a.LastName);
			Map(a => a.Tag);
		}
	}
}