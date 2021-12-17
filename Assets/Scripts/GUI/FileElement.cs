using Assets.Scripts.Events;
using Assets.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FileElement : MonoBehaviour
{
    public int SaveSlot;
    public GameObject Text;
    public GameData GameData;

    private SaveData _saveData;
    // Start is called before the first frame update
    void Start()
    {
        SaveData data = null;
        if(SaveManager.Instance.Load(SaveSlot, out data))
        {
            Text.SetActive(false);
            _saveData = data;
        }
        else
            Text.SetActive(true);
    }

    public void StartGame()
    {
        if (_saveData != null)
        {
            StartCoroutine(LoadGameAsync());
        }
        else
        {
            StartCoroutine(NewGameAsync());
        }
    }

    IEnumerator LoadGameAsync()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        yield return new WaitForSecondsRealtime(3.0f);
        GameData.LoadSaveData(SaveSlot, _saveData);
        var asyncLoad = SceneManager.LoadSceneAsync(GameData.Checkpoint.SceneName);
        while(!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    IEnumerator NewGameAsync()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        yield return new WaitForSecondsRealtime(3.0f);
        var defaults = new SaveData
        {
            SaveSlot = SaveSlot,
            HP = 2,
            MaxHP = 2,
            Score = 0,
            CheckpointSceneName = "Level0x0",
            CheckpointX = 3,
            CheckpointY = 3,
            CheckpointZ = 0,
            CheckpointFacing = Direction.RIGHT,
            Lives = 2,
            PowerUpMask = 0,            
        };
        GameData.LoadSaveData(SaveSlot, defaults);
        var asyncLoad = SceneManager.LoadSceneAsync(GameData.Checkpoint.SceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
