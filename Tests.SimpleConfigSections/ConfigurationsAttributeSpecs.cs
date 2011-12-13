using System.Configuration;
using Machine.Specifications;
using SimpleConfigSections;
using Configuration = SimpleConfigSections.Configuration;

namespace Tests.SimpleConfigSections
{
    public class when_readingc_configuration_with_attribute_that_is_not_Present
    {
        private It should_throw_configuration_error_exception =
            () => Catch.Exception(() => Configuration.Get<ISectionWithRequiredAttribute>())
                      .ShouldBeOfType(typeof (ConfigurationErrorsException));
    }

    public interface ISectionWithRequiredAttribute
    {
    }

    public class SectionWithRequiredAttribute : ConfigurationSection<ISectionWithRequiredAttribute>
    {
    }
}