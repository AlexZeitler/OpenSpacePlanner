using System;
using System.Collections.Generic;

namespace OpenSpacePlanner.Contracts {
	public interface INosSessionClient {
		IList<INosSession> GetAllSessions();
		IList<IRoom> GetAllRooms();
		void UpdateSession(INosSession nosSession);
	}
}