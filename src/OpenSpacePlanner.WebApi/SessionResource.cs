using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.ApplicationServer.Http;
using OpenSpacePlanner.Contracts;
using OpenSpacePlanner.Domain;

namespace OpenSpacePlanner.WebApi {
	[ServiceContract]
	public class SessionResource {
		readonly INosSessionRepository _sessionRepository;

		public SessionResource(INosSessionRepository sessionRepository ) {
			_sessionRepository = sessionRepository;
		}

		[WebInvoke(UriTemplate = "{id}", Method = "PUT")]
		public HttpResponseMessage<NosSession> Put(NosSession session, HttpRequestMessage<NosSession> request) {
			_sessionRepository.Update(session);
			session = _sessionRepository.Get(session.Id) as NosSession;
			return new HttpResponseMessage<NosSession>(session, HttpStatusCode.Accepted);
		}

		[WebGet(UriTemplate = "{id}")]
		public NosSession Get(Guid id) {
			return _sessionRepository.Get(id) as NosSession;
		}
	}
}