using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if AT_MASTERAUDIO_PRESET
using DarkTonic.MasterAudio;
#endif

namespace Atavism
{
    public class CoordParticleEffect : CoordinatedEffect
    {

        public string slot;
        public GameObject particleObject;
        public List<GameObject> particleObjectList = new List<GameObject>();
        public ParticleSystem particle;
        public List<ParticleSystem> particleList = new List<ParticleSystem>();
        //Sounds
        public List<int> genderIds = new List<int>();
        public List<GenderClips> genderClips = new List<GenderClips>();
        [Obsolete]public List<AudioClip> soundClip;
        [Obsolete]public List<AudioClip> soundClipFemale;
#if AT_MASTERAUDIO_PRESET
        [Obsolete]public List<string> soundClipName;
        [Obsolete]public List<string> soundClipNameFemale;
#endif
        public bool soundClip3D = true;
        public float soundVolume = 1.0f;
        public string mixerGroupName = "SFX";
        public int maxDistance = 100;
        public bool linear = true;
        public bool parentSlot = true;
        public bool highFromColider = false;
        public bool emiterRadiusResize = false;
        public bool hitLookAt = false;
        public bool ignoreOverrideLength = false;
        public bool interruptCanTerminateCoordEffect = true;
        GameObject effectParticle;
        GameObject soundObject;
        GameObject effectObject;
        AtavismObjectNode node;
        Transform slotTransform = null;
        int claimId = -1;
        int claimObjectId = -1;

        void Start()
        {
            AtavismEventSystem.RegisterEvent("CASTING_CANCELLED", this);

        }

        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("CASTING_CANCELLED", this);

        }
        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "CASTING_CANCELLED")
            {
                //  Debug.LogError("CASTING_CANCELLED " + name);
              //  Debug.LogError("CASTING_CANCELLED " + name + "|" + eData.eventArgs[0] + "|" + eData.eventArgs[1]);
                if (OID.fromString(eData.eventArgs[1]) != null && node != null && OID.fromString(eData.eventArgs[1]).ToLong() == node.Oid)
                    CancelCoordEffect();
            }


        }

        // Update is called once per frame
        void Update()
        {
            if (activationTime != 0 && Time.time > activationTime)
            {
                Run();
            }
        }

        public override void Execute(Dictionary<string, object> props)
        {
            if (!enabled)
                return;
            base.props = props;
            AtavismLogger.LogDebugMessage("Executing CoordParticleEffect with num props: " + props.Count);
            /*foreach (string prop in props.Keys) {
                Debug.Log(prop + ":" + props[prop]);
            }*/
            if(props.ContainsKey("claimID"))
                claimId = (int)props["claimID"];
            if(props.ContainsKey("claimObjID"))
                claimObjectId = (int)props["claimObjID"];

            if (props.ContainsKey("length") && !ignoreOverrideLength)
            {
                duration = (float)props["length"];
            }

            if (activationDelay == 0)
            {
                Run();
            }
            else
            {
                activationTime = Time.time + activationDelay;
            }
        }

        void Run()
        {
            activationTime = 0;
          //  GameObject effectParticle = null;
         //   GameObject effectObject;
          
            casterOid = (OID)props["sourceOID"];
            if (props.ContainsKey("targetOID"))
                targetOid = (OID)props["targetOID"];
            else
                targetOid = casterOid;
            if (target == CoordinatedEffectTarget.Caster)
            {
                node = ClientAPI.WorldManager.GetObjectNode(casterOid);
                if (node != null && node.GameObject != null)
                {
                    if (node.GameObject.GetComponent<AtavismMobAppearance>() == null)
                    {
                        Debug.LogWarning("Missing AtavismMobAppearance Component ");
                        return;
                    }
                    slotTransform = node.GameObject.GetComponent<AtavismMobAppearance>().GetSocketTransform(slot);
                }
            }
            else
            {
                // Attach to the target
                node = ClientAPI.WorldManager.GetObjectNode(targetOid);
                if (node != null && node.GameObject != null)
                    slotTransform = node.GameObject.GetComponent<AtavismMobAppearance>().GetSocketTransform(slot);
                if (claimId > 0 && claimObjectId > 0)
                {
                    ClaimObject co = null;
                    if(WorldBuilder.Instance.GetClaim(claimId).claimObjects.ContainsKey(claimObjectId))
                        co = WorldBuilder.Instance.GetClaim(claimId).claimObjects[claimObjectId];
                    if (co != null)
                    {
                        slotTransform = co.gameObject.transform;
                    }
                }

                if (casterOid == targetOid)
                {
                //    return;
                }
            }
            if (slotTransform == null)
                return;
            OID TOid;
            if (target == CoordinatedEffectTarget.Caster)
            {
                TOid = targetOid;
            }
            else
            {
                TOid = casterOid;
            }

            if (particle != null)
            {
                AtavismObjectNode nodeT = ClientAPI.WorldManager.GetObjectNode(TOid);
                Vector3 traSlotY = Vector3.zero;
                if (highFromColider && nodeT != null && node != null)
                    traSlotY.y = node.GameObject.GetComponent<CharacterController>().height;

                effectParticle = (GameObject)Instantiate(particle.gameObject, slotTransform.position + traSlotY, slotTransform.rotation);
                if (parentSlot)
                    effectParticle.transform.SetParent(slotTransform);
                if (nodeT != null && hitLookAt)
                    effectParticle.transform.LookAt(nodeT.GameObject.transform);
                Destroy(effectParticle, duration);
            }
            if (particleList.Count > 0)
            {
                AtavismObjectNode nodeT = ClientAPI.WorldManager.GetObjectNode(TOid);
                Vector3 traSlotY = Vector3.zero;
                if (highFromColider && nodeT != null && node != null)
                    traSlotY.y = node.GameObject.GetComponent<CharacterController>().height;
               ParticleSystem ps =  particleList[new System.Random().Next(particleList.Count - 1)];
                if (ps != null)
                {
                    effectParticle = (GameObject)Instantiate(ps.gameObject, slotTransform.position + traSlotY, slotTransform.rotation);
                    if (parentSlot)
                        effectParticle.transform.SetParent(slotTransform);
                    if (nodeT != null && hitLookAt)
                        effectParticle.transform.LookAt(nodeT.GameObject.transform);
                    Destroy(effectParticle, duration);
                }
            }



            if (particleObject != null)
            {
                AtavismObjectNode nodeT = ClientAPI.WorldManager.GetObjectNode(TOid);
                Vector3 traSlotY = Vector3.zero;


                if (parentSlot && nodeT != null)
                    effectObject = (GameObject)Instantiate(particleObject, slotTransform.position + traSlotY, slotTransform.rotation, slotTransform);
                else
                    effectObject = (GameObject)Instantiate(particleObject, slotTransform.position + traSlotY, slotTransform.rotation);
                if (highFromColider && nodeT != null)
                {
                    traSlotY.y = nodeT.GameObject.GetComponent<CharacterController>().height;
                    if (parentSlot)
                        effectObject.transform.localPosition = effectObject.transform.localPosition + traSlotY;
                    else
                        effectObject.transform.position = nodeT.GameObject.transform.position + traSlotY;
                }
                if (emiterRadiusResize && nodeT != null)
                {
                    if (effectObject.GetComponentInChildren<ParticleSystem>() != null && effectObject.GetComponentInChildren<ParticleSystem>().shape.enabled)
                    {
                        ParticleSystem.ShapeModule shape = effectObject.GetComponentInChildren<ParticleSystem>().shape;
                        shape.radius = nodeT.GameObject.GetComponent<CharacterController>().radius;
                    }
                }

                if (nodeT != null && hitLookAt)
                    effectObject.transform.LookAt(nodeT.GameObject.transform);

                Destroy(effectObject, duration);
            }
            if (particleObjectList.Count > 0)
            {
                AtavismObjectNode nodeT = ClientAPI.WorldManager.GetObjectNode(TOid);
                Vector3 traSlotY = Vector3.zero;
                GameObject go = particleObjectList[new System.Random().Next(particleObjectList.Count - 1)];
                if (go != null)
                {
                    if (parentSlot && nodeT != null)
                        effectObject = (GameObject)Instantiate(go, slotTransform.position + traSlotY, slotTransform.rotation, slotTransform);
                    else
                        effectObject = (GameObject)Instantiate(go, slotTransform.position + traSlotY, slotTransform.rotation);
                    if (highFromColider && nodeT != null)
                    {
                        traSlotY.y = nodeT.GameObject.GetComponent<CharacterController>().height;
                        if (parentSlot)
                            effectObject.transform.localPosition = effectObject.transform.localPosition + traSlotY;
                        else
                            effectObject.transform.position = nodeT.GameObject.transform.position + traSlotY;
                    }
                    if (emiterRadiusResize && nodeT != null)
                    {
                        if (effectObject.GetComponentInChildren<ParticleSystem>() != null && effectObject.GetComponentInChildren<ParticleSystem>().shape.enabled)
                        {
                            ParticleSystem.ShapeModule shape = effectObject.GetComponentInChildren<ParticleSystem>().shape;
                            shape.radius = nodeT.GameObject.GetComponent<CharacterController>().radius;
                        }
                    }

                    if (nodeT != null && hitLookAt)
                        effectObject.transform.LookAt(nodeT.GameObject.transform);

                    Destroy(effectObject, duration);
                }
            }

            if (genderClips.Count > 0)
            {
                int gender = -1;
                if (node != null)
                {
                    gender = node.PropertyExists("genderId") ? (int) node.GetProperty("genderId") : -1;
                }

                var g = GetGander(gender);
                if (g != null)
                {
                    if (g.soundClip != null && g.soundClip.Count > 0)
                    {
                        soundObject = new GameObject();
                        soundObject.transform.position = slotTransform.position;
                        soundObject.transform.parent = slotTransform;
                        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
                        audioSource.clip = g.soundClip[new System.Random().Next(g.soundClip.Count - 1)];
                        if (soundClip3D)
                            audioSource.spatialBlend = 1.0f;
                        audioSource.volume = soundVolume;
                        audioSource.maxDistance = maxDistance;
                        if (linear)
                            audioSource.rolloffMode = AudioRolloffMode.Linear;
                        audioSource.dopplerLevel = 0f;
                        if (AtavismSettings.Instance.masterMixer != null)
                            audioSource.outputAudioMixerGroup = AtavismSettings.Instance.masterMixer.FindMatchingGroups(mixerGroupName)[0];
                        audioSource.Play();
                        if (duration > 0)
                        {
                            //   Debug.LogError("Setup Sound Destroy");
                            Destroy(soundObject, duration);
                        }
                   }
#if AT_MASTERAUDIO_PRESET
                            string soundName = "";
                            if (g.soundClipName != null && g.soundClipName.Count > 0)
                            {
                                soundName = g.soundClipName[new System.Random().Next(g.soundClipName.Count - 1)];
                            }

                            if (!string.IsNullOrEmpty(soundName))
                                MasterAudio.PlaySound3DAtTransform(soundName, slotTransform, soundVolume);
                            if (duration > 0) Destroy(this, duration);
#endif
                }
            }

          /*  if ((soundClip != null && soundClip.Count > 0) || (soundClipFemale != null && soundClipFemale.Count > 0))
            {
                // Play sound clip on caster
                soundObject = new GameObject();
                soundObject.transform.position = slotTransform.position;
                soundObject.transform.parent = slotTransform;
                AudioSource audioSource = soundObject.AddComponent<AudioSource>();
                if (soundClipFemale != null  && node != null && node.Properties.ContainsKey("gender") && node.Properties["gender"].Equals("Female"))
                    audioSource.clip = soundClipFemale[new System.Random().Next(soundClipFemale.Count - 1)];
                else
                    audioSource.clip = soundClip[new System.Random().Next(soundClip.Count - 1)];
                if (soundClip3D)
                    audioSource.spatialBlend = 1.0f;
                audioSource.volume =  soundVolume;
                audioSource.dopplerLevel = 0f;
                if (linear)
                    audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.maxDistance = maxDistance;
                if (AtavismSettings.Instance.masterMixer != null)
                    audioSource.outputAudioMixerGroup = AtavismSettings.Instance.masterMixer.FindMatchingGroups(mixerGroupName)[0];

                audioSource.Play();
                Destroy(soundObject, duration);
            }
#if AT_MASTERAUDIO_PRESET
            node = ClientAPI.WorldManager.GetObjectNode(casterOid);
            if ( node != null && node.Properties.ContainsKey("gender") && node.Properties["gender"].Equals("Female") && (soundClipNameFemale != null && soundClipNameFemale.Count > 0))
            {
                MasterAudio.PlaySound3DAtTransform(soundClipNameFemale[new System.Random().Next(soundClipNameFemale.Count - 1)], slotTransform, soundVolume);//,100f);
            }
            else if (soundClipName != null && soundClipName.Count > 0)
            {
                MasterAudio.PlaySound3DAtTransform(soundClipName[new System.Random().Next(soundClipName.Count - 1)], slotTransform, soundVolume);
            }
#endif
        */
            if (effectParticle != null)
                Destroy(effectParticle, duration);
            if (destroyWhenFinished)
                Destroy(gameObject, duration);
        }
        GenderClips GetGander(int gender)
        {

            int gindex = genderIds.IndexOf(gender);
            if (gindex > -1)
            {
                return genderClips[gindex];
            }

            //Get any Gender Definition
            gindex = genderIds.IndexOf(0);
            if (gindex > -1)
            {
                return genderClips[gindex];
            }

            return null;
        }

        void CancelCoordEffect()
        {
            if (!interruptCanTerminateCoordEffect)
                return;
            //   Debug.LogError("CancelCoordEffect", gameObject);
           // if (animationType == 0 && node != null)
             //   node.GameObject.GetComponent<AtavismMobController>().PlayAnimation("", 0);
            if (effectObject != null)
                DestroyImmediate(effectObject);
            if (soundObject != null)
                DestroyImmediate(soundObject);
            if (effectParticle != null)
                DestroyImmediate(effectParticle);
#if AT_MASTERAUDIO_PRESET
            if (slotTransform != null)
                MasterAudio.StopAllSoundsOfTransform(slotTransform);
#endif
            Destroy(gameObject);
        }
    }
}