using System;
using System.Reflection;

namespace SimpleConfigSections
{
    public interface INamingConvention
    {        
        string SectionNameByInterfaceType(Type interfaceType);
        string SectionNameByInterfaceTypeAndPropertyName(Type propertyType, string propertyName);
        string AddToCollectionElementName(Type collectionElementType, string propertyName);
        string RemoveFromCollectionElementName(Type collectionElementType, string propertyName);
        string ClearCollectionElementName(Type collectionElementType, string propertyName);
        string SectionNameByInterfaceOrClassType(Type classOrInterface);
        string SectionNameByClassType(Type classOrInterface);
        string AttributeName(PropertyInfo propertyInfo);
    }
}