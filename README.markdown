﻿SimpleConfigSections
====================

An attempt to simplify defining .NET ConfigurationSections.
If you don't like repeating yourself in code and ugly string values referencing property names this package is definitely for you.

Installation
---------------------
From Nuget Package Manager Console
	
	Install-Package SimpleConfigSections

Simple way of defining ConfigurationSection
---------------------
```c#
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
```

Accessing configuration:
	
```c#
ISimpleSection config = Configuration.Get<ISimpleSection>();
```

You can also use classes for defining configuration section:

```c#
public class Question 
{
	public virtual string Value {get;set;}
	public virtual string Answer {get;set;}
	
	public string AnswerAndValue 
	{
		get { return Answer + Value; }
	}
}
public class QuestionSection : ConfigurationSection<Question> {}

Question questionConfiguration = Configuration.Get<Question>();
```

Note that properties that are to be read from configuration file must be virtual.
	
Classic way of defining ConfigurationSection
---------------------
```c#
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
```

Accessing configuration:
	
```c#
ClassicSection config = (ClassicSection)ConfigurationManager.Get("ClassicSection");
```

All configuration sections have to be declared in application configuration file

```c#
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
```


Naming Convention
---------------------
You can change how configuration elements are named by customizing `NamingConvention`.
	
```c#
Configuration.WithNamingConvention(new MyCustomNamingConvention());

class MyCustomNamingConvention : NamingConvention 
{
	public override string AddToCollectionElementName(Type collectionElementType, string propertyName)
	{
		return "addToCollection";
	}
}
```

Validation
---------------------
You can mark your properties with attributes deriving from `System.ComponentModel.DataAnnotations.ValidationAttribute` to get validation
	
```c#
public interface ISectionWithValidators
{
    [StringLength(3)]
    string ToLong { get; set; }
    
    [Required]
    string RequiredButNotPresent { get; set; }
}
```

Note that, as with any `ConfigurationSection` a validation occurs when retrieving it. If your configuration section is not valid the

```c#
 System.Configuration.ConfigurationErrorsException
```

will be thrown when you call

```c#
Configuration.Get<ISectionWithValidators>();
```