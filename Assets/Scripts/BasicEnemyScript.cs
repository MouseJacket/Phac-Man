using UnityEngine;
using UnityEngine.AI;
public class BasicEnemyScript : MonoBehaviour
{
    [SerializeField] private Transform player;
    private NavMeshAgent _navAgent;
    void Start()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        player = FindFirstObjectByType<PlayerController>().transform;
    }

    // Update is called once per framed
    void Update()
    {
        _navAgent.SetDestination(player.position);
    }
}
