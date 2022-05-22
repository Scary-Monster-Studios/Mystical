using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System;
using System.IO;

namespace Atavism
{
    public class Help
    {
        public HelpEditing helpEditing;
        private string helpFile = "";

        public Help(string file)
        {
            helpFile = Path.Combine(Application.dataPath, "Dragonsan/AtavismEditor/Editor/Help/" + file + ".xml");
        }

        public void Load()
        {
            helpEditing = new HelpEditing();

            if (CheckFile())
            {
                helpEditing = helpEditing.Load(helpFile);
            }
            else
            {
                Save();
            }
        }

        public List<HelpEntry> Get()
        {
            return helpEditing.HelpData;
        }

        public bool CheckFile()
        {
            if (!File.Exists(helpFile))
            {
                FileStream fs = File.Create(helpFile);
                fs.Close();
                return false;
            }

            return true;
        }

        public void Save()
        {
            helpEditing.Save(helpFile);
        }

        public void Add(string item, string text)
        {
            helpEditing.HelpData.Add(new HelpEntry(item, text));
        }
    }

    [XmlRoot("HelpEditing")]
    public class HelpEditing
    {
        [XmlArray("HelpData")]
        [XmlArrayItem("HelpEntry")]
        public List<HelpEntry> HelpData;

        public HelpEditing()
        {
            HelpData = new List<HelpEntry>();
        }

        public void Save(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(HelpEditing));
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(stream, this);
            }
        }

        public HelpEditing Load(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(HelpEditing));
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                return serializer.Deserialize(stream) as HelpEditing;
            }
        }

        //Loads the xml directly from the given string. Useful in combination with www.text.
        public HelpEditing LoadFromText(string text)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(HelpEditing));
            return serializer.Deserialize(new StringReader(text)) as HelpEditing;
        }
    }



    public class HelpEntry
    {
        [XmlAttribute("item")]
        public string Item;

        public string Text;

        public HelpEntry()
        {
            Item = "none";
        }

        public HelpEntry(string item, string text)
        {
            Item = item;
            Text = text;
        }
    }
}