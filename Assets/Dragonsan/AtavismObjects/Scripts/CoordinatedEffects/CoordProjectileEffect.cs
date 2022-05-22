using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if AT_MASTERAUDIO_PRESET
using DarkTonic.MasterAudio;
#endif

namespace Atavism
{
    enum ProjectileState
    {
        Setup,
        Moving,
        Hit
    }
    class ProjectileObject
    {
        public Transform targetTransform;
        public GameObject projectile;
        public bool returnObject = false;

    }

    public class CoordProjectileEffect : CoordinatedEffect
    {
        public int repeatProjectile = 1;
        public float projectileDestroyDelay = 0f;
        public float repeatDelay = 0f;

        public string casterSlot;
        public float casterSlotAdditionalY = 0f;
        public bool projectileWeaponObject = false;
        public bool trailOnProjectaileWeapon = false;
        public bool ObjectReturnToCaster = false;
        public bool rotateObject = false;
        public float rotateSpeed = 20f;
        public bool hideWeapon = false;
        public float hideTime = 0f;

        public GameObject projectileObject;
        public ParticleSystem projectileParticle;
        public List<AudioClip> projectileSound;

        public List<string> projectileSoundClipName;

        public bool soundClip3D = true;
        public float soundVolume = 1.0f;
        public float speed = 15; // metres per second
        public string mixerGroupName = "SFX";
        public int maxDistance = 100;
        public bool linear = true;
        public string targetSlot;
        public string hitSlot;
        public float hitSlotAdditionalY = 0f;
        public bool forceToHitSlot = false;
        public GameObject hitObject;

        public ParticleSystem hitParticle;
        public List<AudioClip> hitSound;

        public List<string> hitSoundClipName;

        public string hitAnimation;
        public int animationType = 0;
        public bool hitSoundClip3D = true;
        public float hitSoundVolume = 1.0f;
        public string hitMixerGroupName = "SFX";
        public int hitMaxDistance = 100;
        public bool hitlinear = true;
        public bool hitSetParent = false;
        public bool hitLookAtCamera = false;

        Transform targetTransform;
        Vector3 positionAdd = Vector3.zero;

        ProjectileState state = ProjectileState.Setup;
        GameObject projectile;
        private List<ProjectileObject> projectiles = new List<ProjectileObject>();
        int repeated = 0;
        float nextProjectileStart = 0f;
        public bool interruptCanTerminateCoordEffect = true;
        GameObject effectParticle;
        GameObject soundObject;
        GameObject effectObject;
        AtavismObjectNode Cnode;
        Transform slotTransform;
        int claimId = -1;
        int claimObjectId = -1;

        void Start()
        {
            AtavismEventSystem.RegisterEvent("CASTING_CANCELLED", this);

        }

        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "CASTING_CANCELLED")
            {
                //  Debug.LogError("CASTING_CANCELLED " + name);
                if (OID.fromString(eData.eventArgs[1]) != null && Cnode!=null  && OID.fromString(eData.eventArgs[1]).ToLong() == Cnode.Oid)
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
            if (((state == ProjectileState.Hit && repeatDelay == 0f) || (repeatDelay > 0f && Time.time > nextProjectileStart)) && repeated < repeatProjectile)
            {
                Run();
            }

            // If the projectile is moving, move it towards the target
            if (state == ProjectileState.Moving)
            {
                // Work out how far it should travel this frame
                float distanceToTravel = speed * Time.deltaTime;
                //	Vector3 newLoc = Vector3.MoveTowards(projectile.transform.position, targetTransform.position, distanceToTravel);
                /*projectile.transform.LookAt(targetTransform);
                projectile.transform.position = newLoc;
                //AtavismLogger.LogDebugMessage("Moving " + name + " to loc: " + newLoc);
                // Check if we have hit the target
                if (newLoc == targetTransform.position) {
                    state = ProjectileState.Hit;
                    Destroy(projectile);
                    PlayHit();
                }*/

                //    var i = 0;
                if (projectiles.Count > 0)
                    foreach (ProjectileObject projectileObj in projectiles.ToList())
                    {
                        if (projectileObj == null)
                            projectiles.Remove(projectileObj);
                        if (projectileObj != null && projectileObj.projectile != null)
                        {
                            if (projectileObj.targetTransform == null)
                            {
                                if (projectileDestroyDelay > 0)
                                    Destroy(projectileObj.projectile, projectileDestroyDelay);
                                else
                                    Destroy(projectileObj.projectile);
                                projectiles.Remove(projectileObj);
                            }
                            else
                            {
                                if (rotateObject)
                                    projectileObj.projectile.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
                                Vector3 newLoc = Vector3.MoveTowards(projectileObj.projectile.transform.position, projectileObj.targetTransform.position + positionAdd, distanceToTravel);
                                if (!rotateObject)
                                    projectileObj.projectile.transform.LookAt(projectileObj.targetTransform.position + positionAdd);
                                projectileObj.projectile.transform.position = newLoc;
                                AtavismLogger.LogDebugMessage("Moving " + name + " to loc: " + newLoc);
                                // Check if we have hit the target
                                //  if (newLoc == projectileObj.targetTransform.position + positionAdd)
                                if (Vector3.Distance(newLoc, projectileObj.targetTransform.position + positionAdd) <= 1f)
                                {
                                    if (projectileObj.returnObject)
                                    {
                                        if (projectileDestroyDelay > 0)
                                            Destroy(projectileObj.projectile, projectileDestroyDelay);
                                        else
                                            Destroy(projectileObj.projectile);
                                        projectiles.Remove(projectileObj);
                                        if (destroyWhenFinished && repeated == repeatProjectile)
                                            Destroy(gameObject, duration);
                                        break;
                                    }
                                    else if (!ObjectReturnToCaster)
                                    {
                                        PlayHit(projectileObj);
#if AT_MASTERAUDIO_PRESET
                                    MasterAudio.StopAllSoundsOfTransform(projectileObj.projectile.transform);
#endif
                                        if (projectileDestroyDelay > 0)
                                            Destroy(projectileObj.projectile, projectileDestroyDelay);
                                        else
                                            Destroy(projectileObj.projectile);
                                        break;
                                    }
                                    else
                                    {
                                        PlayHit(projectileObj);
                                    }
                                }
                            }
                        }

                    }

            }
        }
        private void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("CASTING_CANCELLED", this);
            AtavismLogger.LogDebugMessage("OnDestroy " + name );

            foreach (ProjectileObject projectileObj in projectiles)
            {
                Destroy(projectileObj.projectile);
            }
        }

        public override void Execute(Dictionary<string, object> props)
        {
            if (!enabled)
                return;
            base.props = props;
            AtavismLogger.LogDebugMessage("Executing " + name + " with num props: " + props.Count);
            casterOid = (OID)props["sourceOID"];
            targetOid = (OID)props["targetOID"];

            if(props.ContainsKey("claimID"))
                claimId = (int)props["claimID"];
            if(props.ContainsKey("claimObjID"))
                claimObjectId = (int)props["claimObjID"];
            //    Debug.LogError("CordProjectileEffect: claimId="+claimId+" claimObjectId="+claimObjectId);
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
            if (repeatDelay > 0f) nextProjectileStart = repeatDelay + Time.time;

            // Create a base GameObject to attach the projectile particle and/or object to

            // Set the starting position to the specified slot of the caster
            AtavismObjectNode Cnode = ClientAPI.WorldManager.GetObjectNode(casterOid);
            //Vector3 projectilePosition = node.GameObject.GetComponent<AtavismMobAppearance>().GetSocketTransform(casterSlot).position;
            AtavismObjectNode Tnode = null;
            if(targetOid != null)
                Tnode = ClientAPI.WorldManager.GetObjectNode(targetOid);
            if (Cnode == null || (Tnode == null && claimId == -1 && claimObjectId == -1))

            {
                Destroy(gameObject);
                return;
            }

            GameObject projectile = new GameObject();
            GameObject weaponObj = null;
            projectile.name = name;
            float distance = 0;
            if (claimId == -1 && claimObjectId == -1)
            {
                distance = Vector3.Distance(Cnode.GameObject.transform.position, Tnode.GameObject.transform.position);
            }
            else
            {
                ClaimObject co = WorldBuilder.Instance.GetClaim(claimId).claimObjects[claimObjectId];
                if (co != null)
                {
                    distance = Vector3.Distance(Cnode.GameObject.transform.position, co.gameObject.transform.position);
                }
            }
            if (ObjectReturnToCaster)
                distance *= 2f;
            //        Debug.LogError("Distance " + distance+" speed="+speed +" "+(Mathf.Abs(distance) / speed));
            Destroy(projectile, (Mathf.Abs(distance)/speed)+1+(projectileDestroyDelay > 0 ? projectileDestroyDelay : 0));

            if (Cnode == null || Cnode.GameObject == null) return;

            AtavismMobAppearance ama = Cnode.GameObject.GetComponent<AtavismMobAppearance>();
            if (ama == null)
            {
                Debug.LogError("AtMobAppearance null");
                Destroy(projectile);
                Destroy(gameObject);
                return;
            }
            Vector3 projectilePosition = ama.GetSocketTransform(casterSlot).position;
            projectile.transform.position = projectilePosition;

            if (projectileWeaponObject)
            {
                GameObject weaponModel = Cnode.GameObject.GetComponent<AtavismMobAppearance>().GetWeaponObjectModel();
                if (weaponModel != null)
                    weaponObj = (GameObject)Instantiate(weaponModel, projectile.transform.position, projectile.transform.rotation, projectile.transform);
                if (weaponObj != null)
                {
                    weaponObj.transform.Rotate(new Vector3(90, 90, 0));
                    /*                if (trailOnProjectaileWeapon) {
                                        XWeaponTrail trail = weaponObj.GetComponentInChildren<XWeaponTrail>(true);
                                        if (trail != null) trail.Activate();
                                    }
                                    */
                }
            }
            else
            {
                // If we have a projectile particle, create an instance of it then attach it to the projectile
                if (projectileParticle != null)
                {
                    GameObject effectParticle = (GameObject)Instantiate(projectileParticle.gameObject, projectile.transform.position, projectile.transform.rotation);
                    effectParticle.transform.position = projectile.transform.position;
                    effectParticle.transform.parent = projectile.transform;
                    effectParticle.transform.position += casterSlotAdditionalY * Vector3.up;
                }

                // If we have a projectile particle, create an instance of it then attach it to the projectile
                if (projectileObject != null)
                {
                    GameObject effectObject = (GameObject)Instantiate(projectileObject);
                    effectObject.transform.position = projectile.transform.position;
                    effectObject.transform.parent = projectile.transform;
                    effectObject.transform.position += casterSlotAdditionalY * Vector3.up;
                }
                // If we have a projectile particle, create an instance of it then attach it to the projectile
                if (projectileSound != null && projectileSound.Count>0)
                {
                    AudioSource audioSource = projectile.AddComponent<AudioSource>();
                    audioSource.clip = projectileSound[new System.Random().Next(projectileSound.Count - 1)];
                    if (soundClip3D)
                        audioSource.spatialBlend = 1.0f;
                    audioSource.volume = /*SoundSystem.SoundEffectVolume * */soundVolume;
                    if (linear)
                        audioSource.rolloffMode = AudioRolloffMode.Linear;
                    audioSource.maxDistance = maxDistance;
                    audioSource.dopplerLevel = 0f;
                    if (AtavismSettings.Instance.masterMixer != null)
                        audioSource.outputAudioMixerGroup = AtavismSettings.Instance.masterMixer.FindMatchingGroups(mixerGroupName)[0];

                    audioSource.Play();
                }
#if AT_MASTERAUDIO_PRESET
            if (projectileSoundClipName != null && projectileSoundClipName.Count>0)
            {
                //  MasterAudio.PlaySound3DFollowTransform
                //  MasterAudio.PlaySound3DFollowTransformAndWaitUntilFinished(projectileSoundClipName, projectile.transform, soundVolume);
                MasterAudio.PlaySound3DFollowTransform(projectileSoundClipName[new System.Random().Next(projectileSoundClipName.Count - 1)], projectile.transform, soundVolume);

            }
#endif
            }


            if (Tnode != null && Tnode.GameObject != null)
            {
                ProjectileObject po = new ProjectileObject();
                po.projectile = projectile;
                if (claimId == -1)
                {
                    if (Tnode.GameObject.GetComponent<AtavismMobAppearance>() == null) return;
                    targetTransform = Tnode.GameObject.GetComponent<AtavismMobAppearance>().GetSocketTransform(targetSlot);
                    if (targetTransform == Tnode.GameObject.GetComponent<AtavismMobAppearance>().GetSocketTransform("Root"))
                        positionAdd = Vector3.up * Tnode.GameObject.GetComponent<CharacterController>().height / 2;
                }
                else
                {
                    ClaimObject co = WorldBuilder.Instance.GetClaim(claimId).claimObjects[claimObjectId];
                    if (co != null)
                    {
                        targetTransform = co.gameObject.transform;
                        positionAdd = Vector3.up * hitSlotAdditionalY;
                    }
                }
                if (projectileWeaponObject)
                {
                    float distancePosition = Vector3.Distance(projectile.transform.position, targetTransform.position);
                    Cnode.GameObject.GetComponent<AtavismMobAppearance>().HideTrail();
                    Cnode.GameObject.GetComponent<AtavismMobAppearance>().HideWeapon(distancePosition / speed);
                }
                else if (hideWeapon)
                {
                    Cnode.GameObject.GetComponent<AtavismMobAppearance>().HideWeapon(hideTime);
                    MobWeapon w = Cnode.GameObject.GetComponentInChildren<MobWeapon>();
                    if (w != null)
                    {
                        w.HideWeapon(hideTime);
                    }
                }
                po.targetTransform = targetTransform;
                projectiles.Add(po);
                //  po = new ProjectileObject();
                //        po.projectile = (GameObject)Instantiate(projectile);
                //       po.targetTransform = targetTransform;
                //       po.targetTransform.position = targetTransform.position + new Vector3(2.5f, 0f, 0);
                //        projectiles.Add(po);

                state = ProjectileState.Moving;
                repeated++;
            }

        }

        // Plays the particles/sound for when the projectile has hit the target
        void PlayHit(ProjectileObject projectileObj)
        {
            if (claimId == -1 && claimObjectId == -1)
            {
                AtavismObjectNode Tnode = ClientAPI.WorldManager.GetObjectNode(targetOid);
                slotTransform = Tnode.GameObject.GetComponent<AtavismMobAppearance>().GetSocketTransform(hitSlot);
               
            }
            else
            {
                ClaimObject co = null;
                if(WorldBuilder.Instance.GetClaim(claimId)!=null && WorldBuilder.Instance.GetClaim(claimId).claimObjects.ContainsKey(claimObjectId))
                    co = WorldBuilder.Instance.GetClaim(claimId).claimObjects[claimObjectId];
                if (co != null)
                {
                    slotTransform = co.gameObject.transform;
                  
                }
            }
            Cnode = ClientAPI.WorldManager.GetObjectNode(casterOid);
            
            if (ObjectReturnToCaster)
            {
                projectileObj.targetTransform = Cnode.GameObject.GetComponent<AtavismMobAppearance>().GetSocketTransform(casterSlot);
                projectileObj.returnObject = true;
               // float distance = Vector3.Distance(Cnode.GameObject.transform.position, Tnode.GameObject.transform.position);
               // Debug.LogError("Distance " + distance + " speed=" + speed + " " + (Mathf.Abs(distance) / speed));
               // Destroy(projectile, (Mathf.Abs(distance) / speed) + 1);
            }
            if (hideWeapon)
            {
                Cnode.GameObject.GetComponent<AtavismMobAppearance>().HideWeapon(hideTime);
                MobWeapon w = Cnode.GameObject.GetComponentInChildren<MobWeapon>();
                if (w != null)
                {
                    w.HideWeapon(0f);
                }
            }
            if (hitObject != null)
            {
                 effectObject = (GameObject)Instantiate(hitObject, slotTransform.position, slotTransform.rotation);
                effectObject.transform.position += hitSlotAdditionalY * Vector3.up;
                if (forceToHitSlot == false) effectObject.transform.position = projectileObj.projectile.transform.position;
                if (hitLookAtCamera) effectObject.transform.LookAt(Camera.main.gameObject.transform);
                if (hitSetParent == true)
                    effectObject.transform.parent = slotTransform;

                
                Destroy(effectObject, duration);
            }
            if (hitParticle != null)
            {
                effectParticle = (GameObject)Instantiate(hitParticle.gameObject, slotTransform.position, slotTransform.rotation);
                effectParticle.transform.position += hitSlotAdditionalY * Vector3.up;
                if (forceToHitSlot == false)
                    effectParticle.transform.position = projectileObj.projectile.transform.position;
                if (hitLookAtCamera)
                    effectParticle.transform.LookAt(Camera.main.gameObject.transform);
                if (hitSetParent == true)
                    effectParticle.transform.parent = slotTransform;
              //  effectParticle.transform.parent = slotTransform;
                Destroy(effectParticle, duration);
            }

            if (hitSound!=null&& hitSound.Count>0)
            {
                soundObject = new GameObject();
                soundObject.transform.position = slotTransform.position;
                if (hitSetParent == true) soundObject.transform.parent = slotTransform;
                AudioSource audioSource = soundObject.AddComponent<AudioSource>();
                audioSource.clip = hitSound[new System.Random().Next(hitSound.Count - 1)];
                if (hitSoundClip3D)
                    audioSource.spatialBlend = 1.0f;
                audioSource.volume =/* SoundSystem.SoundEffectVolume * */hitSoundVolume;
                if (hitlinear)
                    audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.maxDistance = hitMaxDistance;
                audioSource.dopplerLevel = 0f;
                if (AtavismSettings.Instance.masterMixer != null)
                    audioSource.outputAudioMixerGroup = AtavismSettings.Instance.masterMixer.FindMatchingGroups(hitMixerGroupName)[0];
            
                audioSource.Play();
                Destroy(soundObject, duration);
            }
#if AT_MASTERAUDIO_PRESET
            if (hitSoundClipName != null && hitSoundClipName.Count>0)
            MasterAudio.PlaySound3DAtTransform(hitSoundClipName[new System.Random().Next(hitSoundClipName.Count - 1)], slotTransform, hitSoundVolume);
#endif
            if (destroyWhenFinished && !ObjectReturnToCaster)
                Destroy(gameObject, duration);
        }

        void CancelCoordEffect()
        {
            if (!interruptCanTerminateCoordEffect)
                return;
            //   Debug.LogError("CancelCoordEffect", gameObject);
            if (projectiles.Count > 0)
                foreach (ProjectileObject projectileObj in projectiles)
                {
                 
#if AT_MASTERAUDIO_PRESET
                    if (projectileObj.projectile != null)
                        MasterAudio.StopAllSoundsOfTransform(projectileObj.projectile.transform);
#endif
                    DestroyImmediate(projectileObj.projectile);
                }
            if (soundObject != null)
                DestroyImmediate(soundObject);
            if (effectObject != null)
                DestroyImmediate(effectObject);
#if AT_MASTERAUDIO_PRESET
            if (slotTransform != null)
                MasterAudio.StopAllSoundsOfTransform(slotTransform);
#endif

            if (effectParticle != null)
                DestroyImmediate(effectParticle);
            Destroy(gameObject);
        }
    }
}