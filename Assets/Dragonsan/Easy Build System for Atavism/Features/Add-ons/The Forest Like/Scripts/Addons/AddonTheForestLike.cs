using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EasyBuildSystem.Features.Scripts.Core.Addons;
using EasyBuildSystem.Features.Scripts.Core.Addons.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using EasyBuildSystem.Features.Scripts.Extensions;
using EasyBuildSystem.Features.Scripts.Core.Base.Event;
using System.Collections;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece.Condition;

[Addon("The Forest Like", AddonTarget.PieceBehaviour)]
public class AddonTheForestLike : AddonBehaviour
{
    #region Fields

    public Transform[] Elements;

    private PieceBehaviour Piece;
    private GameObject Preview;
    private List<Renderer> CacheRenderers;

    #endregion

    #region Methods

    private void Awake()
    {
        Piece = GetComponent<PieceBehaviour>();
        Piece.AppearanceIndex = 0;

        if (BuildManager.Instance != null)
            BuildManager.Instance.DefaultState = StateType.Queue;
    }

    private void Start()
    {
        //ALLOW TO WAIT THAT THE PIECE/SOCKET ARE CORRECTLY INIT.
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.1f);

        if (Piece.CurrentState != StateType.Queue)
            yield break;

        if (Preview == null)
        {
            Preview = Instantiate(Piece.gameObject, transform.position, transform.rotation);

            Preview.GetComponent<PieceBehaviour>().EnableAllColliders();
            Preview.GetComponent<PieceBehaviour>().EnableAllCollidersTrigger();

            CacheRenderers = Preview.GetComponentsInChildren<Renderer>(true).ToList();

            gameObject.ChangeAllMaterialsInChildren(CacheRenderers.ToArray(), Piece.PreviewMaterial);
            Preview.ChangeAllMaterialsColorInChildren(CacheRenderers.ToArray(), Piece.PreviewAllowedColor);

            GameObject OnlyPreview = new GameObject("Preview");

            for (int i = 0; i < CacheRenderers.Count; i++)
                CacheRenderers[i].transform.SetParent(OnlyPreview.transform);

            OnlyPreview.transform.localScale += OnlyPreview.transform.localScale * 0.001f;

            Destroy(Preview.gameObject);

            OnlyPreview.transform.SetParent(transform);

            Preview = OnlyPreview;

            GameObject OnlyInteractionPreview = Instantiate(OnlyPreview);

            OnlyInteractionPreview.name = "Interaction Preview";

            OnlyInteractionPreview.transform.SetParent(transform, false);

            for (int i = 0; i < OnlyInteractionPreview.GetComponentsInChildren<Renderer>().Length; i++)
                Destroy(OnlyInteractionPreview.GetComponentsInChildren<Renderer>()[i]);

            OnlyInteractionPreview.SetLayerRecursively(LayerMask.GetMask("Interaction"));

            for (int i = 0; i < Elements.Length; i++)
                Elements[i].gameObject.SetActive(false);

            BuildEvent.Instance.OnPieceDestroyed.AddListener(OnPieceDestroyed);
            BuildEvent.Instance.OnPieceChangedState.AddListener(OnPieceChangedState);
        }
    }

    private void OnPieceChangedState(PieceBehaviour piece, StateType state)
    {
        if (piece == Piece)
        {
            if (state == StateType.Queue)
            {
                if (Preview == null)
                    return;

                gameObject.ChangeAllMaterialsInChildren(Piece.Renderers.ToArray(), Piece.InitialRenderers);
                Preview.ChangeAllMaterialsColorInChildren(CacheRenderers.ToArray(), Piece.PreviewAllowedColor);
            }
        }
    }

    private void OnPieceDestroyed(PieceBehaviour piece)
    {
        if (Piece != piece)
            return;

        Destroy(gameObject);
    }

    /// <summary>
    /// This method allows to upgrade the base part.
    /// </summary>
    public void Upgrade(string tag)
    {
        Piece.AppearanceIndex++;

        gameObject.ChangeAllMaterialsInChildren(Piece.Renderers.ToArray(), Piece.InitialRenderers);

        Elements.FirstOrDefault(x => !x.gameObject.activeSelf && x.tag == tag).gameObject.SetActive(true);

        PickableController.Instance.TempElements.Remove(tag);

        if (IsCompleted())
        {
            Piece.DisableAllCollidersTrigger();

            Destroy(Preview);

            for (int i = 0; i < Elements.Length; i++)
                if (Piece != null)
                    Elements[i].gameObject.SetActive(true);

            Piece.ChangeState(StateType.Placed);

            InternalPhysicsCondition Physics = Piece.GetComponent<InternalPhysicsCondition>();

            if (Physics != null)
                if (!Physics.CheckStability())
                    Physics.ApplyPhysics();
        }
    }

    /// <summary>
    /// This method allows to get the current upgrade progression.
    /// </summary>
    public int GetCurrentProgression()
    {
        return Piece.AppearanceIndex;
    }

    /// <summary>
    /// This method allows to check if the part progression is complete.
    /// </summary>
    public bool IsCompleted()
    {
        if (Piece == null)
            return false;

        return Elements.Length <= Piece.AppearanceIndex;
    }

    #endregion
}