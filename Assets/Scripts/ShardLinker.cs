using UnityEngine;

[ExecuteAlways]
public class ShardLinker : MonoBehaviour
{
    public ShardView shardA;
    public ShardView shardB;

    void Update()
    {
        if (!shardA || !shardB)
            return;

        var texA = shardA.Texture;
        var texB = shardB.Texture;

        if (texA == null || texB == null)
            return;

        shardA.SetDisplayTexture(texB);
        shardB.SetDisplayTexture(texA);
    }
}
