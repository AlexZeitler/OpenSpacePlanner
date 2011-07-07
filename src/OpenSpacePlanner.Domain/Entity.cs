using System;

namespace OpenSpacePlanner.Domain {
	public class Entity {
		private Guid _id;

		protected Entity() {
			_id = Guid.NewGuid();
		}

		protected Entity(Guid id) {
			_id = id;
		}

		public virtual Guid Id {
			get { return _id; }
			set { _id = value; }
		}

		public virtual bool Equals(Entity other) {
			if (ReferenceEquals(null, other)) {
				return false;
			}

			if (ReferenceEquals(this, other)) {
				return true;
			}

			return Id.Equals(default(Guid)) ? base.Equals(other) : other.Id.Equals(Id);
		}

		public override bool Equals(object obj) {
			return Equals(obj as Entity);
		}

		public override int GetHashCode() {
			return Id.Equals(default(Guid)) ? base.GetHashCode() : Id.GetHashCode();
		}

		public static bool operator ==(Entity left, Entity right) {
			return Equals(left, right);
		}

		public static bool operator !=(Entity left, Entity right) {
			return !Equals(left, right);
		}
	}
}