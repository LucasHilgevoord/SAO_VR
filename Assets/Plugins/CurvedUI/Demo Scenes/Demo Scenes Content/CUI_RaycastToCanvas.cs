﻿using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

namespace CurvedUI
{
    public class CUI_RaycastToCanvas : MonoBehaviour
    {

        CurvedUISettings mySettings;

        // Use this for initialization
        void Start()
        {
            mySettings = GetComponentInParent<CurvedUISettings>();
        }

        // Update is called once per frame
        void Update()
        {

            Vector2 pos = Vector2.zero;
            mySettings.RaycastToCanvasSpace(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out pos);
            this.transform.localPosition = pos;

        }
    }
}
