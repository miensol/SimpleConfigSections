SimpleConfigSections
====================

An attempt to simplfy defining .net ConfigurationSections.
If you don't like repeating yourself in code and ugly string values referencing property names this package is definitely for you.

Installation
---------------------
From Nuget Package Manager Console
	
	Install-Package SimpleConfigSections


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
	
All configuration sections have to be declared in application configuration file

	<?xml version="1.0"?>
	<configuration>
	  <configSections>
		<section type="MyNameSpace.ClassicSection, MyNameSpace" name="ClassicSection"/>
		<section type="MyNameSpace.SimpleSection, MyNameSpace" name="SimpleSection"/>
	  </configSections>
	  
	  <SimpleSection StringValue="YourValue">
		<Child ChildDouble="3.14" />
		<Elements>
			<add ChildDouble="2.8" />
			<add ChildDouble="-2.8" />
		</Elements>
	  </SimpleSection>
	  <!-- Classic configuration section element looks pretty much the same -->
	</configuration>

	
Naming Convention
---------------------
You can change how configuration elements are named by customizing NamingConvention.
	
	Configuration.WithNamingConvention(new MyCustomNamingConvention());
	
	class MyCustomNamingConvention : NamingConvention 
	{
		public override string AddToCollectionElementName(Type collectionElementType, string propertyName)
		{
			return "addToCollection";
		}
	}
	
Validation
---------------------
You can mark your properties with attributes deriving from System.ComponentModel.DataAnnotations.ValidationAttribute to get validation
	
	public interface ISectionWithValidators
    {
        [StringLength(3)]
        string ToLong { get; set; }
        
	    [Required]
        string RequiredButNotPresent { get; set; }
    }

Note that, as with any ConfigurationSection a validation occurs when retreiving it. If your configuration section is not valid the

	 System.Configuration.ConfigurationErrorsException
 
will be thrown when you call

	Configuration.Get<ISectionWithValidators>();