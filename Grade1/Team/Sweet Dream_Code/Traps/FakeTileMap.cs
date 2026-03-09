using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FakeTileMap : MonoBehaviour
{
    private Tilemap tilemap;
    [SerializeField] private Tilemap subTile;
    private Color tilemapColor;
    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(SetAlpha(false));
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(SetAlpha(true));
        }
    }
    IEnumerator SetAlpha(bool inOut)
    {
        for (float i = 0; i <= 1.1f; i += 0.1f)
        {
            yield return new WaitForSeconds(0.015f);
            tilemapColor = tilemap.color;
            if (inOut)
            {
                tilemapColor.a = i;
                tilemap.color = tilemapColor;
                subTile.color = tilemapColor;
            }
            else
            {
                tilemapColor.a = 1 - i;
                tilemap.color = tilemapColor;
                subTile.color = tilemapColor;
            }
        }
    }
}
