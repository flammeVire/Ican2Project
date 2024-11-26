using UnityEngine;

public class DynamicHole : MonoBehaviour
{
    public Material holeMaterial; // Matériau utilisant le shader
    public GameObject targetObject; // Objet créant le trou
    public Transform holeSpriteTransform; // Transform du sprite troué
    [HideInInspector]public float maskSize; // Taille du trou en unités mondiales
    [SerializeField] public float MaskSize;

    public GameObject FakePoint;
    private void Start()
    {
        maskSize = MaskSize;
    }

    void Update()
    {
        if (holeMaterial != null && targetObject != null && holeSpriteTransform != null)
        {
            Vector3 localPosition = holeSpriteTransform.InverseTransformPoint(targetObject.transform.position);

            Vector3 localScale = holeSpriteTransform.localScale;
            float adjustedMaskSize = maskSize / Mathf.Max(localScale.x, localScale.y);

            holeMaterial.SetVector("_MaskPosition", new Vector4(localPosition.x, localPosition.y, 0, 0));
            holeMaterial.SetFloat("_MaskSize", adjustedMaskSize);
        }
    }
}
