using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class ClientAI : MonoBehaviour
{
    public ClientState state;

    [Header("Timers")]
    public float waitForTableTime = 40f;
    public float orderTime = 30f;

    private float timer;

    private NavMeshAgent agent;
    private Table currentTable;

    [Header("Spawn / Exit")]
    public Transform spawnPoint;

    [Header("Order System")]
    public string[] orderPhrases =
    {
        "Сегодня такой хороший день, дайте мне {0}",
        "Можно, пожалуйста, {0}",
        "Я буду {0}",
        "Хочу {0}"
    };

    public string[] drinks =
    {
        "напиток 1",
        "напиток 2",
        "напиток 3",
        "напиток 4"
    };

    private List<string> orderedDrinks = new List<string>();
    private string orderText;

    [Header("UI")]
    public GameObject orderBubble;          // текст заказа
    public TextMeshPro orderTextMesh;
    public TextMeshPro timerTextMesh;       // ⏱ ТАЙМЕР

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        transform.position = spawnPoint.position;

        orderBubble.SetActive(false);
        timerTextMesh.text = "";

        EnterState(ClientState.FreeSeat);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        // 🔢 ОБНОВЛЯЕМ ТАЙМЕР ВСЕГДА
        UpdateTimerText();

        switch (state)
        {
            case ClientState.FreeSeat:
                UpdateFreeSeat();
                break;

            case ClientState.MovingToTable:
                UpdateMovingToTable();
                break;

            case ClientState.Ordering:
                if (timer <= 0)
                {
                    HideOrder();
                    Leave();
                }
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
        {
            timer = orderTime;
            GenerateOrder();
        }
    }

    void UpdateFreeSeat()
    {
        Table freeTable = TableManager.Instance.GetFreeTable();
        if (freeTable == null) return;

        currentTable = freeTable;
        currentTable.Occupy();

        agent.SetDestination(currentTable.seatPoint.position);
        EnterState(ClientState.MovingToTable);
    }

    void UpdateMovingToTable()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            agent.isStopped = true;
            EnterState(ClientState.Ordering);
        }
    }

    void UpdateLeaving()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
            Destroy(gameObject);
    }

    void Leave()
    {
        currentTable.SpawnMainTrash();
        agent.isStopped = false;
        agent.SetDestination(spawnPoint.position);
        state = ClientState.Served;
    }

    // =========================
    // ЗАКАЗ
    // =========================

    void GenerateOrder()
    {
        orderedDrinks.Clear();

        int count = Random.Range(1, 3);
        while (orderedDrinks.Count < count)
        {
            string drink = drinks[Random.Range(0, drinks.Length)];
            if (!orderedDrinks.Contains(drink))
                orderedDrinks.Add(drink);
        }

        string drinksText = string.Join(" и ", orderedDrinks);
        string phrase = orderPhrases[Random.Range(0, orderPhrases.Length)];

        orderText = string.Format(phrase, drinksText);
    }

    void ShowOrder()
    {
        orderBubble.SetActive(true);
        orderTextMesh.text = orderText;
    }

    void HideOrder()
    {
        orderBubble.SetActive(false);
    }

    // =========================
    // ТАЙМЕР (ЧИСЛА)
    // =========================

    void UpdateTimerText()
    {
        if (timer <= 0)
        {
            timerTextMesh.text = "";
            return;
        }

        int secondsLeft = Mathf.CeilToInt(timer);
        timerTextMesh.text = secondsLeft.ToString();
    }


    // =========================
    // ТРИГГЕР
    // =========================

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (state == ClientState.Ordering)
            ShowOrder();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        HideOrder();
    }
}
