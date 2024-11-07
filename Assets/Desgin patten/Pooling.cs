using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class Pooling
{
       public static Dictionary<int, Pool> pools;
    
    public static void Init(GameObject prefab = null)
    {
        if (pools == null)
        {
            pools = new Dictionary<int, Pool>();
        }
    
        if (prefab != null)
        {
            var prefabID = prefab.GetInstanceID();
            if (!pools.ContainsKey(prefabID))
            {
                pools[prefabID] = new Pool(prefab);
            }
        }
    }
    // Spawn ra 1 prefab
    public static GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        Init(prefab);
        return pools[prefab.GetInstanceID()].Spawn(pos, rot);
    }
    
    public static GameObject Spawn(GameObject prefab)
    {
        return Spawn(prefab, Vector3.zero, Quaternion.identity);
    }
    
    public static T Spawn<T>(T prefab) where T : Component
    {
        return Spawn(prefab, Vector3.zero, Quaternion.identity);
    }
    
    public static T Spawn<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component
    {
        Init(prefab.gameObject);
        return pools[prefab.gameObject.GetInstanceID()].Spawn<T>(pos, rot);
    }
    
    // Trả 1 GameObject
    public static void Despawn(GameObject gameObject, UnityAction actionDespawn = null)
    {
        Pool p = null;
        foreach (var pool in pools.Values)
        {
            if (pool.memberIDs.Contains(gameObject.GetInstanceID()))
            {
                p = pool;
                break;
            }
        }

        if (p == null)
        {
            Debug.LogFormat("Object '{0}' wasn't spawned from a pool. Destroying it instead.", gameObject.name);
            Object.Destroy(gameObject);
        }
        else
        {
            actionDespawn?.Invoke();
            p.Despawn(gameObject);
        }
    }
    // Clear Dictionary
    public static void ClearPool()
    {
        if (pools != null)
        {
            pools.Clear();
        }
    }
}

public class Pool
{
    private int id = 1;
    
    private readonly Queue<GameObject> notActiveQueue;
    
    public readonly HashSet<int> memberIDs;
    
    private readonly GameObject prefab;

    // Constructor
    public Pool(GameObject prf)
    {
        prefab = prf;
        notActiveQueue = new Queue<GameObject>();
        memberIDs = new HashSet<int>();
    }
    public GameObject Spawn(Vector3 pos, Quaternion rot)
    {
        while (true)
        {
            GameObject gameObject;
            if (notActiveQueue.Count == 0)
            {
                gameObject = Object.Instantiate(prefab, pos, rot);
                gameObject.name = prefab.name + " " + id;
                memberIDs.Add(gameObject.GetInstanceID());
            }
            else
            {
                // Lấy ra GameObject cuối cùng 
                gameObject = notActiveQueue.Dequeue();
                if (gameObject == null)
                {
                    continue;
                }
            }

            gameObject.transform.position = pos;
            gameObject.transform.rotation = rot;
            gameObject.SetActive(true);
            return gameObject;
        }
    }

    public T Spawn<T>(Vector3 pos, Quaternion rot)
    {
        return Spawn(pos, rot).GetComponent<T>();
    }

    // Đưa đối tượng về lại pool để tái sử dụng
    public void Despawn(GameObject gameObject)
    {
        // Nếu GameObject đó đã tắt rồi thì thôi
        if (!gameObject.activeSelf)
        {
            return;
        }
    
        // Còn không thì tắt nó rồi trả nó về hàng đợi
        gameObject.SetActive(false);
        notActiveQueue.Enqueue(gameObject);
    }
}