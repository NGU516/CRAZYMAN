using UnityEngine;

public class POVVisionSetup : MonoBehaviour
{
    [Header("머티리얼 설정")]
    public Material stencilMaskMaterial;     // POV/WriteMask
    public Material overlayDarkenMaterial;   // POV/DarkenOutsideView

    [Header("Sphere 설정")]
    public float sphereRadius = 10f;

    [Header("Overlay 설정")]
    public float overlayDistance = 0.5f;
    public Vector2 overlayScale = new Vector2(1.5f, 1.5f);

    [Header("어두움 세기 (0 = 투명, 1 = 완전 암전)")]
    [Range(0f, 1f)]
    public float overlayDarkness = 0.85f;

    void Start()
    {
        SetupPOVSphere();
        SetupOverlayQuad();
    }

    private void SetupPOVSphere()
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.name = "POV_Sphere";
        sphere.transform.SetParent(transform);
        sphere.transform.localPosition = Vector3.zero;
        sphere.transform.localRotation = Quaternion.identity;
        sphere.transform.localScale = new Vector3(-sphereRadius, sphereRadius, sphereRadius); // X 음수로 뒤집기

        if (stencilMaskMaterial != null)
        {
            Renderer renderer = sphere.GetComponent<Renderer>();
            renderer.material = stencilMaskMaterial;
        }

        Destroy(sphere.GetComponent<Collider>());
    }

    private void SetupOverlayQuad()
    {
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = "POV_Overlay";
        quad.transform.SetParent(transform);
        quad.transform.localPosition = new Vector3(0f, 0f, overlayDistance);
        quad.transform.localRotation = Quaternion.identity;
        quad.transform.localScale = new Vector3(overlayScale.x, overlayScale.y, 1f);

        if (overlayDarkenMaterial != null)
        {
            Renderer renderer = quad.GetComponent<Renderer>();
            renderer.material = overlayDarkenMaterial;
            renderer.material.SetFloat("_DarknessAmount", overlayDarkness);
        }

        Destroy(quad.GetComponent<Collider>());
    }
}
