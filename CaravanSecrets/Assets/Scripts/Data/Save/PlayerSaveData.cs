using System;
using System.Collections.Generic;

namespace CaravanSecrets.Data.Save
{
    [Serializable]
    public sealed class PlayerSaveData
    {
        public int SaveVersion = 1;
        public string CurrentLevelId = "desert_01";
        public int Coins;
        public int MapFragments;
        public string LanguageCode = "ar";
        public List<LevelProgressData> Levels = new();
    }

    [Serializable]
    public sealed class LevelProgressData
    {
        public string LevelId;
        public bool IsComplete;
        public int BestMoveCount;
        public int BestStars;
    }
}
