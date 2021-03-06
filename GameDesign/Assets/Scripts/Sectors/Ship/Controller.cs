﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using System.IO;
/*
 * Created by Kelby
 * Controller Script for player object
 * 
 * ToDo:  ***************************************************************************************
 * have fuel draining in place
 * create event to be called to check fuel.
 * use event when moving or inside coroutine instead of update
 * 
 * 
 * Comment rest of code
 * 
 * Notes:******************************************************************************************
 * Use as little code in the update as possible.
 *  use if statements as little as possible in update. they are slower than switch and it runs every frame
 * 
 */
public class Controller : NetworkBehaviour {
    #region public variables
    public double fuel, maxFuel = 100, drainRate;
    public Sprite[] textures = new Sprite[4];
    public float shipThrust;
    [SyncVar]
    public int sectorX = 0, sectorY = 0;
    public SectorCenter sector;

    #endregion
    #region private variables
    bool canMove = true;
    int hor, vert;
    private float thrust = 0, turn = 0, shipRotationSpeed = 150.0f;
    Rigidbody2D rb;
    Transform trans;
    Camera cam;
    SpriteRenderer rend;
    SoundOscillator sound;
    #endregion

    private void Start()
    {
        //getting the camera component when we start
        cam = gameObject.GetComponentInChildren<Camera>();
        //getting the rigidbody2d and assigning it to rb
         rb = gameObject.GetComponent<Rigidbody2D>();
        //assigning the transform to trans
        trans = transform;
        gameObject.GetComponentInChildren<Skybox>().material.SetInt("_Formuparam", UnityEngine.Random.Range(450, 550));
        rend = trans.GetChild(2).GetComponent<SpriteRenderer>();
        sound = gameObject.GetComponent<SoundOscillator>();
        drainRate = maxFuel / 400;
    }
    private void Update()
    {
        hor = (int)Input.GetAxis("Horizontal");
        vert = (int)Input.GetAxis("Vertical");
        if (!isLocalPlayer)
        {
            cam.enabled = false;
            return;

        }
        if(hor != 0)
        {
            turn += hor * shipRotationSpeed * .001f * Time.deltaTime;

        }else if(Mathf.Abs(turn) > .1f)
        {
            turn -= (turn / 50f);
        }
        else
        {
            turn = 0;
        }
     
            
            switch (vert)
            {
                case -1:
                    rend.sprite = textures[1];
                    break;
                case 1:
                    rend.sprite = textures[2];
                    break;
                case 0:
                    rend.sprite = textures[0];
                    break;

            }
            sound.frequency = Mathf.Lerp(this.GetComponent<SoundOscillator>().frequency, (100 + Mathf.Abs(vert)*100), 1.5f * Time.deltaTime);
            thrust = vert * shipThrust;
    }
    private void FixedUpdate()
    {
        rb.AddForce(thrust * transform.forward * Time.deltaTime);
        //add rotation using transform.rotation and apply drag in update
        
        trans.Rotate(new Vector3(0, turn, 0));   
        
    }
    #region public functions
    public void addHydrogen()
    {
        fuel += drainRate;
       // Debug.Log(fuel);
    }

    #endregion
    /*
    public double maxSpeed;

    public double hydrogen;
    public double fuelPerTime;
    double maxH = 100;
    public float forceAmount, currentSpeed, thrust, turn, shipRotationSpeed, shipThrust, boostThrust = 1.5f;
	public bool allowMovement;
	public Rigidbody rb;
    public GameObject sun;
    public GameObject station;
    public GameObject planet;
    public int[] upgrades ;
    public Sprite[] textures;
    public static double drainRate;
    public Camera cam;
    NetworkIdentity iden;
    private float boostFuel, regFuel, sWidth, guiRatio, originalFuel, originalBoost, originalThrust;
    private bool full, high, mid, low, empty;

    public GUISkin guiSkin;
    //create a scale Vector3 with the above ratio  
    private Vector3 GUIsF;

    // Use this for initialization
    void Start () {
        iden = gameObject.GetComponent<NetworkIdentity>();
        drainRate = .1;
        originalThrust = 90;
		rb = transform.GetComponent<Rigidbody>();
        //calculating the fuel usage. it should come out to .09/sec
        originalFuel = regFuel = shipThrust / 100000;
        //the boost fuel usage is equal to the regular usage * the boost thrust 
        originalBoost = boostFuel = regFuel * boostThrust;
        calculateValues();
	}
    private string clientName()
    {
        StreamReader read = new StreamReader(Application.persistentDataPath + "/name.dat");
        return read.ReadLine();
    }
   
    public override void OnStartClient()
    {
        ClientScene.RegisterPrefab(planet);
        ClientScene.RegisterPrefab(station);
        ClientScene.RegisterPrefab(sun);
        cam.gameObject.SetActive(true);
        Skybox camera = gameObject.GetComponentInChildren<Skybox>();
        GameObject.Find("pause").GetComponent<pause>().canvas.gameObject.SetActive(true);

        GameObject.Find("Stats").GetComponent<GUIStats>().ship = this;
        GameObject.Find("pause").GetComponent<menu>().ship = this;
        GameObject.Find("pause").GetComponent<pause>().ship = this;

        //this.name = clientName();

    }

    public override void OnStartAuthority()
    {
        

    }


    //At this script initialization  
    void Awake()
    {

        upgrades = new int[3];
        
        //get the screen's width  
        sWidth = Screen.width;
        //calculate the scale ratio  
        guiRatio = sWidth / 1920;
        //create a scale Vector3 with the above ratio  
        GUIsF = new Vector3(guiRatio, guiRatio, 1);
    }
    void OnGUI()
    {
        //scale and position the GUI element to draw it at the screen's top left corner  
        if (hydrogen > maxH * 4/5)
        {
            GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width - 140 * GUIsF.x, 85 * GUIsF.y, 0), Quaternion.identity, GUIsF);
            //these labels should all be same
            GUI.Label(new Rect(0, 0, 100, 20), "", guiSkin.customStyles[4]);
        }
        else
        {
            GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width - 140 * GUIsF.x, 85 * GUIsF.y, 0), Quaternion.identity, GUIsF);
            //these labels should all be same
            GUI.Label(new Rect(0, 0, 100, 20), "", guiSkin.customStyles[5]);
        }
        if (hydrogen > maxH * 3/5)
        {
            //beneath the first bar
            GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width - 140 * GUIsF.x, 115 * GUIsF.y, 0), Quaternion.identity, GUIsF);
            //draw GUI on the bottom right  
            GUI.Label(new Rect(0, 0, 100, 20), "", guiSkin.customStyles[3]);
        }
        else
        {
            GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width - 140 * GUIsF.x, 115 * GUIsF.y, 0), Quaternion.identity, GUIsF);
            //draw GUI on the bottom right  
            GUI.Label(new Rect(0, 0, 100, 20), "", guiSkin.customStyles[5]);
        }
        if (hydrogen > maxH * 2/5)
        {
            //beneath second bar
            GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width - 140 * GUIsF.x, 150 * GUIsF.y, 0), Quaternion.identity, GUIsF);
            GUI.Label(new Rect(0, 0, 100, 20), "", guiSkin.customStyles[2]);
        }
        else
        {
            GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width - 140 * GUIsF.x, 150 * GUIsF.y, 0), Quaternion.identity, GUIsF);
            GUI.Label(new Rect(0, 0, 100, 20), "", guiSkin.customStyles[5]);
        }
        if (hydrogen > maxH/5)
        {
            //beneath the third
            GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width - 140 * GUIsF.x, 185 * GUIsF.y, 0), Quaternion.identity, GUIsF);
            GUI.Label(new Rect(0, 0, 100, 20), "", guiSkin.customStyles[1]);
        }
        else
        {
            GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width - 140 * GUIsF.x, 185 * GUIsF.y, 0), Quaternion.identity, GUIsF);
            GUI.Label(new Rect(0, 0, 100, 20), "", guiSkin.customStyles[5]);
        }
        if (hydrogen > 0)
        {
            //beneath the fourth
            GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width - 140 * GUIsF.x, 220 * GUIsF.y, 0), Quaternion.identity, GUIsF);
            GUI.Label(new Rect(0, 0, 100, 20), "", guiSkin.customStyles[0]);
        }
        else
        {
            GUI.matrix = Matrix4x4.TRS(new Vector3(Screen.width - 140 * GUIsF.x, 220 * GUIsF.y, 0), Quaternion.identity, GUIsF);
            GUI.Label(new Rect(0, 0, 100, 20), "", guiSkin.customStyles[5]);
        }

    }

    // Update is called once per frame
    [Client]
    void Update ()
	{
        
        if (!isLocalPlayer)
        {
            cam.enabled = false;
            gameObject.GetComponentInChildren<AudioListener>().enabled = false;
            Debug.Log("return");
            Debug.Log(playerControllerId);
            Debug.Log(netId);
            return;
        }
            
		rb.mass = 1 + (float)(hydrogen/500);
		if (hydrogen > 0) {
			allowMovement = true;
		} else {
			allowMovement = false;
		}

		currentSpeed = rb.velocity.magnitude;
		if (allowMovement) {
			if (Input.GetAxis ("Vertical") != 0) {
				thrust = Input.GetAxis ("Vertical") * shipThrust;

                if (Input.GetAxis("Vertical") > 0f)
                {
                    this.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = textures[1];
                    this.GetComponent<SoundOscillator>().frequency = Mathf.Lerp(this.GetComponent<SoundOscillator>().frequency, 200, 1.5f * Time.deltaTime);
                } else if(Input.GetAxis("Vertical") < 0f)
                {
                    this.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = textures[2];
                    this.GetComponent<SoundOscillator>().frequency = Mathf.Lerp(this.GetComponent<SoundOscillator>().frequency, 200, 1.5f * Time.deltaTime);
                }

                // sutracting used fuel from hydrogen
                
				if (Input.GetKey(KeyCode.LeftShift)) {
                    this.GetComponent<SoundOscillator>().frequency = Mathf.Lerp(this.GetComponent<SoundOscillator>().frequency, 250, 3f * Time.deltaTime);
                    thrust *= boostThrust;
                    hydrogen -= boostFuel;
				}
                hydrogen -= regFuel;
            }
			if (Input.GetAxis ("Horizontal") != 0) {
				turn = Input.GetAxis ("Horizontal") * shipRotationSpeed;
                hydrogen -= (regFuel / 10);
			}
            if(Input.GetAxis("Vertical") == 0f)
            {
                this.transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = textures[0];
                this.GetComponent<SoundOscillator>().frequency = Mathf.Lerp(this.GetComponent<SoundOscillator>().frequency, 100, 1.5f * Time.deltaTime);
            }
		

			
		}
        #region sector switch
        if( transform.position.z <7000 && transform.position.z >-7000 &&transform.position.x >= 7000 )
        {
            loadData.data.saveSector();
            loadData.data.secX++;
            transform.position = new Vector3((0 - transform.position.x) + 50, transform.position.y, transform.position.z);
            loadData.data.load();
        }else if (transform.position.z < 7000 && transform.position.z > -7000 && transform.position.x <= -7000)
        {
            loadData.data.saveSector();

            loadData.data.secX--;
            transform.position = new Vector3((0 - transform.position.x) - 50, transform.position.y, transform.position.z);
            loadData.data.load();

            
        }
        else if (transform.position.x < 7000 && transform.position.x > -7000 && transform.position.z >= 7000)
        {
            loadData.data.saveSector();

            loadData.data.secZ++;
            transform.position = new Vector3(transform.position.x, transform.position.y, (0 - transform.position.z) + 50);
            loadData.data.load();

            
        }else if (transform.position.x < 7000 && transform.position.x > -7000 && transform.position.z <= -7000)
        {
            loadData.data.saveSector();

            loadData.data.secZ--;
            transform.position = new Vector3(transform.position.x, transform.position.y, (0 - transform.position.z) - 50);
            loadData.data.load();

            
        }
        else if (transform.position.z <=-7000 && transform.position.x <= -7000)
        {
            loadData.data.saveSector();

            loadData.data.secX--;
            loadData.data.secZ--;
            transform.position = new Vector3((0 - transform.position.x) - 50, transform.position.y, (0 - transform.position.z) - 50);
            loadData.data.load();

            
        }
        else if (transform.position.z >= 7000 && transform.position.x >= 7000)
        {
            loadData.data.saveSector();

            loadData.data.secX++;
            loadData.data.secZ++;
            transform.position = new Vector3((0 - transform.position.x) + 50, transform.position.y, (0 - transform.position.z) + 50);
            loadData.data.load();

           
        }
        else if (transform.position.z >= 7000 && transform.position.x <= -7000)
        {
            loadData.data.saveSector();

            loadData.data.secX--;
            loadData.data.secZ++;
            transform.position = new Vector3((0 - transform.position.x) - 50, transform.position.y, (0 - transform.position.z) + 50);

            loadData.data.load();

        }
        else if (transform.position.z <= -7000 && transform.position.x >= 7000)
        {
            loadData.data.saveSector();

            loadData.data.secX++;
            loadData.data.secZ--;
            transform.position = new Vector3((0 - transform.position.x) + 50, transform.position.y, (0 - transform.position.z) - 50);
            loadData.data.load();

            
        }
        #endregion
    }

    public bool vacuum ()
	{
        if(hydrogen < maxH)
        {
            hydrogen += drainRate;
            return true;
        }else
        {
            return false;
        }
	}
    public void transfer()
    {
        hydrogen -= drainRate;
    }
    public double getHydrogen()
    {
        return hydrogen;
    }
    public double getMaxHydrogen()
    {
        return maxH;
    }
    public void calculateValues()
    {
        regFuel = originalFuel - (upgrades[(int)IEnum.ShipUpgrades.effeciency] * .00015f);
        boostFuel = originalBoost - ( upgrades[(int)IEnum.ShipUpgrades.effeciency] * .0015f);
        shipThrust =(originalThrust + ((float)Math.Pow(upgrades[(int)IEnum.ShipUpgrades.speed], 2) * 45));
        maxH = 100 + (Math.Pow(upgrades[(int)IEnum.ShipUpgrades.capacity], 2) * 50);
        drainRate = maxH / 600;

        Debug.Log(boostFuel);
        Debug.Log(regFuel);
        Debug.Log(maxH);
    }
    private int randomIntFromSeed(int min, int max)
    {
        int number = UnityEngine.Random.Range(min, max);
        return number;
    }
    */
}
