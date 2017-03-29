using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour {

	private static Text LoginText;
	private static Text Yes;
	private static Text No;
	private static Text _firstNameLabel;
	private static Text _lastNameLabel;
	private static bool _registering;

	public static InputField FirstName;

	private string _firstName;

	// Use this for initialization
	void Start () {
		LoginText = GameObject.Find ("LoginText").GetComponent<Text> ();
		Yes = GameObject.Find ("Yes").GetComponent<Text> ();
		No = GameObject.Find ("No").GetComponent<Text> ();
		_firstNameLabel = GameObject.Find ("FirstNameLabel").GetComponent<Text> ();
		_lastNameLabel = GameObject.Find ("LastNameLabel").GetComponent<Text> ();

		FirstName = GameObject.Find ("FirstName").GetComponent<InputField> ();

		LoginText.text = "";
		Yes.text = "";
		No.text = "";
		_firstNameLabel.text = "";
		_lastNameLabel.text = "";

		_registering = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (GamePlay.ViewingLoginInitial)
			LoginYesNoController ();

		if (_registering) {
			GamePlay.ViewingLoginInitial = false;
			Yes.text = "";
			No.text = "";
			Registering ();
		}
	}

	public static void SetupLogin() {
		GamePlay.ViewingLoginInitial = true;
		//StartScreen.ClearStartScreen ();
		LoginText.text = "HAVE YOU PLAYED BEFORE?";
		Yes.text = "YES";
		No.text = "NO";
		HighlightText (No);


	}

	public static void LoginYesNoController() {
		//print ("inside controller: " + GamePlay.ViewingLoginInitial);

		if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) {
			if (IsHighlighted (Yes)) {  
				HighlightText (No);
			} else {
				HighlightText (Yes);
			}
		}

		if (Input.GetKeyDown (KeyCode.Return)) {
			if (IsHighlighted (Yes)) {
				print ("pressed yes");
			} else {
				print ("pressed nah");
				_registering = true;
			}
		}
	}

	public static void Registering() {
		LoginText.text = "PLEASE ENTER YOUR NAME";
		_firstNameLabel.text = "FIRST NAME";
		_lastNameLabel.text = "LAST NAME";
		HighlightText (LoginText);

		FirstName.ActivateInputField ();

		if (Input.GetKeyDown (KeyCode.A)) {

		}
			
	}

	public void FirstNameTextChanged(string firstName) {
		_firstName = firstName;
		print (_firstName);
	}

	public static void HighlightText(Text text) {
		text.color = Color.yellow;

		if (text == Yes)
			No.color = Color.white;
		if (text == No)
			Yes.color = Color.white;
	}

	public static bool IsHighlighted(Text text) {
		if (text.color == Color.yellow)
			return true;
		else
			return false;
	}
}
