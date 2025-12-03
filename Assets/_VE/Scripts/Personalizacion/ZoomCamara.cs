using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCamara : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform nearTransform;
    [SerializeField] private Transform halfBodyTransform;
    [SerializeField] private Transform farTransform;
    Vector3 posObjetivo;



	private void Start()
	{
        posObjetivo = cameraTransform.position;
	}

	private void OnDestroy()
    {
    }

	private void FixedUpdate()
	{
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, posObjetivo, 0.1f);
    }

	[ContextMenu("Ver rostro")]
    public void VistaRostro()
    {
        posObjetivo = nearTransform.position;
    }

    [ContextMenu("Ver cuerpo completo")]
    public void VistaCuerpo()
    {
        posObjetivo = farTransform.position;
    }

    [ContextMenu("Ver medio cuerpo")]
    public void VistaPies()
    {
        posObjetivo = halfBodyTransform.position;
    }
}
