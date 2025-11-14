using UnityEngine;

[CreateAssetMenu(menuName = "Weaving/RopeConfig")]
public class RopeConfig : ScriptableObject
{
    public float ropeWidth = 0.1f;
    public float swingAmount = 0.04f;
    public float swingSpeed = 6f;
    public Material ropeMaterial;
}
