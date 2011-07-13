using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.ApplicationServer.Http;
using OpenSpacePlanner.Contracts;
using OpenSpacePlanner.Domain;

namespace OpenSpacePlanner.Client {
	public class NosClient : INosClient {
		readonly Uri _baseAddress;
		MediaTypeWithQualityHeaderValue _json;

		public NosClient(Uri baseAddress) {
			_baseAddress = baseAddress;
			_json = new MediaTypeWithQualityHeaderValue("application/json");
		}

		public IList<INosSession> GetAllSessions() {
			using(HttpClient httpClient = new HttpClient(_baseAddress)) {
				httpClient.DefaultRequestHeaders.Accept.Add(_json);
				HttpResponseMessage response = httpClient.Get("sessions");
				List<NosSession> sessions = response.Content.ReadAs<List<NosSession>>(new List<MediaTypeFormatter>() {new JsonMediaTypeFormatter()});
				return new List<INosSession>(sessions);
			}
		}
		public INosSession UpdateSession(INosSession nosSession) {
			using(HttpClient httpClient = new HttpClient(_baseAddress)) {
				httpClient.DefaultRequestHeaders.Accept.Add(_json);
				string sessionUri = string.Format("session/{0}", nosSession.Id);

				JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
				byte[] customerBytes = Encoding.UTF8.GetBytes(jsonSerializer.Serialize(nosSession));
				using (MemoryStream stream = new MemoryStream(customerBytes)) {
					StreamContent sessionContent = new StreamContent(stream);
					sessionContent.Headers.ContentType = _json;
					using (HttpResponseMessage response = httpClient.Put(sessionUri, sessionContent)) {
						nosSession = jsonSerializer.Deserialize<NosSession>(response.Content.ReadAsString());
					}
				}
				return nosSession;
			}
		}
	}
}