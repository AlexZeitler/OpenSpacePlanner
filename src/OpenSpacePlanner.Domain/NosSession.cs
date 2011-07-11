using System;
using OpenSpacePlanner.Contracts;

namespace OpenSpacePlanner.Domain {
	public class NosSession : TaggedEntity, INosSession {
		public virtual DateTime Start { get; set; }
		public virtual DateTime End { get; set; }
		public virtual string Title { get; set; }
		public virtual string Owner { get; set; }
		public virtual string OwnerTag { get; set; }
		public virtual string Description { get; set; }
		public virtual DateTime CreatedOn { get; set; }
		public virtual string Room { get; set; }
	}
}