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
        if (other.gameObject.CompareTag("Player"))
        {
            InteractTextController._setInteractionText(false, "");
        }
    }

    // <color="red">Spike Trap</color><br> They are triggered <color="yellow">shortly</color> after being stepped on.<br>The spikes <color="red">cannot</color> be dashed through.
    // <color="red">Saw Blades Trap</color><br>The blades <color="red">cannot</color> be dashed through.
    // <color="red">Turning Blade</color><br>The blades <color="green">can</color> be dashed through, in case you need to.
    // You can only fall to death in <color="red">Pitfalls</color><br>. Dash on the lightning ball to avoid falling.<br><color="yellow">Tip: </color>Stand still and aim to dash in that direction.<br>Dashing while moving will make the player dash in the movement direction.
    // <color="red">Lasers</color> move back and forth along a corridor. They <color="green">can</color> be dashed through.
    // <color="yellow">Map</color><br>Keyboard: M | Controller: Up (D-Pad)<br><color="yellow">Pause</color><br>Keyboard: P | Controller: Options/Start<br>
    // <color="red">cannot</color>
}
