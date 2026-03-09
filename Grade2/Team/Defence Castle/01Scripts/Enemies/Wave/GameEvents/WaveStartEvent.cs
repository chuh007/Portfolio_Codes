using System;
using Chipmunk.GameEvents;
using Code.CoreSystem;

namespace Work.CHUH._01Scripts.Enemies.Wave.GameEvents
{
    public class WaveStartEvent : IEvent
    {
        public int currentWave;

        public WaveStartEvent(int currentWave)
        {
            this.currentWave = currentWave;
        }
    }
}