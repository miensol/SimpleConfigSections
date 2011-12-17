SimpleConfigSections
====================

An attempt to simplfy defining .net ConfigurationSections.
Below you can find a comparison of classic defintion vs the simplified one.

Simple way of defining ConfigurationSection
---------------------
	public class SimpleSection : ConfigurationSection<ISimpleSection>
    {
    }
	
	public interface ISimpleSection
    {
        string StringValue { get; }
        IChildSection Child {get;}
        IEnumerable<IChildSection> Elements { get; }
    }
	
    public interface IChildSection
    {
        double ChildDouble { get; }
    }

Accessing configuration:
	
	ISimpleSection config = Configuration.Get<ISimpleSection>();
	
Classic way of defining ConfigurationSection
---------------------
	public class ClassicSection : ConfigurationSection
    {
        [ConfigurationProperty("StringValue")]
        public string StringValue
        {
            get { return (string) base["StringValue"]; }
        }
		
        [ConfigurationProperty("Child")]
        public ChildClassicElement Child
        {
            get { return (ChildClassicElement)base["Child"]; }
        }

        [ConfigurationProperty("Elements")]
        public ElementsCollection Elements
        {
            get { return (ElementsCollection) base["Elements"]; }
        }
    }
	
    public class ElementsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ChildClassicElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return Guid.NewGuid();
        }
    }
	
    public class ChildClassicElement : ConfigurationElement
    {
        [ConfigurationProperty("ChildDouble")]
        public double ChildDouble
        {
            get { return (double) base["ChildDouble"]; }
        }
    }
	
Accessing configuration:
	
	ClassicSection config = (ClassicSection)ConfigurationManager.Get("ClassicSection");