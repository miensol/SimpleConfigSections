namespace SimpleConfigSections
{
    public class ConfigurationSection<T> : ConfigurationSectionForInterface
    {
        public ConfigurationSection()
            : base(typeof (T))
        {
        }
    }
}