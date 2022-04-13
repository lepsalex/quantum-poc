using System;
using Quantum;
using UnityEngine;

public unsafe class PlayerSetup : MonoBehaviour
{
    private PlayerColor _playerColor;
    private Renderer _renderer;

    // Called from OnEntityInstantiate
    // Assigned via the inspector!
    public void Initialize()
    {
        var entityRef = GetComponent<EntityView>().EntityRef;
        var playerLink = QuantumRunner.Default.Game.Frames.Verified.Unsafe.GetPointer<PlayerLink>(entityRef);

        _playerColor = playerLink->Color;
        _renderer = GetComponentInChildren<Renderer>();

        _renderer.material.SetColor("_Color",
            new Color(convertRGB(_playerColor.R), convertRGB(_playerColor.G), convertRGB(_playerColor.B), convertRGB(_playerColor.A)));
    }

    private float convertRGB(Int32 value)
    {
        return value / 255f;
    }
}