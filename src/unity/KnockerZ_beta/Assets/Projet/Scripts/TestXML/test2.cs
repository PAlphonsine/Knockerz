using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("MonsterCollection")]
public class test2
{
	[XmlArray("Monsters"),XmlArrayItem("Monster")]
	public Monster[] Monsters;
	
	public void Save(string path)
	{
		var serializer = new XmlSerializer(typeof(test2));
		using(var stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, this);
		}
	}
	
	public static test2 Load(string path)
	{
		var serializer = new XmlSerializer(typeof(test2));
		using(var stream = new FileStream(path, FileMode.Open))
		{
			return serializer.Deserialize(stream) as test2;
		}
	}
}