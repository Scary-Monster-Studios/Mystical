using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if AT_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif
namespace Atavism
{
    public class GetAddressableModel : MonoBehaviour
    {
        [SerializeField] private string m_Key;
        [SerializeField] Vector3 ScaleForModel = Vector3.one;
#if AT_ADDRESSABLES
         private AsyncOperationHandle<GameObject> m_Handle;

    private void Start()
    {
        if (string.IsNullOrEmpty(m_Key) == false)
        {
            m_Handle = Addressables.InstantiateAsync(m_Key);
            m_Handle.Completed += AddressableLoaded;
        }
    }

    private void AddressableLoaded(AsyncOperationHandle<GameObject> obj)
    {
        m_Handle.Completed -= AddressableLoaded;

        if (obj.Result != null)
        {
            obj.Result.transform.SetParent(this.transform);
            obj.Result.transform.localPosition = new Vector3(0, 0, 0);
            obj.Result.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            obj.Result.transform.localScale = ScaleForModel;
            obj.Result.SetActive(true);
            AtavismMobSockets ams = obj.Result.GetComponent<AtavismMobSockets>();
            if (ams != null)
            {
                AtavismMobAppearance ama = GetComponent<AtavismMobAppearance>();
                if (ama != null)
                {
                    ama.SetupSockets(ams);
                }
            }
        }
    }
#endif
    }
}