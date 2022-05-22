using EasyBuildSystem.Features.Scripts.Core.Addons;
using EasyBuildSystem.Features.Scripts.Core.Addons.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Event;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece.Enums;
using EasyBuildSystem.Features.Scripts.Extensions;
using UnityEngine;

[System.Serializable]
public class DestructibleAppearance
{
    public int AppearanceIndex = 0;
    public GameObject FracturedAppearance;
    public float FracturedLifeTime = 5f;
}

[Addon("Destructible Appearances", AddonTarget.PieceBehaviour)]
public class AddonDestructibleAppearance : AddonBehaviour
{
    #region Fields

    public DestructibleAppearance[] Destructibles;
    public float MaxDepenetrationVelocity = .5f;

    private PieceBehaviour Piece;
    private bool IsExiting;

    #endregion

    #region Methods

    private void Awake()
    {
        Piece = GetComponent<PieceBehaviour>();
    }

    private void Start()
    {
        BuildEvent.Instance.OnPieceDestroyed.AddListener(OnDestroyedPiece);
    }

    private void OnApplicationQuit()
    {
        IsExiting = true;
    }

    private void OnDestroy()
    {
        if (IsExiting)
            return;

        if (Piece.CurrentState == StateType.Remove || Piece.CurrentState == StateType.Placed)
            InstantiateObjects();
    }

    private void OnDestroyedPiece(PieceBehaviour part)
    {
        if (part != Piece)
            return;

        if (Piece.CurrentState == StateType.Remove || Piece.CurrentState == StateType.Placed)
            InstantiateObjects();
    }

    private bool AlreadyInstantiated;
    private void InstantiateObjects()
    {
        if (IsExiting) return;

        if (AlreadyInstantiated) return;

        AlreadyInstantiated = true;

        gameObject.ChangeAllMaterialsInChildren(Piece.Renderers.ToArray(), Piece.InitialRenderers);

        for (int i = 0; i < Destructibles.Length; i++)
        {
            if (Piece.AppearanceIndex == Destructibles[i].AppearanceIndex)
            {
                if (Destructibles[i] == null)
                    return;

                Destructibles[i].FracturedAppearance.SetActive(true);

                GameObject Temp = Instantiate(Destructibles[i].FracturedAppearance.gameObject,
                    Destructibles[i].FracturedAppearance.transform.position, 
                    Destructibles[i].FracturedAppearance.transform.rotation);

                for (int x = 0; x < Temp.transform.childCount; x++)
                    Temp.transform.GetChild(x).GetComponent<Rigidbody>().maxDepenetrationVelocity = MaxDepenetrationVelocity;

                Destroy(Temp, Destructibles[i].FracturedLifeTime);

                Destroy(gameObject);

                break;
            }
        }
    }

    #endregion
}