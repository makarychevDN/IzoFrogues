using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrePushVisualization : MonoBehaviour
{
    [SerializeField] private List<TargetWithDirection> arrows;
    [SerializeField] private Vector2IntContainer prePushValueContainer;

    void Update()
    {
        arrows.ForEach(arrow => arrow.target.SetActive(arrow.direction == prePushValueContainer.Content));
    }

    [Serializable]
    public struct TargetWithDirection
    {
        public GameObject target;
        public Vector2Int direction;
    }
}

