using EasyBuildSystem.Features.Scripts.Core.Addons;
using EasyBuildSystem.Features.Scripts.Core.Addons.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Event;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using EasyBuildSystem.Features.Scripts.Core.Base.Socket;
using EasyBuildSystem.Features.Scripts.Core.Base.Storage;
using UnityEngine;
using UnityEngine.AI;

[Addon("Dynamic Nav Mesh", AddonTarget.BuildManager)]
public class AddonNavMesh : AddonBehaviour
{
    #region Public Fields

    public static AddonNavMesh Instance;

    #endregion

    #region Private Fields

    public NavMeshSurface Surface;

    #endregion

    #region Private Methods

    private void OnEnable()
    {
        if (BuildEvent.Instance == null) return;

        if (FindObjectOfType<BuildStorage>() != null && FindObjectOfType<BuildStorage>().ExistsStorageFile())
            BuildEvent.Instance.OnStorageLoadingResult.AddListener(OnStorageLoadingDone);

        BuildEvent.Instance.OnPieceInstantiated.AddListener(OnPlacedPart);
        BuildEvent.Instance.OnPieceDestroyed.AddListener(OnDestroyedPart);
    }

    private void OnDisable()
    {
        BuildEvent.Instance.OnPieceInstantiated.RemoveListener(OnPlacedPart);
        BuildEvent.Instance.OnPieceDestroyed.RemoveListener(OnDestroyedPart);
    }

    private void Awake()
    {
        UpdateMeshData();

        Instance = this;

        if (Surface == null)
            Debug.LogWarning("AddonNavMesh: Please complete empty field to use NavMeshSurface component.");
    }

    private void OnApplicationQuit()
    {
        Surface.BuildNavMesh();
    }

    private void OnStorageLoadingDone(PieceBehaviour[] pieces)
    {
        if (pieces == null) return;

        UpdateMeshData();

        BuildEvent.Instance.OnPieceInstantiated.AddListener(OnPlacedPart);
        BuildEvent.Instance.OnPieceDestroyed.AddListener(OnDestroyedPart);
    }

    private void OnPlacedPart(PieceBehaviour piece, SocketBehaviour socket)
    {
        if (piece.CurrentState != EasyBuildSystem.Features.Scripts.Core.Base.Piece.Enums.StateType.Placed) return;

        UpdateMeshData();
    }

    private void OnDestroyedPart(PieceBehaviour piece)
    {
        UpdateMeshData();
    }

    #endregion

    #region Public Methods

    public void UpdateMeshData()
    {
        Surface.UpdateNavMesh(Surface.navMeshData);
    }

    #endregion
}