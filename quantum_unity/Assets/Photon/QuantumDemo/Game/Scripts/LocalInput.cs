using System;
using Photon.Deterministic;
using Quantum;
using UnityEngine;

public class LocalInput : MonoBehaviour {
    
  private void OnEnable() {
    QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
  }

  public void PollInput(CallbackPollInput callback) {
    Quantum.Input i = new Quantum.Input();

    i.Jump = UnityEngine.Input.GetButton("Jump");

    var x = UnityEngine.Input.GetAxis("Horizontal");
    var y = UnityEngine.Input.GetAxis("Vertical");

    // safe to convert from float to fp on unity side
    i.Direction = new Vector2(x, y).ToFPVector2();
    
    callback.SetInput(i, DeterministicInputFlags.Repeatable);
  }
}
