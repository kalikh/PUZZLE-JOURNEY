using System;
using CaravanSecrets.Game.Board;

namespace CaravanSecrets.Game.Results
{
    public sealed class LevelResult
    {
        public string LevelId { get; }
        public bool IsComplete { get; }
        public int MoveCount { get; }
        public int RecommendedMoves { get; }
        public bool UsedBooster { get; }
        public int Stars { get; }

        internal LevelResult(string levelId, bool isComplete, int moveCount, int recommendedMoves, bool usedBooster, int stars)
        { LevelId = levelId; IsComplete = isComplete; MoveCount = moveCount; RecommendedMoves = recommendedMoves; UsedBooster = usedBooster; Stars = stars; }
    }

    public static class LevelResultCalculator
    {
        public static LevelResult Calculate(BoardGame game, int recommendedMoves)
        {
            if (game == null) throw new ArgumentNullException(nameof(game));
            var complete = game.State.IsComplete;
            var stars = complete ? 1 : 0;
            if (complete && recommendedMoves > 0 && game.State.MoveCount <= recommendedMoves) stars++;
            if (complete && !game.HasUsedBooster) stars++;
            return new LevelResult(game.State.LevelId, complete, game.State.MoveCount,
                Math.Max(0, recommendedMoves), game.HasUsedBooster, stars);
        }
    }
}
