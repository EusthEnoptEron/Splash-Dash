using UnityEngine;
using System.Collections;

public static class GamePresets {
    public static int CarNo;
    public static bool IsSlave;
    public static string ServerName
    {
        get
        {
            if (_serverName == "" || _serverName == null)
                _serverName = "Untitled #" + Random.Range(1, 10000);

            return _serverName;
        }
        set { _serverName = value; }
    }

    public static string PlayerName {
        get
        {
            if (_playerName == "" || _playerName == null)
                _playerName = "Anonymous #" + Random.Range(1, 10000);

            return _playerName;
        }
        set { _playerName = value; }
    }

    private static string _playerName;
    private static string _serverName;

    public static HostData Host;

    public const string TYPE_NAME = "BFH.GameDev.SplashDash";


    public static bool Colorful = true;

    public static int Laps = 3;
}
