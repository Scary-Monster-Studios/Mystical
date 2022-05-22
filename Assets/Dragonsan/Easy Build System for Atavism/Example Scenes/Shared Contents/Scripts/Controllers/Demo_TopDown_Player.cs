using EasyBuildSystem.Features.Scripts.Core.Base.Builder.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Event;
using System;
using UnityEngine;
using UnityEngine.AI;

public class Demo_TopDown_Player : MonoBehaviour
{
    #region Public Fields

    public LayerMask MovementLayers;

    #endregion

    #region Private Fields

    private NavMeshAgent Agent;

    #endregion

    #region Private Methods

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        BuildEvent.Instance.OnChangedBuildMode.AddListener((BuildMode mode) =>
        {
            if (mode == BuildMode.None)
                Agent.isStopped = false;
            else
                Agent.isStopped = true;
        });
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit Hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out Hit, Mathf.Infinity, MovementLayers))
                Agent.destination = Hit.point;
        }
    }

    #endregion
}