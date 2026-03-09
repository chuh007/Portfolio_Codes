using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class moveTilewow : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;

    PlayerMove player;
    private void Start()
    {
        player = FindObjectOfType<PlayerMove>();
        player.Move += PlayerMoveGamJi;
        gameObject.SetActive(false);
    }
    public void PlayerMoveGamJi(Vector3 wow)
    {
        MovecardUI.isSeeArrow = false;
        MovecardUI.isUseMove = false;
        transform.position += wow;
        gameObject.SetActive(false);
    }
    public void TurnEndCardOff()
    {
        if (MovecardUI.isUseMove) return;
        gameObject.SetActive(false);
    }

    public void CheckAndSetActive()
    {
        Vector3Int tilePos = _tilemap.WorldToCell(transform.position);
        TileBase tile = _tilemap.GetTile(tilePos);
        if (tile == null) {
            gameObject.SetActive(true);
        }
    }
    public void CancelUse()
    {
        gameObject.SetActive(false);
    }
    
}
