using UnityEngine;
using System;
using System.Collections.Generic;
//gpg
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
//for encoding
using System.Text;
using UnityEngine.SocialPlatforms;

public class GPG_CloudSaveSystem
{
    private static GPG_CloudSaveSystem _instance;
    public static GPG_CloudSaveSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GPG_CloudSaveSystem();
            }
            return _instance;
        }
    }
    //keep track of saving or loading during callbacks.
    private bool m_saving;
    //save name. This name will work, change it if you like.
    private static string m_saveName = "game_save_name";
    //This is the saved file. Put this in seperate class with other variables for more advanced setup. Remember to change merging, toBytes and fromBytes for more advanced setup.
    private string saveString = "";
    //check with GPG (or other*) if user is authenticated. *e.g. GameCenter
    private bool Authenticated
    {
        get
        {
            return Social.Active.localUser.authenticated;
        }
    }
    //merges loaded bytearray with old save
    private void ProcessCloudData(byte[] cloudData)
    {
        if (cloudData == null)
        {
            Debug.Log("No data saved to the cloud yet...");
            return;
        }
        Debug.Log("Decoding cloud data from bytes.");
        string progress = FromBytes(cloudData);
        Debug.Log("Merging with existing game progress.");
        MergeWith(progress);
    }

    //load save from cloud
    public void LoadFromCloud()
    {
        Debug.Log("Loading game progress from the cloud.");
        m_saving = false;
        ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
            m_saveName, //name of file.
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            SavedGameOpened);
    }

    //overwrites old file or saves a new one
    public void SaveToCloud()
    {
        if (Authenticated)
        {
            Debug.Log("Saving progress to the cloud... filename: " + m_saveName);
            m_saving = true;
            //save to named file
            ((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution(
                m_saveName, //name of file. If save doesn't exist it will be created with this name
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime,
                SavedGameOpened);
        }
        else
        {
            Debug.Log("Not authenticated!");
        }
    }

    //save is opened, either save or load it.
    private void SavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        //check success
        if (status == SavedGameRequestStatus.Success)
        {
            //saving
            if (m_saving)
            {
                //read bytes from save
                byte[] data = ToBytes();
                //create builder. here you can add play time, time created etc for UI.
                SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
                SavedGameMetadataUpdate updatedMetadata = builder.Build();
                //saving to cloud
                ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game, updatedMetadata, data, SavedGameWritten);
                //loading
            }
            else
            {
                ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, SavedGameLoaded);
            }
            //error
        }
        else
        {
            Debug.LogWarning("Error opening game: " + status);
        }
    }

    //callback from SavedGameOpened. Check if loading result was successful or not.
    private void SavedGameLoaded(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("SaveGameLoaded, success=" + status);
            ProcessCloudData(data);
        }
        else
        {
            Debug.LogWarning("Error reading game: " + status);
        }
    }

    //callback from SavedGameOpened. Check if saving result was successful or not.
    private void SavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("Game " + game.Description + " written");
        }
        else
        {
            Debug.LogWarning("Error saving game: " + status);
        }
    }

    //merge local save with cloud save. Here is where you change the merging betweeen cloud and local save for your setup.
    private void MergeWith(string other)
    {
        if (other != "")
        {
            saveString = other;
        }
        else
        {
            Debug.Log("Loaded save string doesn't have any content");
        }
    }

    //return saveString as bytes
    private byte[] ToBytes()
    {
        byte[] bytes = Encoding.UTF8.GetBytes(saveString);
        return bytes;
    }

    //take bytes as arg and return string
    private string FromBytes(byte[] bytes)
    {
        string decodedString = Encoding.UTF8.GetString(bytes);
        return decodedString;
    }

    // -------------------- ### Extra UI for testing ### -------------------- 

    //call this with Unity button to view all saves on GPG. Bug: Can't close GPG window for some reason without restarting.
    public void showUI()
    {
        // user is ILocalUser from Social.LocalUser - will work when authenticated
        ShowSaveSystemUI(Social.localUser, (SelectUIStatus status, ISavedGameMetadata game) => {
            // do whatever you need with save bundle
        });
    }
    //displays savefiles in the cloud. This will only include one savefile if the m_saveName hasn't been changed
    private void ShowSaveSystemUI(ILocalUser user, Action<SelectUIStatus, ISavedGameMetadata> callback)
    {
        uint maxNumToDisplay = 3;
        bool allowCreateNew = true;
        bool allowDelete = true;

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (savedGameClient != null)
        {
            savedGameClient.ShowSelectSavedGameUI(user.userName + "\u0027s saves",
                maxNumToDisplay,
                allowCreateNew,
                allowDelete,
                (SelectUIStatus status, ISavedGameMetadata saveGame) => {
                     // some error occured, just show window again
                     if (status != SelectUIStatus.SavedGameSelected)
                    {
                        ShowSaveSystemUI(user, callback);
                        return;
                    }

                    if (callback != null)
                        callback.Invoke(status, saveGame);
                });
        }
        else
        {
            // this is usually due to incorrect APP ID
            Debug.LogError("Save Game client is null...");
        }
    }

}
