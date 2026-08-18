#if UNITY_EDITOR
using CaravanSecrets.Game.Board;

namespace CaravanSecrets.Features.Gameplay
{
    public static class GameplayPlaytestOverride
    {
        public static LevelDefinition PendingLevel { get; set; }
    }
}
#endif
