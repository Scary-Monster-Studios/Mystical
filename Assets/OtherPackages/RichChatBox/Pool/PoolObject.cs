using UnityEngine;
using System.Collections;

public class PoolObject : MonoBehaviour {

    int age;

    public int Age
    {
        get { return age; }
        set { age = value; }
    }

    public virtual bool IsAvailable()
    {
        return !this.gameObject.activeSelf;
    }

    public virtual void MakeAvailable()
    {
        this.gameObject.SetActive(false);
    }
}
