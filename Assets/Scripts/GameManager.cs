using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public BoardManager boardManager;

    private int level = 3;

    void Awake()
    {
        boardManager = GetComponent<BoardManager>();
        InitGame();
    }

    public void InitGame()
    {
        boardManager.SetupScene(level);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
