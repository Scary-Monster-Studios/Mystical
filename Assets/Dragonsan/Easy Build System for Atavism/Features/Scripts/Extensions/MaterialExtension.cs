using System.Collections.Generic;
using UnityEngine;

namespace EasyBuildSystem.Features.Scripts.Extensions
{
    public static class MaterialExtension
    {
        #region Methods

        public static void ChangeAllMaterialsColorInChildren(this GameObject go, Renderer[] renderers, Color color, float lerpTime = 15.0f, bool lerp = false)
        {
            // Debug.LogError("ChangeAllMaterialsColorInChildren "+go.scene.name);
            if (go != null && go.scene != null && go.scene.name != null)
            {
                Renderer[] Renderers = go.GetComponentsInChildren<Renderer>();

                for (int i = 0; i < Renderers.Length; i++)
                {
                    if (Renderers[i] != null)
                    {

                        for (int x = 0; x < Renderers[i].materials.Length; x++)
                        {
                            if (lerp)
                            {
                                Renderers[i].materials[x].color = Color.Lerp(Renderers[i].materials[x].color, color, lerpTime * Time.deltaTime);
                            }
                            else
                            {
                                Renderers[i].materials[x].color = color;
                            }
                        }
                    }
                }
            }
        }

        public static void ChangeAllMaterialsInChildren(this GameObject go, Renderer[] renderers, Material material)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    Material[] materials = new Material[renderers[i].sharedMaterials.Length];

                    for (int x = 0; x < renderers[i].sharedMaterials.Length; x++)
                    {
                        materials[x] = material;
                    }

                    renderers[i].sharedMaterials = materials;
                }
            }
        }

        public static void ChangeAllMaterialsInChildren(this GameObject go, Renderer[] renderers, Dictionary<Renderer, Material[]> materials)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] CacheMaterials = renderers[i].sharedMaterials;

                for (int c = 0; c < CacheMaterials.Length; c++)
                {
                    CacheMaterials[c] = materials[renderers[i]][c];
                }

                renderers[i].materials = CacheMaterials;
            }
        }

        #endregion
    }
}