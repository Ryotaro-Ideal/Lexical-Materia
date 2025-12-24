using UnityEngine;
using System.Collections.Generic;
public class CraftInvManager : MonoBehaviour
{
    public static CraftInvManager Instance { get; private set; }
    [SerializeField] CraftSlotManager[] craftSlots;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
        craftSlots = FindObjectsByType<CraftSlotManager>(FindObjectsSortMode.None);
    }

    // Update is called once per frame
    void Update()
    {

    }

}