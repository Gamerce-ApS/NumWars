using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;

using UnityEngine;
using UnityEngine.UI;




public class Startup : MonoBehaviourPunCallbacks
{

    public Dictionary<string, UserDataRecord> myData;

    public string displayName = "";
    public string MyPlayfabID = "";

    public GameObject SearchingForGameObject=null;

    public List<BoardData> openGamesList = new List<BoardData>();

    PlayfabHelperFunctions _PlayfabHelperFunctions;



    public BoardData GameToLoad = null;

    public static Startup _instance=null;

    // Start is called before the first frame update
    void Start()
    {
        if (Startup._instance != null)
        {
            
            Destroy(gameObject);
            return;
        }


        DontDestroyOnLoad(gameObject);
        _instance = this;
        _PlayfabHelperFunctions = gameObject.GetComponent<PlayfabHelperFunctions>();
        Board.instance.Init();

        LoadingOverlay.instance.ShowLoading("Connecting to photon");

        if (PhotonNetwork.IsConnected)
        {}else{
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1";
        }
    }


    public void Refresh()
    {
        _PlayfabHelperFunctions.Refresh();
    }
    public void Refresh(float aDelay)
    {
        StartCoroutine(DelayRefresh(aDelay));
    }
    public IEnumerator DelayRefresh(float aDelay = 0.5f)
    {
        yield return new WaitForSeconds(aDelay);
        Refresh();
    }
    public void UpdateDisplayName()
    {
        //_PlayfabHelperFunctions.UpdateDisplayName(_txtLabel.text);
    }


    // Update is called once per frame
    void Update()
    {

    }
    public void ChangeValueFor(string aData)
    {
        _PlayfabHelperFunctions.ChangeValueFor(aData);
    }

    #region MonoBehaviourPunCallbacks CallBacks
    public override void OnConnectedToMaster()
    {
        LoadingOverlay.instance.DoneLoading("Connecting to photon");
        // we don't want to do anything if we are not attempting to join a room. 
        // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
        // we don't want to do anything.
        //if (isConnecting)
        {
            Debug.Log("OnConnectedToMaster: Next -> try to Join Random Room");
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");
        }

        _PlayfabHelperFunctions.Login();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        LoadingOverlay.instance.DoneLoading("JoinRandomRoom");
        LoadingOverlay.instance.ShowLoading("CreateRoom");
        Debug.Log("<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room");
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        string newRoomName = string.Format("{0}-{1}", displayName+"_"+ MyPlayfabID+"_", Random.Range(0, 1000000).ToString())+ Random.Range(0, 1000000).ToString();    // for int, Random.Range is max-exclusive!
        TypedLobby tl = new TypedLobby("", LobbyType.AsyncRandomLobby);
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 0;
        ro.EmptyRoomTtl = 300000;
        ro.PlayerTtl = int.MaxValue;
        ro.IsVisible = true;
        ro.IsOpen = true;
        ro.CustomRoomPropertiesForLobby = new string[] { MyPlayfabID, displayName, newRoomName };
        PhotonNetwork.CreateRoom(newRoomName, ro, tl);
        
    }
    public override void OnCreatedRoom()
    {
        LoadingOverlay.instance.DoneLoading("CreateRoom");
        Debug.Log("<Color=Green>OnCreatedRoom</Color> ");

        _PlayfabHelperFunctions.SetPlayfabCreatedRoom(PhotonNetwork.CurrentRoom.PropertiesListedInLobby[0], PhotonNetwork.CurrentRoom.PropertiesListedInLobby[2]);
        // Created room and added to players playfab list in shared data, now we wait for people to join

    }
 

    public override void OnJoinedRoom()
    {
        LoadingOverlay.instance.DoneLoading("JoinRandomRoom");
        LoadingOverlay.instance.DoneLoading("CreateRoom");
        Debug.Log("<Color=Green>OnJoinedRoom</Color> with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");

        if( PhotonNetwork.CurrentRoom.PropertiesListedInLobby[0] == MyPlayfabID )
        {
            Debug.Log("Joined your own room, waiting for other player");
            if(SearchingForGameObject == null)
            {
                SearchingForGameObject = (GameObject)GameObject.Instantiate(PlayfabHelperFunctions.instance.SearchingForGamePrefab, MainMenuController.instance._GameListParent);
                SearchingForGameObject.transform.SetAsFirstSibling();
                SearchingForGameObject.SetActive(true);
            }
            else
            {
                Startup._instance.SearchingForGameObject.SetActive(true);
                SearchingForGameObject.transform.SetAsFirstSibling();
            }
            _PlayfabHelperFunctions.RemoveAbandonedGames();
        }
        else
        {
            SetPlayfabSecondPlayerInRoom(PhotonNetwork.CurrentRoom.PropertiesListedInLobby[0],MyPlayfabID, PhotonNetwork.CurrentRoom.PropertiesListedInLobby[1],  PhotonNetwork.CurrentRoom.PropertiesListedInLobby[2]);
        }
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        LoadingOverlay.instance.DoneLoading("JoinRandomRoom");

        LoadingOverlay.instance.DoneLoading("CreateRoom");

        for (int i = 0; i < openGamesList.Count; i++)
        {
            if (openGamesList[i].player1_PlayfabId == MyPlayfabID && openGamesList[i].player2_PlayfabId == "")
            {
                PlayfabHelperFunctions.instance.RemoveRoomFromList(openGamesList[i].RoomName, openGamesList[i].GetJson());
                if(Startup._instance.SearchingForGameObject != null)
                Startup._instance.SearchingForGameObject.SetActive(false);
                return;
            }
        }
        Debug.Log("Failed to join open room, need to remove the serach for new game");

        //_PlayfabHelperFunctions.RemoveAbandonedGames();
    }
    public void FinishedGettingGameListCheckForOpenGames()
    {
        for (int i = 0; i < openGamesList.Count; i++)
        {
            if (openGamesList[i].player1_PlayfabId == MyPlayfabID && openGamesList[i].player2_PlayfabId == "")
            {
                if(PhotonNetwork.InRoom == false)
                    PhotonNetwork.JoinRoom(openGamesList[i].RoomName);
                return;
            }
        }

        _PlayfabHelperFunctions.RemoveAbandonedGames();



        GameObject obj = (GameObject)GameObject.Instantiate(_PlayfabHelperFunctions._FinishedTitleListItem, MainMenuController.instance._GameListParent);

        string[] stringSeparators = new string[] { "[splitter]" };
        string[] oldGameList = GetComponent<Startup>().myData["OldGames"].Value.Split(stringSeparators, System.StringSplitOptions.None);
        for (int i = 0; i < oldGameList.Length; i++)
        {
            if (oldGameList[i].Length > 2)
            {
                BoardData bd = new BoardData(CompressString.StringCompressor.DecompressString(oldGameList[i]));
                GameObject obj2 = (GameObject)GameObject.Instantiate(_PlayfabHelperFunctions._GameListItem, MainMenuController.instance._GameListParent);
                obj2.GetComponent<GameListItem>().Init(bd,true);
            }

        }


     

        


    }

    public void SetPlayfabSecondPlayerInRoom(string player1_playfabID,string player2_playfabID,string player1_displayName, string roomName)
    {
        // The game can start
        //1. Remove room
        //2. Set shared data for p1 and p2
        //3. Start game

        //1
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LeaveRoom();

        //2
        //Run cloud script with data GetSerializedRoomData() and player1_playfabID,player2_playfabID,roomName
        //PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        //{
        //    FunctionName = "SetPlayfabSecondPlayerInRoom", // Arbitrary function name (must exist in your uploaded cloud.js file)
        //    FunctionParameter = new { id1 = player1_playfabID, id2 = player2_playfabID, roomName = roomName, roomData = GetSerializedRoomData() }, // The parameter provided to your function
        //    GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        //}, OnCloudHelloWorld, OnErrorShared);



        // Add SharedGroup
        _PlayfabHelperFunctions.AddPlayerToSharedGroup(player1_playfabID, player2_playfabID, player1_displayName, roomName);




 






         

    }
    public bool GetHasActiveGameSearch()
    {
        // Check from shared data if you have a game serach in progress
        for(int i = 0; i< openGamesList.Count;i++)
        {
            if (openGamesList[i].player1_PlayfabId == MyPlayfabID && openGamesList[i].player2_PlayfabId == "")
                return true;
        }
        return false;
    }



    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("<Color=Failed>OnCreateRoomFailed</Color> ");
        LoadingOverlay.instance.DoneLoading("CreateRoom");
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("<Color=Red>OnDisconnected</Color> " + cause);
        Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");
    }

    #endregion




    string GetSerializedRoomData()
    {
        return "";
    }


}
