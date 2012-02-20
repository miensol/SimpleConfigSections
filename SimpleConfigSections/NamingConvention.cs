using System;
using SimpleConfigSections.BasicExtensions;

namespace SimpleConfigSections
{
    public class NamingConvention
    {
        private static NamingConvention _current = new NamingConvention();

        public static NamingConvention Current
        {
            get { return _current; }
            internal set
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

        private class CheckForInvalidNamesDecorator : NamingConvention
        {
            private readonly NamingConvention _realConvention;
            private readonly NamingConvention _defaultConvention;

            public CheckForInvalidNamesDecorator(NamingConvention realConvention)
            {
                _realConvention = realConvention;
                _defaultConvention = new NamingConvention();
            }

            public override string AddToCollectionElementName(Type collectionElementType, string propertyName)
            {
                return
                    IfEmptyStringThenDefault(
                        conv => conv.AddToCollectionElementName(collectionElementType, propertyName));
            }
            public override string RemoveFromCollectionElementName(Type collectionElementType, string propertyName)
            {
                return
                    IfEmptyStringThenDefault(
                        conv => conv.RemoveFromCollectionElementName(collectionElementType, propertyName));
            }

            public override string ClearCollectionElementName(Type collectionElementType, string propertyName)
            {
                return
                    IfEmptyStringThenDefault(
                        conv => conv.ClearCollectionElementName(collectionElementType, propertyName));
            }

            public override string SectionNameByIntefaceType(Type interfaceType)
            {
                return IfEmptyStringThenDefault(conv => conv.SectionNameByIntefaceType(interfaceType));
            }

            private string IfEmptyStringThenDefault(Func<NamingConvention, string> convention)
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