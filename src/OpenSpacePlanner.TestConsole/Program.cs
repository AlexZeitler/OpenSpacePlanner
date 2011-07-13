using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenSpacePlanner.Client;

namespace OpenSpacePlanner.TestConsole {
	class Program {
		static void Main(string[] args) {
			NosClient nosClient = new NosClient();
			Console.WriteLine(nosClient.GetAllSessions().Count.ToString());
			Console.ReadLine();
		}
	}
}
