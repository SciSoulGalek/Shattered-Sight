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

    [Header("Rotation Controls")]
    [Tooltip("Additive Rotation of Camera by Z")]
    public float cameraRotationZ = 0f;

    [Tooltip("Additive Rotation of Quad by Z")]
    public float surfaceRotationZ = 0f;

    private RenderTexture _rt;
    private MaterialPropertyBlock _block;

    private Quaternion _baseCamLocalRot;
    private Quaternion _baseSurfaceLocalRot;
    private bool _rotBaseCached;

    public RenderTexture Texture => _rt;

    void OnEnable()
    {
        Setup();
        CacheBaseRotations();
        ApplyRotations();
    }

    void OnDisable()
    {
        Cleanup();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!shardCamera)
            shardCamera = GetComponentInChildren<Camera>();
        if (!shardSurface)
            shardSurface = GetComponentInChildren<MeshRenderer>();

        CacheBaseRotations();
        ApplyRotations();
    }
#endif

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

    void CacheBaseRotations()
    {
        if (!shardCamera || !shardSurface) return;
        if (_rotBaseCached) return;

        _baseCamLocalRot     = shardCamera.transform.localRotation;
        _baseSurfaceLocalRot = shardSurface.transform.localRotation;
        _rotBaseCached       = true;
    }

    void ApplyRotations()
    {
        if (!_rotBaseCached || !shardCamera || !shardSurface) return;

        Quaternion camOffset = Quaternion.AngleAxis(cameraRotationZ, Vector3.forward);
        shardCamera.transform.localRotation = _baseCamLocalRot * camOffset;

        Quaternion surfOffset = Quaternion.AngleAxis(surfaceRotationZ, Vector3.forward);
        shardSurface.transform.localRotation = _baseSurfaceLocalRot * surfOffset;
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

    public Vector2 TransformVectorForCamera(Vector2 v)
    {
        float rad = cameraRotationZ * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        float x = v.x * cos - v.y * sin;
        float y = v.x * sin + v.y * cos;
        return new Vector2(x, y);
    }

    public Vector2 TransformVectorForSurface(Vector2 v)
    {
        float rad = surfaceRotationZ * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        float x = v.x * cos - v.y * sin;
        float y = v.x * sin + v.y * cos;
        return new Vector2(x, y);
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
