using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [SerializeField] private PlayerHealth _playerHealth;
    void SingletonCheck()
    {
        // If there is an instance, and it's not this one, delete this one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set the instance
        Instance = this;
    }

    private void Awake()
    {
        SingletonCheck();

        // Clear possible previous states
        Time.timeScale = 1f;
        if (AudioManager.Instance != null) AudioManager.Instance.PauseAudio(false);
    }

    public void EndTutorial()
    {
        Debug.Log("tutorial ended, back to menu");
        SceneManager.LoadScene(SceneIndexes.MainMenuSceneIndex);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public IEnumerator RefillHealth()
    {
        yield return new WaitForSeconds(2f);
        _playerHealth.SetMaxHealth();
    }



}
