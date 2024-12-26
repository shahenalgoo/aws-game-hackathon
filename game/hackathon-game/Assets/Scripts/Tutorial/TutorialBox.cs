using UnityEngine;

public class TutorialBox : MonoBehaviour
{
    [SerializeField] private string _tutorialInfo;
    public string TutorialInfo { get { return _tutorialInfo; } set { _tutorialInfo = value; } }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            InteractTextController._setInteractionText(true, _tutorialInfo);
        }

    }

    void OnTriggerExit(Collider other)
    {
        InteractTextController._setInteractionText(false, "");
    }
}
