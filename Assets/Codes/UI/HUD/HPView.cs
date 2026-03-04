using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Object;

public class HPView : MonoBehaviour
{
    private PlayerHealth playerHealth;
    public GameObject HPBarObject;
    private Image[] HPBars;

    public Color activeHP = Color.white;
    public Color lostColor = Color.black;
    public Color lowHP = Color.red;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        HPBars = gameObject.GetComponentsInChildren<Image>();
        playerHealth.OnHPChanged += UpdateHP;
        playerHealth.OnMaxHPChanged += CreateHP;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateHP(int currentHP, int maxHP)
    {
        for (int i = 0; i < HPBars.Length; i++)
        {
            if (i < currentHP)
            {
                if (currentHP <= maxHP / 4)
                {
                    HPBars[i].color = lowHP;
                }
                else
                {
                    HPBars[i].color = activeHP;
                }

            }
            else
            {
                HPBars[i].color = lostColor;
            }
        }

    }
    public void CreateHP(int oldMaxHP, int newMaxHP)
    {
        for (int i = 0; i < newMaxHP - oldMaxHP; i++)
        {
            GameObject hpBar = Instantiate(HPBarObject);
            hpBar.transform.SetParent(transform);
        }
    }
}
