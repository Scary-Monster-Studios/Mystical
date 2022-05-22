using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using XftWeapon;
using System;

namespace Atavism
{
    enum SpawnState
    {
        Setup,
        Spawn,
        Hit
    }
    [System.Obsolete("Class Obsotete", true)]

    public class CoordSpawnEffect : CoordinatedEffect
    {

        //    public int repeatProjectile = 1;
        //    public AttachmentSocket casterSlot;
        //    public bool projectileWeaponObject=false;
        //   public bool trailOnProjectaileWeapon = false;
        //    public bool ObjectReturnToCaster = false;
        //    public bool rotateObject = false;
        //    public float rotateSpeed = 20f;
        public bool hideWeapon = false;
        public float hideTime = 0f;
        public float spawnJump = 1.2f;
        public float TimeSpawn = 0.01f;
        public bool spawnTargetDirection = true;
        public GameObject spawnObject;
        public ParticleSystem spawnParticle;
        public AudioClip spawnSound;
        public float spawnLifeTime = 1f;
        public bool soundClip3D = true;
        public float soundVolume = 1.0f;
        public float speed = 15; // metres per second

        public string targetSlot;
        public string hitSlot;
        public GameObject[] hitObject;
        public ParticleSystem hitParticle;
        public AudioClip hitSound;
        public string hitAnimation;
        public bool hitSoundClip3D = true;
        public float hitSoundVolume = 1.0f;
        public bool hitSetParent = false;
        private int counter = 0;
        private int Number = 0;
        private Vector3 Direction = Vector3.forward;
        private float timeTemp;
        Transform targetTransform;
        Transform casterTransform;
        SpawnState state = SpawnState.Setup;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (activationTime != 0 && Time.time > activationTime)
            {
                Run();
            }
            if (state == SpawnState.Spawn)
            {
                if (counter >= Number - 1) { state = SpawnState.Hit; PlayHit(); }
                if (TimeSpawn > 0.0f)
                    if (Time.time > timeTemp + TimeSpawn)
                    {
                        counter += 1;
                        Direction = casterTransform.forward;
                        if (spawnTargetDirection)
                            Direction = (targetTransform.position - casterTransform.position).normalized;
                        Spawn(casterTransform.position + (Direction * spawnJump * counter));
                        timeTemp = Time.time;
                    }
            }
        }
        void Spawn(Vector3 position)
        {
            if (spawnObject != null)
            {
                Quaternion rotate = this.transform.rotation;
                //      if (!FixRotation)
                //         rotate = spawnObject.transform.rotation;

                GameObject fx = (GameObject)GameObject.Instantiate(spawnObject, position, rotate);
                //      if (Normal)
                //          fx.transform.forward = this.transform.forward;

                if (spawnLifeTime > 0)
                    GameObject.Destroy(fx.gameObject, spawnLifeTime);

                if (spawnParticle != null)
                {
                    GameObject effectParticle = (GameObject)Instantiate(spawnParticle.gameObject, fx.transform.position, fx.transform.rotation);
                    effectParticle.transform.position = fx.transform.position;
                    effectParticle.transform.SetParent(fx.transform);
                }
                // If we have a projectile particle, create an instance of it then attach it to the projectile
                if (spawnSound != null)
                {
                    AudioSource audioSource = fx.AddComponent<AudioSource>();
                    audioSource.clip = spawnSound;
                    if (soundClip3D)
                        audioSource.spatialBlend = 1.0f;
                    audioSource.volume = SoundSystem.SoundEffectVolume * soundVolume;
                    audioSource.Play();
                }
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
            AtavismObjectNode Cnode = ClientAPI.WorldManager.GetObjectNode(casterOid);
            casterTransform = Cnode.GameObject.GetComponent<AtavismMobAppearance>().GetSocketTransform("Root");
            AtavismObjectNode Tnode = ClientAPI.WorldManager.GetObjectNode(targetOid);
            targetTransform = Tnode.GameObject.GetComponent<AtavismMobAppearance>().GetSocketTransform(targetSlot);
            float distancePosition = Vector3.Distance(casterTransform.position, targetTransform.position);
            Number = (int)Math.Ceiling(distancePosition / spawnJump);
            state = SpawnState.Spawn;
            if (hideWeapon)
            {
                Cnode.GameObject.GetComponent<AtavismMobAppearance>().HideWeapon(hideTime);
            }
        }

        // Plays the particles/sound for when the projectile has hit the target
        void PlayHit()
        {
            AtavismObjectNode Tnode = ClientAPI.WorldManager.GetObjectNode(targetOid);
            Transform slotTransform = Tnode.GameObject.GetComponent<AtavismMobAppearance>().GetSocketTransform(hitSlot);

            if (hitObject != null)
            {
                GameObject effectObject = (GameObject)Instantiate(hitObject[UnityEngine.Random.Range(0, hitObject.Length)], slotTransform.position, slotTransform.rotation);
                //effectObject.transform.position = slotTransform.position;
                if (hitSetParent == true)
                {
                    effectObject.transform.parent = slotTransform;
                }

                Destroy(effectObject, duration);
            }
            if (hitParticle != null)
            {
                GameObject effectParticle = (GameObject)Instantiate(hitParticle.gameObject, slotTransform.position, slotTransform.rotation);
                if (hitSetParent == true)
                {
                    effectParticle.transform.parent = slotTransform;
                }
                Destroy(effectParticle, duration);
            }

            if (hitSound != null)
            {
                GameObject soundObject = new GameObject();
                soundObject.transform.position = slotTransform.position;
                if (hitSetParent == true)
                {
                    soundObject.transform.parent = slotTransform;
                }
                AudioSource audioSource = soundObject.AddComponent<AudioSource>();
                audioSource.clip = hitSound;
                if (hitSoundClip3D)
                    audioSource.spatialBlend = 1.0f;
                audioSource.volume = SoundSystem.SoundEffectVolume * hitSoundVolume;
                audioSource.Play();
                Destroy(soundObject, duration);
            }
            if (hitAnimation != null & hitAnimation != "")
            {
                Tnode.GameObject.SendMessage("PlayAnimationTrigger", hitAnimation);
            }

            if (destroyWhenFinished)
            {
                //wyłącznie blokady ruchu po skilu
                //   AtavismObjectNode Cnode = ClientAPI.WorldManager.GetObjectNode(casterOid);
                //   Cnode.GameObject.SendMessage("NoMove", false);
                Destroy(gameObject, duration);
            }
        }
    }
}