using System;
using FluentNHibernate.Testing;
using Machine.Fakes;
using Machine.Specifications;
using OpenSpacePlanner.Domain;
using OpenSpacePlanner.Repositories.Mappings;
using PDMLab.Common.NHibernate;

namespace OpenSpacePlanner.Repositories.Tests.Mappings {
	public class Given_an_attendee_when_being_mapped : WithSubject<AttendeeMap> {
		static PersistenceSpecification<Attendee> _specification;

		Establish context = () => {
									With<NHibernateSqliteSessionProviderLoaded>();
								};

		Because of
			= () => {
				_specification =
					new PersistenceSpecification<Attendee>(The<INHibernateSessionProvider>().GetSession());
			};

		It should_be_mapped_correctly
			= () => {
				_specification
					.CheckProperty(a => a.FirstName, "Alexander")
					.CheckProperty(a => a.LastName, "Zeitler")
					.CheckProperty(a => a.Tag, "2arc")
					.VerifyTheMappings();
			};
	}
}