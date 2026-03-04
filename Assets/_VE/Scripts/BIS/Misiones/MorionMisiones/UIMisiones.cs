using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMisiones : MonoBehaviour
{
    public UIAutoAnimation manoBien;


    public void IniciarManoBien()
    {
        StartCoroutine(ManoBienFuncionando());
    }

    IEnumerator ManoBienFuncionando()
    {
        manoBien.EntranceAnimation();
        yield return new WaitForSeconds(1);
        manoBien.ExitAnimation();
    }
}
