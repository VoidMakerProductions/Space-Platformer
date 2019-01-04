using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SceneManagement;

public class MainMenuEvents : MonoBehaviour
{
    public Text authStatus;
    public Text signInButtonText;
    public Button StartButton;
    public Button joinBtn;
    public Button AchButton;
    public Button mgen;
    public Button fgen;
    // Use this for initialization
    void Start()
    {

        
        // Try silent sign-in (second parameter is isSilent)
        if (PlayerPrefs.HasKey("username"))
            PlayGamesPlatform.Instance.Authenticate(SignInCallback, true);
        if (!PlayerPrefs.HasKey("gender"))
        {
            PlayerPrefs.SetString("gender", "m");
            mgen.interactable = false;
            fgen.interactable = true;

        }
        else {
            switch (PlayerPrefs.GetString("gender")) {
                case "m":
                    mgen.interactable = false;
                    fgen.interactable = true;
                    break;
                case "f":
                    mgen.interactable = true;
                    fgen.interactable = false;
                    break;
            }
        }
        if (!PlayerPrefs.HasKey("language"))
        {
            PlayerPrefs.SetString("language","ru");
        }
        if (!PlayerPrefs.HasKey("playerSprite")) {
            PlayerPrefs.SetInt("playerSprite", 0);
        }
    }

    public void SetPlayerGender(string g) {
        PlayerPrefs.SetString("gender", g);
        switch (g) {
            case "m":
                mgen.interactable = false;
                fgen.interactable = true;
                break;
            case "f":
                mgen.interactable = true;
                fgen.interactable = false;
                break;
        }
    }
    public void Single() {
        Syncer.Instance.singlplayer = true;
        SceneManager.LoadScene(2);
    }
    public void Join() {
        Syncer.Instance.singlplayer = false;
        PlayGamesPlatform.Instance.RealTime.AcceptFromInbox(Syncer.Instance);
    }
    public void ShowAchievments() {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }
        else
        {
            Debug.Log("Cannot show Achievements, not logged in");
        }
    }
    public void SignIn() {
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // Sign in with Play Game Services, showing the consent dialog
            // by setting the second parameter to isSilent=false.
            PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
        }
        else
        {
            // Sign out of play games
            PlayGamesPlatform.Instance.SignOut();

            // Reset UI
            signInButtonText.text = "Sign In";
            authStatus.text = "";
            if (PlayerPrefs.HasKey("username"))
                PlayerPrefs.DeleteKey("username");
        }
    }

    public void SignInCallback(bool success)
    {
        StartButton.interactable = success;
        AchButton.interactable = success;
        joinBtn.interactable = success;
        if (success)
        {
            Debug.Log("(Nebullarix) Signed in!");

            // Change sign-in button text
            signInButtonText.text = "Sign out";

            // Show the user's name
            authStatus.text = "Signed in as: " + Social.localUser.userName;
            if (!PlayerPrefs.HasKey("username")) {
                PlayerPrefs.SetString("username", Social.localUser.userName);
            }
        }
        else
        {
            Debug.Log("(Nebullarix) Sign-in failed...");

            // Show failure message
            signInButtonText.text = "Sign in";
            authStatus.text = "Sign-in failed";
        }
    }
}
