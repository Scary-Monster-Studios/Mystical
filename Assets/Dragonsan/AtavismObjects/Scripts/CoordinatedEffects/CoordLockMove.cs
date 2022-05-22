// Rafał Dorobisz Play Effect on target 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Atavism
{
    public class CoordLockMove : CoordinatedEffect
    {

        //	public bool parent;

        AtavismObjectNode node;
       // [System.Obsolete]
       // public float restTime = 5f;
        public float lockMove = 1f;

        public bool moveCharacter = false;
        public float moveSpeed = 0f;
        public float duringTime = 0f;
        float activeMoveTime = 0f;
        float activationMoveTime = 0f;
        public float activationMoveDelay = 0f;
        public bool jumpCharacter = false;
        public float activationJumpDelay = 0f;
        float activationJumpTime = 0f;

        public bool interruptCanTerminateCoordEffect = true;

        // Use this for initialization
        void Start()
        {
            AtavismEventSystem.RegisterEvent("CASTING_CANCELLED", this);

        }

        void OnDestroy()
        {
            AtavismEventSystem.UnregisterEvent("CASTING_CANCELLED", this);
            StopRun();
        }
        public void OnEvent(AtavismEventData eData)
        {
            if (eData.eventType == "CASTING_CANCELLED")
            {
                //  Debug.LogError("CASTING_CANCELLED " + name);
              //  if (abilityID > 0 && int.Parse(eData.eventArgs[0]) != abilityID)
               //     return;
                if (OID.fromString(eData.eventArgs[1]) != null && node != null && OID.fromString(eData.eventArgs[1]).ToLong() == node.Oid)
                    CancelCoordEffect();
            }


        }
        // Update is called once per frame
        void Update()
        {
            if (activationMoveTime != 0f && Time.time > activationMoveTime)
            {
                Run();
            }
            if (activeMoveTime != 0f && Time.time > activeMoveTime)
            {
                StopRun();
            }
            if (activationJumpTime != 0f && Time.time > activationJumpTime)
            {
                if (ClientAPI.GetPlayerOid() == ((OID)props["sourceOID"]).ToLong())// casterOid.compareTo((OID)ClientAPI.GetPlayerOid()))
                    Jump();
            }

        }
        void Jump()
        {
            activationJumpTime = 0f;
            //  if (node = ClientAPI.WorldManager.GetObjectNode((OID)props["sourceOID"]);)
            if (ClientAPI.GetPlayerObject() == node)
                ClientAPI.GetPlayerObject().GameObject.SendMessage("StartJump");
        }
        void Run()
        {
            activationMoveTime = 0f;
            activeMoveTime = Time.time + duringTime;
            if (ClientAPI.GetPlayerObject() == node)
                ClientAPI.GetPlayerObject().GameObject.SendMessage("SkillMove", moveSpeed);
        }
        void StopRun()
        {
            float val = 0f; 
            if (ClientAPI.GetPlayerObject() == node)
                ClientAPI.GetPlayerObject().GameObject.SendMessage("SkillMove", val);
            activeMoveTime = 0f;
            //  Destroy(gameObject, 5f);
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

            node.GameObject.SendMessage("NoMove", lockMove);
            if (moveCharacter)
                if (activationMoveDelay == 0)
                {
                    Run();
                }
                else
                {
                    activationMoveTime = Time.time + activationMoveDelay;
                }
            if (jumpCharacter)
                if (activationJumpDelay == 0)
                {
                    if (ClientAPI.GetPlayerOid() == ((OID)props["sourceOID"]).ToLong())// casterOid.compareTo((OID)ClientAPI.GetPlayerOid()))
                        Jump();
                }
                else
                {
                    activationJumpTime = Time.time + activationJumpDelay;
                }

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

            StopRun();
            Destroy(gameObject);
            }
    }
}