namespace SimpleConfigSections
{
    public class Configuration
    {
        public static TInterface Get<TInterface>() where TInterface : class
        {
            return new ConfigurationSource().Get<TInterface>();
        }
    }
}