using System;
using System.Collections.Generic;
using System.Linq;
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

	public class Given_a_attendee_repository_when_requesting_to_get_an_attendee_by_its_id : WithSubject<AttendeeRepository> {
		static IAttendee _expectedAttendee;
		static IAttendee _actualAttendee;

		Establish context = () =>
		                    	{
		                    		With<NHibernateSqliteSessionProviderLoaded>();
									_expectedAttendee = new Attendee() { FirstName = "Alexander", LastName = "Zeitler", Tag = "2arc" };
									using(var session = The<INHibernateSessionProvider>().GetSession()) {
										session.Save(_expectedAttendee);
										session.Flush();
									}
		                    	};

		Because of = () =>
		             	{
		             		_actualAttendee = Subject.Get(_expectedAttendee.Id);
		             	};

		It should_yield_attendee = () => { _actualAttendee.ShouldEqual(_expectedAttendee); };
	}

	public class Given_a_attendee_repository_when_requesting_to_get_all_attendees : WithSubject<AttendeeRepository> {
		static IEnumerable<IAttendee> _actualAttendees;

		Establish context = () =>
		                    	{
		                    		With<NHibernateSqliteSessionProviderLoaded>();
									IAttendee attendee1 = new Attendee() { FirstName = "Alexander", LastName = "Zeitler", Tag = "2arc" };
									IAttendee attendee2 = new Attendee() { FirstName = "Frank", LastName = "Pfattheicher", Tag = "3fra" };
									IAttendee attendee3 = new Attendee() { FirstName = "Ralf", LastName = "Schoch", Tag = "arh1" };
									using (var session = The<INHibernateSessionProvider>().GetSession()) {
										session.Save(attendee1);
										session.Save(attendee2);
										session.Save(attendee3);
										session.Flush();
									}
		                    	};

		Because of = () => { _actualAttendees = Subject.Get(); };

		It should_yield_all_attendees = () => { _actualAttendees.Count().ShouldEqual(3); };
	}
}