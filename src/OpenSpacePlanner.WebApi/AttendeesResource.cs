using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.ApplicationServer.Http;
using OpenSpacePlanner.Contracts;
using OpenSpacePlanner.Domain;

namespace OpenSpacePlanner.WebApi {
	[ServiceContract]
	public class AttendeesResource {
		readonly IAttendeeRepository _attendeeRepository;

		public AttendeesResource(IAttendeeRepository attendeeRepository) {
			_attendeeRepository = attendeeRepository;
		}

		[WebInvoke(UriTemplate = "", Method = "POST")]
		public HttpResponseMessage<Attendee> Post(Attendee attendee, HttpRequestMessage<Attendee> request) {
			_attendeeRepository.Insert(attendee);
			attendee = _attendeeRepository.Get(attendee.Id) as Attendee;
			return new HttpResponseMessage<Attendee>(attendee, HttpStatusCode.Created);
		}

		[WebGet(UriTemplate = "")]
		public List<Attendee> Get() {
			return _attendeeRepository.Get() as List<Attendee>;
		}
	}
}