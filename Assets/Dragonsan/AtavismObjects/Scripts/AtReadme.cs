using System;
using UnityEngine;

namespace Atavism
{
	public class AtReadme : ScriptableObject
	{
		public Texture2D icon;
		public string title;
		public Section[] sections;
		//public bool loadedLayout;

		[Serializable]
		public class Section
		{
			public string title, text, linkText, url;
		}
	}
}