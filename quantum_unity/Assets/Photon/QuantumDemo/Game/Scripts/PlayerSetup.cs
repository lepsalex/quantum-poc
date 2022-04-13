using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Quantum;
using UnityEngine;

public unsafe class PlayerSetup : MonoBehaviour
{
    private PlayerColor _playerColor;
    private Renderer _renderer;

    public void Initialize()
    {
        var entityRef = GetComponent<EntityView>().EntityRef;
        var playerLink = QuantumRunner.Default.Game.Frames.Verified.Unsafe.GetPointer<PlayerLink>(entityRef);

        _playerColor = playerLink->Color;
        _renderer = GetComponentInChildren<Renderer>();

        _renderer.material.SetColor("_Color",
            new Color(_playerColor.R.AsFloat, _playerColor.G.AsFloat, _playerColor.B.AsFloat, _playerColor.A.AsFloat));
    }
}