using System;
using FluentNHibernate.Testing;
using Machine.Fakes;
using Machine.Specifications;
using OpenSpacePlanner.Domain;
using OpenSpacePlanner.Repositories.Mappings;
using PDMLab.Common.NHibernate;

namespace OpenSpacePlanner.Repositories.Tests.Mappings
{
	public class Given_a_room_when_mapped : WithSubject<RoomMap>
	{
		static PersistenceSpecification<Room> _specification;

		Establish context = () => { With<NHibernateSqliteSessionProviderLoaded>(); };

		Because of = () => { _specification = new PersistenceSpecification<Room>(The<INHibernateSessionProvider>().GetSession()); };

		It should_be_mapped_correctly 
			= () => { _specification
				.CheckProperty(r => r.Name, "Raum 1")
				.CheckProperty(r => r.Capacity, 25)
				.VerifyTheMappings(); };
	}
}