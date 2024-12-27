using UnityEngine;

public class TutorialExtractionActivator : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ExtractionController _extractor = FindObjectOfType<ExtractionController>();
            _extractor.ActivatePlatform();
        }

    }
}
