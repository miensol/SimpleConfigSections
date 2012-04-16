using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
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

        private It should_be_able_to_read_property_as_simple_type_as_long_as_there_is_a_converter_from_string =
            () => section.Matcher.ToString().ShouldEqual("123abc456");


        private It should_be_able_to_call_simple_properties =
            () => section.NameAndCount.ShouldEqual("SimpleName42");

        private It should_be_able_to_call_simple_virutal_properties =
            () => section.NameAndCountVirtual.ShouldEqual("SimpleName42");

        private It should_read_child_class_configuration_properly =
            () => section.Child.ShouldNotBeNull();

        private Establish ctx =
            () =>
                {
                    TypeDescriptor.AddAttributes(typeof (Regex),
                                                 new TypeConverterAttribute(typeof (RegexFromStringConverter)));
                };

        private static SimpleConfiguration section;
    }

    internal class RegexFromStringConverter : TypeConverter
    {
        public override bool  CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof (string);
        }

        public override object  ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            return new Regex((string) value);
        }
    }

    public class SimpleConfiguration
    {
        public virtual string Name { get; set; }
        public virtual int Count { get; set; }

        public virtual SimpleConfiguration Child { get; set; }

        public virtual Regex Matcher { get; set; }

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