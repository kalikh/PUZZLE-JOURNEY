using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaravanSecrets.Game.Board
{
    public enum SolverStatus { Solved, Unsolvable, LimitReached, Unsupported, Invalid }

    public sealed class SolverOptions
    {
        public int MaxVisitedStates { get; set; } = 100000;
        public int MaxDepth { get; set; } = 200;
    }

    public sealed class SolverResult
    {
        public SolverStatus Status { get; }
        public IReadOnlyList<string> Moves { get; }
        public int VisitedStates { get; }
        public string Message { get; }
        public int MinimumMoves => Status == SolverStatus.Solved ? Moves.Count : -1;

        internal SolverResult(SolverStatus status, IReadOnlyList<string> moves, int visitedStates, string message)
        {
            Status = status;
            Moves = moves ?? Array.Empty<string>();
            VisitedStates = visitedStates;
            Message = message ?? string.Empty;
        }
    }

    public static class LevelSolver
    {
        private sealed class Node
        {
            public IReadOnlyList<string> Moves { get; }
            public Node(IReadOnlyList<string> moves) => Moves = moves;
        }

        public static SolverResult Solve(LevelDefinition level, SolverOptions options = null)
        {
            options ??= new SolverOptions();
            if (level != null && level.Cells.Values.Any(value => !Enum.IsDefined(typeof(CellType), value)))
                return new SolverResult(SolverStatus.Unsupported, null, 0, "The level contains an unsupported cell type.");
            var validation = LevelValidator.Validate(level);
            if (validation.Count > 0)
                return new SolverResult(SolverStatus.Invalid, null, 0, string.Join("; ", validation));
            if (options.MaxVisitedStates < 1 || options.MaxDepth < 0)
                return new SolverResult(SolverStatus.Invalid, null, 0, "Solver limits are invalid.");
            return SolveFromGame(new BoardGame(level), options);
        }

        public static SolverResult Solve(BoardGame startingGame, SolverOptions options = null)
        {
            if (startingGame == null) return new SolverResult(SolverStatus.Invalid, null, 0, "Starting game is required.");
            options ??= new SolverOptions();
            if (options.MaxVisitedStates < 1 || options.MaxDepth < 0)
                return new SolverResult(SolverStatus.Invalid, null, 0, "Solver limits are invalid.");
            return SolveFromGame(startingGame.Fork(), options);
        }

        private static SolverResult SolveFromGame(BoardGame startingGame, SolverOptions options)
        {
            var queue = new Queue<Node>();
            queue.Enqueue(new Node(Array.Empty<string>()));
            var visited = new HashSet<string>();
            visited.Add(StateKey(startingGame.State));
            var depthLimitReached = false;

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                var game = Replay(startingGame, node.Moves);
                if (game.State.IsComplete)
                    return new SolverResult(SolverStatus.Solved, node.Moves, visited.Count, "Minimum solution found.");
                if (node.Moves.Count >= options.MaxDepth) { depthLimitReached = true; continue; }

                var objectIds = game.State.Carts.Select(cart => cart.Id).Concat(game.State.Cargo.Select(cargo => cargo.Id))
                    .OrderBy(id => id, StringComparer.Ordinal);
                foreach (var cartId in objectIds)
                {
                    var nextGame = Replay(startingGame, node.Moves);
                    if (!nextGame.MoveObject(cartId)) continue;
                    var key = StateKey(nextGame.State);
                    if (!visited.Add(key)) continue;
                    if (visited.Count > options.MaxVisitedStates)
                        return new SolverResult(SolverStatus.LimitReached, null, visited.Count, "Visited-state limit reached.");
                    var moves = new List<string>(node.Moves) { cartId };
                    if (nextGame.State.IsComplete)
                        return new SolverResult(SolverStatus.Solved, moves, visited.Count, "Minimum solution found.");
                    queue.Enqueue(new Node(moves));
                }
            }

            return depthLimitReached
                ? new SolverResult(SolverStatus.LimitReached, null, visited.Count, "Search depth limit reached.")
                : new SolverResult(SolverStatus.Unsolvable, null, visited.Count, "No solution exists within the supported rules.");
        }

        private static BoardGame Replay(BoardGame startingGame, IReadOnlyList<string> moves)
        {
            var game = startingGame.Fork();
            foreach (var move in moves) game.MoveObject(move);
            return game;
        }

        private static string StateKey(BoardState state)
        {
            var builder = new StringBuilder();
            builder.Append(state.BarriersOpen ? '1' : '0').Append('|')
                .Append(state.StoredCartId ?? string.Empty).Append('|')
                .Append(state.HasFailed ? '1' : '0');
            foreach (var switchId in state.ActivatedSwitchIds.OrderBy(id => id, StringComparer.Ordinal))
                builder.Append("|s:").Append(switchId);
            foreach (var rock in state.TemporarilyRemovedRocks.OrderBy(item => item.X).ThenBy(item => item.Y))
                builder.Append("|r:").Append(rock.X).Append(',').Append(rock.Y);
            foreach (var cart in state.Carts.OrderBy(cart => cart.Id, StringComparer.Ordinal))
                builder.Append('|').Append(cart.Id).Append(':').Append(cart.Position.X).Append(',')
                    .Append(cart.Position.Y).Append(',').Append((int)cart.Direction).Append(',').Append(cart.HasExited ? '1' : '0');
            foreach (var cargo in state.Cargo.OrderBy(cargo => cargo.Id, StringComparer.Ordinal))
                builder.Append('|').Append(cargo.Id).Append(':').Append(cargo.Position.X).Append(',')
                    .Append(cargo.Position.Y).Append(',').Append((int)cargo.Direction).Append(',').Append(cargo.IsDelivered ? '1' : '0');
            return builder.ToString();
        }
    }
}
