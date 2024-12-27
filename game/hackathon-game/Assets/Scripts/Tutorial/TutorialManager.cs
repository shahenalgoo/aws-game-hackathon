using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [SerializeField] private float _tileSize = 8f;
    [SerializeField] private TutorialBoxSpecs[] _tutorialBoxSpecs;
    [SerializeField] private GameObject _tutorialBox;
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

    void Start()
    {
        CreateTutorialBoxes();
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

    public void CreateTutorialBoxes()
    {
        for (int i = 0; i < _tutorialBoxSpecs.Length; i++)
        {
            // Vector3 worldPos = new Vector3(_tutorialBoxSpecs[i]._gridPosition.x * _tileSize, 0, _tutorialBoxSpecs[i]._gridPosition.y * _tileSize);
            // GameObject box = Instantiate(_tutorialBox, worldPos, Quaternion.identity);


            GameObject box = Instantiate(_tutorialBox, _tutorialBoxSpecs[i]._worldPosition, Quaternion.identity);
            box.transform.localScale = new Vector3(_tutorialBoxSpecs[i]._size.x * _tileSize, 1, _tutorialBoxSpecs[i]._size.y * _tileSize);
            box.GetComponent<TutorialBox>().TutorialInfo = _tutorialBoxSpecs[i]._info;
        }
    }
}

[Serializable]
public class TutorialBoxSpecs
{
    public Vector3 _worldPosition;
    public Vector2 _size;
    public string _info;
}
