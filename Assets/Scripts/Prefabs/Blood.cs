using UnityEngine;

public class Blood : MonoBehaviour, IPooledObject
{
    float DisableTime = 0;
    
    private void Update()
    {
        if (DisableTime < Time.time) gameObject.SetActive(false);
    }

    public void OnObjectSpawn()
    {
        DisableTime = UnityEngine.Random.Range(2f, 6f);
        DisableTime += Time.time;
    }

    

}
