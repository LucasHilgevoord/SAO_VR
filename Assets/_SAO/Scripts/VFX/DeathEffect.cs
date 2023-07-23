using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeathEffect : MonoBehaviour
{
    [SerializeField] private Renderer[] objectRenderers = new Renderer[0];
    [SerializeField] private GameObject deathTriangleVFXPrefab;

    private float currentColorChangeValue;
    private float targetColorChangeValue;
    private ParticleSystem trianglePS;
    private MeshRenderer meshRenderer;

    private void Start()
	{
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        trianglePS = deathTriangleVFXPrefab.GetComponent<ParticleSystem>();

        var sh = trianglePS.shape;
        sh.meshRenderer = meshRenderer;

        Debug.Log(sh.meshRenderer);
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TriggerDeathEffect();
        }

        currentColorChangeValue = Mathf.Lerp(currentColorChangeValue, targetColorChangeValue, 2f * Time.deltaTime);

        foreach (Renderer renderer in objectRenderers)
        {
            Material[] materials = renderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetFloat("_amount", currentColorChangeValue);
            }

            renderer.materials = materials;
        }

        if(currentColorChangeValue > 0.95f)
		{
            var deathTriangleVFX = Instantiate(deathTriangleVFXPrefab, transform.position, transform.rotation);

            gameObject.SetActive(false);
            Destroy(gameObject,2f);
        }
    }

    public void TriggerDeathEffect()
	{
        targetColorChangeValue = 1;
    }
}
