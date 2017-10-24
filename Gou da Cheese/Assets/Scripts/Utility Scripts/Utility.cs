using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour {

	public static string GetGameObjectPath(GameObject obj)
	{
		string path = "/" + obj.name;
		while (obj.transform.parent != null)
		{
			obj = obj.transform.parent.gameObject;
			path = "/" + obj.name + path;
		}
		return path;
	}

	public static Transform FindChildWithName(GameObject parent, string name) {
		Transform[] children = parent.GetComponentsInChildren<Transform>(true);
		foreach (Transform child in children) {
			if (child.name.Equals(name)) {
				return child;
			}
		}
		Debug.LogError("Couldn't find " + name + " in " + parent.name);
		return null;
	}
}
