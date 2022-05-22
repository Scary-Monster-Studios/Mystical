using EasyBuildSystem.Features.Scripts.Core.Addons;
using EasyBuildSystem.Features.Scripts.Core.Addons.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Event;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using EasyBuildSystem.Features.Scripts.Core.Base.Piece.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Socket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Addon("Internal Remove Grass", AddonTarget.PieceBehaviour)]
public class InternalRemoveGrassAddon : AddonBehaviour
{
    #region Fields

    public bool RemoveGrass = true;
    public float RemoveGrassRadius = 5.0f;

    public static Terrain ActiveTerrain;
    public static TerrainData Data;
    public static Dictionary<int, int[,]> TerrainDetails = new Dictionary<int, int[,]>();
    public static float[,] TerrainHeights;

    public static bool Initialized;

    PieceBehaviour Piece;

    #endregion Fields

    #region Methods

    private void Awake()
    {
        Piece = GetComponent<PieceBehaviour>();

        ActiveTerrain = FindObjectOfType<Terrain>();
        if (ActiveTerrain != null)
        {
            Data = ActiveTerrain.terrainData;

            SaveTerrainData();
        }
    }

    private void OnApplicationQuit()
    {
        LoadTerrainData();
    }

    private void Start()
    {
        if (Piece.CurrentState == StateType.Placed)
        {
            if (RemoveGrass)
            {
                if (!CheckDetailtAt(transform.position, RemoveGrassRadius))
                    return;

                StartCoroutine(RemoveGrassToPosition(transform.position, RemoveGrassRadius));
            }
        }

        BuildEvent.Instance.OnPieceInstantiated.AddListener(OnPieceInstantiated);
    }

    private void OnPieceInstantiated(PieceBehaviour part, SocketBehaviour socket)
    {
        if (part != Piece) return;

        if (RemoveGrass)
        {
            if (part.CurrentState != StateType.Placed) return;

            if (!CheckDetailtAt(transform.position, RemoveGrassRadius))
                return;

            StartCoroutine(RemoveGrassToPosition(part.transform.position, RemoveGrassRadius));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, RemoveGrassRadius);
    }

    public IEnumerator RemoveGrassToPosition(Vector3 position, float radius)
    {
        if (ActiveTerrain == null)
            yield break;

        for (int Layer = 0; Layer < ActiveTerrain.terrainData.detailPrototypes.Length; Layer++)
        {
            int TerrainDetailMapSize = ActiveTerrain.terrainData.detailResolution;

            if (ActiveTerrain.terrainData.size.x != ActiveTerrain.terrainData.size.z)
                yield break;

            float DetailSize = TerrainDetailMapSize / ActiveTerrain.terrainData.size.x;

            Vector3 TexturePoint3D = position - ActiveTerrain.transform.position;

            TexturePoint3D = TexturePoint3D * DetailSize;

            float[] Matrix = new float[4];
            Matrix[0] = TexturePoint3D.z + radius;
            Matrix[1] = TexturePoint3D.z - radius;
            Matrix[2] = TexturePoint3D.x + radius;
            Matrix[3] = TexturePoint3D.x - radius;

            int[,] Data = ActiveTerrain.terrainData.GetDetailLayer(0, 0, ActiveTerrain.terrainData.detailWidth, ActiveTerrain.terrainData.detailHeight, Layer);

            for (int y = 0; y < ActiveTerrain.terrainData.detailHeight; y++)
            {
                for (int x = 0; x < ActiveTerrain.terrainData.detailWidth; x++)
                {
                    if (Matrix[0] > x && Matrix[1] < x && Matrix[2] > y && Matrix[3] < y)
                    {
                        Data[x, y] = 0;
                    }
                }
            }

            ActiveTerrain.terrainData.SetDetailLayer(0, 0, Layer, Data);
        }
    }

    public static void SaveTerrainData()
    {
        if (Initialized) return;

        Initialized = true;

        TerrainDetails = new Dictionary<int, int[,]>();
        if (Data == null)
            return;
        for (int Layer = 0; Layer < Data.detailPrototypes.Length; Layer++)
        {
            TerrainDetails.Add(Layer, Data.GetDetailLayer(0, 0, Data.detailWidth, Data.detailHeight, Layer));
        }

#if UNITY_2019_4_OR_NEWER
        TerrainHeights = Data.GetHeights(0, 0, Data.heightmapResolution, Data.heightmapResolution);
#else
            TerrainHeights = Data.GetHeights(0, 0, Data.heightmapWidth, Data.heightmapHeight);
#endif
    }

    public static void LoadTerrainData()
    {
        
        if (Data == null)
        {
            return ;
        }
        for (int Layer = 0; Layer < Data.detailPrototypes.Length; Layer++)
        {
            Data.SetDetailLayer(0, 0, Layer, TerrainDetails[Layer]);
        }

        Data.SetHeights(0, 0, TerrainHeights);
    }

    public bool CheckDetailtAt(Vector3 position, float radius)
    {
        bool Result = false;

        if (Data == null || ActiveTerrain==null)
        {
            return Result;
        }
        
        for (int Layer = 0; Layer < Data.detailPrototypes.Length; Layer++)
        {
            int TerrainDetailMapSize = Data.detailResolution;

            float DetailSize = TerrainDetailMapSize / Data.size.x;

            Vector3 WorldPoint = position - ActiveTerrain.transform.position;

            WorldPoint *= DetailSize;

            float[] Matrix = new float[4];
            Matrix[0] = WorldPoint.z + radius;
            Matrix[1] = WorldPoint.z - radius;
            Matrix[2] = WorldPoint.x + radius;
            Matrix[3] = WorldPoint.x - radius;

            for (int y = 0; y < Data.detailHeight; y++)
            {
                for (int x = 0; x < Data.detailWidth; x++)
                {
                    if (Matrix[0] > x && Matrix[1] < x && Matrix[2] > y && Matrix[3] < y)
                    {
                        if (TerrainDetails[Layer][x, y] != 0)
                        {
                            Result = true;
                        }
                    }
                }
            }
        }

        return Result;
    }

    #endregion Methods
}