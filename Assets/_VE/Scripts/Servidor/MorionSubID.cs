using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("Morion Servidor/Morion Sub ID")]

public class MorionSubID : MorionID
{
	public MorionID morionID;

	[ConditionalHide("idPersonalizada", true)]
	public string ID_publica = "";


	private void OnDrawGizmosSelected()
	{
		if (morionID == null)
		{
			morionID = GetComponentInParent<MorionID>();
		}
		if ((ID_publica.Equals("") && !idPersonalizada))
		{
			GenerarID();
		}
	}

	public override void GenerarID()
	{
		ID_publica = "_" + Random.Range(10000, 99999).ToString();
	}

	public override string GetID()
	{
		if (morionID != null)
		{
			ID = morionID.GetID() + ID_publica;
		}
		return ID;
	}

	public override bool GetOwner()
	{
		return morionID.GetOwner();
	}
}
