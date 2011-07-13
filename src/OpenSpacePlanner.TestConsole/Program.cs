using System;
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
			IList<INosSession> allSessions = nosClient.GetAllSessions();
			Console.WriteLine(allSessions.Count.ToString());
			Console.ReadLine();

			INosSession session = allSessions[0];
			session.Room = "Raum 3";
			nosClient.UpdateSession(session);

			allSessions = nosClient.GetAllSessions();
			Console.WriteLine(allSessions[0].Room);
			Console.ReadLine();
		}
	}
}
