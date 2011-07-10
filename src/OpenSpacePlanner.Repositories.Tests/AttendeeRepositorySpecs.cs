using System;
using Machine.Fakes;
using Machine.Specifications;
using OpenSpacePlanner.Contracts;
using OpenSpacePlanner.Domain;
using OpenSpacePlanner.Repositories.Tests.Mappings;
using PDMLab.Common.NHibernate;

namespace OpenSpacePlanner.Repositories.Tests {
	public class Given_a_attendee_repository_when_requesting_to_insert_an_atttendee : WithSubject<AttendeeRepository> {
		static IAttendee _expectedAttendee;
		static IAttendee _actualAttendee;

		Establish context = () => {
									With<NHibernateSqliteSessionProviderLoaded>();
									_expectedAttendee = new Attendee() {
										FirstName = "Alexander",
										LastName = "Zeitler",
										Tag = "2arc"
									};
								};

		Because of
			= () => {
					Subject.Insert(_expectedAttendee);
					using (var session = The<INHibernateSessionProvider>().GetSession()) {
						_actualAttendee = session.Get<Attendee>(_expectedAttendee.Id);
					}
				};

		It should_insert_attendee = () => { _actualAttendee.ShouldEqual(_expectedAttendee); };
	}

	public class Given_a_attendee_repository_when_requesting_to_get_an_attendee_by_its_tag : WithSubject<AttendeeRepository> {

		static IAttendee _expectedAttendee;
		static IAttendee _actualAttendee;

		Establish context
			= () => {
					With<NHibernateSqliteSessionProviderLoaded>();
					_expectedAttendee = new Attendee() { FirstName = "Alexander", LastName = "Zeitler", Tag = "2arc" };
				};

		Because of = () => {
							using (var session = The<INHibernateSessionProvider>().GetSession()) {
								session.Save(_expectedAttendee);
								session.Flush();
							}
		                    _actualAttendee = Subject.Get(_expectedAttendee.Tag);
		};

		It should_yield_attendee = () => { _actualAttendee.ShouldEqual(_expectedAttendee); };
	}
}