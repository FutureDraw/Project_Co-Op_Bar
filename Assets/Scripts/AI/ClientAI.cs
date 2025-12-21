using UnityEngine;
using UnityEngine.AI;

public class ClientAI : MonoBehaviour
{
    public ClientState state;

    public float waitForTableTime = 40f;
    public float orderTime = 30f;

    private float timer;

    private NavMeshAgent agent;
    private Table currentTable;

    [Header("Spawn / Exit")]
    public Transform spawnPoint;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
        }

        EnterState(ClientState.FreeSeat);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        switch (state)
        {
            case ClientState.FreeSeat:
                UpdateFreeSeat();
                break;

            case ClientState.MovingToTable:
                UpdateMovingToTable();
                break;

            case ClientState.Ordering:
                UpdateOrdering();
                break;
            
            case ClientState.Served:
                UpdateLeaving();
                break;
        }
    }

    void EnterState(ClientState newState)
    {
        state = newState;

        if (state == ClientState.FreeSeat)
            timer = waitForTableTime;

        if (state == ClientState.Ordering)
            timer = orderTime;
    }

    void UpdateFreeSeat()
    {
        Table freeTable = TableManager.Instance.GetFreeTable();

        if (freeTable != null)
        {
            currentTable = freeTable;
            currentTable.Occupy();
            agent.SetDestination(currentTable.seatPoint.position);
            EnterState(ClientState.MovingToTable);
            return;
        }

        if (timer <= 0)
        {
            OrderScaleManager.Instance.AddPoints(-10);

            Leave(false);
        }
    }

    void UpdateMovingToTable()
    {
        if (agent.remainingDistance < 0.2f)
        {
            EnterState(ClientState.Ordering);
        }
    }

    void UpdateOrdering()
    {
        if (timer <= 0)
        {
            OrderScaleManager.Instance.AddPoints(-15);
            Leave(false);
        }
    }

    void Leave(bool wasServed)
    {
        if (currentTable != null)
        {
            currentTable.SpawnMainTrash();

            if (!wasServed)
            {
                int extraTrash = Random.Range(0, 3);
                // Можно спавнить мусор вручную, если нужно
                for (int i = 0; i < extraTrash; i++)
                {
                    Vector3 pos = currentTable.transform.position + Random.insideUnitSphere * 1.2f;
                    pos.y = currentTable.transform.position.y;

                    GameObject t = Instantiate(currentTable.trashPrefab, pos, Quaternion.identity);
                    Trash trash = t.GetComponent<Trash>();
                    trash.blocksTable = false; // не блокирует стол
                }
            }

            //currentTable.Free();
        }

        agent.SetDestination(spawnPoint.position);
        state = ClientState.Served;
    }

    void UpdateLeaving()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            Destroy(gameObject);
        }
    }

    // ?????????? ??????????
    public void TakeOrder()
    {
        EnterState(ClientState.WaitingForDrink);
    }
}
