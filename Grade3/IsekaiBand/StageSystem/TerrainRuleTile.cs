using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem
{
    public enum TerrainType
    {
        Ground,
        Mint,
        Water,
        Wall
    }

    [CreateAssetMenu(fileName = "TerrainRuleTile", menuName = "Tiles/Terrain Rule Tile")]
    public class TerrainRuleTile : RuleTile<TerrainRuleTile.Neighbor>
    {
        public TerrainType terrainType;

        public class Neighbor : RuleTile.TilingRule.Neighbor
        {
            public const int Ground = 3;
            public const int Mint = 4;
            public const int Water = 5;
            public const int Wall = 6;
            public const int AnyTerrain = 7;
            public const int Empty = 8;
        }

        public override bool RuleMatch(int neighbor, TileBase tile)
        {
            TerrainRuleTile terrainTile = tile as TerrainRuleTile;

            return neighbor switch
            {
                Neighbor.Ground => terrainTile != null && terrainTile.terrainType == TerrainType.Ground,
                Neighbor.Mint => terrainTile != null && terrainTile.terrainType == TerrainType.Mint,
                Neighbor.Water => terrainTile != null && terrainTile.terrainType == TerrainType.Water,
                Neighbor.Wall => terrainTile != null && terrainTile.terrainType == TerrainType.Wall,
                Neighbor.AnyTerrain => terrainTile != null,
                Neighbor.Empty => tile == null,
                _ => base.RuleMatch(neighbor, tile)
            };
        }
    }
}
