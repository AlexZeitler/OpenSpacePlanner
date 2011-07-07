using System;

namespace OpenSpacePlanner.Contracts {
	public interface INosSession : IEntity {
		string Tag { get; } // nicht Samstag/Sonntag sondern Kürzel...
		DateTime Start { get; }
		DateTime End { get; }
		string Title { get; }
		string Owner { get; }
		string OwnerTag { get; }
		string Description { get; }
		DateTime CreatedOn { get; }
		string Room { get; }
	}
}