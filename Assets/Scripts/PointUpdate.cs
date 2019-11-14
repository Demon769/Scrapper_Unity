﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointUpdate : MonoBehaviour
{
    public GameObject player;
    Agent playerAgent;

    TextMeshProUGUI amountText;
    public Vector3 pos;
    float panelWidth;
    float panelHeight;

    private void Start()
    {
        playerAgent = player.GetComponent<Agent>();
        GetComponentInChildren<RawImage>().color = playerAgent.color;
        amountText = GetComponentInChildren<TextMeshProUGUI>();

        panelWidth = Screen.width;
        panelHeight = Screen.height;
    }

    private void Update()
    {
        if (player.name == "Player1(Clone)")
        {
            transform.position = Camera.main.WorldToScreenPoint(player.transform.GetChild(0).transform.position);

            transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -(Screen.width + 20), Screen.width - 20), 100+Mathf.Clamp(transform.localPosition.y, -(Screen.height + 20), Screen.height - 20), transform.localPosition.z);
            transform.localScale = new Vector3(2, 2.5F, 2);

            amountText.text = playerAgent.amount.ToString();
        }
        else
        {
            Destroy(gameObject);
        }
    }


}
