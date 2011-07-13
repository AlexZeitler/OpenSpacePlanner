using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
			try {
				return new List<NosSession>(_sessionRepository.Get() as IEnumerable<NosSession>);
			}
			catch (Exception e) {
				//Trace.Listeners.Add(new TextWriterTraceListener(@"C:\Webs\NOSSued\OpenSpacePlanner\trace.log"));
				//Trace.WriteLine(DateTime.Now);
				//Trace.WriteLine(e.ToString());
				//Trace.Flush();
				return null;
			}
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

		[WebGet(UriTemplate = "unplanned")]
		public List<NosSession> GetUnPlannedSessions() {
			try {
				//Trace.Listeners.Add(new TextWriterTraceListener(@"C:\Webs\NOSSued\OpenSpacePlanner\trace.log"));
				//Trace.WriteLine(DateTime.Now);
				//Trace.Flush();			
				IList<NosSession> plannedSessions = ConvertToListOf<NosSession>(_sessionRepository.GetUnPlannedSessions().ToList());
				return (List<NosSession>)plannedSessions;

			}
			catch (Exception e) {
				//Trace.Listeners.Add(new TextWriterTraceListener(@"C:\Webs\NOSSued\OpenSpacePlanner\trace.log"));
				//Trace.WriteLine(DateTime.Now);
				//Trace.WriteLine(e.ToString());
				//Trace.Flush();
				return null;
			}
		}

		public static IList<T> ConvertToListOf<T>(IList iList) {
			IList<T> result = new List<T>();
			foreach (T value in iList)
				result.Add(value);
			return result;
		}
	}


}