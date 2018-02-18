using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageService
{
	private static ImageService instance;
	private ImageService(){}

	public static ImageService Instance
	{
		get { return instance ?? (instance = new ImageService()); }
	}

	public BookVO LoadImageJSON()
	{
		TextAsset json = Resources.Load("book") as TextAsset;
		BookVO book = JsonUtility.FromJson<BookVO>(json.ToString());
		return book;
	}
}
