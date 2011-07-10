using System.Collections.Generic;
using System.Linq;
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
	}
}