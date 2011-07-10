using FluentNHibernate.Mapping;
using OpenSpacePlanner.Domain;

namespace OpenSpacePlanner.Repositories.Mappings {
	public class NosSessionMap : ClassMap<NosSession> {
		public NosSessionMap() {
			Id(n => n.Id).Column("NosSessionId").GeneratedBy.Assigned();
			Map(n => n.CreatedOn);
			Map(n => n.Title);
			Map(n => n.Description);
			Map(n => n.Tag);
			Map(n => n.Start);
			Map(n => n.End).Column("[End]");
			Map(n => n.Owner);
			Map(n => n.OwnerTag);
			Map(n => n.Room);
		}
	}
}