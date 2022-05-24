using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool
{
    public int max = 100;
    public int start = 5;
    public PoolObject prefab;
    public Transform initialParent;

    List<PoolObject> pool;

    public ObjectPool(PoolObject prefab)
    {
        this.prefab = prefab;
        Initialize();
    }

    public ObjectPool(PoolObject prefab, int start, int max)
    {
        this.prefab = prefab;
        this.start = start;
        this.max = max;
        Initialize();
    }

    public ObjectPool(PoolObject prefab, int start, int max, Transform parent)
    {
        this.initialParent = parent;
        this.prefab = prefab;
        this.start = start;
        this.max = max;
        Initialize();
    }


    public void Initialize()
    {
        if (prefab == null)
        {
            Debug.LogError("Missing prefab");
            return;
        }

        pool = new List<PoolObject>(start * 2);
        for (int i = 0; i < start; i++)
        {
            var instance = GameObject.Instantiate(prefab);
            instance.gameObject.SetActive(false);

            //instance.transform.SetParent(initialParent);
            instance.Age = i;
            pool.Add(instance);
        }
    }

    public PoolObject Retrieve()
    {
        if (pool == null)
        {
            Initialize();
        }

        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].IsAvailable())
            {
                pool[i].MakeAvailable();
                return pool[i];
            }
        }

        if (prefab != null)
        {
            var instance = GameObject.Instantiate(prefab);
            instance.MakeAvailable();
            pool.Add(instance);
            return instance;
        }

        return null;
    }

    public PoolObject Recycle()
    {
        if (pool == null)
        {
            Initialize();
        }

        int oldestIndex = 0;
        int oldestAge = int.MinValue;
        int youngestAge = int.MaxValue;
        for (int i = 0; i < pool.Count; i++)
        {
            youngestAge = Mathf.Min(pool[i].Age, youngestAge);
            pool[i].Age += 1;

            if (pool[i].Age > oldestAge)
            {
                oldestIndex = i;
                oldestAge = pool[i].Age;
            }
        }

        var oldest = pool[oldestIndex];
        oldest.MakeAvailable();
        oldest.Age = youngestAge;
        
        return oldest;
    }

    public List<PoolObject> RetrieveAll()
    {
        return pool;
    }

    public void Empty()
    {
        for (int i = 0; i < pool.Count; i++)
            GameObject.Destroy(pool[i]);

        pool.Clear();
    }
}
