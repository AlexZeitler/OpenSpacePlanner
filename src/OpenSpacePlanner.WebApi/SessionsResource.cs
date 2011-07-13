using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.ApplicationServer.Http;
using OpenSpacePlanner.Contracts;
using OpenSpacePlanner.Domain;

namespace OpenSpacePlanner.WebApi {
	[ServiceContract]
	public class SessionsResource {
		readonly INosSessionRepository _sessionRepository;

		public SessionsResource(INosSessionRepository sessionRepository) {
			_sessionRepository = sessionRepository;
		}

		[WebGet(UriTemplate = "")]
		public List<NosSession> Get() {
			return new List<NosSession>(_sessionRepository.Get() as IEnumerable<NosSession>);
		}

		[WebInvoke(UriTemplate = "", Method = "POST")]
		public HttpResponseMessage<NosSession> Post(NosSession nosSession, HttpRequestMessage<NosSession> request) {
			_sessionRepository.Insert(nosSession);
			nosSession = _sessionRepository.Get(nosSession.Id) as NosSession;
			return new HttpResponseMessage<NosSession>(nosSession, HttpStatusCode.Created);
		}

		[WebGet(UriTemplate = "planned")]
		public List<NosSession> GetPlannedSessions() {
			IList<NosSession> plannedSessions = ConvertToListOf<NosSession>(_sessionRepository.GetPlannedSessions().ToList());
			return (List<NosSession>) plannedSessions;
		}

		public static IList<T> ConvertToListOf<T>(IList iList) {
			IList<T> result = new List<T>();
			foreach (T value in iList)
				result.Add(value);

			return result;
		}
	}


}