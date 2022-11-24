using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Winner : MonoBehaviour
{
    public static Winner Instance;
    public GameObject bg;
    public TextMeshProUGUI text;
    private void Awake()
    {
        Instance = this;
    }

    public void SetWinner(string win)
    {
        bg.SetActive(true);
        text.text = $"Winner: {win}!";
    }
}
