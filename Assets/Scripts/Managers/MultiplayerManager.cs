using UnityEngine;
using System.Collections;
using PlayerIOClient;
using System.Collections.Generic;

namespace DiosesModernos {
    public class MultiplayerManager : Singleton<MultiplayerManager> {
        #region Properties
        [Header ("Configuration")]
        [SerializeField]
        [Tooltip ("Turn true to play in online, false to play in solo (no network)")]
        bool _online = true;
        [SerializeField]
        [Tooltip ("Delay between 2 regular sends to the server")]
        float _serverRate = 0.2f;

        [Header ("Debug configuration")]
        [SerializeField]
        [Tooltip ("Turn true to test multiplpayer on the same machine")]
        bool _debug = false;
        [SerializeField]
        [Tooltip ("Turn true to use local server")]
        bool _localhost = false;
        [SerializeField]
        [Tooltip ("Turn true to test multiplayer on a development server")]
        bool _developmentServer = false;
        [SerializeField]
        [Tooltip ("IP of the development server")]
        string _ipDevServer = "192.168.1.3";
        
        [Header ("Links")]
        [SerializeField]
        [Tooltip ("Ally prefab")]
        GameObject _allyPrefab;
        #endregion

        #region Getters
        public bool connected {
            get { return _pioConnection != null ? _pioConnection.Connected : false; }
        }

        public bool online {
            get { return _online; }
            set { _online = value; }
        }
        #endregion

        #region API
        // Disconnects the player
        /*public void Disconnect () {
            if (!_pioConnection.Connected) return;
            _pioConnection.Disconnect ();
        }*/

        public void SendBossDamage (int damage) {
            if (!_joinedRoom) return;
            _pioConnection.Send ("Boss Damage", damage);
        }

        public void SendPlayerShoot () {
            if (!_joinedRoom) return;
            Player player = GameManager.instance.player;
            _pioConnection.Send (
                "Player Shoot",
                player.transform.position.x,
                player.transform.position.z,
                player.transform.rotation.eulerAngles.y
            );
        }
        #endregion

        #region Unity
        void Start () {
            if (_online) StartConnection ();
        }
        #endregion

        #region Handlers
        void Disconnected (object sender, string error) {
            Debug.LogWarning ("Disconnected !");
        }

        void HandleMessage (object sender, PlayerIOClient.Message message) {
            _messages.Add (message);
        }
        #endregion

        #region Private properties
        // PlayerIO connection
        Connection _pioConnection;
        // PlayerIO client
        PlayerIOClient.Client _pioClient;
        // Messages from the server
        List<PlayerIOClient.Message> _messages = new List<PlayerIOClient.Message> ();
        // Is true if the player has joined a room
        bool _joinedRoom = false;
        // Other players connected
        List<Ally> allies = new List<Ally> ();
        #endregion

        #region Private methods
        // Processes the message queue from the server
        IEnumerator GotMessages () {
            do {
                foreach (PlayerIOClient.Message message in _messages) {
                    switch (message.Type) {
                        // The new health of the boss
                        case "Boss Health":
                            GameManager.instance.boss.health = message.GetInt (0);
                            break;
                        // The boss has a new target
                        case "Boss Target":
                            BossTarget (message.GetString (0));
                            break;
                        // A player sends a message to the other players
                        case "Chat":
                            Debug.Log ("Message from " + message.GetString (0) + " : " + message.GetString (1));
                            break;
                        // The server sends a message for debugging
                        case "Debug":
                            Debug.Log ("Message from server : " + message.GetString (0));
                            break;
                        // A player left the room
                        case "Player Left":
                            PlayerLeft (message.GetString (0));
                            break;
                        // A player joined the room
                        case "Player Joined":
                            PlayerJoined (message.GetString (0));
                            break;
                        // The new position / rotation of a player
                        case "Player Position":
                            PlayerPosition (message.GetString (0), message.GetFloat (1), message.GetFloat (2), message.GetFloat (3));
                            break;
                        // A player shoots
                        case "Player Shoot":
                            PlayerShoot (message.GetString (0), message.GetFloat (1), message.GetFloat (2), message.GetFloat (3));
                            break;
                    }
                }
                // Clear message queue after it's been processed
                _messages.Clear ();
                yield return new WaitForFixedUpdate ();
            } while (connected);
        }

        void GoOffline () {
            _online = false;
            Boss boss = GameManager.instance.boss;
            boss.AddTarget (GameManager.instance.player.transform);
            boss.StartCoroutine ("UpdateTarget");
        }

        // The player joins the room
        void JoinGameRoom (string roomId) {
            _pioClient.Multiplayer.CreateJoinRoom (
                roomId,
                "DiosesModernos",   // The room type started on the server
                false,		        // Should the room be visible in the lobby?
                null,
                null,
                delegate (Connection connection) {
                    SuccessfullJoin (connection, roomId);
                },
                delegate (PlayerIOError error) {
                    Debug.LogError ("Error Joining Room : " + error.ToString ());
                    GoOffline ();
                }
            );
        }

        // Regularly send position and rotation of the player to the server
        IEnumerator SendPlayerPosition () {
            do {
                Transform playerTransform = GameManager.instance.player.transform;
                _pioConnection.Send ("Player Position", playerTransform.position.x, playerTransform.position.z, playerTransform.rotation.eulerAngles.y);
                yield return new WaitForSeconds (_serverRate);
            } while (true);
        }

        // Start the connection
        void StartConnection () {
            // SystemInfo.deviceUniqueIdentifier returns the same id if the machine runs several clients
            // In localhost generates a random id
            string playerId = _debug ? "" + Random.Range (1, 1000000000) : GameManager.instance.player.id;

            // Connection
            Debug.Log ("Connection...");
            PlayerIOClient.PlayerIO.Connect (
                "dioses-modernos-qxra1omlxkmsh9rxnzsonq",   // Game id
                "public",							        // The id of the connection, as given in the settings section of the admin panel. By default, a connection with id='public' is created
                playerId,							        // The id of the user connecting
                null,								        // If the connection identified by the connection id only accepts authenticated requests, the auth value generated based on playerId is added here
                null,
                null,
                delegate (Client client) {
                    SuccessfullConnect (client);
                },
                delegate (PlayerIOError error) {
                    Debug.Log ("Error connecting : " + error.ToString ());
                    GoOffline ();
                }
            );
        }

        // The connection has been established
        void SuccessfullConnect (Client client) {
            Debug.Log ("Connected to the server");

            if (_localhost) {
                client.Multiplayer.DevelopmentServer = new ServerEndpoint ("127.0.0.1", 8184);
            }
            else if (_developmentServer) {
                client.Multiplayer.DevelopmentServer = new ServerEndpoint (System.String.IsNullOrEmpty (_ipDevServer) ? "192.168.1.96" : _ipDevServer, 8184);
            }
            // Create or join the room	
            string roomId = "DiosesModernos";
            _pioClient = client;
            JoinGameRoom (roomId);
        }

        // The player has successfully joined the room
        void SuccessfullJoin (Connection connection, string roomId) {
            Debug.Log ("Joined Room : " + roomId);
            // We successfully joined a room so set up the message handler
            _pioConnection = connection;
            _pioConnection.OnMessage += HandleMessage;
            _pioConnection.OnDisconnect += Disconnected;
            _joinedRoom = true;
            StartCoroutine ("GotMessages");
            StartCoroutine ("SendPlayerPosition");
            GuiManager.instance.ActivateOnlineButton ();
        }
        #endregion

        #region Server requests treatment
        void BossTarget (string playerId) {
            foreach (Ally ally in allies) {
                if (ally.id == playerId) {
                    GameManager.instance.boss.target = ally.transform;
                    return;
                }
            }
            // No target found among the players, by logic the target is therefore the player
            GameManager.instance.boss.target = GameManager.instance.player.transform;
        }

        void PlayerJoined (string playerId) {
            Debug.Log (playerId + " has joined !");
            GameObject allyObject = ObjectPool.Spawn (_allyPrefab);
            allyObject.GetComponent<Ally> ().id = playerId;
            allies.Add (allyObject.GetComponent<Ally> ());
            //GameManager.instance.boss.AddTarget (allyObject.transform);
        }

        void PlayerLeft (string playerId) {
            Debug.Log (playerId + " left the room !");
            foreach (Ally ally in allies) {
                if (ally.id == playerId) {
                    GameManager.instance.boss.RemoveTarget (ally.transform);
                    ally.Recycle ();
                    allies.Remove (ally);
                    break;
                }
            }
        }

        void PlayerPosition (string playerId, float px, float pz, float ry) {
            foreach (Ally ally in allies) {
                if (ally.id == playerId) {
                    //ally.ClampPosition (new Vector3 (px, 1, pz));
                    ally.InterpolatePosition (new Vector3 (px, 1, pz));
                    ally.transform.rotation = Quaternion.Euler (0, ry, 0);
                    return;
                }
            }
        }

        void PlayerShoot (string playerId, float px, float pz, float ry) {
            foreach (Ally ally in allies) {
                if (ally.id == playerId) {
                    //ally.ClampPosition (new Vector3 (px, 1, pz));
                    ally.InterpolatePosition (new Vector3 (px, 1, pz));
                    ally.transform.rotation = Quaternion.Euler (0, ry, 0);
                    ally.Shoot ();
                    return;
                }
            }
        }
        #endregion
    }
}