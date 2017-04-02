using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using Mono.Data.SqliteClient;

public class Login : MonoBehaviour {

	public static Login instance;

	public Text LoginText;
	private Text LoginHelp;

	private InputField UserName;

	Button NextButton;
	Button BackButton;
	Button YesButton;
	Button NoButton;

	StartScreen startScreen;

	private string UsernameString;
	public static int LeaderboardID;
	public static int UserID;

	private static string construct;
	private static IDbConnection dbConnection;
	private static IDbCommand dbCommand;
	private static IDataReader dataReader;

	bool usernameAdded;

	void Awake() {
		construct = "URI=file:" + Application.dataPath + "\\TrainingData.db";
		instance = this;

		LoginText = GameObject.Find ("LoginText").GetComponent<Text> ();
		LoginHelp = GameObject.Find ("LoginHelp").GetComponent<Text> ();
		YesButton = GameObject.Find ("YesButton").GetComponent<Button> ();
		NoButton = GameObject.Find ("NoButton").GetComponent<Button> ();
		UserName = GameObject.Find ("UsernameInput").GetComponent<InputField> ();
		NextButton = GameObject.Find ("NextButton").GetComponent<Button> ();
		BackButton = GameObject.Find ("BackButton").GetComponent<Button> ();
	}

	// Use this for initialization
	void Start () {
		usernameAdded = false;
		HideText (LoginText);
		UsernameString = "";
	}
	
	// Update is called once per frame
	void Update () {

		print ((GamePlay.ActiveScreen)GamePlay.ActiveScreenValue);

		switch (GamePlay.ActiveScreenValue) {
		case (int)GamePlay.ActiveScreen.playedBeforeQuestion:
			SetupPlayedBeforeQuestion ();
			break;
		case (int)GamePlay.ActiveScreen.preGame:
			SetUpPlayingGame ();
			break;
		case (int)GamePlay.ActiveScreen.mainGame:
			SetUpPlayingGame ();
			break;
		case (int)GamePlay.ActiveScreen.welcome:
			Welcome ();
			break;
		case (int)GamePlay.ActiveScreen.registerName:
			SetupRegister ();
			break;
		}
	}

	void SetUpPlayingGame() {
		HideButton (NextButton);
		HideButton (BackButton);
		HideButton (YesButton);
		HideButton (NoButton);

		HideText(LoginText);
		HideText(LoginHelp);

		HideInputField(UserName);
	}

	void HideButton(Button button) {
		button.image.enabled = false;
		button.GetComponentInChildren<Text> ().text = "";
	}

	void ShowButton(Button button) {
		button.image.enabled = true;
	
		switch (button.name) {
		case "NextButton":
			button.GetComponentInChildren<Text> ().text = "NEXT";
			break;
		case "BackButton":
			button.GetComponentInChildren<Text> ().text = "BACK";
			break;
		case "YesButton":
			button.GetComponentInChildren<Text> ().text = "YES";
			break;
		case "NoButton":
			button.GetComponentInChildren<Text> ().text = "NO";
			break;
		}
	}

	void HideInputField(InputField inputField) {
		inputField.image.enabled = false;
		inputField.placeholder.enabled = false;
		inputField.text = "";
		inputField.interactable = false;
		inputField.placeholder.GetComponentInChildren<Text> ().text = "";
	}

	public void HideAskNameScreen() {
		HideButton (NextButton);
		HideButton (BackButton);
		HideInputField (UserName);
		HideText (LoginText);
		HideText (LoginHelp);
	}

	void ShowInputField(InputField inputField) {
		inputField.image.enabled = true;
		inputField.placeholder.enabled = true;
		inputField.enabled = true;
		inputField.interactable = true;
		inputField.placeholder.GetComponentInChildren<Text> ().text = "Username...";
	}

	void HideText(Text text) {
		text.text = "";
	}

	public void BackButton_Pressed_BackToStart() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.startScreen;
	}

	public void BackButton_Pressed_BackToPlayedBeforeQuestion() {
		//SetupPlayedBeforeQuestion ();
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.playedBeforeQuestion;
	}

	public void SetupPlayedBeforeQuestion() {
		LoginText.text = "HAVE YOU PLAYED BEFORE?";
		HideInputField (UserName);
		HighlightText (LoginText);
		HideText (LoginHelp);
		ShowButton (YesButton);
		ShowButton (NoButton);
		ShowButton (BackButton);
		HideButton (NextButton);
		BackButton.onClick.AddListener (BackButton_Pressed_BackToStart);
		YesButton.onClick.AddListener (SetupLogin);
		NoButton.onClick.AddListener (SetupRegister);
	}

	public void SetupLogin() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.enterName;

		LoginText.text = "PLEASE ENTER YOUR USERNAME";
		HighlightText (LoginText);
		ShowInputField (UserName);

		ShowButton (NextButton);
		ShowButton (BackButton);

		HideButton (YesButton);
		HideButton (NoButton);

		HideText (LoginHelp);

		BackButton.onClick.AddListener (BackButton_Pressed_BackToPlayedBeforeQuestion);
		NextButton.onClick.AddListener (ReadUsernameInput);
	}

	public void ReadUsernameInput() {
		if (UserName.text != "") {
			UsernameString = UserName.text;
			if (DoesUsernameExist ()) {
				if (GamePlay.ActiveScreenValue == (int)GamePlay.ActiveScreen.enterName) {
					WelcomeBack ();
					AddNewLeaderboardRow ();
				}
			}
			else
				LoginHelp.text = "Username not found";
		} else
			LoginHelp.text = "Enter a Username";
		
	}

	public void ReadUsernameInput_Register() {
		if (UserName.text != "") {
			UsernameString = UserName.text;
			if (DoesUsernameExist () && !usernameAdded)
				LoginHelp.text = "Username already exists";
			else if (usernameAdded) {
				LoginHelp.text = "Username added";
				NextButton.onClick.AddListener (Welcome);

				//usernameAdded = false;
			} else {
				AddNewUsername ();
			}
		} else
			LoginHelp.text = "Enter a Username";
	}

	public void SetupRegister() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.registerName;

		LoginText.text = "PLEASE ENTER A USERNAME";
		HighlightText (LoginText);

		ShowInputField (UserName);

		ShowButton (NextButton);
		ShowButton (BackButton);

		HideButton (YesButton);
		HideButton (NoButton);

		BackButton.onClick.AddListener (BackButton_Pressed_BackToPlayedBeforeQuestion);
		NextButton.onClick.AddListener (ReadUsernameInput_Register);
	}

	public void WelcomeBack() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.welcomeBack;

		LoginText.text = "WELCOME BACK " + UsernameString.ToUpper ();
		HighlightText (LoginText);
		HideInputField (UserName);

		ShowButton (NextButton);
		ShowButton (BackButton);

		NextButton.GetComponentInChildren<Text> ().text = "Play";

		HideButton (YesButton);
		HideButton (NoButton);

		BackButton.onClick.AddListener (ReadUsernameInput);
		NextButton.onClick.AddListener (GamePlay.SetUpPregame);
		HideText (LoginHelp);
		LoginHelp.enabled = false;
	}

	public void Welcome() {
		GamePlay.ActiveScreenValue = (int)GamePlay.ActiveScreen.welcome;

		LoginText.text = "WELCOME " + UsernameString.ToUpper ();
		HighlightText (LoginText);
		HideText (LoginHelp);
		HideInputField (UserName);

		ShowButton (NextButton);
		ShowButton (BackButton);

		NextButton.GetComponentInChildren<Text> ().text = "Play";

		HideButton (YesButton);
		HideButton (NoButton);

		BackButton.onClick.AddListener (ReadUsernameInput_Register);
		NextButton.onClick.AddListener (GamePlay.SetUpPregame);
	}

	public void HighlightText(Text text) {
		text.color = Color.yellow;
	}

	public bool IsHighlighted(Text text) {
		if (text.color == Color.yellow)
			return true;
		else
			return false;
	}

	private bool DoesUsernameExist() {
		dbConnection = new SqliteConnection (construct);
		dbConnection.Open ();
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = "SELECT * FROM Users WHERE Username = '" + UsernameString + "';";
		dataReader = dbCommand.ExecuteReader ();

		bool usernameFound = false;

		while (dataReader.Read ()) {
			if (dataReader ["Username"].ToString () != "") {
				usernameFound = true;
			}
		}

		dbConnection.Close ();

		return usernameFound;
	}

	private void AddNewUsername() {

		int userId = 0;
		int score = 0;

		dbConnection = new SqliteConnection (construct);
		dbConnection.Open ();
		dbCommand = dbConnection.CreateCommand ();

		dbCommand.CommandText = "INSERT INTO Users(Username) values ('" + UsernameString + "')";
		dbCommand.ExecuteNonQuery ();

		dbCommand.CommandText = "SELECT ID FROM Users WHERE Username = '" + UsernameString + "';";
		dataReader = dbCommand.ExecuteReader ();

		while (dataReader.Read ()) {
			userId = (int)(dataReader ["ID"]);
		}

		UserID = userId;

		dbCommand.CommandText = "INSERT INTO Leaderboard(UserID, Score) values ('" + userId + "', '" + score + "')";
		dbCommand.ExecuteNonQuery ();

		dbCommand.CommandText = "SELECT ID FROM Leaderboard WHERE UserId = '" + userId + "' AND Score = '0';";
		dataReader = dbCommand.ExecuteReader ();

		while (dataReader.Read ()) {
			LeaderboardID = (int)(dataReader ["ID"]);
		}

		dbConnection.Close ();

		usernameAdded = true;
	}

	private void AddNewLeaderboardRow() {

		int userId = 0;
		int score = 0;

		dbConnection = new SqliteConnection (construct);
		dbConnection.Open ();
		dbCommand = dbConnection.CreateCommand ();

		dbCommand.CommandText = "SELECT ID FROM Users WHERE Username = '" + UsernameString + "';";
		dataReader = dbCommand.ExecuteReader ();

		while (dataReader.Read ()) {
			userId = (int)(dataReader ["ID"]);
		}

		UserID = userId;

		dbCommand.CommandText = "INSERT INTO Leaderboard(UserID, Score) values ('" + userId + "', '" + score + "')";
		dbCommand.ExecuteNonQuery ();

		dbCommand.CommandText = "SELECT ID FROM Leaderboard WHERE UserId = '" + userId + "' AND Score = '0';";
		dataReader = dbCommand.ExecuteReader ();

		while (dataReader.Read ()) {
			LeaderboardID = (int)(dataReader ["ID"]);
		}

		dbConnection.Close ();
	}
}
