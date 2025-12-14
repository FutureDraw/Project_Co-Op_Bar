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

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
            Destroy(gameObject);
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
            currentTable.Free();
            Destroy(gameObject);
        }
    }

    // ?????????? ??????????
    public void TakeOrder()
    {
        EnterState(ClientState.WaitingForDrink);
    }
}
