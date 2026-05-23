using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using NetworkDiscoveryUnity;

public class LoadScene : NetworkBehaviour
{
    public static LoadScene loadScene;
    [SerializeField] List<string> scenes = new List<string>();
    [SerializeField] Slider slider;
    [SerializeField] GameObject visualContainer;

    AsyncOperation _currentLoading;
    bool _isLoading = false;

    private void Awake() {
        visualContainer.SetActive(false);
        DontDestroyOnLoad(gameObject);
        if(loadScene == null)
            loadScene = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void OnNetworkSpawn()
    {
        NetworkManager.SceneManager.OnSceneEvent += HandleSceneEvent;
        visualContainer.SetActive(false);

        scenes.Clear();

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);

            string nameScene = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            scenes.Add(nameScene);
        }
    }

    public void ChangeScene(int idScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(scenes[idScene],LoadSceneMode.Single);
        NetworkDiscovery.Instance.CloseServerUdpClient();
    }

    void HandleSceneEvent(SceneEvent sceneEvent)
    {
        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.LoadEventCompleted :
                visualContainer.SetActive(false);
                _isLoading = false;
                _currentLoading = null;
                slider.value = 0;
            break;

            case SceneEventType.Load :
                _isLoading = true;
                _currentLoading = sceneEvent.AsyncOperation;
                visualContainer.SetActive(true);
            break;
        }
    }

    private void Update() {
        if(_isLoading && _currentLoading != null)
        {
            slider.value = Mathf.Clamp01(_currentLoading.progress / 0.9f);
        }
    }

}
