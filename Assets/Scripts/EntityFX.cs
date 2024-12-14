using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    private SpriteRenderer[] sr;

    [Header("Flash FX")]
    [SerializeField] private Material hitMat;
    [HideInInspector]
    [SerializeField] private float flashDuration;
    private Material[] originalMats;
    private Color[] originalColors;

    private void Start()
    {
        sr = GetComponentsInChildren<SpriteRenderer>();
        originalMats = new Material[sr.Length];
        originalColors = new Color[sr.Length];

        // Store the original materials and colors
        for (int i = 0; i < sr.Length; i++)
        {
            originalMats[i] = sr[i].material;
            originalColors[i] = sr[i].color;
        }
    }

    private IEnumerator FlashFX()
    {
        // Apply the hit material and set the color to white
        foreach (var renderer in sr)
        {
            renderer.material = hitMat;
            renderer.color = Color.white;
        }

        yield return Helpers.GetWait(flashDuration);

        // Revert to the original materials and colors
        for (int i = 0; i < sr.Length; i++)
        {
            sr[i].material = originalMats[i];
            sr[i].color = originalColors[i];
        }
    }

    // Call this method to start the Flash effect
    public void TriggerFlash()
    {
        StartCoroutine(FlashFX());
    }

    private void RedColorBlink()
    {
        for (int i = 0; i < sr.Length; i++)
        {
            if (sr[i].color != originalColors[i])
                sr[i].color = originalColors[i];
            else
                sr[i].color = Color.red;
        }
    }

    public void CancelRedBlink()
    {
        CancelInvoke();
        for (int i = 0; i < sr.Length; i++)
        {
            sr[i].color = originalColors[i];
        }

    }

}
