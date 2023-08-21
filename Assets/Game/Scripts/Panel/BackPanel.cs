using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackPanel : MonoBehaviour
{
    [SerializeField] Button backBtn;
    private bool isDestroy;
    private Panel curGameObject;

    private void Start()
    {
        backBtn.onClick.AddListener(OnBack);
    }

    private void OnBack()
    {
     //   EventManager.OnBackPanel?.Invoke(transform.parent.gameObject);
        //if(isDestroy)
        //{
        //    curGameObject.Hide();
        //}
        //else
        //{
        //    curGameObject.gameObject.SetActive(false);
        //}
        curGameObject.Hide();
    }
    public void Setup(Panel curGameObject_, bool isDestroy_)
    {
        isDestroy = isDestroy_;
        curGameObject = curGameObject_;
    }
}
