using UnityEngine;

public class DynamicHole : MonoBehaviour
{
    public Material holeMaterial; // Mat�riau utilisant le shader
    public GameObject targetObject; // Objet cr�ant le trou
    public Transform holeSpriteTransform; // Transform du sprite trou�
    [HideInInspector]public float maskSize; // Taille du trou en unit�s mondiales
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