using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour {


    [SerializeField] private GameObject HasProgressGameObject;
    [SerializeField] private Image progressBarImage;

    private IHasProgress hasProgress;
    private void Start()
    {
        hasProgress = HasProgressGameObject.GetComponent<IHasProgress>();
        if (hasProgress == null)
        {
            Debug.LogError("HasProgressGameObject does not have a component that implements IHasProgress!");
            return;
        }
        hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;
        progressBarImage.fillAmount = 0f;
        Hide();
    }

    private void HasProgress_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        
        progressBarImage.fillAmount = e.progressNormalized;

        if (e.progressNormalized > 0f && e.progressNormalized < 1f)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
