namespace OpenSpacePlanner.Domain {
	public class TaggedEntity : Entity {
		string _tag;

		public virtual string Tag {
			get {
				if (string.IsNullOrEmpty(_tag)) {
					_tag = Id.ToString().Substring(0, 6);
				}
				return _tag;
			}
			set { _tag = value; }
		}
		 
	}
}