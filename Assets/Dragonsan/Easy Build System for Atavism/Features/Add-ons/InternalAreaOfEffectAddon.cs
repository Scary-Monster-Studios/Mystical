using EasyBuildSystem.Features.Scripts.Core.Addons;
using EasyBuildSystem.Features.Scripts.Core.Addons.Enums;
using EasyBuildSystem.Features.Scripts.Core.Base.Manager;
using UnityEngine;

[Addon("Internal Area Of Effect", AddonTarget.BuilderBehaviour)]
public class InternalAreaOfEffectAddon : AddonBehaviour
{
    #region Fields

    public bool AffectAreas = true;
    public bool AffectParts = false;
    public bool AffectSockets = true;
    public float Radius = 30f;
    public float RefreshInterval = 0.5f;

    #endregion Fields

    #region Methods

    private void Start()
    {
        InvokeRepeating("Refresh", RefreshInterval, RefreshInterval);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, Radius);
    }

    private void Refresh()
    {
        if (AffectAreas)
            for (int i = 0; i < BuildManager.Instance.CachedAreas.Count; i++)
                BuildManager.Instance.CachedAreas[i].gameObject.SetActive((Vector3.Distance(transform.position, BuildManager.Instance.CachedAreas[i].transform.position) <= Radius));

        if (AffectParts)
            for (int i = 0; i < BuildManager.Instance.CachedParts.Count; i++)
                BuildManager.Instance.CachedParts[i].gameObject.SetActive((Vector3.Distance(transform.position, BuildManager.Instance.CachedParts[i].transform.position) <= Radius));

        if (AffectSockets)
            for (int i = 0; i < BuildManager.Instance.CachedSockets.Count; i++)
                BuildManager.Instance.CachedSockets[i].gameObject.SetActive(Vector3.Distance(transform.position, BuildManager.Instance.CachedSockets[i].transform.position) <= Radius);
    }

    #endregion Methods
}