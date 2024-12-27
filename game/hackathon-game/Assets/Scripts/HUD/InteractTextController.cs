using System;
using TMPro;
using UnityEngine;

public class InteractTextController : MonoBehaviour
{

    public static Action<bool, string> _setInteractionText;
    public TextMeshProUGUI _interactionText;
    void Awake()
    {
        _setInteractionText += ShowText;
        _interactionText = GetComponent<TextMeshProUGUI>();
        if (gameObject.activeSelf) gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        _setInteractionText -= ShowText;
    }

    private void ShowText(bool show, string text = "")
    {
        if (show)
        {
            _interactionText.text = text;
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
