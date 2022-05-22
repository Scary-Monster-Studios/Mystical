using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace Atavism
{
      [CustomEditor(typeof(CoordParticleEffect))]
    public class CoordParticleEditor : Editor
    {

        public int[] genderIds = new int[] {-1};
        public string[] genderOptions = new string[] {"~ none ~"};
        bool help = false;
        GUIContent[] slots;
        private bool loaded = false;
        
        public override void OnInspectorGUI()
        {
          //  var indentOffset = EditorGUI.indentLevel * 5f;
            var lineRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(lineRect.x, lineRect.y, EditorGUIUtility.currentViewWidth - 60f, lineRect.height);
            var fieldRect = new Rect(labelRect.xMax, lineRect.y, lineRect.width - labelRect.width - 60f, lineRect.height);
            var buttonRect = new Rect(fieldRect.xMax, lineRect.y, 60f, lineRect.height);
            CoordParticleEffect obj = target as CoordParticleEffect;

            GUIContent content = new GUIContent("Help");
            content.tooltip = "Click to show or hide help information";
            if (GUI.Button(buttonRect, content, EditorStyles.miniButton))
                help = !help;
            GUIStyle topStyle = new GUIStyle(GUI.skin.box);
            topStyle.normal.textColor = Color.white;
            topStyle.fontStyle = FontStyle.Bold;
            topStyle.alignment = TextAnchor.UpperLeft;
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.textColor = Color.cyan;
            boxStyle.fontStyle = FontStyle.Bold;
            boxStyle.alignment = TextAnchor.UpperLeft;
            GUILayout.BeginVertical("Atavism CoordEffect Configuration", topStyle);
            GUILayout.Space(20);
            GUILayout.BeginVertical("Particle Configuration", boxStyle);
            GUILayout.Space(20);
            if (!loaded)
            {
                // ServerOptionChoices.LoadAtavismChoiceOptions("Race", false, out raceIds, out raceOptions, true);
                // ServerOptionChoices.LoadAtavismChoiceOptions("Class", false, out classIds, out classOptions, true);
                ServerOptionChoices.LoadAtavismChoiceOptions("Gender", false, out genderIds, out genderOptions, true);
                loaded = true;
            }
            if(slots==null)
                slots =  ServerItems.LoadSlotsOptions(true);
            
              content = new GUIContent("Activation Delay");
            content.tooltip = "Define activation delay (in seconds)";
            obj.activationDelay = EditorGUILayout.FloatField(content, obj.activationDelay);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Duration");
            content.tooltip = "Define duration (in seconds)";
            obj.duration = EditorGUILayout.FloatField(content, obj.duration);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Ignore Override Length");
            content.tooltip = "Ignore override length  and duration sent from server";
            obj.ignoreOverrideLength = EditorGUILayout.Toggle(content, obj.ignoreOverrideLength);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            content = new GUIContent("Destroy When Finished");
            content.tooltip = "Is this object should be destroyed when its duration time + activation delay finish. In case of multiple Coordinated scripts on the object, keep only one Destroy When Finished ticked";
            obj.destroyWhenFinished = EditorGUILayout.Toggle(content, obj.destroyWhenFinished);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Interruption Can Terminate CoordEffect");
            content.tooltip = "Interruption of the ability should stop this effect and destroy the object";
            obj.interruptCanTerminateCoordEffect = EditorGUILayout.Toggle(content, obj.interruptCanTerminateCoordEffect);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Target");
            content.tooltip = "Select target, you can choose between caster or target values, depending who is invoking this coordinated effect";
            obj.target = (CoordinatedEffectTarget)EditorGUILayout.EnumPopup(content, obj.target);
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                
            content = new GUIContent("Target Slot");
            content.tooltip = "Select target slot in which particle will be instantiated. Slots can be defined for particular model in the AtavismMobAppearance component on the character prefab";
            int ii=0;
            int j = 0;
            
                foreach (GUIContent c in slots)
                {
                    if (c.text.Equals(obj.slot))
                        ii = j;
                    j++;
                }

                ii = EditorGUILayout.Popup(content, ii, slots);
                obj.slot = slots[ii].text;
                // obj.slot = (AttachmentSocket)EditorGUILayout.EnumPopup(content, obj.slot);
           

            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Particle Object");
            content.tooltip = "Define object effect which will be spawned in the defined slot";
          //  obj.particleObject = (GameObject)EditorGUILayout.ObjectField(content, obj.particleObject, typeof(GameObject), false);
            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            if (obj.particleObject != null)
            {
                obj.particleObjectList.Add(obj.particleObject);
                obj.particleObject = null;
            }

            for (int i = 0; i < obj.particleObjectList.Count; i++)
            {
                obj.particleObjectList[i] = (GameObject)EditorGUILayout.ObjectField(obj.particleObjectList[i], typeof(GameObject), false);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.particleObjectList.Add(null);
            }
            if (GUILayout.Button("Remove"))
            {
                if (obj.particleObjectList.Count > 0)
                    obj.particleObjectList.RemoveAt(obj.particleObjectList.Count - 1);
            }
            EditorGUILayout.EndHorizontal();



            content = new GUIContent("Particle");
            content.tooltip = "Define particle object which will be spawned in the defined slot";
          //  obj.particle = (ParticleSystem)EditorGUILayout.ObjectField(content, obj.particle, typeof(ParticleSystem), false);
            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            if (obj.particle != null)
            {
                obj.particleList.Add(obj.particle);
                obj.particle = null;
            }

            for (int i = 0; i < obj.particleList.Count; i++)
            {
                obj.particleList[i] = (ParticleSystem)EditorGUILayout.ObjectField(obj.particleList[i], typeof(ParticleSystem), false);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.particleList.Add(null);
            }
            if (GUILayout.Button("Remove"))
            {
                if (obj.particleList.Count > 0)
                    obj.particleList.RemoveAt(obj.particleList.Count - 1);
            }
            EditorGUILayout.EndHorizontal();

            content = new GUIContent("Particle Parent To Slot");
            content.tooltip = "Parent particles or object to defined Slot";
            obj.parentSlot = EditorGUILayout.Toggle(content, obj.parentSlot);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Particle Emiter Radius Resize");
            content.tooltip = "Resize particles emiter to collider size";
            obj.emiterRadiusResize = EditorGUILayout.Toggle(content, obj.emiterRadiusResize);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Particle look At");
            content.tooltip = "Define if particles effect should be rotated to the caster";
            obj.hitLookAt = EditorGUILayout.Toggle(content, obj.hitLookAt);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            GUILayout.EndVertical();
            GUILayout.Space(2);
            GUILayout.BeginVertical("Sound Configuration", boxStyle);
            GUILayout.Space(20);

   GUILayout.Space(3);
            for (int g = 0; g < obj.genderIds.Count; g++)
            {
                GUI.backgroundColor = Color.blue;
                GUILayout.BeginVertical("", topStyle);
                GUI.backgroundColor = Color.white;

                content = new GUIContent("Gender");
                content.tooltip = "Select Gender";
                int selected = GetOptionPosition(obj.genderIds[g], genderIds);
                selected = EditorGUILayout.Popup(content, selected, genderOptions);
                obj.genderIds[g] = genderIds[selected];
                
                content = new GUIContent("Sound Clip");
                content.tooltip = "Define sound clips for the characters";
                EditorGUILayout.LabelField(content);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                if (obj.genderClips[g].soundClip == null)
                    obj.genderClips[g].soundClip = new System.Collections.Generic.List<AudioClip>();
                for (int i = 0; i < obj.genderClips[g].soundClip.Count; i++)
                {
                    obj.genderClips[g].soundClip[i] = (AudioClip)EditorGUILayout.ObjectField(obj.genderClips[g].soundClip[i], typeof(AudioClip), false);
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Clip"))
                {
                    obj.genderClips[g].soundClip.Add(null);
                }
                if (GUILayout.Button("Remove Clip"))
                {
                    if (obj.genderClips[g].soundClip.Count > 0)
                        obj.genderClips[g].soundClip.RemoveAt(obj.genderClips[g].soundClip.Count - 1);
                }
                EditorGUILayout.EndHorizontal();
                
#if AT_MASTERAUDIO_PRESET
            GUILayout.Space(2);
            GUILayout.BeginVertical("", boxStyle);
            content = new GUIContent("MasterAudio Sound Name");
            content.tooltip = "Define Master Audio clips names for the male characters";
            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.genderClips[g].soundClipName == null)
                obj.genderClips[g].soundClipName = new System.Collections.Generic.List<string>();
            for (int i = 0; i < obj.genderClips[g].soundClipName.Count; i++)
            {
                obj.genderClips[g].soundClipName[i] = EditorGUILayout.TextField(obj.genderClips[g].soundClipName[i]);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Clip Name"))
            {
                obj.genderClips[g].soundClipName.Add(null);
            }
            if (GUILayout.Button("Remove Clip Name"))
            {
              if (obj.genderClips[g].soundClipName.Count > 0)
                obj.genderClips[g].soundClipName.RemoveAt(obj.genderClips[g].soundClipName.Count - 1);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();
#endif
                
                
                
                
                GUILayout.Space(2);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Remove Gender"))
                {
                    if (obj.genderIds.Count > 0)
                        obj.genderIds.RemoveAt(g);
                    if (obj.genderClips.Count > 0)
                        obj.genderClips.RemoveAt(g);
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(2);
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Gender"))
            {
                obj.genderIds.Add(-1);
                obj.genderClips.Add(new GenderClips());
            }
            EditorGUILayout.EndHorizontal();
            
            GUI.backgroundColor = Color.red;
            GUILayout.BeginVertical("Obsolete configuration", boxStyle);
            GUI.backgroundColor = Color.white;
            GUILayout.Space(20);
           content = new GUIContent("Sound Clip Male");
            content.tooltip = "Define sound clips for the male characters";
            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.soundClip == null)
                obj.soundClip = new System.Collections.Generic.List<AudioClip>();
            for (int i = 0; i < obj.soundClip.Count; i++)
            {
                obj.soundClip[i] = (AudioClip)EditorGUILayout.ObjectField(obj.soundClip[i], typeof(AudioClip), false);
            }
         /*   EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.soundClip.Add(null);
            }
            if (GUILayout.Button("Remove"))
            {
                if (obj.soundClip.Count > 0)
                    obj.soundClip.RemoveAt(obj.soundClip.Count - 1);
            }
            EditorGUILayout.EndHorizontal();*/

            content = new GUIContent("Sound Clip Female");
            content.tooltip = "Define sound clips for the female characters";
            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.soundClipFemale == null)
                obj.soundClipFemale = new System.Collections.Generic.List<AudioClip>();
            for (int i = 0; i < obj.soundClipFemale.Count; i++)
            {
                obj.soundClipFemale[i] = (AudioClip)EditorGUILayout.ObjectField(obj.soundClipFemale[i], typeof(AudioClip), false);
            }
          /*  EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.soundClipFemale.Add(null);
            }
            if (GUILayout.Button("Remove"))
            {
                if (obj.soundClipFemale.Count > 0)
                    obj.soundClipFemale.RemoveAt(obj.soundClipFemale.Count - 1);
            }
            EditorGUILayout.EndHorizontal();*/
          EditorGUILayout.EndVertical();
/*
#if AT_MASTERAUDIO_PRESET
            GUILayout.BeginVertical("", boxStyle);
            content = new GUIContent("MasterAudio Sound Name Male");
            content.tooltip = "Define Master Audio clips names for the male characters";
            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.soundClipName == null)
                obj.soundClipName = new System.Collections.Generic.List<string>();
            for (int i = 0; i < obj.soundClipName.Count; i++)
            {
                obj.soundClipName[i] = EditorGUILayout.TextField(obj.soundClipName[i]);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.soundClipName.Add(null);
            }
            if (GUILayout.Button("Remove"))
            {
               if (obj.soundClipName.Count>0)
                 obj.soundClipName.RemoveAt(obj.soundClipName.Count - 1);
            }
            EditorGUILayout.EndHorizontal();

            content = new GUIContent("MasterAudio Sound Name Female");
            content.tooltip = "Define Master Audio clips names for the female characters";
            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.soundClipNameFemale == null)
                obj.soundClipNameFemale = new System.Collections.Generic.List<string>();
            for (int i = 0; i < obj.soundClipNameFemale.Count; i++)
            {
                obj.soundClipNameFemale[i] = EditorGUILayout.TextField(obj.soundClipNameFemale[i]);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.soundClipNameFemale.Add(null);
            }
            if (GUILayout.Button("Remove"))
            {
               if (obj.soundClipNameFemale.Count>0)
                 obj.soundClipNameFemale.RemoveAt(obj.soundClipNameFemale.Count - 1);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();
            if( obj.soundClip.Count > 0 ||  obj.soundClipFemale.Count > 0 ||obj.soundClipNameFemale.Count > 0 || obj.soundClipName.Count > 0  ){
#else*/
            if (/*obj.soundClip.Count > 0 || obj.soundClipFemale.Count > 0*/obj.genderClips.Count > 0)
            {
//#endif

                  content = new GUIContent("Sound Volume");
                content.tooltip = "Define sound volume, where 1 is 100% volume";
                obj.soundVolume = EditorGUILayout.FloatField(content, obj.soundVolume);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            }
            if (/*obj.soundClip.Count > 0 || obj.soundClipFemale.Count > 0*/obj.genderClips.Count > 0)
            {
                content = new GUIContent("Sound Clip 3D");
                content.tooltip = "Define if the sound clip 3D";
                obj.soundClip3D = EditorGUILayout.Toggle(content, obj.soundClip3D);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


                content = new GUIContent("Volume Rolloff Linear");
                content.tooltip = "Volume Rolloff Linear";
                obj.linear = EditorGUILayout.Toggle(content, obj.linear);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                content = new GUIContent("Sound Max Distance");
                content.tooltip = "Define maximum distance for sounds";
                obj.maxDistance = EditorGUILayout.IntField(content, obj.maxDistance);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                content = new GUIContent("Sound Mixer Group Name");
                content.tooltip = "Define mixer group name, which is the group name of the mixer defined in the Login scene -> Scripts";
                obj.mixerGroupName = EditorGUILayout.TextField(content, obj.mixerGroupName);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }


            GUILayout.EndVertical();
            GUILayout.EndVertical();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(obj);
                EditorUtility.SetDirty(target);
            }
        }
          
        public int GetOptionPosition(int id, int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                if (ids[i] == id)
                    return i;
            }

            return 0;
        }
    }
}