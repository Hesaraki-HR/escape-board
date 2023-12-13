using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UI : MonoBehaviour
{
    public static UI Instance { get; private set; }

    [SerializeField]
    GameObject _uiPanel = default;
    [SerializeField]
    TextMeshProUGUI _messagePlaceHolder = default;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        var uiCamera = GameObject.FindWithTag("UI Camera").GetComponent<Camera>();
        Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(uiCamera);
    }

    public void HideUI()
    {
        _uiPanel.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        _uiPanel.SetActive(true);
        _messagePlaceHolder.text = message;
    }
}
