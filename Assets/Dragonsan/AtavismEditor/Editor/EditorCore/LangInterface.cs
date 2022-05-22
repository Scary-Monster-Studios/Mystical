using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using System;
using System.IO;
using UnityEditor;

namespace Atavism
{
    public static class Lang
    {
        static LangEditing langEditing;
        private static string langFile = "";
        public static string[] LangOptions = new string[] { "English" };
    
        public static void Load()
        {
            langEditing = new LangEditing();
            string _lang = Language;
            langFile = Path.Combine(Application.dataPath, "Dragonsan/AtavismEditor/Editor/Language/" + _lang + ".xml");
            if (CheckFile(langFile))
            {
                langEditing = langEditing.Load(langFile);
            }
            else
            {
                Save();
            }
        }
      /*  public static string GetLang()
        {
            string _lang = EditorPrefs.GetString("aoLanguage");
            if (string.IsNullOrEmpty(_lang))
            {
                EditorPrefs.SetString("aoLanguage", "EN");
                _lang = "EN";
            }
            return _lang;
        }*/
        public static string GetTranslate(string key)
        {
            
            if (langEditing == null || langEditing.LangData.Count == 0)
            {
               // Debug.LogError("Lang: " + _lang + " " + langFile);
                Load();
            }
            foreach (LangEntry le in langEditing.LangData)
            {
                if (le.Item.Equals(key))
                    return le.Text;
            }
            Add(key, key);
            Save();
            return "No translate for " + key;
        }

        public static List<LangEntry> Get()
        {
            return langEditing.LangData;
        }
        public static string Language
        {
            get
            {
                string _lang = EditorPrefs.GetString("aoLanguage");
                string dirFile = Path.Combine(Application.dataPath, "Dragonsan/AtavismEditor/Editor/Language/");
                string[] files = Directory.GetFiles(dirFile, "*.xml", SearchOption.TopDirectoryOnly);
                LangOptions = new string[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    string _name = files[i].Remove(0, files[i].LastIndexOf("/") + 1);
                    LangOptions[i] = _name.Remove(_name.IndexOf("."));
                }
                if (string.IsNullOrEmpty(_lang))
                {
                    EditorPrefs.SetString("aoLanguage", LangOptions[0]);
                    _lang = LangOptions[0];
                }
                return _lang;
            }
            set 
            {
                if (value != EditorPrefs.GetString("aoLanguage"))
                {
                    EditorPrefs.SetString("aoLanguage", value);
                    Load();
                }
            }
        }

        static bool CheckFile(string langFile)
        {
         //   Debug.Log(langFile);
            if (!File.Exists(langFile))
            {
                FileStream fs = File.Create(langFile);
                fs.Close();
                return false;
            }

            return true;
        }

        public static void Save()
        {
            langEditing.Save(langFile);

        }

        public static void Add(string item, string text)
        {
            langEditing.LangData.Add(new LangEntry(item, text));
        }
    }

    [XmlRoot("LangEditing")]
    public class LangEditing
    {
        [XmlArray("LangData")]
        [XmlArrayItem("LangEntry")]
        public List<LangEntry> LangData;

        public LangEditing()
        {
            LangData = new List<LangEntry>();
        }

        public void Save(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(LangEditing));
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(stream, this);
            }
        }

        public LangEditing Load(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(LangEditing));
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                return serializer.Deserialize(stream) as LangEditing;
            }
        }

        //Loads the xml directly from the given string. Useful in combination with www.text.
        public LangEditing LoadFromText(string text)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(LangEditing));
            return serializer.Deserialize(new StringReader(text)) as LangEditing;
        }
    }



    public class LangEntry
    {
        [XmlAttribute("item")]
        public string Item;

        public string Text;

        public LangEntry()
        {
            Item = "none";
        }

        public LangEntry(string item, string text)
        {
            Item = item;
            Text = text;
        }
    }
}