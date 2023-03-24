using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPool<T> : IPool<T> where T : MonoBehaviour
{
    private readonly GameObject prefab;
    private readonly Queue<T> pool;

    public PrefabPool(GameObject prefab, int capacity)
    {
        this.prefab = prefab;
        pool = new Queue<T>(capacity);
    }

    public T Get(Transform container)
    {
        if(pool.Count > 0)
        {
            var obj = pool.Dequeue();
            obj.transform.SetParent(container);
            obj.gameObject.SetActive(true);
            return obj;
        }
        return GameObject.Instantiate(prefab, container).GetComponent<T>();
    }

    public void Release(T obj)
    {
        pool.Enqueue(obj);
        obj.gameObject.SetActive(false);
    }
}
