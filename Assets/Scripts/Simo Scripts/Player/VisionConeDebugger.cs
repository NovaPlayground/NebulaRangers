using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionConeDebugger : MonoBehaviour
{
    [SerializeField] private float visionConeAngleHorizontal = 45f; // Angolo orizzontale del cono di visione in gradi
    [SerializeField] private float visionConeAngleVertical = 30f; // Angolo verticale del cono di visione in gradi
    [SerializeField] private float visionConeDistance = 50f; // Distanza massima del cono di visione

    private void OnDrawGizmos()
    {
        // Imposta il colore del Gizmo
        Gizmos.color = Color.yellow;

        // Calcola il raggio del cono
        float horizontalRadius = visionConeDistance * Mathf.Tan(visionConeAngleHorizontal * Mathf.Deg2Rad / 2);
        float verticalRadius = visionConeDistance * Mathf.Tan(visionConeAngleVertical * Mathf.Deg2Rad / 2);

        // Disegna il cono orizzontale
        DrawCone(transform.position, transform.forward, horizontalRadius, verticalRadius, visionConeDistance);
    }

    private void DrawCone(Vector3 origin, Vector3 direction, float horizontalRadius, float verticalRadius, float height)
    {
        // Disegna il cono come una serie di linee
        int segments = 36; // Numero di segmenti per il cerchio base

        // Calcola il passo angolare per il cerchio base
        float angleStep = 360f / segments;
        Vector3 lastPoint = origin + direction * height;

        // Disegna il cerchio base
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep;
            Vector3 basePoint = lastPoint + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * horizontalRadius, Mathf.Sin(angle * Mathf.Deg2Rad) * verticalRadius, 0);
            Gizmos.DrawLine(lastPoint, basePoint);
            lastPoint = basePoint;
        }

        // Disegna le linee laterali del cono
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep;
            Vector3 basePoint = lastPoint + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * horizontalRadius, Mathf.Sin(angle * Mathf.Deg2Rad) * verticalRadius, 0);
            Gizmos.DrawLine(origin, basePoint);
        }
    }
}
