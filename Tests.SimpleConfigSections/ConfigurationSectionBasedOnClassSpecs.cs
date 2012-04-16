using Machine.Specifications;
using SimpleConfigSections;

namespace Tests.SimpleConfigSections
{
    public class when_reading_configuration_section_defined_as_class
    {
        private Because b =
            () => section = Configuration.Get<SimpleConfiguration>();

        private It should_return_not_empty_configuration =
            () => section.ShouldNotBeNull();

        private It should_be_able_to_read_properies =
            () =>
                {
                    section.Name.ShouldEqual("SimpleName");
                    section.Count.ShouldEqual(42);
                };

        private It should_be_able_to_call_simple_properties =
            () => section.NameAndCount.ShouldEqual("SimpleName42");

        private It should_be_able_to_call_simple_virutal_properties =
            () => section.NameAndCountVirtual.ShouldEqual("SimpleName42");

        private It should_read_child_class_configuration_properly =
            () => section.Child.ShouldNotBeNull();

        private static SimpleConfiguration section;
    }

    public class SimpleConfiguration
    {
        public virtual string Name { get; set; }
        public virtual int Count { get; set; }

        public virtual SimpleConfiguration Child { get; set; }

        public string NameAndCount
        {
            get { return Name + Count; }
        }

        public virtual string NameAndCountVirtual
        {
            get { return NameAndCount; }
        }
    }

    public class SimpleConfigurationSection : ConfigurationSection<SimpleConfiguration>
    {
    }
}