namespace SimpleConfigSections
{
    internal interface IBaseValueProvider
    {
        object this[string propertyName] { get; }
    }
}