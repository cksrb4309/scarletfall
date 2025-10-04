using System.Collections;
using UnityEngine;

public class LightningMaterial : MonoBehaviour
{
    static LightningMaterial instance;

    public Texture[] lightning_Textures;

    public Material lightning_1_Material;
    public Material lightning_2_Material;

    float delay = 0.1f;

    int lightning_1_Index = 0;
    int lightning_2_Index = 2;


    private void Awake()
    {
        instance = this;
    }
    public static void StartTextureChange()
    {
        instance.StopAllCoroutines();

        instance.StartCoroutine(instance.TextureChangeCoroutine());
    }
    IEnumerator TextureChangeCoroutine()
    {
        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForSeconds(delay);

            NextTextureSelect();
        }
    }
    void NextTextureSelect()
    {
        if (++lightning_1_Index >= lightning_Textures.Length)
            lightning_1_Index = 0;
        if (++lightning_2_Index >= lightning_Textures.Length)
            lightning_2_Index = 0;

        lightning_1_Material.SetTexture("_MainTexture", lightning_Textures[lightning_1_Index]);
        lightning_2_Material.SetTexture("_MainTexture", lightning_Textures[lightning_2_Index]);
    }
}
