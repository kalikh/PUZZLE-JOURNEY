using System;
using CaravanSecrets.Game.Board;

namespace CaravanSecrets.Game.Boosters
{
    public enum BoosterStatus { Applied, Ineligible, NoSolution }

    public sealed class BoosterRequest
    {
        public BoardGame Game { get; }
        public GridPosition? Target { get; }
        public BoosterRequest(BoardGame game, GridPosition? target = null)
        { Game = game ?? throw new ArgumentNullException(nameof(game)); Target = target; }
    }

    public sealed class BoosterResult
    {
        public BoosterStatus Status { get; }
        public string SuggestedObjectId { get; }
        public GridPosition? AffectedPosition { get; }
        public bool Applied => Status == BoosterStatus.Applied;
        private BoosterResult(BoosterStatus status, string suggestedObjectId = null, GridPosition? affectedPosition = null)
        { Status = status; SuggestedObjectId = suggestedObjectId; AffectedPosition = affectedPosition; }
        public static BoosterResult Success(string suggestion = null, GridPosition? position = null) => new(BoosterStatus.Applied, suggestion, position);
        public static BoosterResult Ineligible() => new(BoosterStatus.Ineligible);
        public static BoosterResult NoSolution() => new(BoosterStatus.NoSolution);
    }

    public interface IBooster
    {
        string BoosterId { get; }
        bool CanUse(BoosterRequest request);
        BoosterResult Use(BoosterRequest request);
    }

    public sealed class CompassBooster : IBooster
    {
        public string BoosterId => "compass";
        public bool CanUse(BoosterRequest request) => request != null && !request.Game.State.IsComplete && !request.Game.State.HasFailed;
        public BoosterResult Use(BoosterRequest request)
        {
            if (!CanUse(request)) return BoosterResult.Ineligible();
            var solution = LevelSolver.Solve(request.Game);
            if (solution.Status != SolverStatus.Solved || solution.Moves.Count == 0) return BoosterResult.NoSolution();
            request.Game.RecordBoosterUse(BoosterId);
            return BoosterResult.Success(solution.Moves[0]);
        }
    }

    public sealed class RopeBooster : IBooster
    {
        public string BoosterId => "rope";
        public bool CanUse(BoosterRequest request) => request != null && request.Target.HasValue &&
            !request.Game.State.IsComplete && !request.Game.State.HasFailed &&
            request.Game.State.IsTemporaryRockRemovalEligible(request.Target.Value);
        public BoosterResult Use(BoosterRequest request)
        {
            if (!CanUse(request)) return BoosterResult.Ineligible();
            var target = request.Target.Value;
            return request.Game.TryRemoveTemporaryRock(target, BoosterId)
                ? BoosterResult.Success(position: target)
                : BoosterResult.Ineligible();
        }
    }

    public sealed class ExtraSpaceBooster : IBooster
    {
        public string BoosterId => "extra_space";
        public bool CanUse(BoosterRequest request) => request != null && request.Target.HasValue &&
            !request.Game.State.IsComplete && !request.Game.State.HasFailed &&
            request.Game.State.IsExtraSpaceEligible(request.Target.Value);
        public BoosterResult Use(BoosterRequest request)
        {
            if (!CanUse(request)) return BoosterResult.Ineligible();
            var target = request.Target.Value;
            return request.Game.TryAddTemporaryStorageSpace(target, BoosterId)
                ? BoosterResult.Success(position: target)
                : BoosterResult.Ineligible();
        }
    }
}
