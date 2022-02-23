using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public PlayerAbilityButtonSlot slot;
    [SerializeField] private AOEWeapon ability;
    [SerializeField] private bool usingNow;
    private PlayerInput _playerInput;
    private Material _material;
    private bool _dragNow;
    private float maxDistanceToClamp = 64;

    private void Awake()
    {
        _material = GetComponent<Image>().material;
        _playerInput = FindObjectOfType<PlayerInput>();
    }

    /*public bool UsingNow
    {
        get => usingNow;
        set
        {
            usingNow = value;
            _material.SetInt("_AbilityUsingNow", value ? 1 : 0);
        }
    }*/

    private void Update()
    {
        _material.SetInt("_AbilityUsingNow", (_playerInput.currentAbility == ability) ? 1 : 0);
    }

    public void PickAbility()
    {
        if (_dragNow)
        {
            _dragNow = false;
            return;
        }
        
        _playerInput.currentAbility = ability;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        //throw new NotImplementedException();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragNow = true;
        transform.parent = slot.transform.parent;
        slot.Content = null;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        PlayerAbilityButtonSlot closestSlot = AbilitiesPanel.Instance.abilitySlots[0];

        foreach (var temp in AbilitiesPanel.Instance.abilitySlots)
        {
            if (Vector3.Distance(closestSlot.transform.position, transform.position) >
                Vector3.Distance(temp.transform.position, transform.position))
                closestSlot = temp;
        }

        if (Vector3.Distance(closestSlot.transform.position, transform.position) < maxDistanceToClamp)
        {
            closestSlot.Content = this;
        }
        else
        {
            slot.Content = this;
        }
    }
}