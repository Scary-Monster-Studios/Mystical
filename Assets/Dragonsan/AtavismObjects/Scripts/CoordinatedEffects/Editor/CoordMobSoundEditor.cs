using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Atavism
{
    [CustomEditor(typeof(CoordMobSoundEffect))]
    public class CoordMobSoundEditor : Editor
    {

      //  private bool effectsLoaded = false;
      //  private bool questsLoaded = false;
      //  private bool tasksLoaded = false;
      // private bool instancesLoaded = false;
      //  string[] interactionTypes;
        bool help = false;
     //   int type = 0;
        public override void OnInspectorGUI()
        {
         //   var indentOffset = EditorGUI.indentLevel * 5f;
            var lineRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(lineRect.x, lineRect.y, EditorGUIUtility.currentViewWidth - 60f, lineRect.height);
            var fieldRect = new Rect(labelRect.xMax, lineRect.y, lineRect.width - labelRect.width - 60f, lineRect.height);
            var buttonRect = new Rect(fieldRect.xMax, lineRect.y, 60f, lineRect.height);
            CoordMobSoundEffect obj = target as CoordMobSoundEffect;

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
            GUILayout.BeginVertical("Sound Event Configuration", boxStyle);
            GUILayout.Space(20);
     /*       EditorGUILayout.LabelField("Object Type");
            content = new GUIContent("Target");
            content.tooltip = "Select Object Type";
            obj.objectType = GUILayout.Toolbar(obj.objectType, new string[] { "Resource Node", "Character" });
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            EditorGUILayout.LabelField("Animation Param Type");
            content = new GUIContent("Target");
            content.tooltip = "Select Animator Param";
            obj.animationType = GUILayout.Toolbar(obj.animationType, new string[] { "Bool", "Float", "Triger", "Int" });
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);*/
         //   if (obj.objectType == 1)
       //     {
                content = new GUIContent("Target");
            content.tooltip = "Select target, you can choose between caster or target values, depending who is invoking this coordinated effect";
            obj.target = (CoordinatedEffectTarget)EditorGUILayout.EnumPopup(content, obj.target);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
         //   }
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

            content = new GUIContent("Destroy When Finished");
            content.tooltip = "Is this object should be destroyed when its duration time + activation delay finish. In case of multiple Coordinated scripts on the object, keep only one Destroy When Finished ticked";
            obj.destroyWhenFinished = EditorGUILayout.Toggle(content, obj.destroyWhenFinished);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            /*      content = new GUIContent("Animation Name");
                  content.tooltip = "Put Name of bool parametr in animation controller";
                  obj.animationName = EditorGUILayout.TextField(content, obj.animationName);
                  if (help)
                      EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                  content = new GUIContent("Animation Length");
                  content.tooltip = "Put Length of bool param will be true";
                  obj.animationLength = EditorGUILayout.FloatField(content, obj.animationLength);
                  if (help)
                      EditorGUILayout.HelpBox(content.tooltip, MessageType.None);*/
            content = new GUIContent("Sound Event");
            content.tooltip = "Select sound event which will be triggered on the selected target. Target entity should have Mob Sound Set component attached";
            obj.soundEvent = (MobSoundEvent)EditorGUILayout.EnumPopup(content, obj.soundEvent);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            GUILayout.EndVertical();
            GUILayout.Space(2);
          /*  GUILayout.BeginVertical("Sound Configuration", boxStyle);
            GUILayout.Space(20);
            content = new GUIContent("Sound Clip");
            content.tooltip = "Put Length of bool param will be true";

            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.soundClip == null)
                obj.soundClip = new System.Collections.Generic.List<AudioClip>();
            for (int i = 0; i < obj.soundClip.Count; i++)
            {
                obj.soundClip[i] = (AudioClip)EditorGUILayout.ObjectField(obj.soundClip[i], typeof(AudioClip), false);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.soundClip.Add(null);
            }
            if (GUILayout.Button("Remove"))
            {
                obj.soundClip.RemoveAt(obj.soundClip.Count - 1);
            }
            EditorGUILayout.EndHorizontal();

            content = new GUIContent("Sound Clip Female");
            content.tooltip = "Put Length of bool param will be true";

            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.soundClipFemale == null)
                obj.soundClipFemale = new System.Collections.Generic.List<AudioClip>();
            for (int i = 0; i < obj.soundClipFemale.Count; i++)
            {
                obj.soundClipFemale[i] = (AudioClip)EditorGUILayout.ObjectField(obj.soundClipFemale[i], typeof(AudioClip), false);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.soundClipFemale.Add(null);
            }
            if (GUILayout.Button("Remove"))
            {
                obj.soundClipFemale.RemoveAt(obj.soundClipFemale.Count - 1);
            }
            EditorGUILayout.EndHorizontal();

#if AT_MASTERAUDIO_PRESET
            GUILayout.BeginVertical("", boxStyle);
            content = new GUIContent("MasterAudio Sound Name");
            content.tooltip = "Put Name Master Audio Sound";
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
                obj.soundClipName.RemoveAt(obj.soundClipName.Count - 1);
            }
            EditorGUILayout.EndHorizontal();

            content = new GUIContent("MasterAudio Sound Name Female");
            content.tooltip = "Put Name Master Audio Sound of Female";
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
                obj.soundClipNameFemale.RemoveAt(obj.soundClipNameFemale.Count - 1);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();
            if(obj.soundClipNameFemale.Count > 0 || obj.soundClipName.Count > 0 || obj.soundClipFemale.Count > 0 || obj.soundClip.Count > 0 ){
#else
            if (obj.soundClipFemale.Count > 0 || obj.soundClip.Count > 0 )
            {
#endif

                content = new GUIContent("Sound Clip 3D");
            content.tooltip = "Sound Clip 3D";
            obj.soundClip3D = EditorGUILayout.Toggle(content, obj.soundClip3D);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Sound Volume");
            content.tooltip = "Put sound Volume";
            obj.soundVolume = EditorGUILayout.FloatField(content, obj.soundVolume);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Sound Delay");
            content.tooltip = "Put Length of sound delay";
            obj.soundDelay = EditorGUILayout.FloatField(content, obj.soundDelay);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Loop Sound");
            content.tooltip = "LoopSound";
            obj.loopSound = EditorGUILayout.Toggle(content, obj.loopSound);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Sound Repeat Time");
            content.tooltip = "Put Length of repeat sound";
            obj.soundRepeatTime = EditorGUILayout.FloatField(content, obj.soundRepeatTime);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
        }
            GUILayout.EndVertical();
            GUILayout.Space(2);
            GUILayout.BeginVertical("Particle Configuration", boxStyle);
            GUILayout.Space(20);
            if (obj.objectType == 1)
            {
                content = new GUIContent("Slot");
                content.tooltip = "Select Slot";
                obj.slot = (AttachmentSocket)EditorGUILayout.EnumPopup(content, obj.slot);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }

            content = new GUIContent("Particle Object");
            content.tooltip = "Put Length of bool param will be true";
         //   EditorGUI.PrefixLabel(labelRect, content);
            obj.particle = (GameObject)EditorGUILayout.ObjectField(content, obj.particle, typeof(GameObject), false);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Particle Delay");
            content.tooltip = "Put sound Volume";
            obj.particleDelay = EditorGUILayout.FloatField(content, obj.particleDelay);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Loop Particle");
            content.tooltip = "Sound Clip 3D";
            obj.loopParticle = EditorGUILayout.Toggle(content, obj.loopParticle);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Particle Repeat Time");
            content.tooltip = "Put sound Volume";
            obj.particleRepeatTime = EditorGUILayout.FloatField(content, obj.particleRepeatTime);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Attach Particle to slot");
            content.tooltip = "Sound Clip 3D";
            obj.attachParticleToSocket = EditorGUILayout.Toggle(content, obj.attachParticleToSocket);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Interrupt When Moving");
            content.tooltip = "Interrupt When Moving";
            obj.interruptWhenMoving = EditorGUILayout.Toggle(content, obj.interruptWhenMoving);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            GUILayout.EndVertical();*/
            GUILayout.EndVertical();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(obj);
                EditorUtility.SetDirty(target);
            }
        }
    }
}