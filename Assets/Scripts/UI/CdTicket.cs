using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CdTicket : NetworkBehaviour
{
    public int tableNumber;
    public Dictionary<string, int> drinks = new Dictionary<string, int>();
    public bool isUsed;
}
