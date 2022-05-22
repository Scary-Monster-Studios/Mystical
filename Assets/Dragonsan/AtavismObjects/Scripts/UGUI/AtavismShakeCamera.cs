using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atavism
{
    public class AtavismShakeCamera : CoordinatedEffect
    {

        public Vector3 ShakePower = Vector3.up;
        [SerializeField] float activationDuration = 0.5f;
        [SerializeField] float range = 50f;
        //  float activationEnd = 0f;
        AtavismObjectNode node;

        // Update is called once per frame
        void Update()
        {
            if (activationTime != 0 && Time.time > activationTime && Time.time < (activationTime + activationDuration))
            {
                Shake();
            }
        }
        void Shake()
        {
            if (range >= Vector3.Distance(node.GameObject.transform.position, Camera.main.transform.position))
            {
                AtavismInputController inputManager = ClientAPI.GetInputController();
                if (inputManager != null)
                    inputManager.SendMessage("Shake", ShakePower);
            }
        }
        public override void Execute(Dictionary<string, object> props)
        {
            if (!enabled)
                return;
            base.props = props;
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
                Shake();
            }
            else
            {
                activationTime = Time.time + activationDelay;
            }
        }
    }
}