using System;
using System.Collections.Generic;
using System.Linq;
using CaravanSecrets.Game.Board;

namespace CaravanSecrets.Features.Gameplay
{
    /// <summary>
    /// Maps typed level objectives to localization keys for the gameplay HUD.
    /// </summary>
    public static class GameplayObjectiveText
    {
        public static string Resolve(int levelIndex, IReadOnlyList<ObjectiveDefinition> objectives, Func<string, string> localize)
        {
            if (localize == null) throw new ArgumentNullException(nameof(localize));
            if (levelIndex < 5) return localize("objective.order");

            var keys = new List<string>();
            if (objectives != null)
            {
                foreach (var objective in objectives)
                {
                    var key = KeyFor(objective.Type);
                    if (!keys.Contains(key)) keys.Add(key);
                }
            }

            if (keys.Count == 0) return localize("objective.exit");
            if (keys.Count == 1) return localize(keys[0]);
            var separator = localize("objective.separator");
            return string.Join(separator, keys.Select(localize));
        }

        public static string KeyFor(ObjectiveType type) => type switch
        {
            ObjectiveType.DeliverAllCargo => "objective.cargo",
            ObjectiveType.ActivateAllSwitches => "objective.switches",
            _ => "objective.exit"
        };
    }
}
