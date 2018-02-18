using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Display : MonoBehaviour
{
	public Text wordField;
	public Text phoneticField;
	public Text descriptionField;

	public void UpdateDisplay(string word, string phonetic, string description)
	{
		wordField.text = word;
		phoneticField.text = phonetic;
		descriptionField.text = description;
	}
}
