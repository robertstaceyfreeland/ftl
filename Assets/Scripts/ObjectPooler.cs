using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string _Tag;
        public GameObject _PREFAB;
        public int _Size;
    }

    public List<Pool> _Pools;

    public static ObjectPooler Instance;

    public Dictionary<string, Queue<GameObject>> _PoolDictionary;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        _PoolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool _Pool in _Pools)
        {
            Queue<GameObject> _ObjectPool = new Queue<GameObject>();

            for (int i = 0; i < _Pool._Size ; i++)
            {
                GameObject _GameObject = Instantiate(_Pool._PREFAB);
                _GameObject.SetActive(false);
                _ObjectPool.Enqueue(_GameObject);
            }

            _PoolDictionary.Add(_Pool._Tag, _ObjectPool);
                    
        }
    }

    public GameObject SpawnFromPool (string pTag, Vector3 pPosition, Quaternion pRotation)
    {
        if (!_PoolDictionary.ContainsKey(pTag)) return null;
        
        GameObject _ObjectToSpawn = _PoolDictionary[pTag].Dequeue();
        _ObjectToSpawn.SetActive(true);
        _ObjectToSpawn.transform.position = pPosition;
        _ObjectToSpawn.transform.rotation = pRotation;

        IPooledObject _IPooledObjects = _ObjectToSpawn.GetComponent<IPooledObject>();

        if(_IPooledObjects != null)
        {
            _IPooledObjects.OnObjectSpawn();
        }

        _PoolDictionary[pTag].Enqueue(_ObjectToSpawn);

        return _ObjectToSpawn;
    }

   
}
