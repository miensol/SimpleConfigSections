using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SimpleConfigSections;

namespace Tests.SimpleConfigSections
{
    public class when_reading_child_config_section_that_is_a_collection
    {
        private Because b =
            () => section = Configuration.Get<IContainingCollectionConfigSection>();

        private It should_read_child_section =
            () => section.Children.ShouldNotBeNull();

        private It should_have_proper_number_of_child_elements =
            () => section.Children.ShouldHaveCount(2);

        private It should_order_child_elements_as_they_appear_in_the_configuration_file =
            () =>
                {
                    section.Children.First().IntProperty.ShouldEqual(1);
                    section.Children.Last().IntProperty.ShouldEqual(2);
                };

        private static IContainingCollectionConfigSection section;
    }

    public interface IContainingCollectionConfigSection
    {
        IEnumerable<IDeclareAppConfiguration> Children { get; set; }
    }

    public class ContainingCollectionConfigSection : ConfigurationSection<IContainingCollectionConfigSection>
    {
    }
}