using System;
using System.Collections.Generic;

namespace OpenSpacePlanner.Contracts {
	public interface INosClient {
		IList<INosSession> GetAllSessions();
		INosSession UpdateSession(INosSession nosSession);
	}
}