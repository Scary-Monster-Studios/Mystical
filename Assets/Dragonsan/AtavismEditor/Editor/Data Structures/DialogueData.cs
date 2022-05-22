using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class DialogueActionEntry
    {
        public DialogueActionEntry(string text, string action, int actionID, int itemReq)
        {
            this.text = text;
            this.action = action;
            this.actionID = actionID;
            this.itemReq = itemReq;
        }

        public string text;
        public string action;
        public int actionID = -1;
        public int itemReq = -1;
    }

    public class DialogueData : DataStructure
    {
                                            // Previous dialogue 
        public int previousDialogueID = -1;
        public int previousActionPosition = 0;

        // General Parameters
        public bool openingDialogue = true;
        public bool repeatable = true;
        public int prereqDialogue;
        public int prereqQuest;
        public int prereqFaction;
        public int prereqFactionStance = 1;
        public bool reactionAutoStart;
        public string text = "";


        public int maxEntries = 8;
        public List<DialogueActionEntry> entries = new List<DialogueActionEntry>();

        public DialogueData()
        {
            // Database fields
            fields = new Dictionary<string, string>() {
        {"name", "string"},
        {"openingDialogue", "bool"},
        {"repeatable", "bool"},
        {"prereqDialogue", "int"},
        {"prereqQuest", "int"},
        {"prereqFaction", "int"},
        {"prereqFactionStance", "int"},
        {"reactionAutoStart", "bool"},
        {"text", "string"},
        {"option1text", "string"},
        {"option1action", "string"},
        {"option1actionID", "int"},
        {"option1itemReq", "int"},
        {"option2text", "string"},
        {"option2action", "string"},
        {"option2actionID", "int"},
        {"option2itemReq", "int"},
        {"option3text", "string"},
        {"option3action", "string"},
        {"option3actionID", "int"},
        {"option3itemReq", "int"},
        {"option4text", "string"},
        {"option4action", "string"},
        {"option4actionID", "int"},
        {"option4itemReq", "int"},
        {"option5text", "string"},
        {"option5action", "string"},
        {"option5actionID", "int"},
        {"option5itemReq", "int"},
        {"option6text", "string"},
        {"option6action", "string"},
        {"option6actionID", "int"},
        {"option6itemReq", "int"},
        {"option7text", "string"},
        {"option7action", "string"},
        {"option7actionID", "int"},
        {"option7itemReq", "int"},
        {"option8text", "string"},
        {"option8action", "string"},
        {"option8actionID", "int"},
        {"option8itemReq", "int"},
   };
        }





        public DialogueData Clone()
        {
            return (DialogueData)this.MemberwiseClone();
        }

        public override string GetValue(string fieldKey)
        {
            switch (fieldKey)
            {
                case "name":
                    return Name;
                case "openingDialogue":
                    return openingDialogue.ToString();
                case "repeatable":
                    return repeatable.ToString();
                case "prereqDialogue":
                    return prereqDialogue.ToString();
                case "prereqQuest":
                    return prereqQuest.ToString();
                case "prereqFaction":
                    return prereqFaction.ToString();
                case "prereqFactionStance":
                    return prereqFactionStance.ToString();
                case "reactionAutoStart":
                    return reactionAutoStart.ToString();
                case "text":
                    return text;
                case "option1text":
                    if (entries.Count > 0)
                        return entries[0].text;
                    else
                        return "";
                case "option1action":
                    if (entries.Count > 0)
                        return entries[0].action;
                    else
                        return "";
                case "option1actionID":
                    if (entries.Count > 0)
                        return entries[0].actionID.ToString();
                    else
                        return "0";
                case "option1itemReq":
                    if (entries.Count > 0)
                        return entries[0].itemReq.ToString();
                    else
                        return "0";
                case "option2text":
                    if (entries.Count > 1)
                        return entries[1].text;
                    else
                        return "";
                case "option2action":
                    if (entries.Count > 1)
                        return entries[1].action;
                    else
                        return "";
                case "option2actionID":
                    if (entries.Count > 1)
                        return entries[1].actionID.ToString();
                    else
                        return "0";
                case "option2itemReq":
                    if (entries.Count > 1)
                        return entries[1].itemReq.ToString();
                    else
                        return "0";
                case "option3text":
                    if (entries.Count > 2)
                        return entries[2].text;
                    else
                        return "";
                case "option3action":
                    if (entries.Count > 2)
                        return entries[2].action;
                    else
                        return "";
                case "option3actionID":
                    if (entries.Count > 2)
                        return entries[2].actionID.ToString();
                    else
                        return "0";
                case "option3itemReq":
                    if (entries.Count > 2)
                        return entries[2].itemReq.ToString();
                    else
                        return "0";
                case "option4text":
                    if (entries.Count > 3)
                        return entries[3].text;
                    else
                        return "";
                case "option4action":
                    if (entries.Count > 3)
                        return entries[3].action;
                    else
                        return "";
                case "option4actionID":
                    if (entries.Count > 3)
                        return entries[3].actionID.ToString();
                    else
                        return "0";
                case "option4itemReq":
                    if (entries.Count > 3)
                        return entries[3].itemReq.ToString();
                    else
                        return "0";
                case "option5text":
                    if (entries.Count > 4)
                        return entries[4].text;
                    else
                        return "";
                case "option5action":
                    if (entries.Count > 4)
                        return entries[4].action;
                    else
                        return "";
                case "option5actionID":
                    if (entries.Count > 4)
                        return entries[4].actionID.ToString();
                    else
                        return "0";
                case "option5itemReq":
                    if (entries.Count > 4)
                        return entries[4].itemReq.ToString();
                    else
                        return "0";
                case "option6text":
                    if (entries.Count > 5)
                        return entries[5].text;
                    else
                        return "";
                case "option6action":
                    if (entries.Count > 5)
                        return entries[5].action;
                    else
                        return "";
                case "option6actionID":
                    if (entries.Count > 5)
                        return entries[5].actionID.ToString();
                    else
                        return "0";
                case "option6itemReq":
                    if (entries.Count > 5)
                        return entries[5].itemReq.ToString();
                    else
                        return "0";
                case "option7text":
                    if (entries.Count > 6)
                        return entries[6].text;
                    else
                        return "";
                case "option7action":
                    if (entries.Count > 6)
                        return entries[6].action;
                    else
                        return "";
                case "option7actionID":
                    if (entries.Count > 6)
                        return entries[6].actionID.ToString();
                    else
                        return "0";
                case "option7itemReq":
                    if (entries.Count > 6)
                        return entries[6].itemReq.ToString();
                    else
                        return "0";
                case "option8text":
                    if (entries.Count > 7)
                        return entries[7].text;
                    else
                        return "";
                case "option8action":
                    if (entries.Count > 7)
                        return entries[7].action;
                    else
                        return "";
                case "option8actionID":
                    if (entries.Count > 7)
                        return entries[7].actionID.ToString();
                    else
                        return "0";
                case "option8itemReq":
                    if (entries.Count > 7)
                        return entries[7].itemReq.ToString();
                    else
                        return "0";
            }
            return "";
        }
    }
}