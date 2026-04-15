using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeObject : MonoBehaviour
{
    [Header("时空状态")]
    public GameObject pastVersion;    // 过去版本
    public GameObject presentVersion; // 现在版本
    public GameObject futureVersion;  // 未来版本（可选）

    [Header("切换效果")]
    public bool hasTransitionEffect = true; // 是否有切换特效
    public float fadeTime = 0.5f; // 淡入淡出时间

    private GameObject currentActiveVersion;

    void Start()
    {
        // 注册到WatchTool
        if (WatchTool.Instance != null)
            WatchTool.Instance.RegisterTimeObject(this);

        // 初始激活当前时空版本
        UpdateForTimeState(TimeState.Present);
    }

    void OnDestroy()
    {
        // 注销
        if (WatchTool.Instance != null)
            WatchTool.Instance.UnregisterTimeObject(this);
    }

    // 根据时空状态更新物体
    public void UpdateForTimeState(TimeState timeState)
    {
        // 停用所有版本
        SetAllVersionsActive(false);

        // 根据时空状态激活对应版本
        GameObject versionToActivate = null;

        switch (timeState)
        {
            case TimeState.Past:
                versionToActivate = pastVersion;
                break;
            case TimeState.Present:
                versionToActivate = presentVersion;
                break;
            case TimeState.Future:
                versionToActivate = futureVersion;
                break;
        }

        // 激活选中的版本
        if (versionToActivate != null)
        {
            versionToActivate.SetActive(true);
            currentActiveVersion = versionToActivate;

            if (hasTransitionEffect)
                StartCoroutine(FadeInEffect(versionToActivate));
        }

        // 如果没有指定版本，保持当前状态
    }

    // 停用所有版本
    void SetAllVersionsActive(bool active)
    {
        if (pastVersion != null) pastVersion.SetActive(active);
        if (presentVersion != null) presentVersion.SetActive(active);
        if (futureVersion != null) futureVersion.SetActive(active);
    }

    // 淡入效果
    IEnumerator FadeInEffect(GameObject obj)
    {
        // 如果有Renderer组件，实现淡入效果
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;
            Color originalColor = mat.color;
            Color transparentColor = originalColor;
            transparentColor.a = 0;

            mat.color = transparentColor;

            float elapsedTime = 0f;
            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / fadeTime;
                mat.color = Color.Lerp(transparentColor, originalColor, t);
                yield return null;
            }
        }
    }

    // 检查当前激活的版本
    public GameObject GetActiveVersion()
    {
        return currentActiveVersion;
    }
}