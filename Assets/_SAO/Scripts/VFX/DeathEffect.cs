using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeathEffect : MonoBehaviour
{
    [SerializeField] private Renderer[] objectRenderers = new Renderer[0];

    private float currentColorChangeValue;
    private float targetColorChangeValue;
    private ParticleSystem trianglePS;
    private Mesh mesh;

    private void Start()
	{
        trianglePS = gameObject.GetComponentInChildren<ParticleSystem>();
        mesh = gameObject.GetComponent<Mesh>();

        var sh = trianglePS.shape;
        sh.enabled = true;
        sh.shapeType = ParticleSystemShapeType.Mesh;
        sh.mesh = mesh;
	}

	private void Update()
    {
        Debug.Log(currentColorChangeValue);
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TriggerDeathEffect();
        }

        currentColorChangeValue = Mathf.Lerp(currentColorChangeValue, targetColorChangeValue, 2f * Time.deltaTime);

        foreach (Renderer renderer in objectRenderers)
        {
            renderer.material.SetFloat("_amount", currentColorChangeValue);
        }

        if(currentColorChangeValue > 0.9f)
		{
            //Destroy(gameObject);
            trianglePS.Play();
		}
    }

    public void TriggerDeathEffect()
	{
        targetColorChangeValue = 1;
    }
}
