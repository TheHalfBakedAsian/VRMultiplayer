using Unity.Netcode;
using UnityEngine;
[RequireComponent(typeof(Light))]
public class Flashlight : NetworkBehaviour

{
    private Light m_light;
    private NetworkVariable<bool> state = new NetworkVariable<bool>(false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);
    void Awake()
    {
        m_light = GetComponent<Light>();
        m_light.enabled = false;
        state.OnValueChanged += OnStateValueChanged;
        state.Value = false;
    }

    public void ToggleLight()
    {
        state.Value = !state.Value;
    }
    private void OnStateValueChanged(bool previousValue, bool newValue)
    {
        UpdateBoolState();
    }
    private void UpdateBoolState()
    {
        m_light.enabled = state.Value;
    }

    void Update()
    {
        
    }
}
