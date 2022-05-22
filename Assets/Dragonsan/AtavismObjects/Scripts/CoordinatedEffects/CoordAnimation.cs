using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if AT_MASTERAUDIO_PRESET
using DarkTonic.MasterAudio;
#endif
namespace Atavism
{
    public enum CoordAnimationType
    {
        AnimationLegacy = 0,
        AnimationMecalim = 1,
    }
    public enum CoordObjectType
    {
        AnimationLegacy=0,
        AnimationMecalim=1,
        Projectile=2

    }
[Serializable]
    public class GenderClips
    {
        public List<AudioClip> soundClip;
#if AT_MASTERAUDIO_PRESET
        public List<string> soundClipName;
#endif
    }
    
    public class CoordAnimation : CoordinatedEffect
    {
        //Animation
        public int objectType = 2;
        public CoordObjectType coordAnimationType = CoordObjectType.AnimationMecalim;
        public int animationType = 0;
        public string animationName;
        public AnimationClip animationClip;
        public float animationFloatValue = 0;
        public float animationFloatValueAfter = 0;
        public int animationIntValue = 0;
        public int animationIntValueAfter = 0;
        public float animationLength=0;
         //Sounds
        public List<int> genderIds = new List<int>();
        public List<GenderClips> genderClips = new List<GenderClips>();
        [Obsolete] public List<AudioClip>  soundClip;
        [Obsolete] public List<AudioClip>  soundClipFemale;
#if AT_MASTERAUDIO_PRESET
   [Obsolete] public List<string> soundClipName;
   [Obsolete] public List<string> soundClipNameFemale;
    string soundName = "";
#endif
        public bool soundClip3D = true;
        public float soundVolume = 1.0f;
        public float soundDelay = 0f;
        public bool loopSound = false;
        public float soundRepeatTime = 0f;
        public string mixerGroupName = "SFX";
        public int maxDistance = 100;
        public bool linear = true;
        //Particle
        public string slot;
        public float additionalSlotY = 0f;
        public GameObject particle;
        public float particleDelay = 0f;
        public bool loopParticle = false;
        public float particleRepeatTime = 0f;
        public bool attachParticleToSocket = true;
        public bool interruptWhenMoving = false;
        bool running = false;
        float nextSoundLoopTime;
        float nextParticleLoopTime;

        public bool hideWeapons = false;
        public float hideTime = 0f;

        AtavismObjectNode node;
        AtavismObjectNode nodeC;
        public bool lookAtTarget = false;
        ResourceNode targetNode;

        GameObject effectParticle;
        public List<GameObject> particleObjectList = new List<GameObject>();

        GameObject soundObject;
        string propName;
        int abilityID = -1;
        Transform slotTransform = null;
        int castingParam = 0;
        // Use this for initialization
        GameObject go;
        public bool ignoreOverrideLength = false;
        Vector3 loc;
        float radius = 0;
        public bool onMount = false;


        string overrideAnimationName;
        float overrideAnimationExpires;
        string overrideIntAnimationName;
        float overrideIntAnimationExpires;
        int overrideIntAnimationAfter;
        string overrideFloatAnimationName;
        float overrideFloatAnimationExpires;
        float overrideFloatAnimationAfter;

        public bool interruptCanTerminateCoordEffect = true;

        void Start()
        {
            AtavismEventSystem.RegisterEvent("CASTING_CANCELLED", this);
           
        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("CASTING_CANCELLED", this);
            if (node != null && propName != null)
                node.RemovePropertyChangeHandler(propName, CancelPropHandler);
            if (nodeC != null)
                nodeC.RemovePropertyChangeHandler("castingParam", CancelPropHandler);
           
            if (coordAnimationType.Equals(CoordObjectType.AnimationMecalim) && animationType == 0)
            {
                if (objectType == 0)
                {
                    if (targetNode != null)
                        if (targetNode.GetComponent<Animator>() != null)
                        {
                            targetNode.GetComponent<Animator>().SetBool(animationName, false);
                        }
                }else if (objectType == 1)
                {
                    if (go != null)
                        if (go.GetComponent<Animator>() != null)
                            go.GetComponent<Animator>().SetBool(animationName, false);
                }

            }

            if (objectType == 2 && onMount)
            {
                if (overrideAnimationName != null && overrideAnimationName != "")
                {
                    node.MobController.Mount.GetComponentInChildren<Animator>().SetBool(overrideAnimationName, false);
                    overrideAnimationName = "";
                }

                if (overrideIntAnimationName != null && overrideIntAnimationName != "")
                {
                    node.MobController.Mount.GetComponentInChildren<Animator>().SetInteger(overrideIntAnimationName, overrideIntAnimationAfter);
                    overrideIntAnimationName = "";
                }

                if (overrideFloatAnimationName != null && overrideFloatAnimationName != "")
                {
                    node.MobController.Mount.GetComponentInChildren<Animator>().SetFloat(overrideFloatAnimationName, overrideFloatAnimationAfter);
                    overrideFloatAnimationName = "";

                }
            }

#if AT_MASTERAUDIO_PRESET
        if (slotTransform!=null)
           MasterAudio.StopAllSoundsOfTransform(slotTransform);
#endif
          //  CancelCoordEffect();
        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "CASTING_CANCELLED")
            {
                // Debug.LogError("CASTING_CANCELLED " + name+"|"+ eData.eventArgs[0]+"|"+ eData.eventArgs[1]);
                if (abilityID > 0 && int.Parse(eData.eventArgs[0]) != abilityID)
                    return;
                if (OID.fromString(eData.eventArgs[1]) != null && node != null && OID.fromString(eData.eventArgs[1]).ToLong() == node.Oid)
                    CancelCoordEffect();
            }

        
        }

        // Update is called once per frame
        void Update()
        {
            if (activationTime != 0 && Time.time > activationTime && !running)
            {
                Run();
            }

            if (running && loopSound && Time.time > nextSoundLoopTime)
            {
                if (loopSound)
                    PlaySound(soundRepeatTime);
                else
                    PlaySound(duration);
                if (soundRepeatTime > 0)
                    nextSoundLoopTime = Time.time + soundRepeatTime;
            }else if (running && Time.time > nextSoundLoopTime && nextSoundLoopTime > 0)
            {
                if (loopSound)
                    PlaySound(soundRepeatTime);
                else
                    PlaySound(duration);
                if (soundRepeatTime > 0)
                {
                    nextSoundLoopTime = Time.time + soundRepeatTime;
                }
                else
                {
                    nextSoundLoopTime = 0;
                }
            }

            if (running && loopParticle && Time.time > nextParticleLoopTime)
            {
                if (loopParticle)
                    PlayParticle(particleRepeatTime);
                else
                    PlayParticle(duration);
                if (particleRepeatTime > 0)
                    nextParticleLoopTime = Time.time + particleRepeatTime;
            }else if (running && Time.time > nextParticleLoopTime && nextParticleLoopTime > 0)
            {
                if (loopParticle)
                    PlayParticle(particleRepeatTime);
                else
                    PlayParticle(duration);
                if (particleRepeatTime > 0)
                {
                    nextParticleLoopTime = Time.time + particleRepeatTime;
                }
                else
                {
                    nextParticleLoopTime = 0;
                }
            }

            if (running && interruptWhenMoving && node.GameObject.GetComponent<CharacterController>() != null)
            {
                if (node.GameObject.GetComponent<CharacterController>().velocity.magnitude > 0.5f)
                {
                    if(animationType==0)
                    node.GameObject.GetComponent<AtavismMobController>().PlayAnimation("", 0);
                    running = false;
                }
            }

            if (objectType == 2 && onMount)
            {
                if (overrideAnimationName != null && overrideAnimationName != "")
                {
                    if (Time.time > overrideAnimationExpires)
                    {
                        node.MobController.Mount.GetComponentInChildren<Animator>().SetBool(overrideAnimationName, false);
                        overrideAnimationName = "";
                    }
                   
                }

                if (overrideIntAnimationName != null && overrideIntAnimationName != "")
                {
                    if (Time.time > overrideIntAnimationExpires)
                    {
                        node.MobController.Mount.GetComponentInChildren<Animator>().SetInteger(overrideIntAnimationName, overrideIntAnimationAfter);
                        overrideIntAnimationName = "";
                    }
                }

                if (overrideFloatAnimationName != null && overrideFloatAnimationName != "")
                {
                    if (Time.time > overrideFloatAnimationExpires)
                    {
                        node.MobController.Mount.GetComponentInChildren<Animator>().SetFloat(overrideFloatAnimationName, overrideFloatAnimationAfter);
                        overrideFloatAnimationName = "";
                    }
                }
            }


        }

        public override void Execute(Dictionary<string, object> props)
        {
            if (!enabled)
                return;
            base.props = props;
            if (AtavismLogger.logLevel <= LogLevel.Debug)
            {
                AtavismLogger.LogDebugMessage("Executing CoordAnimationEffect with num props: " + props.Count);
                foreach (string prop in props.Keys)
                {
                    AtavismLogger.LogDebugMessage(prop + ":" + props[prop]);
               //     AtavismLogger.LogError(prop + ":" + props[prop]);
                }
        /*        string ss = "";
            foreach (string s in props.Keys)
                ss += s + "=" + props[s]+"\n";
            Debug.LogError("Coordanimation: "+name+" props "+ ss);*/
            }
            if (objectType != 3)
            {

                if (props.ContainsKey("resourceNodeID") && objectType == 1)
                {
                    //  Debug.LogError("CoordAmination " + name + " Object Type Error should be selected Resoutce Node");
                    string ss = "";
                    foreach (string s in props.Keys)
                        ss += s + "=" + props[s] + "\n";
                    //   Debug.LogError("Coordanimation: " + name + " props " + ss);
                    if (destroyWhenFinished && duration != -1)
                    {
                        Destroy(gameObject, duration);
                    }
                    return;
                }
                else
                if (props.ContainsKey("gameObject") && objectType != 1)
                {
                    //   Debug.LogError("CoordAmination " + name + " Object Type Error should be selected InteractiveObject");
                    string ss = "";
                    foreach (string s in props.Keys)
                        ss += s + "=" + props[s] + "\n";
                    // Debug.LogError("Coordanimation: " + name + " props " + ss);
                    if (destroyWhenFinished && duration != -1)
                    {
                        Destroy(gameObject, duration);
                    }
                    return;
                }
                else
                if (props.ContainsKey("targetOID") && props.ContainsKey("sourceOID") && !props.ContainsKey("resourceNodeID")  && !props.ContainsKey("interObjId") && objectType != 2)
                {
                    //  Debug.LogError("CoordAmination " + name + " Object Type Error should be selected Character");
                    string ss = "";
                    foreach (string s in props.Keys)
                        ss += s + "=" + props[s] + "\n";
                    //   Debug.LogError("Coordanimation: " + name + " props " + ss);
                    if (destroyWhenFinished && duration != -1)
                    {
                        Destroy(gameObject, duration);
                    }
                    return;
                }



                if (objectType != 1)
                {

                    nodeC = ClientAPI.WorldManager.GetObjectNode((OID)props["sourceOID"]);
                    if (nodeC != null)
                    {
                        nodeC.RegisterPropertyChangeHandler("castingParam", CancelPropHandler);
                        if (nodeC.Properties.ContainsKey("castingParam"))
                            castingParam = (int)nodeC.Properties["castingParam"];
                    }

                    if (props.ContainsKey("length") && !ignoreOverrideLength)
                    {
                        animationLength = (float)props["length"];
                        // I think we should set that as well
                        duration = animationLength;
                    }

                    if (props.ContainsKey("abilityID"))
                    {
                        abilityID = (int)props["abilityID"];
                    }
                    if (lookAtTarget)
                    {
                        if (props.ContainsKey("resourceNodeID"))
                        {
                            targetNode = Crafting.Instance.GetResourceNode((int) props["resourceNodeID"]);
                                Vector3 relativePos = targetNode.transform.position;
                                Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                                if (nodeC.MobController != null && nodeC.MobController.Mount != null)
                                {
                                    //nodeC.MobController.Mount.transform.rotation = rotation;
                                    relativePos.y = nodeC.MobController.Mount.transform.position.y;
                                    nodeC.MobController.Mount.transform.LookAt(relativePos);
                                }
                                else if (nodeC.GameObject != null)
                                {

                                    //nodeC.GameObject.transform.rotation = rotation;
                                    relativePos.y = nodeC.GameObject.transform.position.y;
                                    nodeC.GameObject.transform.LookAt(relativePos);
                                }

                        }
                        else
                        {
                       //  nodeC = ClientAPI.WorldManager.GetObjectNode((OID)props["sourceOID"]);
                            AtavismObjectNode nodeT = ClientAPI.WorldManager.GetObjectNode((OID) props["targetOID"]);
                            if (nodeT != null && nodeC != null)
                            {
                                Vector3 relativePos = (nodeT.MobController == null || (nodeT.MobController != null && nodeT.MobController.Mount == null))
                                    ? nodeT.GameObject.transform.position
                                    : nodeT.MobController.Mount.transform.position; // - nodeC.GameObject.transform.position;

                                // the second argument, upwards, defaults to Vector3.up
                               // if(relativePos.magnitude!=0f)
                             //   Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                                if (nodeC.MobController != null && nodeC.MobController.Mount != null)
                                {
                                    //nodeC.MobController.Mount.transform.rotation = rotation;
                                    relativePos.y = nodeC.MobController.Mount.transform.position.y;
                                    nodeC.MobController.Mount.transform.LookAt(relativePos);
                                }
                                else if (nodeC.GameObject != null)
                                {

                                    //nodeC.GameObject.transform.rotation = rotation;
                                    relativePos.y = nodeC.GameObject.transform.position.y;
                                    nodeC.GameObject.transform.LookAt(relativePos);
                                }

                                //   nodeC.GameObject.transform.LookAt(nodeT.GameObject.transform);
                            }
                        }
                    }
                }
            }
            else
            {

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

        public void Run()
        {
         

            if ( objectType == 0 && props.ContainsKey("resourceNodeID"))
            {
                targetNode = Crafting.Instance.GetResourceNode((int)props["resourceNodeID"]);
                node = ClientAPI.WorldManager.GetObjectNode((OID)props["sourceOID"]);
                if (targetNode == null)
                {
                    CancelCoordEffect();
                    return;
                }
            }
          else if (objectType == 1 && (props.ContainsKey("gameObject") || props.ContainsKey("interObjId")))
                     {
                         if(props.ContainsKey("gameObject"))
                             go = (GameObject)props["gameObject"];
                         if (props.ContainsKey("interObjId"))
                         {
                             int interObjId = (int)props["interObjId"];
                             var io = InteractiveObjectsManager.Instance.getInteractiveObject(interObjId);
                             if (io != null)
                                 go = io.gameObject;
                         }


                // Play animation
                if (coordAnimationType.Equals(CoordObjectType.AnimationLegacy))
                {

                    if (animationClip != null)
                    {
                        go.GetComponent<Animation>().clip = animationClip;
                        go.GetComponent<Animation>().Play();
                    }
                }
                else if (coordAnimationType.Equals(CoordObjectType.AnimationMecalim))
                {
                    if (animationName != null && animationName != "")
                    {
                        if (animationType == 0)
                        {
                            if (go.GetComponent<Animator>()!=null)
                                go.GetComponent<Animator>().SetBool(animationName, true);
                        }
                        else if (animationType == 1)
                        {
                            if (go.GetComponent<Animator>() != null)
                                go.GetComponent<Animator>().SetFloat(animationName, animationFloatValue);

                        }
                        else if (animationType == 2)
                        {
                            if (go.GetComponent<Animator>() != null)
                                go.GetComponent<Animator>().SetTrigger(animationName);

                        }
                        else if (animationType == 3)
                        {
                            if (go.GetComponent<Animator>() != null)
                                go.GetComponent<Animator>().SetInteger(animationName, animationIntValue);

                        }
                    }

                }



            }
            else if (objectType == 2)
            {
                if (target == CoordinatedEffectTarget.Caster)
                {
                    node = ClientAPI.WorldManager.GetObjectNode((OID)props["sourceOID"]);
                }
                else
                {
                    node = ClientAPI.WorldManager.GetObjectNode((OID)props["targetOID"]);
                }
            }
            if (props.ContainsKey("cancelOnProp"))
            {
                propName = (string)props["cancelOnProp"];
                node.RegisterPropertyChangeHandler(propName, CancelPropHandler);
            }
            if (objectType != 3)
            {

                // Play attack animation
                if (coordAnimationType.Equals(CoordObjectType.AnimationMecalim))
                {
                    if (animationName != null && animationName != "")
                    {

                        if (animationType == 0)
                        {
                            if (objectType == 2 && onMount)
                            {
                                if (node != null && node.MobController != null && node.MobController.Mount != null && node.MobController.Mount.GetComponentInChildren<Animator>() != null)
                                {
                                    node.MobController.Mount.GetComponentInChildren<Animator>().SetBool(animationName, true);
                                    overrideAnimationName = animationName;
                                    overrideAnimationExpires = Time.time + animationLength;
                                }
                            }
                            else
                            {
                                if (targetNode != null)
                                {
                                    if (targetNode.gameObject.GetComponent<Animator>() != null)
                                    {
                                        targetNode.gameObject.GetComponent<Animator>().SetBool(animationName, true);
                                    }
                                    else
                                    {
                                        Animator[] anims = targetNode.gameObject.GetComponentsInChildren<Animator>();
                                        foreach (Animator a in anims)
                                        {
                                            a.SetBool(animationName, true);
                                        }
                                    }
                                }

                                if (node != null && node.GameObject != null && node.GameObject.GetComponent<AtavismMobController>() != null)
                                    node.GameObject.GetComponent<AtavismMobController>().PlayAnimation(animationName, animationLength);
                            }
                        }
                        if (animationType == 1)
                        {
                            if (objectType == 2 && onMount)
                            {
                                if (node != null && node.MobController != null && node.MobController.Mount != null && node.MobController.Mount.GetComponentInChildren<Animator>() != null)
                                {
                                    node.MobController.Mount.GetComponentInChildren<Animator>().SetFloat(animationName, animationFloatValue);
                                    overrideFloatAnimationName = animationName;
                                    overrideFloatAnimationExpires = Time.time + animationLength;
                                    overrideFloatAnimationAfter = animationFloatValueAfter;
                                }
                            }
                            else
                            {
                                if (targetNode != null)
                                {
                                    if (targetNode.gameObject.GetComponent<Animator>() != null)
                                    {
                                        targetNode.gameObject.GetComponent<Animator>().SetFloat(animationName, animationFloatValue);
                                    }
                                    else
                                    {
                                        Animator[] anims = targetNode.gameObject.GetComponentsInChildren<Animator>();
                                        foreach (Animator a in anims)
                                        {
                                            a.SetFloat(animationName, animationFloatValue);
                                        }
                                    }
                                }

                                if (node != null && node.GameObject != null && node.GameObject.GetComponent<AtavismMobController>() != null)
                                    node.GameObject.GetComponent<AtavismMobController>().PlayAnimationFloat(animationName, animationFloatValue, animationLength, animationFloatValueAfter);
                            }
                        }
                        if (animationType == 2)
                        {
                            if (objectType == 2 && onMount)
                            {
                                if (node != null && node.MobController != null && node.MobController.Mount!=null && node.MobController.Mount.GetComponentInChildren<Animator>() != null)
                                    node.MobController.Mount.GetComponentInChildren<Animator>().SetTrigger(animationName);
                            }
                            else
                            {
                                if (targetNode != null)
                                {
                                    if (targetNode.gameObject.GetComponent<Animator>() != null)
                                    {
                                        targetNode.gameObject.GetComponent<Animator>().SetTrigger(animationName);
                                    }
                                    else
                                    {
                                        Animator[] anims = targetNode.gameObject.GetComponentsInChildren<Animator>();
                                        foreach (Animator a in anims)
                                        {
                                            a.SetTrigger(animationName);
                                        }
                                    }
                                }

                                if (node != null && node.GameObject != null && node.GameObject.GetComponent<AtavismMobController>() != null)
                                    node.GameObject.GetComponent<AtavismMobController>().PlayAnimationTrigger(animationName);
                            }
                        }
                        if (animationType == 3)
                        { if (objectType == 2 && onMount)
                            {
                                if (node != null && node.MobController != null && node.MobController.Mount != null && node.MobController.Mount.GetComponentInChildren<Animator>() != null)
                                {
                                    node.MobController.Mount.GetComponentInChildren<Animator>().SetInteger(animationName, animationIntValue);
                                    overrideIntAnimationName = animationName;
                                    overrideIntAnimationExpires = Time.time + animationLength;
                                    overrideIntAnimationAfter = animationIntValueAfter;
                                }
                            }
                            else
                            {
                                if (targetNode != null)
                                {
                                    if (targetNode.gameObject.GetComponent<Animator>() != null)
                                    {
                                        targetNode.gameObject.GetComponent<Animator>().SetInteger(animationName, animationIntValue);
                                    }
                                    else
                                    {
                                        Animator[] anims = targetNode.gameObject.GetComponentsInChildren<Animator>();
                                        foreach (Animator a in anims)
                                        {
                                            a.SetInteger(animationName, animationIntValue);
                                        }
                                    }
                                }

                                if (node != null && node.GameObject != null && node.GameObject.GetComponent<AtavismMobController>() != null)
                                    node.GameObject.GetComponent<AtavismMobController>().PlayAnimationInt(animationName, animationIntValue, animationLength, animationIntValueAfter);
                            }
                        }
                    }
                }
                else if (coordAnimationType.Equals(CoordObjectType.AnimationLegacy))
                {

                    if (animationClip != null)
                    {
                        if (objectType == 2)
                        {
                            if (onMount)
                            {
                                if (node.MobController.Mount != null)
                                {
                                    Animation anim = node.MobController.Mount.GetComponentInChildren<Animation>();
                                    if (anim != null)
                                    {
                                        anim.clip = animationClip;
                                        anim.Play();
                                    }
                                }
                            }
                            else
                            {
                                Animation anim = node.GameObject.GetComponent<Animation>();
                                if (anim == null)
                                    node.GameObject.GetComponentInChildren<Animation>();
                                if (anim != null)
                                {
                                    anim.clip = animationClip;
                                    anim.Play();
                                }
                            }
                        }
                        if (objectType == 1)
                        {
                            go.GetComponent<Animation>().clip = animationClip;
                            go.GetComponent<Animation>().Play();
                        }
                        if (objectType == 0)
                        {
                            if (targetNode != null)
                            {
                                if (targetNode.gameObject.GetComponent<Animation>() != null)
                                {
                                    targetNode.gameObject.GetComponent<Animation>().clip = animationClip;
                                    targetNode.gameObject.GetComponent<Animation>().Play();
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                node = ClientAPI.WorldManager.GetObjectNode((OID)props["sourceOID"]);
                int abilityId = (int)props["abilityID"];
                radius = (float)props["locRadius"];
                loc = new Vector3((float)props["locX"], (float)props["locY"], (float)props["locZ"]);

            }

            if ((/*(soundClip != null && soundClip.Count > 0) || (soundClipFemale.Count > 0 && soundClipFemale!=null)||*/ genderClips.Count>0) && soundDelay == 0)
            {
                if (loopSound)
                    PlaySound(soundRepeatTime);
                else
                    PlaySound(duration);
            }

            if ((particle != null || particleObjectList.Count>0) && particleDelay == 0)
            {
                if (loopParticle)
                    PlayParticle(particleRepeatTime);
                else
                    PlayParticle(duration);
            }

            // Now destroy this object
            if (destroyWhenFinished && duration != -1)
            {
               // if ()

                Destroy(gameObject, duration + soundDelay);
            }
            if (loopSound && (soundRepeatTime > 0 || soundDelay > 0))
            {
                if (soundDelay == 0)
                {
                    nextSoundLoopTime = Time.time + soundRepeatTime;
                }
                else
                {
                    nextSoundLoopTime = Time.time + soundDelay;
                }
            }else if (soundDelay > 0)
            {
                nextSoundLoopTime = Time.time + soundDelay;
            }
            if (loopParticle && (particleRepeatTime > 0 || particleDelay > 0))
            {
                if (particleDelay == 0)
                {
                    nextParticleLoopTime = Time.time + particleRepeatTime;
                }
                else
                {
                    nextParticleLoopTime = Time.time + particleDelay;
                }
            }
            else if (particleDelay > 0)
            {
                nextParticleLoopTime = Time.time + particleDelay;
            }

            if (nodeC != null)
            {
                if (hideWeapons && hideTime > 0)
                {
                    nodeC.GameObject.GetComponent<AtavismMobAppearance>().HideWeapon(hideTime);
                }
            }

            running = true;
        }

        void PlaySound(float duration)
        {
          //  Debug.LogError("PlaySound " + duration);
            int gender = -1;
            if (node != null)
            {
                gender = node.PropertyExists("genderId") ? (int) node.GetProperty("genderId") : -1;
            }
            if (node != null && node.GameObject != null)
            {
                if (node.GameObject.GetComponent<AtavismMobAppearance>() == null) return;
                slotTransform = node.GameObject.GetComponent<AtavismMobAppearance>().GetSocketTransform("Root");
                
            }
            if (objectType == 0)
                slotTransform = targetNode.transform;
            if (objectType == 1)
                slotTransform = go.transform;
         
                if (slotTransform != null)
                {
                    var g = GetGander(gender);
                    if (g != null)
                    {
                        if (g.soundClip != null && g.soundClip.Count > 0)
                        {
                            soundObject = new GameObject();
                            if (objectType != 3)
                            {
                                soundObject.transform.position = slotTransform.position;
                            }
                            else
                            {
                                soundObject.transform.position = loc;
                            }
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


/*
                    if ((soundClip != null && soundClip.Count > 0) || (soundClipFemale != null && soundClipFemale.Count > 0))
                    {
                        soundObject = new GameObject();
                        soundObject.transform.position = slotTransform.position;
                        // soundObject.transform.parent = slotTransform;
                        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
                        if (soundClipFemale != null && soundClipFemale.Count > 0 && node != null && node.Properties.ContainsKey("gender") && node.Properties["gender"].Equals("Female"))
                        {
                            audioSource.clip = soundClipFemale[new System.Random().Next(soundClipFemale.Count - 1)];
                        }
                        else if (soundClip != null && soundClip.Count > 0)
                        {
                            audioSource.clip = soundClip[new System.Random().Next(soundClip.Count - 1)];
                        }
                        if (soundClip3D)
                            audioSource.spatialBlend = 1.0f;
                        audioSource.volume =soundVolume;
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
                if (node!=null && node.Properties.ContainsKey("gender") && node.Properties["gender"].Equals("Female") && soundClipNameFemale != null && soundClipNameFemale.Count > 0)
                {
                    soundName = soundClipNameFemale[new System.Random().Next(soundClipNameFemale.Count - 1)];
                }
                else if (soundClipName != null && soundClipName.Count > 0)
                {
                    soundName = soundClipName[new System.Random().Next(soundClipName.Count - 1)];
                }
                if (!string.IsNullOrEmpty(soundName))
                    MasterAudio.PlaySound3DAtTransform(soundName, slotTransform, soundVolume);
                if (duration > 0) Destroy(this, duration);
#endif
                    */
                }
        /*    }
            else
            {
                if ((soundClip != null && soundClip.Count > 0) || (soundClipFemale != null && soundClipFemale.Count > 0))
                {
                    soundObject = new GameObject();
                    soundObject.transform.position = loc;
                    // soundObject.transform.parent = slotTransform;
                    AudioSource audioSource = soundObject.AddComponent<AudioSource>();
                    if (soundClipFemale != null && soundClipFemale.Count > 0 && node != null && node.Properties.ContainsKey("gender") && node.Properties["gender"].Equals("Female"))
                    {
                        audioSource.clip = soundClipFemale[new System.Random().Next(soundClipFemale.Count - 1)];
                    }
                    else if (soundClip != null && soundClip.Count > 0)
                    {
                        audioSource.clip = soundClip[new System.Random().Next(soundClip.Count - 1)];
                    }
                    if (soundClip3D)
                        audioSource.spatialBlend = 1.0f;
                    audioSource.volume = SoundSystem.SoundEffectVolume * soundVolume;
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
               
               
                if (node!=null && node.Properties.ContainsKey("gender") && node.Properties["gender"].Equals("Female") && soundClipNameFemale != null && soundClipNameFemale.Count > 0)
                {
                    soundName = soundClipNameFemale[new System.Random().Next(soundClipNameFemale.Count - 1)];
                }
                else if (soundClipName != null && soundClipName.Count > 0)
                {
                    soundName = soundClipName[new System.Random().Next(soundClipName.Count - 1)];
                }
                if (!string.IsNullOrEmpty(soundName))
                    MasterAudio.PlaySound3DAtTransform(soundName, slotTransform, soundVolume);
                if (duration > 0) Destroy(this, duration);
#endif
            }*/
        }

        void PlayParticle(float duration)
        {
            if (particle == null && particleObjectList.Count == 0)
            {
                return;
            }

            Vector3 transformSlotY;
            transformSlotY = Vector3.zero;
            transformSlotY.y = additionalSlotY;
            if (node != null && node.GameObject != null)
            {
                if (node.GameObject.GetComponent<AtavismMobAppearance>() == null)
                    return;
                slotTransform = node.GameObject.GetComponent<AtavismMobAppearance>().GetSocketTransform(slot);
            }

            if (objectType == 0)
            {
                slotTransform = targetNode.transform;
            }

            if (objectType == 1)
                slotTransform = go.transform;
            if (objectType != 3)
            {
                if (objectType == 2 && onMount)
                {
                    if (particleObjectList.Count > 0)
                        particle = particleObjectList[new System.Random().Next(particleObjectList.Count - 1)];
                    if (node != null && node.MobController != null && node.MobController.Mount != null)
                        effectParticle = (GameObject) Instantiate(particle, node.MobController.Mount.transform.position + transformSlotY, node.MobController.Mount.transform.rotation);
                    if (attachParticleToSocket)
                        effectParticle.transform.parent = node.MobController.Mount.transform;
                    if (duration > 0)
                        Destroy(effectParticle, duration);
                }
                else if (slotTransform != null)
                {
                    if (particleObjectList.Count > 0)
                         particle = particleObjectList[new System.Random().Next(particleObjectList.Count - 1)];
                    effectParticle = (GameObject) Instantiate(particle, slotTransform.position + transformSlotY, slotTransform.rotation);
                    if (attachParticleToSocket)
                        effectParticle.transform.parent = slotTransform;
                    if (duration > 0)
                        Destroy(effectParticle, duration);
                }
            }
            else
            {
                if (particleObjectList.Count > 0)
                    particle = particleObjectList[new System.Random().Next(particleObjectList.Count - 1)];
                effectParticle = (GameObject) Instantiate(particle, loc + transformSlotY, Quaternion.identity);
                if (duration > 0)
                    Destroy(effectParticle, duration);
            }
        }

        public void CancelPropHandler(object sender, PropertyChangeEventArgs args)
        {
        //       Debug.LogError("CancelPropHandler:"+ args.PropertyName, gameObject);

            if (nodeC != null)
            {
             //          Debug.LogError("CancelPropHandler: node not null ", gameObject);
                if (nodeC.Properties.ContainsKey("castingParam"))
                {
                //               Debug.LogError("CancelPropHandler: has param ", gameObject);
                    if (((int)nodeC.Properties["castingParam"]) == -1 && castingParam != ((int)nodeC.Properties["castingParam"]))
                    {
                    //                   Debug.LogError("CancelPropHandler run "+ nodeC.Properties["castingParam"], gameObject);
                        CancelCoordEffect();
                    }
                }
            }
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
            if(animationType==0 && node !=null)
                node.GameObject.GetComponent<AtavismMobController>().PlayAnimation("", 0);
            if (node != null && hideWeapons && hideTime > 0)
            {
                nodeC.GameObject.GetComponent<AtavismMobAppearance>().HideWeapon(0);
            }
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