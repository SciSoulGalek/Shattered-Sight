using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Resolutions : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private GameObject exclusiveButton; 
    [System.Serializable]
    public struct ResolutionOption
    {
        public string label;
        public int width;
        public int height;
    }

    [Header("Fixed options (один и тот же аспект, напр. 16:9)")]
    public ResolutionOption[] options = new ResolutionOption[]
    {
        new ResolutionOption { label = "1280 x 720",  width = 1280, height = 720 },
        new ResolutionOption { label = "1600 x 900",  width = 1600, height = 900 },
        new ResolutionOption { label = "1920 x 1080", width = 1920, height = 1080 },
    };

    private const string PREF_FULLSCREEN = "Video_Fullscreen";
    private const string PREF_RES_INDEX  = "Video_ResolutionIndex";

    private bool _isInitializing = false;

    void Start()
    {
        if (!resolutionDropdown)
            Debug.LogError("[Resolutions] resolutionDropdown is not assigned");
        if (!fullscreenToggle)
            Debug.LogError("[Resolutions] fullscreenToggle is not assigned");

        if (exclusiveButton != null)
            exclusiveButton.SetActive(false);

        _isInitializing = true; 

        InitDropdown();
        LoadPrefsAndInitUI();

        _isInitializing = false;  

        ApplyFromUI();
        SavePrefsFromUI();
    }


    void InitDropdown()
    {
        if (options == null || options.Length == 0)
        {
            Debug.LogWarning("[Resolutions] No options set");
            return;
        }

        resolutionDropdown.ClearOptions();

        var labels = new List<string>();
        foreach (var opt in options)
        {
            string label = string.IsNullOrEmpty(opt.label)
                ? $"{opt.width} x {opt.height}"
                : opt.label;
            labels.Add(label);
        }

        resolutionDropdown.AddOptions(labels);
    }

    void LoadPrefsAndInitUI()
    {
        bool fullscreen = PlayerPrefs.GetInt(PREF_FULLSCREEN, 1) == 1;
        fullscreenToggle.isOn = fullscreen;

        int savedIndex = PlayerPrefs.GetInt(PREF_RES_INDEX, options.Length - 1);
        savedIndex = Mathf.Clamp(savedIndex, 0, options.Length - 1);

        resolutionDropdown.value = savedIndex;
        resolutionDropdown.RefreshShownValue();

        if (exclusiveButton != null)
            exclusiveButton.SetActive(fullscreen);
    }

    public void OnResolutionDropdownChanged(int _)
    {
        if (_isInitializing) return;

        ApplyFromUI();
        SavePrefsFromUI();
    }

    public void OnFullscreenToggleChanged(bool isOn)
    {
        if (_isInitializing) return;

        if (exclusiveButton != null)
            exclusiveButton.SetActive(isOn);

        ApplyFromUI();
        SavePrefsFromUI();
    }

    void ApplyFromUI()
    {
        if (options == null || options.Length == 0) return;

        int idx = Mathf.Clamp(resolutionDropdown.value, 0, options.Length - 1);
        ResolutionOption opt = options[idx];
        bool fullscreen = fullscreenToggle.isOn;

        if (fullscreen)
        {
            int w = Display.main.systemWidth;
            int h = Display.main.systemHeight;

            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            Screen.SetResolution(w, h, true);

            Debug.Log($"[Resolutions] Fullscreen ON → native {w}x{h}, idx {idx}");
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(opt.width, opt.height, false);

            Debug.Log($"[Resolutions] Windowed → {opt.width}x{opt.height}, idx {idx}");
        }
    }

    void SavePrefsFromUI()
    {
        int idx = Mathf.Clamp(resolutionDropdown.value, 0, options.Length - 1);
        bool fullscreen = fullscreenToggle.isOn;

        PlayerPrefs.SetInt(PREF_RES_INDEX, idx);
        PlayerPrefs.SetInt(PREF_FULLSCREEN, fullscreen ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log($"[Resolutions] SavePrefs → idx={idx}, fullscreen={fullscreen}");
    }

    public void SetExclusiveFullScreen()
    {
        int idx = Mathf.Clamp(resolutionDropdown.value, 0, options.Length - 1);
        ResolutionOption opt = options[idx];

        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        Screen.SetResolution(opt.width, opt.height, true);

        Debug.Log($"[Resolutions] ForceExclusive → {opt.width}x{opt.height}, idx={idx}");
    }
}
