using System.Configuration;
using SimpleConfigSections.BasicExtensions;

namespace SimpleConfigSections
{
    public class ConfigurationSource
    {
        public TInterface Get<TInterface>() where TInterface : class
        {
            return new ConcreteConfiguration<TInterface>(this.GetSection<TInterface>()).ClientValue();
        }

        public ConfigurationSection<TInterface> GetSection<TInterface>()
        {
            var sectionName = new NamingConvention().SectionNameByIntefaceType<TInterface>();
            var section = ConfigurationManager.GetSection(sectionName);
            if (section != null)
            {
                return section as ConfigurationSection<TInterface>;
            }
            throw new ConfigurationErrorsException("There is no section named {0}".ToFormat(sectionName));
        }
    }
}