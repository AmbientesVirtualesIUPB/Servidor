using UnityEngine;

public class ValidaPiezasFueraMapa : MonoBehaviour
{

    public ValidadorPiezasMesa validadorPiezasMesa;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("11111111111");
        if (other.CompareTag("Pieza"))
        {
            Debug.Log("22222222");
            validadorPiezasMesa.ValidarYRestaurarHijoEspecifico(other.gameObject);
        }     
    }
}
