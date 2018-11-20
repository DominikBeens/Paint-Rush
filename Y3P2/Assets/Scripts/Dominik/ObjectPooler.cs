using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    [System.Serializable]
    public class ObjectPool
    {
        public string poolName;
        public GameObject poolPrefab;
        public int poolSize;
        public bool autoExpand;
        public Queue<GameObject> poolQueue = new Queue<GameObject>();
    }

    public static ObjectPooler instance;

    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
    public List<ObjectPool> pools = new List<ObjectPool>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        for (int i = 0; i < pools.Count; i++)
        {
            for (int ii = 0; ii < pools[i].poolSize; ii++)
            {
                GameObject newPooledObject = Instantiate(pools[i].poolPrefab, Vector3.down * 100, Quaternion.identity, transform);
                newPooledObject.SetActive(false);

                pools[i].poolQueue.Enqueue(newPooledObject);
            }

            poolDictionary.Add(pools[i].poolName, pools[i].poolQueue);
        }
    }

    public GameObject GrabFromPool(string poolName, Vector3 position, Quaternion rotation)
    {
        ObjectPool pool = null;
        for (int i = 0; i < pools.Count; i++)
        {
            if (pools[i].poolName == poolName)
            {
                pool = pools[i];
                break;
            }
        }

        if (poolDictionary[poolName].Count == 0 && pool.autoExpand)
        {
            ExpandPool(pool);
        }

        GameObject toGrab = poolDictionary[poolName].Dequeue();

        if (!pool.autoExpand)
        {
            AddToPool(poolName, toGrab);
        }

        toGrab.transform.position = position;
        toGrab.transform.rotation = rotation;

        toGrab.SetActive(true);

        return toGrab;
    }

    public void AddToPool(string poolName, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(poolName))
        {
            return;
        }

        if (!poolDictionary[poolName].Contains(obj))
        {
            obj.SetActive(false);
            poolDictionary[poolName].Enqueue(obj);
        }
    }

    private void ExpandPool(ObjectPool pool)
    {
        GameObject newPooledObject = Instantiate(pool.poolPrefab);
        newPooledObject.SetActive(false);

        pool.poolQueue.Enqueue(newPooledObject);
    }
}
