using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Work.CHUH.Code.StageSystem.Editor
{
    internal sealed class InfiniteMapChunkEditSession
    {
        public InfiniteMapChunkDataSO Chunk;
        public InfiniteMapChunkLibrarySO Library;
        public TileBase Brush;
        public List<string> ValidationMessages;

        public void ResizeChunk(int width, int height)
        {
            Undo.RecordObject(Chunk, "Resize Infinite Map Chunk");
            Chunk.Resize(width, height);
            EditorUtility.SetDirty(Chunk);
            ValidationMessages = null;
        }

        public void FillWithBrush()
        {
            Undo.RecordObject(Chunk, "Fill Infinite Map Chunk");
            for (int y = 0; y < Chunk.Height; y++)
            {
                for (int x = 0; x < Chunk.Width; x++)
                {
                    Chunk.SetTile(x, y, Brush);
                }
            }

            EditorUtility.SetDirty(Chunk);
            ValidationMessages = null;
        }

        public void ClearChunk()
        {
            Undo.RecordObject(Chunk, "Clear Infinite Map Chunk");
            for (int y = 0; y < Chunk.Height; y++)
            {
                for (int x = 0; x < Chunk.Width; x++)
                {
                    Chunk.SetTile(x, y, null);
                }
            }

            EditorUtility.SetDirty(Chunk);
            ValidationMessages = null;
        }

        public void PaintCell(int x, int y)
        {
            Undo.RecordObject(Chunk, "Paint Infinite Map Chunk Cell");
            Chunk.SetTile(x, y, Brush);
            EditorUtility.SetDirty(Chunk);
            ValidationMessages = null;
        }
    }
}
