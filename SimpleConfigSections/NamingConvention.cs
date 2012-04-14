using System;
using SimpleConfigSections.BasicExtensions;

namespace SimpleConfigSections
{
    public class NamingConvention : INamingConvention
    {
        private static INamingConvention _current = new NamingConvention();

        public static INamingConvention Current
        {
            get { return _current; }
            set
            {
                if(value == null)
                {
                    throw new ArgumentException("Convention must not be null","value");
                }
                
                _current = new CheckForInvalidNamesDecorator(value);
            }
        }

        public string SectionNameByIntefaceType<T>()
        {
            var interfaceType = typeof (T);
            return SectionNameByIntefaceType(interfaceType);
        }

        public virtual string SectionNameByIntefaceType(Type interfaceType)
        {
            return interfaceType.Name.Substring(1);
        }

        public virtual string SectionNameByIntefaceTypeAndPropertyName(Type propertyType, string propertyName)
        {
            return propertyName;
        }

        public virtual string AddToCollectionElementName(Type collectionElementType, string propertyName)
        {
            return "add";
        }

        public virtual string RemoveFromCollectionElementName(Type collectionElementType, string propertyName)
        {
            return "remove";
        }

        public virtual string ClearCollectionElementName(Type collectionElementType, string propertyName)
        {
            return "clear";
        }

        private class CheckForInvalidNamesDecorator : INamingConvention
        {
            private readonly INamingConvention _realConvention;
            private readonly INamingConvention _defaultConvention;

            public CheckForInvalidNamesDecorator(INamingConvention realConvention)
            {
                _realConvention = realConvention;
                _defaultConvention = new NamingConvention();
            }

            public string AddToCollectionElementName(Type collectionElementType, string propertyName)
            {
                return
                    IfEmptyStringThenDefault(
                        conv => conv.AddToCollectionElementName(collectionElementType, propertyName));
            }
            public string RemoveFromCollectionElementName(Type collectionElementType, string propertyName)
            {
                return
                    IfEmptyStringThenDefault(
                        conv => conv.RemoveFromCollectionElementName(collectionElementType, propertyName));
            }

            public string ClearCollectionElementName(Type collectionElementType, string propertyName)
            {
                return
                    IfEmptyStringThenDefault(
                        conv => conv.ClearCollectionElementName(collectionElementType, propertyName));
            }


            public string SectionNameByIntefaceType(Type interfaceType)
            {
                return IfEmptyStringThenDefault(conv => conv.SectionNameByIntefaceType(interfaceType));
            }

            public string SectionNameByIntefaceTypeAndPropertyName(Type propertyType, string propertyName)
            {
                return
                    IfEmptyStringThenDefault(
                        conv => conv.SectionNameByIntefaceTypeAndPropertyName(propertyType, propertyName));
            }

            private string IfEmptyStringThenDefault(Func<INamingConvention, string> convention)
            {
                var propsedName = convention(_realConvention);
                if(propsedName.IsNullOrEmptyOrWhiteSpace())
                {
                    return convention(_defaultConvention);
                }
                return propsedName;
            }
        }
    }
}