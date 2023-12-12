using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;


public class UI : MonoBehaviour
{
    GameObject _uiPanel = default;
    TextMeshProUGUI _messagePlaceHolder = default;

    public void HideUI()
    {
        if (SceneManager.GetSceneByName("UI").isLoaded)
        {
            _uiPanel.SetActive(false);
        }
    }

    public void ShowMessage(string message)
    {
        if (SceneManager.GetSceneByName("UI").isLoaded)
        {
            _uiPanel.SetActive(true);
            _messagePlaceHolder.text = message;
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "UI")
        {
            var objects = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            _uiPanel = objects.Where(x => x.name == "UI Panel").FirstOrDefault().gameObject;
            _messagePlaceHolder = _uiPanel.transform.FindInChildrenRecursive("Message").gameObject.GetComponent<TextMeshProUGUI>();

            var uiCamera = GameObject.FindWithTag("UI Camera").GetComponent<Camera>();
            Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(uiCamera);
        }
    }
}
