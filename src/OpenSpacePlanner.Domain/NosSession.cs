using System;
using OpenSpacePlanner.Contracts;

namespace OpenSpacePlanner.Domain {
	public class NosSession : Entity, INosSession {
		public virtual string Tag { get; private set; }
		public virtual DateTime Start { get; private set; }
		public virtual DateTime End { get; private set; }
		public virtual string Title { get; private set; }
		public virtual string Owner { get; private set; }
		public virtual string OwnerTag { get; private set; }
		public virtual string Description { get; private set; }
		public virtual DateTime CreatedOn { get; private set; }
		public virtual string Room { get; private set; }
	}
}