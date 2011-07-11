using Machine.Specifications;

namespace OpenSpacePlanner.Domain.Tests {
	public class Given_a_tagged_entity_when_requesting_its_tag_after_instancing_it {
		static TaggedEntity _taggedEntity;
		Establish context = () => {  };

		Because of = () => { _taggedEntity = new TaggedEntity(); };

		It should_yield_correct_tag = () => { _taggedEntity.Tag.ShouldEqual(_taggedEntity.Id.ToString().Substring(0, 6)); };
	}
}