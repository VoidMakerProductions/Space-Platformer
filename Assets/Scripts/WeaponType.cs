using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "WeaponType", menuName = "Weapon Type", order = 1)]
public class WeaponType : ScriptableObject
{
    public int energyConsumtion=1;
    public GameObject projectile;
    public float bsf;
    public bool automatic;
    public float fireCooldown;
    public float batteryLocktime;
}