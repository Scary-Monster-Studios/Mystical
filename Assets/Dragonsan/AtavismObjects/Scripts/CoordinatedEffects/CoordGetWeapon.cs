// Rafał Dorobisz Play Effect on target 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class CoordGetWeapon : CoordinatedEffect
    {

        //	public bool parent;

        AtavismObjectNode node;

        public float restTime = 5f;
        public bool showTrail = false;
        public bool hideTrail = false;
        public float hideTime = 0f;
        private float hideTrialTime = 0f;
        public bool DucTrail = false;
        //  [SerializeField] Material ducTrialMaterial;

        //  [SerializeField] bool DucRedTrial =true;
        //  [SerializeField] bool DucBlueTrial = false;
        public bool lookAtTarget = false;
        public bool interruptCanTerminateCoordEffect = true;

        
        public string mainSlotMoveToRest;
        public bool moveAllWeaponSlotToRest = false;


        public string slotMoveFromRestToMain;
        
        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("CASTING_CANCELLED", this);

        }
        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "CASTING_CANCELLED")
            {
                //  Debug.LogError("CASTING_CANCELLED " + name);
                //if (abilityID > 0 && int.Parse(eData.eventArgs[0]) != abilityID)
               //     return;

                if (OID.fromString(eData.eventArgs[1])!=null && node != null && OID.fromString(eData.eventArgs[1]).ToLong() == node.Oid)
                    CancelCoordEffect();
            }


        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("CASTING_CANCELLED", this);
            if (node != null && !DucTrail)
            {
                node.GameObject.SendMessage("HideTrail");
                /*   MeleeWeaponTrail[] mwts = node.GameObject.GetComponent<AtavismMobAppearance>().GetWeaponObject().GetComponentsInChildren<MeleeWeaponTrail>();
                   foreach (MeleeWeaponTrail mwt in mwts) {
                       if (mwt != null) {
                           mwt.gameObject.SetActive(false);
                       }
                   }
                   */
            }
        }
        // Update is called once per frame
        void Update()
        {
            if (activationTime != 0 && Time.time > activationTime)
            {
                Run();
            }
            if (hideTrialTime != 0f && Time.time > hideTrialTime)
            {
                if (node != null && !DucTrail) node.GameObject.SendMessage("HideTrail");
                if (DucTrail && node != null)
                {
                    /*  MeleeWeaponTrail[] mwts = node.GameObject.GetComponent<AtavismMobAppearance>().GetWeaponObject().GetComponentsInChildren<MeleeWeaponTrail>();
                      foreach (MeleeWeaponTrail mwt in mwts) {
                          if (mwt != null) {
                                  mwt.gameObject.SetActive(false);
                          }
                      }
                      */
                }
                hideTrialTime = 0f;
            }
        }


        public override void Execute(Dictionary<string, object> props)
        {
            if (!enabled)
                return;
            base.props = props;
            AtavismLogger.LogDebugMessage("Executing " + name + " with num props: " + props.Count);
            //	casterOid = (OID)props["sourceOID"];
            //	targetOid = (OID)props["targetOID"];
            if (target == CoordinatedEffectTarget.Caster)
            {
                node = ClientAPI.WorldManager.GetObjectNode((OID)props["sourceOID"]);
            }
            else
            {
                node = ClientAPI.WorldManager.GetObjectNode((OID)props["targetOID"]);

            }

            if (activationDelay == 0)
            {
                Run();
            }
            else
            {
                activationTime = Time.time + activationDelay;
            }
            if (destroyWhenFinished && duration > 0)
            {
                Destroy(gameObject, duration + activationDelay);
            }
        }
        void Run()
        {
            if (lookAtTarget)
            {
                AtavismObjectNode nodeC = ClientAPI.WorldManager.GetObjectNode((OID)props["sourceOID"]);
                AtavismObjectNode nodeT = ClientAPI.WorldManager.GetObjectNode((OID)props["targetOID"]);
                if (nodeT != null && nodeC != null)
                {
                    Vector3 relativePos = (nodeT.MobController.Mount == null ? nodeT.GameObject.transform.position : nodeT.MobController.Mount.transform.position);// - nodeC.GameObject.transform.position;

                    // the second argument, upwards, defaults to Vector3.up
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
                    //   nodeC.GameObject.transform.LookAt(nodeT.GameObject.transform);
                }
            }
            /*     if (props.ContainsKey("cancelOnProp"))
                 {
                     propName = (string)props["cancelOnProp"];
                     node.RegisterPropertyChangeHandler(propName, CancelPropHandler);
                 }
              */
            if (node != null)
            {
                 if(moveAllWeaponSlotToRest)
                    node.GameObject.GetComponent<AtavismMobAppearance>().WeaponToRest();
                if(mainSlotMoveToRest.Length>0 && mainSlotMoveToRest !="None")
                    node.GameObject.GetComponent<AtavismMobAppearance>().WeaponToRest(mainSlotMoveToRest,restTime);
                if(slotMoveFromRestToMain.Length>0 && slotMoveFromRestToMain !="None")
                    node.GameObject.GetComponent<AtavismMobAppearance>().GetWeapon(slotMoveFromRestToMain, restTime);

            }

            if (!DucTrail && showTrail && node!=null)
                node.GameObject.SendMessage("ShowTrail");
            if (DucTrail && showTrail)
            {
                if (node == null) return;
                if (node.GameObject == null) return;

                /*   MeleeWeaponTrail[] mwts = node.GameObject.GetComponent<AtavismMobAppearance>().GetWeaponObject().GetComponentsInChildren<MeleeWeaponTrail>(true);
                   foreach (MeleeWeaponTrail mwt in mwts)
                   {
                       if (mwt != null)
                       {
                           if (mwt.name == "TrailRed" && DucRedTrial)mwt.gameObject.SetActive(true);
                           if (mwt.name == "TrailBlue" && DucBlueTrial) mwt.gameObject.SetActive(true);
                       }
                   }
                   */
            }
            if (hideTrail) hideTrialTime = Time.time + hideTime;


        }
        // Plays the particles/sound for when the projectile has hit the target
        
            public void CancelPropHandler(object sender, PropertyChangeEventArgs args)
            {
                CancelCoordEffect();
            }

            void CancelCoordEffect()
            {
            if (!interruptCanTerminateCoordEffect)
                return;

            if (node != null && !DucTrail)
            {
                node.GameObject.SendMessage("HideTrail");
                if(slotMoveFromRestToMain.Length>0 && slotMoveFromRestToMain !="None")
                    node.GameObject.GetComponent<AtavismMobAppearance>().GetWeapon(slotMoveFromRestToMain, 0);

            }
            Destroy(gameObject);
            }
    }
}