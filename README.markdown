ďťżSimpleConfigSections
====================

An attempt to simplfy defining .net ConfigurationSections.
Below you can find a comparison of classic defintion vs the simplified one.

Simple way of defining ConfigurationSection
---------------------
	public interface IPerformanceSection
    {
        string StringValue { get; }
        IChildPerformanceSection Child {get;}
        IEnumerable<IChildPerformanceSection> Elements { get; }
    }
	
    public interface IChildPerformanceSection
    {
        double ChildDouble { get; }
    }

    public class PerformanceSection : ConfigurationSection<IPerformanceSection>
    {
    }
	
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
        public ChildPerformanceElement Child
        {
            get { return (ChildPerformanceElement)base["Child"]; }
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
            return new ChildPerformanceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return Guid.NewGuid();
        }
    }
	
    public class ChildPerformanceElement : ConfigurationElement
    {
        [ConfigurationProperty("ChildDouble")]
        public double ChildDouble
        {
            get { return (double) base["ChildDouble"]; }
        }
    }