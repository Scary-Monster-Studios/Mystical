using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Atavism
{
    // This is the base class for Atavism Plugins Functions
    public class AtavismFunction : ScriptableObject
    {

        public string functionName;
        public string breadCrumb = "";
        public Texture functionIcon;
        public string functionCategory;
        public bool showSave = false;
        public bool showDelete = false;
        public bool showCancel = false;

        public Help help = null;
        private string newItem = "";
        private string newText = "";
        private bool editMode = false;

        // Result text
        public string result = "";
        public float resultTimeout = -1;
        public float resultDuration = 5;


        public virtual void Activate()
        {
        }

        // Draw and control the function parameters
        public virtual void Draw(Rect box)
        {
            GUI.Label(box, "Select a Plugin and Function to Edit");
        }

        public virtual float DrawHelp(Rect box)
        {
            if (help == null)
            {
                help = new Help(functionName);
                help.Load();
            }

            // Setup the layout
            Rect pos = box;
            pos.x += ImagePack.innerMargin;
            pos.y += ImagePack.innerMargin;
            pos.width -= ImagePack.innerMargin;
            pos.height = ImagePack.fieldHeight;
            // Draw the content database info
            pos.y += ImagePack.fieldHeight;
            ImagePack.DrawLabel(pos.x, pos.y, functionName + " Editor Help");
            pos.y += ImagePack.fieldHeight;

            List<HelpEntry> helpInfo = help.Get();
            for (int i = 0; i < helpInfo.Count; i++)
            {
                if (editMode)
                {
                    Rect tempPos = pos;
                    tempPos.width *= 0.2f;
                    helpInfo[i].Item = ImagePack.DrawField(tempPos, helpInfo[i].Item);
                    tempPos.x += tempPos.width;
                    tempPos.width *= 0.8f / 0.2f;
                    helpInfo[i].Text = ImagePack.DrawField(tempPos, helpInfo[i].Text); 

                    Rect rightSide = new Rect(pos.x + pos.width - 20, pos.y, 20, pos.height);
                    if (ImagePack.DrawSmallButton(rightSide, "X"))
                    {
                        helpInfo.Remove(helpInfo[i]);
                        help.Save();
                    }
                    pos.y += ImagePack.fieldHeight;
                }
                else
                {
                    float numLines = ImagePack.DrawMultilineText(pos, "<b>"+helpInfo[i].Item + ":</b> " + helpInfo[i].Text);
                    if (numLines < ImagePack.fieldHeight * 1.75f)
                    {
                        pos.y += numLines+ ImagePack.fieldHeight * 1.25f;
                    }
                    else
                    {
                        pos.y += numLines + ImagePack.fieldHeight; // + ImagePack.fieldHeight;// + (ImagePack.fieldHeight * 0.90f)*numLines/2;
                    }
                }
                //pos.y += ImagePack.fieldHeight;
            }
            pos.y += ImagePack.fieldHeight;

            if (editMode)
            {
                Rect tempPos = pos;
                tempPos.width /= 2;
                if (ImagePack.DrawButton(tempPos, "Save and Exit"))
                {
                    help.Save();
                    editMode = false;
                }
                tempPos.x += tempPos.width;
                if (ImagePack.DrawButton(tempPos, "Exit without Save"))
                {
                    help.Load();
                    editMode = false;
                }
                pos.y += ImagePack.fieldHeight;

                ImagePack.DrawLabel(pos, "New Help line:");
                pos.y += ImagePack.fieldHeight;
                pos.width /= 2;
                newItem = ImagePack.DrawField(pos, "Item:", newItem, 0.6f);
                pos.width *= 2;
                pos.y += ImagePack.fieldHeight;
                newText = ImagePack.DrawField(pos, "Text:", newText, 0.8f, ImagePack.fieldHeight * 3);
                pos.y += ImagePack.fieldHeight * 3;
                if (ImagePack.DrawButton(pos.x, pos.y, "Save Line") && (newItem != ""))
                {
                    help.Add(newItem, newText);
                    help.Save();
                    newItem = "";
                    newText = "";
                }
            }
            else
            {
          /*      if (ImagePack.DrawButton(pos.x, pos.y, "Enter Edit Mode"))
                {
                    editMode = true;
                }*/
            }
            showCancel = false;
            showDelete = false;
            showSave = false;
            return pos.y - box.y + ImagePack.fieldHeight;
            //return helpInfo.Count*ImagePack.fieldHeight;		
        }
        public virtual void save()
        {
        }
        public virtual void delete()
        {
        }
        public virtual void cancel()
        {
        }
    }
}