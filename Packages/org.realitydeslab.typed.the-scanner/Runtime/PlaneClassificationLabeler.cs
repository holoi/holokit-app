﻿// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Sizheng Hao <sizheng@reality.design>
// SPDX-License-Identifier: MIT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace RealityDesignLab.Typed.TheScanner
{
    /// <summary>
    /// Manages the label and plane material color for each recognized plane based on
    /// the PlaneClassification enumeration defined in ARSubsystems.
    /// </summary>

    [RequireComponent(typeof(ARPlane))]
    [RequireComponent(typeof(MeshRenderer))]
    public class PlaneClassificationLabeler : MonoBehaviour
    {
        public List<Material> _mats = new List<Material>();
        ARPlane m_ARPlane;
        MeshRenderer m_PlaneMeshRenderer;
        TextMesh m_TextMesh;
        GameObject m_TextObj;
        Vector3 m_TextFlipVec = new Vector3(0, 180, 0);

        void Awake()
        {
            m_ARPlane = GetComponent<ARPlane>();
            m_PlaneMeshRenderer = GetComponent<MeshRenderer>();

            // Setup label
            m_TextObj = new GameObject();
            m_TextMesh = m_TextObj.AddComponent<TextMesh>();
            m_TextMesh.characterSize = 0.05f;
            m_TextMesh.color = Color.black;
        }

        void Update()
        {
            UpdateLabel();
            UpdatePlaneColor();
        }

        void UpdateLabel()
        {
            // Update text
            m_TextMesh.text = m_ARPlane.classification.ToString();

            // Update Pose
            m_TextObj.transform.position = m_ARPlane.center;
            m_TextObj.transform.LookAt(Camera.main.transform);
            m_TextObj.transform.Rotate(m_TextFlipVec);
        }

        void UpdatePlaneColor()
        {
            Color planeMatColor = Color.cyan;
            Material _cMat = _mats[0];
            switch (m_ARPlane.classification)
            {
                case PlaneClassification.None:
                    _cMat = _mats[0];
                    break;
                case PlaneClassification.Wall:
                    _cMat = _mats[1];
                    break;
                case PlaneClassification.Floor:
                    _cMat = _mats[2];
                    break;
                case PlaneClassification.Ceiling:
                    _cMat = _mats[3];
                    break;
                case PlaneClassification.Table:
                    _cMat = _mats[4];
                    break;
                case PlaneClassification.Seat:
                    _cMat = _mats[5];
                    break;
                case PlaneClassification.Door:
                    _cMat = _mats[6];
                    break;
                case PlaneClassification.Window:
                    _cMat = _mats[7];
                    break;
            }

            //planeMatColor.a = 0.33f;
            m_PlaneMeshRenderer.material = _cMat;
        }

        void OnDestroy()
        {
            Destroy(m_TextObj);
        }
    }
}