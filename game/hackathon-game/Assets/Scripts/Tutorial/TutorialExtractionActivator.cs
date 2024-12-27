using UnityEngine;

public class TutorialExtractionActivator : MonoBehaviour
{

    private bool _platformOn;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !_platformOn)
        {
            ExtractionController _extractor = FindObjectOfType<ExtractionController>();
            _extractor.ActivatePlatform();
            _platformOn = true;
        }

    }
}
