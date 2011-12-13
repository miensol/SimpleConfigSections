namespace SimpleConfigSections
{
    public class NamingConvention
    {
        public string SectionNameByIntefaceType<T>()
        {
            return typeof(T).Name.Substring(1);
        }
    }
}