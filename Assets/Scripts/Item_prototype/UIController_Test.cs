using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController_Test : MonoBehaviour
{
    public GameObject hpText;
    private PlayerStatus playerStatus;

    void Start()
    {
        playerStatus = GameObject.Find("Player").GetComponent<PlayerStatus>();
    }

    void Update()
    {
        hpText.GetComponent<TMPro.TextMeshProUGUI>().text = "HP : " + playerStatus.currentHealth;
    }
}
