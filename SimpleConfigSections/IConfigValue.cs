namespace SimpleConfigSections
{
    internal interface IConfigValue
    {
        object Value(string propertyName);
    }
}