using UnityEngine;
using System.Collections;

public class SearchController : MonoBehaviour, IPUCode {

	public PUInputField SearchField;

	public void PerformSearch(Hashtable args){
		string searchString = SearchField.field.text;
		Debug.Log ("search for " + searchString);
	}
}