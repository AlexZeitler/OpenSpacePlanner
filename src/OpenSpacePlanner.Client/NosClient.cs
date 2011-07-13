using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.ApplicationServer.Http;
using OpenSpacePlanner.Contracts;
using OpenSpacePlanner.Domain;

namespace OpenSpacePlanner.Client {
	public class NosClient : INosClient {
		public IList<INosSession> GetAllSessions() {
			using(HttpClient httpClient = new HttpClient("http://localhost:21973/")) {
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				HttpResponseMessage response = httpClient.Get("sessions");
				List<NosSession> sessions = response.Content.ReadAs<List<NosSession>>(new List<MediaTypeFormatter>() {new JsonMediaTypeFormatter()});
				return new List<INosSession>(sessions);
			}
		}
		public void UpdateSession(INosSession nosSession) {
			throw new System.NotImplementedException();
		}
	}
}