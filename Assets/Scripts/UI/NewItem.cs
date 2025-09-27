using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//The purpose of this script is to display tips when you find something. 
//In the case of the cube it will be the bouncer and launcher.

[System.Serializable]
public class NewItem
{
	public string name = "";
	public bool found;
	public NewItem(string name = "unknown")
	{
		this.name = name;
		found = false;
	}

	public override string ToString()
	{

		return "Its called a " + name + " and it being found is " + found;
	}
}
