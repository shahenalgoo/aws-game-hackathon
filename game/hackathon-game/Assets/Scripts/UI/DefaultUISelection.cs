using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// UNUSED
public class DefaultUISelection : MonoBehaviour
{
    private void OnEnable()
    {
        SetAsDefaultSelection();
    }

    private void Update()
    {
        // If nothing is selected, reselect this object
        if (EventSystem.current && EventSystem.current.firstSelectedGameObject == null)
        {
            SetAsDefaultSelection();
        }
    }

    private void SetAsDefaultSelection()
    {
        if (EventSystem.current != null && gameObject.activeInHierarchy)
        {
            // Debug.Log("event system exists");
            EventSystem.current.firstSelectedGameObject = gameObject;
            gameObject.GetComponent<Button>().Select();
        }
    }

    // Optional: If you want to force selection when this object is enabled
    private void OnDisable()
    {
        if (EventSystem.current && EventSystem.current.currentSelectedGameObject == gameObject)
        {
            // Debug.Log("set selected null");

            EventSystem.current.firstSelectedGameObject = null;
        }
    }
}
