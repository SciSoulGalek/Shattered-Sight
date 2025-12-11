using UnityEngine;

[ExecuteAlways]
public class ShardView : MonoBehaviour
{
    [Header("Refs")]
    public Camera shardCamera;
    public MeshRenderer shardSurface;

    [Header("Render Texture")]
    public int textureWidth = 1024;
    public int textureHeight = 1024;
    public int depthBuffer = 16;

    private RenderTexture _rt;
    private MaterialPropertyBlock _block;

    public RenderTexture Texture => _rt;

    void OnEnable()
    {
        Setup();
    }

    void OnDisable()
    {
        Cleanup();
    }

    void Setup()
    {
        if (!shardCamera)
            shardCamera = GetComponentInChildren<Camera>();
        if (!shardSurface)
            shardSurface = GetComponentInChildren<MeshRenderer>();

        if (!shardCamera || !shardSurface)
        {
            Debug.LogError($"[ShardView] No camera or surface on {name}");
            enabled = false;
            return;
        }

        if (_rt == null)
        {
            _rt = new RenderTexture(textureWidth, textureHeight, depthBuffer);
            _rt.name = $"ShardRT_{name}";
            _rt.hideFlags = HideFlags.DontSave;
        }

        shardCamera.targetTexture = _rt;

        if (_block == null)
            _block = new MaterialPropertyBlock();

        ApplyTexture(_rt);
    }

    void Cleanup()
    {
        if (shardCamera)
            shardCamera.targetTexture = null;

        if (_rt != null)
        {
            if (Application.isPlaying)
                _rt.Release();
            else
                DestroyImmediate(_rt);

            _rt = null;
        }
    }

    void ApplyTexture(Texture tex)
    {
        if (_block == null)
            _block = new MaterialPropertyBlock();

        shardSurface.GetPropertyBlock(_block);

        if (tex != null)
        {
            var baseMat = shardSurface.sharedMaterial;
            if (baseMat != null && baseMat.HasProperty("_BaseMap"))
                _block.SetTexture("_BaseMap", tex);
            else
                _block.SetTexture("_MainTex", tex); 
        }

        shardSurface.SetPropertyBlock(_block);
    }

    public void SetDisplayTexture(RenderTexture tex)
    {
        ApplyTexture(tex != null ? tex : _rt);
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying && shardCamera != null && _rt != null)
        {
            shardCamera.Render();
        }
#endif
    }
}
