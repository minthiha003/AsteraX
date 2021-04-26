#define DEBUG_PlayerShip_RespawnNotifications

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class PlayerShip : MonoBehaviour
{
    // This is a somewhat protected private singleton for PlayerShip
    private static PlayerShip   _S;

    public bool isImmune;

    public static PlayerShip    S
    {
        get => _S;
        set
        {
            if (_S != null)
            {
                Debug.LogWarning("Second attempt to set PlayerShip singleton _S.");
            }
            _S = value;
        }
    }

    [Header("Set in Inspector")]
    public float        shipSpeed = 10f;
    public GameObject   bulletPrefab;

    private Rigidbody rigid;


    private void Awake()
    {
        S = this;

        // NOTE: We don't need to check whether or not rigid is null because of [RequireComponent()] above
        rigid = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        // Using Horizontal and Vertical axes to set velocity
        var aX = CrossPlatformInputManager.GetAxis("Horizontal");
        var aY = CrossPlatformInputManager.GetAxis("Vertical");

        var vel = new Vector3(aX, aY);
        if (vel.magnitude > 1)
        {
            // Avoid speed multiplying by 1.414 when moving at a diagonal
            vel.Normalize();
        }

        rigid.velocity = vel * shipSpeed;

        // Mouse input for firing
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            Fire();
        }
    }


    private void Fire()
    {
        // Get direction to the mouse
        Vector3 mPos = Input.mousePosition;
        if (!(Camera.main is null))
        {
            mPos.z = -Camera.main.transform.position.z;
            Vector3 mPos3D = Camera.main.ScreenToWorldPoint(mPos);

            // Instantiate the Bullet and set its direction
            GameObject go = Instantiate<GameObject>(bulletPrefab);
            go.transform.position = transform.position;
            go.transform.LookAt(mPos3D);
        }
    }
    
    // Immune the ship 
    private IEnumerator ImmuneShip()
    {
        isImmune = true;

        yield return new WaitForSeconds(1);

        isImmune = false; 
    }

    private void OnCollisionEnter(Collision other)
    {
        if (isImmune)
        {
            return;
        }
        
        // Reduce the jump (live)
        if (AsteraX.Instance.totalLive > 1)
        {
            StartCoroutine(ImmuneShip());
            AsteraX.Instance.AddLive(-1);
        }
        else
        {
            AsteraX.Instance.AddLive(-1);
            // Game Over 
            AsteraX.Instance.panelGameOver.SetActive(true); 
            
            Destroy(gameObject);
        }
        
        var xpos = Random.Range(-11f, 11f);
        var ypos = Random.Range(-8f, 8f);

        transform.position = new Vector2(xpos, ypos);
        
       
    }

    public static float MAX_SPEED
    {
        get
        {
            return S.shipSpeed;
        }
    }
    
	public static Vector3 POSITION
    {
        get
        {
            return S.transform.position;
        }
    }
}
