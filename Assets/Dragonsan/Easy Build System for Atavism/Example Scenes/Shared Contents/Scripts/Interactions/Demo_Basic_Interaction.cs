using EasyBuildSystem.Features.Scripts.Core.Base.Event;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Demo_Basic_Interaction : MonoBehaviour
{
    #region Public Fields

    [Header("Interaction Settings")]
    public float InteractionDistance = 3.0f;

    public KeyCode InteractionKey = KeyCode.E;
    public GUIStyle Font;
    public LayerMask Layers;

    [Serializable] public class Interacted : UnityEvent<GameObject> { }
    public Interacted OnInteracted;

    #endregion

    #region Private Methods

    private Demo_Interactable LastInteractable;

    private void Start()
    {
        BuildEvent.Instance.OnPieceDestroyed.AddListener((PieceBehaviour piece) =>
        {
            if (LastInteractable != null)
                LastInteractable.Hide();
        });
    }

    private void Update()
    {
        Ray Ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        RaycastHit Hit;

        if (Physics.Raycast(Ray, out Hit, InteractionDistance, Layers))
        {
            if (Hit.collider.GetComponentInParent<Demo_Interactable>() && Hit.collider.GetComponentInParent<Demo_Interactable>().enabled)
            {
                LastInteractable = Hit.collider.GetComponentInParent<Demo_Interactable>();

                Hit.collider.GetComponentInParent<Demo_Interactable>().Show(Hit.point);

                if (Input.GetKeyUp(InteractionKey))
                {
                    OnInteracted.Invoke(Hit.collider.gameObject);

                    Hit.collider.GetComponentInParent<Demo_Interactable>().Interaction();

                    LastInteractable.Hide();
                }
            }
            else
                if (LastInteractable != null)
                    LastInteractable.Hide();
        }
        else
            if (LastInteractable != null)
                LastInteractable.Hide();
    }

    #endregion
}