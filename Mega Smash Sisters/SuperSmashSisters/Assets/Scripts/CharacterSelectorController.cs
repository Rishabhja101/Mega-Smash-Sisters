using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectorController : MonoBehaviour
{
    private int players;

    [SerializeField]
    private GameObject playerBoxPrefab;

	// Use this for initialization
	void Start ()
    {
        players = 2;
        displayPlayerBoxes();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void displayPlayerBoxes()
    {
        float width = (1020 - 20 * (players + 1)) / players;
        float first = width / 2 + 20;
        GameObject[] playerBoxes = new GameObject[players];
        for (int i = 0; i < players; i++)
        {
            float pos = first + i * (width + 20);
            pos -= 1020 / 2;
            playerBoxes[i] = Instantiate(playerBoxPrefab, new Vector3(0, 0, 0), Quaternion.identity, GameObject.Find("/Canvas").transform);
            playerBoxes[i].transform.localPosition = new Vector3(pos, -270, 0);
            playerBoxes[i].GetComponent<RectTransform>().sizeDelta = new Vector2(width, 200);
        }
    }
}
