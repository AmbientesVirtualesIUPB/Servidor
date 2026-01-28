using UnityEngine;

public class MantenerRotacionInicial : MonoBehaviour
{
    public Vector3 rot;

    // Start is called before the first frame update
    void Start()
    {
        //rot = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.Euler(rot),0.05f);
    }
}
