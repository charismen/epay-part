using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Plexform.Base.Accounting.XML
{
    [XmlRoot(ElementName = "Item")]
    public class Item
    {
        [XmlAttribute(AttributeName = "Length")]
        public int Length { get; set; }
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "Value")]
        public string Value { get; set; }
        [XmlAttribute(AttributeName = "Type")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "Items")]
    public class Items
    {
        [XmlElement(ElementName = "Item")]
        public List<Item> Item { get; set; }
    }

    [XmlRoot(ElementName = "header")]
    public class Header
    {
        [XmlElement(ElementName = "Items")]
        public Items Items { get; set; }
    }

    [XmlRoot(ElementName = "section1")]
    public class Section1
    {
        [XmlElement(ElementName = "Items")]
        public Items Items { get; set; }
    }

    [XmlRoot(ElementName = "section2")]
    public class Section2
    {
        [XmlElement(ElementName = "Items")]
        public Items Items { get; set; }
    }

    [XmlRoot(ElementName = "body")]
    public class Bodys
    {
        [XmlElement(ElementName = "section1")]
        public Section1 Section1 { get; set; }
        [XmlElement(ElementName = "section2")]
        public Section2 Section2 { get; set; }
    }

    [XmlRoot(ElementName = "Document")]
    public class Document
    {
        [XmlElement(ElementName = "header")]
        public Header Header { get; set; }
        [XmlElement(ElementName = "body")]
        public Bodys Body { get; set; }
    }

}
