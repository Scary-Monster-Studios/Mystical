using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Atavism
{
    [CustomEditor(typeof(CoordProjectileEffect))]
    public class CoordProjectileEditor : Editor
    {

        bool help = false;
        GUIContent[] slots;
        public override void OnInspectorGUI()
        {
            var lineRect = GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(lineRect.x, lineRect.y, EditorGUIUtility.currentViewWidth - 60f, lineRect.height);
            var fieldRect = new Rect(labelRect.xMax, lineRect.y, lineRect.width - labelRect.width - 60f, lineRect.height);
            var buttonRect = new Rect(fieldRect.xMax, lineRect.y, 60f, lineRect.height);
            CoordProjectileEffect obj = target as CoordProjectileEffect;

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
            GUILayout.BeginVertical("Projectile Configuration", boxStyle);
            GUILayout.Space(20);

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
            content.tooltip = "Select target slot which particle projectile will follow/try to reach. Slots can be defined for particular model in the AtavismMobAppearance component on the character prefab";
            int ii=0;
            int j = 0;
            foreach (GUIContent c in slots)
            {
                if (c.text.Equals( obj.targetSlot))
                    ii = j;
                j++;
            }
            ii = EditorGUILayout.Popup(content,ii,slots);
            obj.targetSlot = slots[ii].text;
         //   obj.targetSlot = (AttachmentSocket)EditorGUILayout.EnumPopup(content, obj.targetSlot);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            content = new GUIContent("Caster Slot");
            content.tooltip = "Select caster slot in which particle projectile will appear, and then move to Target Slot. Slots can be defined for particular model in the AtavismMobAppearance component on the character prefab";
            ii=0;
             j = 0;
            foreach (GUIContent c in slots)
            {
                if (c.text.Equals( obj.casterSlot))
                    ii = j;
                j++;
            }
            ii = EditorGUILayout.Popup(content,ii,slots);
            obj.casterSlot = slots[ii].text;
          //  obj.casterSlot = (AttachmentSocket)EditorGUILayout.EnumPopup(content, obj.casterSlot);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Caster Slot Additional Y");
            content.tooltip = "Define offset in Y axis for the defined projectile";
            obj.casterSlotAdditionalY = EditorGUILayout.FloatField(content, obj.casterSlotAdditionalY);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Repeat Projectile");
            content.tooltip = "Define how many times defined projectile should be instantiatied. Use it in combination with Repeat Delay parameter";
            obj.repeatProjectile = EditorGUILayout.IntField(content, obj.repeatProjectile);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Repeat Delay");
            content.tooltip = "Define repeat delay, only if you are using Repeat Projectile (in seconds). Projectile will be instantiated with destroy time set as the Repeat Delay parameter";
            obj.repeatDelay = EditorGUILayout.FloatField(content, obj.repeatDelay);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Projectile Destroy Delay");
            content.tooltip = "Define destroy delay (in seconds). Projectile after reaching the target, the object will be destroyed with a delay";
            obj.projectileDestroyDelay = EditorGUILayout.FloatField(content, obj.projectileDestroyDelay);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Move object speed");
            content.tooltip = "Define how fast the projectile should be move";
            obj.speed = EditorGUILayout.FloatField(content, obj.speed);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Projectile Weapon Object");
            content.tooltip = "Define if the projectile should be player weapon instead of Projectile Object";
            obj.projectileWeaponObject = EditorGUILayout.Toggle(content, obj.projectileWeaponObject);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            /*
             content = new GUIContent("Trail On Projectaile Weapon");
            content.tooltip = "trailOnProjectaileWeapon";
            obj.trailOnProjectaileWeapon = EditorGUILayout.Toggle(content, obj.trailOnProjectaileWeapon);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            */

            content = new GUIContent("Object Return To Caster");
            content.tooltip = "ObjectReturnToCaster";
            obj.ObjectReturnToCaster = EditorGUILayout.Toggle(content, obj.ObjectReturnToCaster);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            content = new GUIContent("Rotate Object");
            content.tooltip = "RotateObject";
            obj.rotateObject = EditorGUILayout.Toggle(content, obj.rotateObject);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            if (obj.rotateObject)
            {
                content = new GUIContent("Rotate Speed");
                content.tooltip = "Put rotate Speed of the object";
                obj.rotateSpeed = EditorGUILayout.FloatField(content, obj.rotateSpeed);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }
            content = new GUIContent("Hide Weapon");
            content.tooltip = "Hide the Weapon";
            obj.hideWeapon = EditorGUILayout.Toggle(content, obj.hideWeapon);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            if (obj.hideWeapon)
            {
                content = new GUIContent("Hide Time");
                content.tooltip = "Put time of hide object";
                obj.hideTime = EditorGUILayout.FloatField(content, obj.hideTime);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }
          
            content = new GUIContent("Projectile Object");
            content.tooltip = "Assign projectile Object";
            obj.projectileObject = (GameObject)EditorGUILayout.ObjectField( content,obj.projectileObject, typeof(GameObject), false);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Projectile Particle");
            content.tooltip = "Assign projectile Object";
            obj.projectileParticle = (ParticleSystem)EditorGUILayout.ObjectField(content, obj.projectileParticle, typeof(ParticleSystem), false);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            GUILayout.BeginVertical("Projectile Sound Configuration", boxStyle);
            GUILayout.Space(20);

            content = new GUIContent("Sound Clip");
            content.tooltip = "Put sound clip";
            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.projectileSound == null)
                obj.projectileSound = new System.Collections.Generic.List<AudioClip>();
            for (int i = 0; i < obj.projectileSound.Count; i++)
            {
                obj.projectileSound[i] = (AudioClip)EditorGUILayout.ObjectField(obj.projectileSound[i], typeof(AudioClip), false);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.projectileSound.Add(null);
            }
            if (GUILayout.Button("Remove"))
            {
                if (obj.projectileSound.Count > 0)
                    obj.projectileSound.RemoveAt(obj.projectileSound.Count - 1);
            }
            EditorGUILayout.EndHorizontal();

#if AT_MASTERAUDIO_PRESET
            GUILayout.BeginVertical("", boxStyle);
            content = new GUIContent("MasterAudio Sound Name");
            content.tooltip = "Put Name Master Audio Sound";
            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.projectileSoundClipName == null)
                obj.projectileSoundClipName = new System.Collections.Generic.List<string>();
            for (int i = 0; i < obj.projectileSoundClipName.Count; i++)
            {
                obj.projectileSoundClipName[i] = EditorGUILayout.TextField(obj.projectileSoundClipName[i]);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.projectileSoundClipName.Add(null);
            }
            if (GUILayout.Button("Remove"))
            {
               if (obj.projectileSoundClipName.Count>0)
                 obj.projectileSoundClipName.RemoveAt(obj.projectileSoundClipName.Count - 1);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (obj.projectileSound.Count > 0 || obj.projectileSoundClipName.Count > 0)
            {
#else
            if ( obj.projectileSound.Count > 0)
            {
#endif

                content = new GUIContent("Sound Volume");
                content.tooltip = "Define sound volume, where 1 is 100% volume";
                obj.soundVolume = EditorGUILayout.FloatField(content, obj.soundVolume);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            }
            if (obj.projectileSound.Count > 0)
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
            GUILayout.Space(2);
            GUILayout.BeginVertical("Hit Configuration", boxStyle);
            GUILayout.Space(20);

            content = new GUIContent("Hit Slot");
            content.tooltip = "Select Slot";
            ii=0;
            j = 0;
            foreach (GUIContent c in slots)
            {
                if (c.text.Equals( obj.hitSlot))
                    ii = j;
                j++;
            }
            ii = EditorGUILayout.Popup(content,ii,slots);
            obj.hitSlot = slots[ii].text;
            //obj.hitSlot = (AttachmentSocket)EditorGUILayout.EnumPopup(content, obj.hitSlot);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Hit Slot Additional Y");
            content.tooltip = "casterSlotAdditionalY";
            obj.hitSlotAdditionalY = EditorGUILayout.FloatField(content, obj.hitSlotAdditionalY);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Force hit to projectile location");
            content.tooltip = "Force To Hit Slot";
            obj.forceToHitSlot = EditorGUILayout.Toggle(content, obj.forceToHitSlot);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Hit Object");
            content.tooltip = "Assign hit Object";
            obj.hitObject = (GameObject)EditorGUILayout.ObjectField(content, obj.hitObject, typeof(GameObject), false);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Hit Particle");
            content.tooltip = "Assign hit Object";
            obj.hitParticle = (ParticleSystem)EditorGUILayout.ObjectField(content, obj.hitParticle, typeof(ParticleSystem), false);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

            content = new GUIContent("Hit Parent To Slot");
            content.tooltip = "Hit Set Parent";
            obj.hitSetParent = EditorGUILayout.Toggle(content, obj.hitSetParent);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);


            content = new GUIContent("Hit effect Look At Camera");
            content.tooltip = "Hit effect Look At Camera";
            obj.hitLookAtCamera = EditorGUILayout.Toggle(content, obj.hitLookAtCamera);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            GUILayout.BeginVertical("Hit Sound Configuration", boxStyle);
            GUILayout.Space(20);

            content = new GUIContent("Hit Sound Clip");
            content.tooltip = "Put sound clip";
            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.hitSound == null)
                obj.hitSound = new System.Collections.Generic.List<AudioClip>();
            for (int i = 0; i < obj.hitSound.Count; i++)
            {
                obj.hitSound[i] = (AudioClip)EditorGUILayout.ObjectField(obj.hitSound[i], typeof(AudioClip), false);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.hitSound.Add(null);
            }
            if (GUILayout.Button("Remove"))
            {
                if (obj.hitSound.Count>0)
                obj.hitSound.RemoveAt(obj.hitSound.Count - 1);
            }
            EditorGUILayout.EndHorizontal();

#if AT_MASTERAUDIO_PRESET
            GUILayout.BeginVertical("", boxStyle);
            content = new GUIContent("MasterAudio Sound Name");
            content.tooltip = "Put Name Master Audio Sound";
            EditorGUILayout.LabelField(content);
            if (help)
                EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            if (obj.hitSoundClipName == null)
                obj.hitSoundClipName = new System.Collections.Generic.List<string>();
            for (int i = 0; i < obj.hitSoundClipName.Count; i++)
            {
                obj.hitSoundClipName[i] = EditorGUILayout.TextField(obj.hitSoundClipName[i]);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                obj.hitSoundClipName.Add(null);
            }
            if (GUILayout.Button("Remove"))
            {
                            if (obj.hitSoundClipName.Count>0)
                obj.hitSoundClipName.RemoveAt(obj.hitSoundClipName.Count - 1);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();
            if ( obj.hitSound.Count > 0 || obj.hitSoundClipName.Count > 0  ){
#else
            if (obj.hitSound.Count > 0)
            {
#endif

                content = new GUIContent("Sound Volume");
                content.tooltip = "Define sound volume, where 1 is 100% volume";
                obj.hitSoundVolume = EditorGUILayout.FloatField(content, obj.hitSoundVolume);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

           
            }
            if (obj.hitSound.Count > 0)
            {
                content = new GUIContent("Sound Clip 3D");
                content.tooltip = "Define if the sound clip 3D";
                obj.hitSoundClip3D = EditorGUILayout.Toggle(content, obj.hitSoundClip3D);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
                content = new GUIContent("Volume Rolloff Linear");
                content.tooltip = "Volume Rolloff Linear";
                obj.hitlinear = EditorGUILayout.Toggle(content, obj.hitlinear);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                content = new GUIContent("Sound Max Distance");
                content.tooltip = "Define maximum distance for sounds";
                obj.hitMaxDistance = EditorGUILayout.IntField(content, obj.hitMaxDistance);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);

                content = new GUIContent("Sound Mixer Group Name");
                content.tooltip = "Define mixer group name, which is the group name of the mixer defined in the Login scene -> Scripts";
                obj.hitMixerGroupName = EditorGUILayout.TextField(content, obj.hitMixerGroupName);
                if (help)
                    EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
            }

            /*  EditorGUILayout.LabelField("Animation Param Type");
              content = new GUIContent("Target");
              content.tooltip = "Select Animator Param";
              obj.animationType = GUILayout.Toolbar(obj.animationType, new string[] { "Bool", "Float", "Triger", "Int" });
              if (help)
                  EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
              content = new GUIContent("Hit Animation");
              content.tooltip = "Put Hit Animation";

              obj.hitAnimation = EditorGUILayout.TextField(content,obj.hitAnimation);
              if (help)
                  EditorGUILayout.HelpBox(content.tooltip, MessageType.None);
  */

            GUILayout.EndVertical();
            GUILayout.EndVertical();
            GUILayout.EndVertical();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(obj);
                EditorUtility.SetDirty(target);
            }
        }
    }
}