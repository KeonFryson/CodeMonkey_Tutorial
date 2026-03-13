using UnityEngine;

[CreateAssetMenu(fileName = "KitchenObject", menuName = "ScriptableObjects/KitchenObject")]
public class KitchenObjectSO : ScriptableObject
{
    public Transform prefab;
    public Sprite sprite;
    public string objectName;
}
