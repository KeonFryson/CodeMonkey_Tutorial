using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;
    private void Start()
    {
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
        if (baseCounter == null)
        {
            Debug.LogError("BaseCounter is not assigned in the inspector for " + gameObject.name);
        }
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        if (e.selectedCounter == baseCounter)
        {
            foreach(GameObject visualGameObject in visualGameObjectArray)
                visualGameObject.SetActive(true);
        }
        else
        {
            foreach (GameObject visualGameObject in visualGameObjectArray)
                visualGameObject.SetActive(false);
        }
    }
}
