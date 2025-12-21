using System.Collections;
using UnityEngine;

public class Dishwasher : MonoBehaviour
{
    [SerializeField] private GameObject glassPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float washTime = 5f;

    [Header("Force Settings")]
    [SerializeField] private float upwardForce = 2f;
    [SerializeField] private float sideForce = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Glass")) return;

        GameObject glass = other.transform.root.gameObject;
        Destroy(glass);

        StartCoroutine(SpawnGlassAfterDelay());
    }

    private IEnumerator SpawnGlassAfterDelay()
    {
        yield return new WaitForSeconds(washTime);

        GameObject newGlass = Instantiate(
            glassPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        Rigidbody rb = newGlass.GetComponent<Rigidbody>();

        Vector3 randomForce = new Vector3(
            Random.Range(-sideForce, sideForce),
            upwardForce,
            Random.Range(-sideForce, sideForce)
        );

        rb.AddForce(randomForce, ForceMode.Impulse);
    }
}
