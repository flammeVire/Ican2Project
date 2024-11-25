using UnityEngine;

public class DynamicHole : MonoBehaviour
{
    public Material holeMaterial; // Matériau utilisant le shader
    public GameObject targetObject; // Objet créant le trou
    public Transform holeSpriteTransform; // Transform du sprite troué
    public float maskSize = 0.5f; // Taille du trou en unités mondiales

    void Update()
    {
        if (holeMaterial != null && targetObject != null && holeSpriteTransform != null)
        {
            // Convertir la position du GameObject générateur de trou dans l'espace local du sprite troué
            Vector3 localPosition = holeSpriteTransform.InverseTransformPoint(targetObject.transform.position);

            // Calculer une taille fixe en compensant l'échelle locale
            Vector3 localScale = holeSpriteTransform.localScale;
            float adjustedMaskSize = maskSize / Mathf.Max(localScale.x, localScale.y); // Échelle uniforme

            // Passer les données au shader
            holeMaterial.SetVector("_MaskPosition", new Vector4(localPosition.x, localPosition.y, 0, 0));
            holeMaterial.SetFloat("_MaskSize", adjustedMaskSize);
        }
    }
}
