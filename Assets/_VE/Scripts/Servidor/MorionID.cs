using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("Morion Servidor/Morion ID")]
public class MorionID : MonoBehaviour
{
	public bool		idPersonalizada		= false;
	public bool		isOwner				= false;

	[ConditionalHide("idPersonalizada", true)]
	public string ID = "";


	private void OnDrawGizmosSelected()
	{

		if ((ID.Equals("") && !idPersonalizada))
		{
			GenerarID();
		}
	}

	public virtual void GenerarID()
	{
		ID = SceneManager.GetActiveScene().name.ToUpper() + "_" + Random.Range(10000, 99999).ToString();
	}

	public virtual string GetID()
	{
		return ID;
	}

	public virtual void SetID(string _id)
	{
		ID = _id;
	}

	public virtual bool GetOwner()
	{
		return isOwner;
	}
}
