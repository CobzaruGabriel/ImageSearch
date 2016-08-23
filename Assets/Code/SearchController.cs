using UnityEngine;
using System.Collections;
using System;
using LitJson;
using System.Collections.Generic;

public class SearchController : MonoBehaviour, IPUCode {

	public PUGridTable ImageResultsTable;
	public PUInputField SearchField;

	public void PerformSearch(Hashtable args) {

		// Google image search stopped having an open API
		// for the purposes of this little demo, we're going to call out to
		// http://api.ababeen.com/api/images.php?q=kittens&count=100

		string searchString = SearchField.field.text;
		string escapedSearchString = Uri.EscapeDataString (searchString);

		string searchURL = string.Format ("http://api.ababeen.com/api/images.php?q={0}&count=500", escapedSearchString);

		Debug.Log ("searchURL: " + searchURL);
		StartCoroutine (DownloadImageResultsJson (searchURL));
	}


	IEnumerator DownloadImageResultsJson(string url) {
		WWW www = new WWW (url);

		yield return www;

		JsonData allImages = JsonMapper.ToObject (System.Text.Encoding.UTF8.GetString (www.bytes));

		List<object> itemsForTable = new List<object> ();

		Debug.Log ("Downloaded " + allImages.Count + " Images");

		for (int i = 0; i < allImages.Count; i++) {
			JsonData imageData = allImages [i];
			itemsForTable.Add (new ImageResult (
				(int)imageData ["width"], (int)imageData ["height"], 
				(int)imageData ["tbWidth"], (int)imageData ["tbHeight"], 
				imageData ["url"].ToString (),
				imageData ["tbUrl"].ToString ()
			));
		}

		ImageResultsTable.SetObjectList (itemsForTable);
		ImageResultsTable.ReloadTable ();
	}
}