using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpown : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefabs;
    [SerializeField] private Transform startPos;
    // 맵 사이즈 : 가로 30 세로 20
    private int galo = 30;
    private int selo = 20;
    private GameObject[,] tiles = new GameObject[20, 30];
    private void Awake()
    {
        for (int i = 0; i < selo; i++)
        {
            for (int j = 0; j < galo; j++)
            {
                tiles[i,j] = Instantiate(tilePrefabs,transform);
            }
        }
    }
    void Start()
    {
        for(int i = 0; i<selo; i++)
        {
            for (int j = 0; j < galo; j++)
            {
                tiles[i, j].transform.position
                    = new Vector2(startPos.position.x + j, startPos.position.y + i);
            }
        }
    }

}
