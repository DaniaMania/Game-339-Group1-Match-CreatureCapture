using System;
using Game.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerControllerView : ObserverMonoBehaviour
{
    [SerializeField] private Button _attackButton;
    [SerializeField] private Button _healButton;
    
    protected override void Subscribe()
    {
        ServiceResolver.Resolve<TurnEngine>().IsPlayerTurn.ChangeEvent += SetInteractable;
    }

    protected override void Unsubscribe()
    {
        ServiceResolver.Resolve<TurnEngine>().IsPlayerTurn.ChangeEvent -= SetInteractable;
    }

    public void AssignListeners(UnityAction PlayerAttack, UnityAction Heal)
    {
        _attackButton.onClick.AddListener(PlayerAttack);
        _healButton.onClick.AddListener(Heal);
    }

    private void SetInteractable(bool value)
    {
        _attackButton.interactable = value;
        _healButton.interactable = value;
    }
}
