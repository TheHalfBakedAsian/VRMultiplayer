using Unity.Netcode;
using UnityEngine;

public class MaterialSync : NetworkBehaviour
{
    private MeshRenderer meshrenderer;
    private Material orginalMaterial;
    [SerializeField] private Material OtherMaterial;
    private NetworkVariable<bool> usingAltMaterial = new NetworkVariable<bool>(false, 
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);


    void Start()
    {
        meshrenderer = GetComponent<MeshRenderer>();
        orginalMaterial = meshrenderer.material;
        usingAltMaterial.OnValueChanged += OnusingAltMaterialChanged;
    }
    private void OnusingAltMaterialChanged(bool previousValue, bool newValue)
    {
        flipMaterial();
    }
    private void flipMaterial()
    {
        if(usingAltMaterial.Value) { meshrenderer.material = OtherMaterial; }
        else { meshrenderer.material = orginalMaterial; }
    }
    public void flipMaterialValue()
    {
        usingAltMaterial.Value = !usingAltMaterial.Value;
    }
    public void SetOriginal()
    {
        usingAltMaterial.Value = false;
    }
    public void SetAlternate()
    {
        usingAltMaterial.Value = true;
    }
}
