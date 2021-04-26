﻿//#define DEBUG_AsteraX_LogMethods

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsteraX : MonoBehaviour
{
    public static AsteraX Instance = null; 
    
    // Private Singleton-style instance. Accessed by static property S later in script
    private static AsteraX _S;

    static List<Asteroid> ASTEROIDS;
    static List<Bullet> BULLETS;

    const float MIN_ASTEROID_DIST_FROM_PLAYER_SHIP = 5;


    // System.Flags changes how eGameStates are viewed in the Inspector and lets multiple 
    //  values be selected simultaneously (similar to how Physics Layers are selected).
    // It's only valid for the game to ever be in one state, but I've added System.Flags
    //  here to demonstrate it and to make the ActiveOnlyDuringSomeGameStates script easier
    //  to view and modify in the Inspector.
    // When you use System.Flags, you still need to set each enum value so that it aligns 
    //  with a power of 2. You can also define enums that combine two or more values,
    //  for example the all value below that combines all other possible values.
    [System.Flags]
    public enum eGameState
    {
        // Decimal      // Binary
        none = 0, // 00000000
        mainMenu = 1, // 00000001
        preLevel = 2, // 00000010
        level = 4, // 00000100
        postLevel = 8, // 00001000
        gameOver = 16, // 00010000
        all = 0xFFFFFFF // 11111111111111111111111111111111
    }


    [Header("Set in Inspector")] [Tooltip("This sets the AsteroidsScriptableObject to be used throughout the game.")]
    public AsteroidsScriptableObject asteroidsSO;

    public Text scoreText;
    public Text liveText;
    public GameObject panelGameOver;

    public int totalScore;
    public int totalLive; 


    private void Awake()
    {
#if DEBUG_AsteraX_LogMethods
        Debug.Log("AsteraX:Awake()");
#endif

        S = this;
        Instance = this; 
    }


    private void Start()
    {
#if DEBUG_AsteraX_LogMethods
        Debug.Log("AsteraX:Start()");
#endif

        totalLive = asteroidsSO.totalLive;
        
        ASTEROIDS = new List<Asteroid>();

        // Spawn the parent Asteroids, child Asteroids are taken care of by them
        for (var i = 0; i < 3; i++)
        {
            SpawnParentAsteroid(i);
        }
    }


    private void SpawnParentAsteroid(int i)
    {
#if DEBUG_AsteraX_LogMethods
        Debug.Log("AsteraX:SpawnParentAsteroid("+i+")");
#endif

        var ast = Asteroid.SpawnAsteroid();
        ast.gameObject.name = "Asteroid_" + i.ToString("00");
        // Find a good location for the Asteroid to spawn
        Vector3 pos;
        do
        {
            pos = ScreenBounds.RANDOM_ON_SCREEN_LOC;
        } while ((pos - PlayerShip.POSITION).magnitude < MIN_ASTEROID_DIST_FROM_PLAYER_SHIP);

        ast.transform.position = pos;
        ast.size = asteroidsSO.initialSize;
    }


    // ---------------- Static Section ---------------- //

    /// <summary>
    /// <para>This static public property provides some protection for the Singleton _S.</para>
    /// <para>get {} does return null, but throws an error first.</para>
    /// <para>set {} allows overwrite of _S by a 2nd instance, but throws an error first.</para>
    /// <para>Another advantage of using a property here is that it allows you to place
    /// a breakpoint in the set clause and then look at the call stack if you fear that 
    /// something random is setting your _S value.</para>
    /// </summary>
    private static AsteraX S
    {
        get
        {
            if (_S == null)
            {
                Debug.LogError("AsteraX:S getter - Attempt to get value of S before it has been set.");
                return null;
            }

            return _S;
        }
        set
        {
            if (_S != null)
            {
                Debug.LogError("AsteraX:S setter - Attempt to set S when it has already been set.");
            }

            _S = value;
        }
    }


    public static AsteroidsScriptableObject AsteroidsSO
    {
        get
        {
            if (S != null)
            {
                return S.asteroidsSO;
            }

            return null;
        }
    }

    public static void AddAsteroid(Asteroid asteroid)
    {
        if (ASTEROIDS.IndexOf(asteroid) == -1)
        {
            ASTEROIDS.Add(asteroid);
        }
    }

    public static void RemoveAsteroid(Asteroid asteroid)
    {
        if (ASTEROIDS.IndexOf(asteroid) != -1)
        {
            ASTEROIDS.Remove(asteroid);
        }
    }

    public void AddScore(int incScore)
    {
        totalScore += incScore;

        scoreText.text = totalScore.ToString();
    }

    public void AddLive(int incLive)
    {
        totalLive += incLive;

        liveText.text = totalLive == 1?  $"Live : {totalLive}" : $"Lives : {totalLive}";
    }
    
    
    public void OnClickRestart()
    {
        panelGameOver.SetActive(false);

        var scene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(scene.name);

        AddLive(3);
        totalScore = 0;
        AddScore(0);
    }
}