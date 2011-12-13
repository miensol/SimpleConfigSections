using System;
using Machine.Specifications;
using SimpleConfigSections;

namespace Tests.SimpleConfigSections
{
    public class when_declaring_configuration_section_that_has_complex_property
    {
        private Because b =
            () => value = Configuration.Get<ISectionWithComplexProperty>();

        private It should_create_complex_property =
            () => value.ComplexProperty.ShouldNotBeNull();

        private It should_read_complex_property_of_complex_property =
            () => value.ComplexProperty.Simple.ShouldNotBeNull();

        private It should_read_simple_values_of_complex_property =
            () => value.ComplexProperty.UriProperty.AbsoluteUri.ShouldEqual("http://google.pl/");

        private static ISectionWithComplexProperty value;
    }

    public class  SectionWithComplexProperty : ConfigurationSection<ISectionWithComplexProperty>
    {
        
    }

    public interface ISectionWithComplexProperty
    {
        IComplexConfigSection ComplexProperty { get; set; }
    }

    public interface IComplexConfigSection
    {
        IDeclareAppConfiguration Simple { get; set; }
        Uri UriProperty { get; set; }
    }
}