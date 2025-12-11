using UnityEngine;

[ExecuteAlways]
public class ShardLinkManager : MonoBehaviour
{
    [System.Serializable]
    public class LinkLine
    {
        [Tooltip("ShardCamera")]
        public ShardView from;

        [Tooltip("ShardScreen")]
        public ShardView to;

        [Tooltip("ACtive")]
        public bool enabled = true;
    }

    [Header("All Links")]
    public LinkLine[] links;

    private void OnEnable()
    {
        ApplyAllLinks();
    }

    private void Start()
    {
        ApplyAllLinks();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            ApplyAllLinks();
        }
    }
#endif

    public void ApplyAllLinks()
    {
        if (links == null) return;

        foreach (var link in links)
        {
            if (!link.enabled) continue;
            if (link.from == null || link.to == null) continue;

            link.to.SetDisplayTexture(link.from.Texture);
        }
    }

    public void ClearAllOverrides()
    {
        if (links == null) return;

        foreach (var link in links)
        {
            if (link.to != null)
            {
                link.to.SetDisplayTexture(null);
            }
        }
    }
}
