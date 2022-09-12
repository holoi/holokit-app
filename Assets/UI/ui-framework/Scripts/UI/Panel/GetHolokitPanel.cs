﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.HoloKit.App.UI
{
    public class GetHolokitPanel : BasePanel
    {
        static readonly string _path = "Prefabs/UI/Panels/GetHolokitPanel";
        public GetHolokitPanel() : base(new UIType(_path)) { }

        public override void OnOpen()
        {
            if (UITool.FindChildGameObject("BackButton"))
            {
                Debug.Log("find button");
            }
            else
            {
                Debug.Log("not find button");
            }
            UITool.FindChildGameObject("BackButton").GetComponent<Button>().onClick.AddListener(() =>
            { 
                // here we do onclick event of this button
                Debug.Log("BackButton is clicked.");
                PanelManager.Pop();
            });
            UITool.FindChildGameObject("Order Button").GetComponent<Button>().onClick.AddListener(() =>
            {
                // here we do onclick event of this button
                Debug.Log("Order Button is clicked.");
            });
        }
    }
}