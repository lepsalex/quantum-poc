using System;
using Quantum;
using UnityEngine;

public unsafe class PlayerSetup : MonoBehaviour
{
    private PlayerLink* _playerLink;
    private PlayerColor _playerColor;
    private Renderer _renderer;

    public void Awake()
    {
        QuantumEvent.Subscribe<EventPlayerDataUpdate>(listener: this, handler: HandlePlayerDataUpdate);
    }

    // Called from OnEntityInstantiate
    // Assigned via the inspector!
    public void Initialize()
    {
        var entityRef = GetComponent<EntityView>().EntityRef;
        _playerLink = QuantumRunner.Default.Game.Frames.Verified.Unsafe.GetPointer<PlayerLink>(entityRef);
        _renderer = GetComponentInChildren<Renderer>();
        SetPlayerColor();
    }

    private void HandlePlayerDataUpdate(EventPlayerDataUpdate e)
    {
        if (_playerLink->PlayerRef.Equals(e.player))
        {
            SetPlayerColor();
        }
    }
    
    private void SetPlayerColor()
    {
        _playerColor = _playerLink->Color;
        _renderer.material.SetColor("_Color",
            new Color(convertRGB(_playerColor.R), convertRGB(_playerColor.G), convertRGB(_playerColor.B),
                convertRGB(_playerColor.A)));
    }

    private float convertRGB(Int32 value)
    {
        return value / 255f;
    }
}