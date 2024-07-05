using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeathEffect : MonoBehaviour
{
    [SerializeField] private Renderer[] objectRenderers = new Renderer[0];

    private float currentColorChangeValue;
    private ParticleSystem trianglePS;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        trianglePS = GetComponentInChildren<ParticleSystem>();

        var sh = trianglePS.shape;
        sh.meshRenderer = meshRenderer;
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TriggerDeathEffect();
        }
    }

    public void TriggerDeathEffect()
    {
        StartCoroutine(DeathEffectCoroutine());
    }

    private IEnumerator DeathEffectCoroutine()
    {
        while (currentColorChangeValue < 0.95f)
        {
            currentColorChangeValue = Mathf.Lerp(currentColorChangeValue, 1, 2f * Time.deltaTime);

            foreach (Renderer renderer in objectRenderers)
            {
                Material[] materials = renderer.materials;

                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].SetFloat("_amount", currentColorChangeValue);
                }
                renderer.materials = materials;
            }
            yield return null;
        }

        if (!trianglePS.isPlaying) trianglePS.Play();
        meshRenderer.enabled = false;
        Destroy(gameObject, 2);
    }
}
