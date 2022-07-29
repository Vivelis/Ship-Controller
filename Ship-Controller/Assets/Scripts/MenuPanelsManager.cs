using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class MenuPanelsManager : MonoBehaviour
{
    [SerializeField]
    private PanelType type;
    private bool state;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    private void UpdateState()
    {
        canvas.enabled = state;
    }

    private void ChangeState()
    {
        state = !state;
        UpdateState();
    }

    private void ChangeState(bool _state)
    {
        state |= _state;
        UpdateState();
    }

    #region Getter

    public PanelType GetPanelType() { return type; }
    
    #endregion
}