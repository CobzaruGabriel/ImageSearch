using UnityEngine;
using System.Collections;
using System;
using LitJson;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageDownloader5 : ImageDownloader {

}

public class ImageDownloader4 : ImageDownloader {

}

public class ImageDownloader3 : ImageDownloader {

}

public class ImageDownloader2 : ImageDownloader {

}

public class ImageDownloader : MonoBehaviour {

	public class ImageDownloadItem {
		public string url;
		public PURawImage img;

		public ImageDownloadItem(string url, PURawImage img){
			this.url = url;
			this.img = img;
		}
	}

	private List<ImageDownloadItem> downloadQueue = new List<ImageDownloadItem> ();

	public int Count {
		get {
			return downloadQueue.Count;
		}
	}

	public void DownloadImageFromURL(string url, PURawImage image) {
		downloadQueue.Add (new ImageDownloadItem (url, image));

		if (downloadQueue.Count == 1) {
			StartCoroutine (PrivateDownloadImage ());
		}
	}

	IEnumerator PrivateDownloadImage() {

		int queueIndex = downloadQueue.Count - 1;
		ImageDownloadItem item = downloadQueue [queueIndex];

		WWW www = new WWW (item.url);

		yield return www;

		item.img.image.texture = www.textureNonReadable;
		item.img.image.color = Color.white;

		downloadQueue.RemoveAt (queueIndex);

		if (downloadQueue.Count > 0) {
			StartCoroutine (PrivateDownloadImage ());
		}
	}
}

public class ImageResult {
	public string url;
	public string tbUrl;
	public int width;
	public int height;
	public int tbWidth;
	public int tbHeight;

	public ImageResult(int w, int h, int tbWidth, int tbHeight, string url, string tbUrl){
		this.width = w;
		this.height = h;
		this.tbWidth = tbWidth;
		this.tbHeight = tbHeight;
		this.url = url;
		this.tbUrl = tbUrl;

		// Ideally, our thumb adheres to our ideal row height
		float aspect = (float)this.tbWidth / (float)this.tbHeight;
		this.tbHeight = 200;
		this.tbWidth = Mathf.RoundToInt((float)this.tbHeight * aspect);

	}
}

public class ImageResultTableCell : PUTableCell {


	public override void UpdateContents() {
		ImageResult data = cellData as ImageResult;

		cellTransform.sizeDelta = new Vector2 (data.tbWidth, data.tbHeight);

		PURawImage image = new PURawImage ();
		image.color = Color.gray;
		image.SetFrame (0, 0, 0, 0, 0, 0, "stretch,stretch");
		image.LoadIntoPUGameObject (puGameObject);
		image.SetStretchStretch (2, 2, 2, 2);

		// Ensure we have our four downloader queues ready
		ImageDownloader downloader1 = scrollRect.gameObject.GetComponent<ImageDownloader> ();
		if (downloader1 == null) {
			downloader1 = scrollRect.gameObject.AddComponent<ImageDownloader> ();
		}
		ImageDownloader downloader2 = scrollRect.gameObject.GetComponent<ImageDownloader2> ();
		if (downloader2 == null) {
			downloader2 = scrollRect.gameObject.AddComponent<ImageDownloader2> ();
		}
		ImageDownloader downloader3 = scrollRect.gameObject.GetComponent<ImageDownloader3> ();
		if (downloader3 == null) {
			downloader3 = scrollRect.gameObject.AddComponent<ImageDownloader3> ();
		}
		ImageDownloader downloader4 = scrollRect.gameObject.GetComponent<ImageDownloader4> ();
		if (downloader4 == null) {
			downloader4 = scrollRect.gameObject.AddComponent<ImageDownloader4> ();
		}
		ImageDownloader downloader5 = scrollRect.gameObject.GetComponent<ImageDownloader4> ();
		if (downloader5 == null) {
			downloader5 = scrollRect.gameObject.AddComponent<ImageDownloader5> ();
		}

		// Find the downloader queue with the least items queued
		ImageDownloader minDownloader = downloader1;
		if (downloader2.Count < minDownloader.Count) {
			minDownloader = downloader2;
		}
		if (downloader3.Count < minDownloader.Count) {
			minDownloader = downloader3;
		}
		if (downloader4.Count < minDownloader.Count) {
			minDownloader = downloader4;
		}
		if (downloader5.Count < minDownloader.Count) {
			minDownloader = downloader5;
		}

		minDownloader.DownloadImageFromURL (data.tbUrl, image);
	}
}