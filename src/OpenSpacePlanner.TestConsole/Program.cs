using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using OpenSpacePlanner.Client;
using OpenSpacePlanner.Contracts;

namespace OpenSpacePlanner.TestConsole {
	class Program {
		static void Main(string[] args) {
			NosClient nosClient = new NosClient(new Uri(ConfigurationManager.AppSettings["baseAddress"]));
			IList<INosSession> unplannedSessions = nosClient.GetUnplannedSessions();
			Console.WriteLine(unplannedSessions.Count.ToString());
			Console.ReadLine();

			INosSession session = unplannedSessions[0];
			session.Room = "Raum 3";
			nosClient.UpdateSession(session);

			IList<INosSession> plannedSessions = nosClient.GetPlannedSessions();
			Console.WriteLine(unplannedSessions[0].Room);
			Console.ReadLine();
		}
	}
}
