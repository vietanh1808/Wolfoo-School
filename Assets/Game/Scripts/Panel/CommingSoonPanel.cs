using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommingSoonPanel : Panel
{
    [SerializeField] Button exitBtn;

    private void Start()
    {
        exitBtn.onClick.AddListener(OnBack);
    }

    private void OnBack()
    {
        base.Hide();
    }
}
