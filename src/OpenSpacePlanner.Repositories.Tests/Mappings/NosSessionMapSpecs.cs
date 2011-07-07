using System;
using FluentNHibernate.Testing;
using Machine.Fakes;
using Machine.Specifications;
using OpenSpacePlanner.Domain;
using OpenSpacePlanner.Repositories.Mappings;
using PDMLab.Common.NHibernate;

namespace OpenSpacePlanner.Repositories.Tests.Mappings {
	public class Given_a_nos_session_when_mapped : WithSubject<NosSessionMap> {
		static PersistenceSpecification<NosSession> _specification;

		Establish context = () => { With<NHibernateSqliteSessionProviderLoaded>(); };

		Because of = () => {
							_specification =
								new PersistenceSpecification<NosSession>(The<INHibernateSessionProvider>().GetSession());
						};

		It should_be_mapped_correctly = () => {
												DateTime now = new DateTime(2011, 7, 7, 22, 0, 0);
												_specification
													.CheckProperty(n => n.CreatedOn, now)
													.CheckProperty(n => n.Title, "TDD")
													.CheckProperty(n => n.Description, "Test-Driven-Development")
													.CheckProperty(n => n.Tag, "att2")
													.CheckProperty(n => n.Start, now)
													.CheckProperty(n => n.End, now)
													.CheckProperty(n => n.Owner, "Alexander Zeitler")
													.CheckProperty(n => n.OwnerTag, "2arc")
													.CheckProperty(n => n.Room, "A2")
													.VerifyTheMappings();
											};
	}
}