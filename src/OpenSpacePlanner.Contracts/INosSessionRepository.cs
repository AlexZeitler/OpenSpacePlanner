using System;
using System.Collections.Generic;

namespace OpenSpacePlanner.Contracts {
	public interface INosSessionRepository {
		void Insert(INosSession nosSession);
		IEnumerable<INosSession> Get();
		INosSession Get(Guid id);
		void Update(INosSession nosSession);
		IList<INosSession> GetPlannedSessions();
	}
}