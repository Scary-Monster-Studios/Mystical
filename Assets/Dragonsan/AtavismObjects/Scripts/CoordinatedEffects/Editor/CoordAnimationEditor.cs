using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using Atavism;

namespace Atavism
{

    [CustomEditor(typeof(CoordAnimation))]
    public class CoordAnimationEditor : Editor
    {
        public int[] genderIds = new int[] {-1};
        public string[] genderOptions = new string[] {"~ none ~"};
        bool help = false;
        GUIContent[] slots;
        private bool loaded = false;

        public override void OnInspectorGUI()
        {
            var lineRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(lineRect.x, lineRect.y, EditorGUIUtility.currentViewWidth - 60f, lineRect.height);
            var fieldRect = new Rect(labelRect.xMax, lineRect.y, lineRect.width - labelRect.width - 60f, lineRect.height);
            var buttonRect = new Rect(fieldRect.xMax, lineRect.y, 60f, lineRect.height);
            CoordAnimation obj = target as CoordAnimation;

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
            GUILayout.BeginVertical("Animation Configuration", boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Object Type");
            if (!loaded)
            {
               // ServerOptionChoices.LoadAtavismChoiceOptions("Race", false, out raceIds, out raceOptions, true);
               // ServerOptionChoices.LoadAtavismChoiceOptions("Class", false, out classIds, out classOptions, true);
                ServerOptionChoices.LoadAtavismChoiceOptions("Gender", false, out genderIds, out genderOptions, true);
                loaded = true;
            }

            if(slots==null)
                slots =  ServerItems.LoadSlotsOptions(true);

            content = new GUIContent("Target");
            content.tooltip = "Select object type";
            //   int type = (int)Convert.ChangeType(obj.coordType, obj.coordType.GetTypeCode());
            obj.objectType = GUILayout.Toolbar(obj.objectType, new string[] { "Resource Node", "Interactive Object", "Character","Location" });
            // obj.coordType = (CoordObjectType)type;
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.objectType != 3)
            {

                content = new GUIContent("Animation Type");
                content.tooltip = "Select animation type, where Legacy is for animation clips, and mecanim is for animation controller";
                EditorGUILayout.LabelField("Animation Type");

                int type = (int)Convert.ChangeType(obj.coordAnimationType, obj.coordAnimationType.GetTypeCode());
                type = GUILayout.Toolbar(type, new string[] { "Legacy", "Mecanim" });
                obj.coordAnimationType = (CoordObjectType)type;
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                if (obj.coordAnimationType.Equals(CoordObjectType.AnimationMecalim))
                {
                    EditorGUILayout.LabelField("Animation Parameter Type");
                    content = new GUIContent("Target");
                    content.tooltip = "Select animator parameter, which will be set on the animation controller, and based on that you should define proper transition within your animation controller to play animation";
                    obj.animationType = GUILayout.Toolbar(obj.animationType, new string[] { "Bool", "Float", "Triger", "Int" });
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                }

                if (obj.objectType == 2)
                {
                    content = new GUIContent("Target");
                    content.tooltip = "Select target, you can choose between caster or target values, depending who is invoking this coordinated effect";
                    obj.target = (CoordinatedEffectTarget)EditorGUILayout.EnumPopup(content, obj.target);
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                    
                    content = new GUIContent("On Mount");
                    content.tooltip = "Run animation on mount";
                    obj.onMount = EditorGUILayout.Toggle(content, obj.onMount);
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                }

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

                if (obj.coordAnimationType.Equals(CoordObjectType.AnimationMecalim))
                {
                    content = new GUIContent("Animation Parameter");
                    content.tooltip = "Define name of the parameter in an animation controller which will be set";
                    obj.animationName = EditorGUILayout.TextField(content, obj.animationName);
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                }
                if (obj.coordAnimationType.Equals(CoordObjectType.AnimationLegacy))
                {
                    content = new GUIContent("Animation Clip");
                    content.tooltip = "Define the name of the clip which will be played";
                    obj.animationClip = (AnimationClip)EditorGUILayout.ObjectField(content, obj.animationClip, typeof(AnimationClip), false);
                    //  obj.animationName = EditorGUILayout.TextField(content, obj.animationName);
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                }

                /*     content = new GUIContent("Animation Name");
                     content.tooltip = "Put Name of bool parametr in animation controller";
                     obj.animationName = EditorGUILayout.TextField(content, obj.animationName);
                     if (help)
                         EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
         */
                if (obj.animationType == 0)
                {
                    content = new GUIContent("Animation Length");
                    content.tooltip = "Define length of the parameter during which it will be set to the Animation Value in case of Int or Float Animation Parameter Type or true in case of Bool";
                    obj.animationLength = EditorGUILayout.FloatField(content, obj.animationLength);
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                }
                else if (obj.animationType == 1)
                {
                    content = new GUIContent("Animation Float Value");
                    content.tooltip = "Define a value for the animation Parameter";
                    obj.animationFloatValue = EditorGUILayout.FloatField(content, obj.animationFloatValue);
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                    content = new GUIContent("Animation Float Value After");
                    content.tooltip = "Define a value for the animation Parameter, which will be set after duration time + delay time";
                    obj.animationFloatValueAfter = EditorGUILayout.FloatField(content, obj.animationFloatValueAfter);
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                    content = new GUIContent("Animation Length");
                    content.tooltip = "Define how long the defined parameter should be set to its value, after which it will be set tot he Animation Float Value After value";
                    obj.animationLength = EditorGUILayout.FloatField(content, obj.animationLength);
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                }
                else if (obj.animationType == 3)
                {
                    content = new GUIContent("Animation Int Value");
                    content.tooltip = "Define a value for the animation Parameter";
                    obj.animationIntValue = EditorGUILayout.IntField(content, obj.animationIntValue);
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                    content = new GUIContent("Animation Int Value After");
                    content.tooltip = "Define a value for the animation Parameter, which will be set after duration time + delay time";
                    obj.animationIntValueAfter = EditorGUILayout.IntField(content, obj.animationIntValueAfter);
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                    content = new GUIContent("Animation Length");
                    content.tooltip = "Define how long the defined parameter should be set to its value, after which it will be set tot he Animation Int Value After value";
                    obj.animationLength = EditorGUILayout.FloatField(content, obj.animationLength);
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                }

            }
            else
            {
                GUILayout.Space(5);

                content = new GUIContent("Duration");
                content.tooltip = "Define duration (in seconds)";
                obj.duration = EditorGUILayout.FloatField(content, obj.duration);
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

               

            }
            content = new GUIContent("Caster Look At Target");
            content.tooltip = "Rotate Caster to Target";
            obj.lookAtTarget = EditorGUILayout.Toggle(content, obj.lookAtTarget);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            GUILayout.EndVertical();
            GUILayout.Space(2);
            
            //Sounds
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
                 /* EditorGUILayout.BeginHorizontal();
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
                 /* EditorGUILayout.BeginHorizontal();
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
                  GUILayout.EndVertical();
#if AT_MASTERAUDIO_PRESET
          /*  GUILayout.BeginVertical("", boxStyle);
            content = new GUIContent("MasterAudio Sound Name");
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
              if (obj.soundClipName.Count > 0)
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
               if (obj.soundClipNameFemale.Count > 0)
              obj.soundClipNameFemale.RemoveAt(obj.soundClipNameFemale.Count - 1);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();*/
            if (obj.soundClipNameFemale.Count > 0 || obj.soundClipName.Count > 0 || obj.soundClipFemale.Count > 0 || obj.soundClip.Count > 0)
            {
#else
            if (/*obj.soundClipFemale.Count > 0 || obj.soundClip.Count > 0||*/ obj.genderClips.Count > 0)
            {
#endif
                content = new GUIContent("Sound Volume");
                content.tooltip = "Define sound volume, where 1 is 100% volume";
                obj.soundVolume = EditorGUILayout.FloatField(content, obj.soundVolume);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                content = new GUIContent("Sound Delay");
                content.tooltip = "Define sound delay (in seconds)";
                obj.soundDelay = EditorGUILayout.FloatField(content, obj.soundDelay);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                content = new GUIContent("Loop Sound");
                content.tooltip = "Define if the sound should be looped/repeated";
                obj.loopSound = EditorGUILayout.Toggle(content, obj.loopSound);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                if (obj.loopSound)
                {

                    content = new GUIContent("Sound Repeat Time");
                    content.tooltip = "Define repeat time, only if you are using Loop Sound, then repeat time should be set for the sound lenght (in seconds)";
                    obj.soundRepeatTime = EditorGUILayout.FloatField(content, obj.soundRepeatTime);
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                }
            }
            if (/*obj.soundClipFemale.Count > 0 || obj.soundClip.Count > 0||*/ obj.genderClips.Count > 0)
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
            GUILayout.Space(2);
            GUILayout.BeginVertical("Particle Configuration", boxStyle);
            GUILayout.Space(20);
            if (obj.objectType == 2)
            {
                content = new GUIContent("Slot");
                content.tooltip = "Select slot in which particle object will appear. Slots can be defined for particular model in the AtavismMobAppearance component on the character prefab";
                int ii=0;
                int j = 0;
                foreach (GUIContent c in slots)
                {
                    if (c.text.Equals( obj.slot))
                        ii = j;
                    j++;
                }
                ii = EditorGUILayout.Popup(content,ii,slots);
                obj.slot = slots[ii].text;
              //  obj.slot = (AttachmentSocket)EditorGUILayout.EnumPopup(content, obj.slot);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }
            content = new GUIContent("Additional Y Position");
            content.tooltip = "The value will be added to the Y position when an object particle is instantiated";
            obj.additionalSlotY = EditorGUILayout.FloatField(content, obj.additionalSlotY);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Particle Object");
            content.tooltip = "Define particle object which can be both particle or prefab. This object will be spawned in the defined slot";
            //   EditorGUI.PrefixLabel(labelRect, content);
           /* obj.particle = (GameObject)EditorGUILayout.ObjectField(content, obj.particle, typeof(GameObject), false);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Particle Object");
            content.tooltip = "Define object effect which will be spawned in the defined slot";*/
            //  obj.particleObject = (GameObject)EditorGUILayout.ObjectField(content, obj.particleObject, typeof(GameObject), false);
            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            if (obj.particle != null)
            {
                obj.particleObjectList.Add(obj.particle);
                obj.particle = null;
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
            
            
            content = new GUIContent("Particle Delay");
            content.tooltip = "Define delay for particles or object (in seconds)";
            obj.particleDelay = EditorGUILayout.FloatField(content, obj.particleDelay);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Loop Particle");
            content.tooltip = "Define if the particles or object should be looped/repeated";
            obj.loopParticle = EditorGUILayout.Toggle(content, obj.loopParticle);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Particle Repeat Time");
            content.tooltip = "Define repeat time, only if you are using Loop Particle (in seconds). Particles or object will be instantiated with destroy time set as the Particle Repeat Time parameter";
            obj.particleRepeatTime = EditorGUILayout.FloatField(content, obj.particleRepeatTime);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.objectType != 3)
            {

                content = new GUIContent("Particle Parent To Slot");
                content.tooltip = "Parent particles or object to defined Slot";
                obj.attachParticleToSocket = EditorGUILayout.Toggle(content, obj.attachParticleToSocket);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                if (obj.objectType != 1)
                {
                    content = new GUIContent("Interrupt When Moving");
                    content.tooltip = "Interrupt coordinated effect When character will move";
                    obj.interruptWhenMoving = EditorGUILayout.Toggle(content, obj.interruptWhenMoving);
                    if (help)
                        EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                }
            }
            GUILayout.EndVertical();
            GUILayout.Space(2);
            GUILayout.BeginVertical("Hide Weapon", boxStyle);
            GUILayout.Space(20);
            content = new GUIContent("Hide Weapons");
            content.tooltip = "Hides player weapons, for example hiding weapons when a player is Morphed";
            obj.hideWeapons = EditorGUILayout.Toggle(content, obj.hideWeapons);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Hide Weapons Duration");
            content.tooltip = "Duration of hidden weapons before they show again.";
            obj.hideTime = EditorGUILayout.FloatField(content, obj.hideTime);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
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