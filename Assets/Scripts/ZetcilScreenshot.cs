using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.IO;
using System.Collections;
using TMPro;

public class ZetcilScreenshot : MonoBehaviour
{
    public enum Platform { PC, Android }

    [Header("Platform Settings")]
    public Platform targetPlatform = Platform.PC;
    public Canvas previewCanvas;

    [Header("Default Paths")]
    public string defaultPathPC = "C:/Screenshots/";
    public string defaultPathAndroid = "/storage/emulated/0/Pictures/Screenshots/";
    public string customPath = "";

    [Header("Flash Effect")]
    public float flashDuration = 0.2f;

    [Header("Screenshot Preview")]
    public float previewScale = 0.7f;
    public float borderWidth = 0.02f;
    public float initialPreviewSize = 1f;
    public float previewDuration = 1f;

    [Header("Countdown Settings")]
    public bool usingCountdown = true;
    public TextMeshProUGUI countdownText;
    public float countdownScaleMin = 0.5f;
    public float countdownScaleMax = 1.5f;
    public float countdownFadeOutDuration = 0.5f;

    [Header("Events")]
    public UnityEvent PreScreenshotEvent;
    public UnityEvent PostScreenshotEvent;

    private string screenshotPath;

    void Start()
    {
        // Ensure the default path exists
        if (targetPlatform == Platform.PC && !Directory.Exists(defaultPathPC))
        {
            Directory.CreateDirectory(defaultPathPC);
        }
    }

    public void InvokeScreenshotCapture()
    {
        StartCoroutine(FlashAndCapture());
    }

    public void InvokeScreenshotCountdown()
    {
        StartCoroutine(CountdownAndCapture());
    }

    IEnumerator CountdownAndCapture()
    {
        if (usingCountdown && countdownText != null)
        {
            for (int i = 3; i > 0; i--)
            {
                countdownText.text = i.ToString();
                StartCoroutine(AnimateCountdownText());
                yield return new WaitForSeconds(1f);
            }
            countdownText.text = "";
        }

        StartCoroutine(FlashAndCapture());
    }

    IEnumerator FlashAndCapture()
    {
        PreScreenshotEvent?.Invoke();

        // Create the flash image at runtime
        GameObject flashObject = new GameObject("FlashImage");
        RectTransform flashTransform = flashObject.AddComponent<RectTransform>();
        flashTransform.SetParent(previewCanvas.transform, false);
        flashTransform.anchorMin = Vector2.zero;
        flashTransform.anchorMax = Vector2.one;
        flashTransform.offsetMin = Vector2.zero;
        flashTransform.offsetMax = Vector2.zero;

        Image flashImage = flashObject.AddComponent<Image>();
        flashImage.color = new Color(1, 1, 1, 0); // Set to transparent white initially

        // Make sure the canvas is enabled for the flash effect
        previewCanvas.enabled = true;

        // Fade-in effect
        float flashTime = flashDuration / 2f;
        float elapsedTime = 0f;

        while (elapsedTime < flashTime)
        {
            flashImage.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, elapsedTime / flashTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure full opacity
        flashImage.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(0.1f); // Hold flash briefly at full opacity

        // Fade-out effect
        elapsedTime = 0f;
        while (elapsedTime < flashTime)
        {
            flashImage.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, elapsedTime / flashTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Clean up the flash object after fading out
        Destroy(flashObject);

        // Capture the screenshot
        StartCoroutine(CaptureScreenshot());
    }

    IEnumerator AnimateCountdownText()
    {
        countdownText.transform.localScale = Vector3.one * countdownScaleMax;
        countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, 1);

        float elapsedTime = 0f;
        while (elapsedTime < countdownFadeOutDuration)
        {
            float scale = Mathf.Lerp(countdownScaleMax, countdownScaleMin, elapsedTime / countdownFadeOutDuration);
            countdownText.transform.localScale = Vector3.one * scale;

            float alpha = Mathf.Lerp(1, 0, elapsedTime / countdownFadeOutDuration);
            countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, 0);
    }

    IEnumerator CaptureScreenshot()
    {
        // Determine the target path
        string path = string.IsNullOrEmpty(customPath) ? GetDefaultPath() : customPath;

        // Generate filename using current date and time
        string fileName = "Screenshot_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        screenshotPath = Path.Combine(path, fileName);

        // Capture screenshot and save
        ScreenCapture.CaptureScreenshot(screenshotPath);

        // Wait for the screenshot to be saved
        yield return new WaitForSeconds(0.5f);

        Debug.Log("Screenshot saved to: " + screenshotPath);

        // Display screenshot preview
        if (previewCanvas != null)
        {
            StartCoroutine(DisplayScreenshotPreview());
        }
    }

    IEnumerator DisplayScreenshotPreview()
    {
        // Load the screenshot as a texture
        yield return new WaitUntil(() => File.Exists(screenshotPath));
        byte[] imageData = File.ReadAllBytes(screenshotPath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);

        // Create a new GameObject for the preview image
        GameObject previewObject = new GameObject("ScreenshotPreview");
        RectTransform previewTransform = previewObject.AddComponent<RectTransform>();
        previewTransform.SetParent(previewCanvas.transform, false);
        previewTransform.anchorMin = new Vector2(0.5f, 0.5f);
        previewTransform.anchorMax = new Vector2(0.5f, 0.5f);
        previewTransform.pivot = new Vector2(0.5f, 0.5f);
        previewTransform.anchoredPosition = Vector2.zero;

        Image previewImage = previewObject.AddComponent<Image>();
        previewImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        // Create a border object
        GameObject borderObject = new GameObject("ScreenshotBorder");
        RectTransform borderTransform = borderObject.AddComponent<RectTransform>();
        borderTransform.SetParent(previewCanvas.transform, false);
        borderTransform.anchorMin = new Vector2(0.5f, 0.5f);
        borderTransform.anchorMax = new Vector2(0.5f, 0.5f);
        borderTransform.pivot = new Vector2(0.5f, 0.5f);
        borderTransform.anchoredPosition = Vector2.zero;

        // Set the border size slightly larger than the image for the border effect
        borderTransform.sizeDelta = new Vector2(texture.width, texture.height) * previewScale * (1f + borderWidth);

        Image borderImage = borderObject.AddComponent<Image>();
        borderImage.color = Color.white;

        // Set the preview size and attach it to the border
        previewTransform.sizeDelta = new Vector2(texture.width, texture.height) * previewScale;
        previewTransform.SetParent(borderTransform, false);

        // Show the preview canvas
        previewCanvas.enabled = true;

        // Wait for the preview duration, then hide it
        yield return new WaitForSeconds(previewDuration);

        // Clean up the border and preview objects
        Destroy(previewObject);
        Destroy(borderObject);

        // Invoke the Unity Event after capturing the screenshot
        PostScreenshotEvent?.Invoke();
    }

    string GetDefaultPath()
    {
        switch (targetPlatform)
        {
            case Platform.PC:
                return defaultPathPC;
            case Platform.Android:
                return defaultPathAndroid;
            default:
                return "";
        }
    }
}
