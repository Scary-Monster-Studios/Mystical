#if VOXELAND
using EasyBuildSystem.Features.Scripts.Core.Addons;
using EasyBuildSystem.Features.Scripts.Core.Addons.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece.Enums;
using System.Collections;
using UnityEngine;
using Voxeland5;

[Addon("(Voxeland) Internal Remove Grass Addon", "Remove the grass on the voxeland terrain when a piece is instantiated.", AddonTarget.PieceBehaviour)]
public class InternalRemoveGrassVoxelandAddon : AddonBehaviour
{
#region Fields

    public bool RemoveGrass = true;
    public float RemoveGrassRadius = 5.0f;

    private Voxeland Terrain;

#endregion Fields

#region Methods

    private void Awake()
    {
        Terrain = FindObjectOfType<Voxeland>();
    }

    private void Start()
    {
        if (GetComponent<PieceBehaviour>().CurrentState == StateType.Placed)
            if (RemoveGrass)
                StartCoroutine(RemoveGrassToPosition(transform.position, RemoveGrassRadius));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, RemoveGrassRadius);
    }

    public IEnumerator RemoveGrassToPosition(Vector3 position, float radius)
    {
        if (Terrain == null)
            yield break;

        CoordDir Dir = Terrain.PointOut(new Ray(position, Vector3.down * 2));

        Terrain.brush.form = Brush.Form.blob;
        Terrain.brush.round = false;
        Terrain.brush.extent = Mathf.RoundToInt(radius / 2);
        Terrain.standardEditMode = Voxeland.EditMode.dig;
        Terrain.grassTypes.selected = 1;

        Terrain.Alter(Dir, Terrain.brush, Voxeland.EditMode.dig,
                            landType: Terrain.landTypes.selected,
                            objectType: Terrain.objectsTypes.selected,
                            grassType: Terrain.grassTypes.selected);

        yield break;
    }

#endregion Methods
}
#endif