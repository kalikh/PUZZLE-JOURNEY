using CaravanSecrets.Features.Gameplay;
using CaravanSecrets.Game.Board;
using NUnit.Framework;

namespace CaravanSecrets.Game.Tests
{
    public sealed class GameplayObjectiveTextTests
    {
        [Test]
        public void Resolve_EarlyLevels_UseOrderTeachingKey()
        {
            var text = GameplayObjectiveText.Resolve(0, new[]
            {
                new ObjectiveDefinition("exit", ObjectiveType.ExitAllCarts)
            }, key => $"[{key}]");
            Assert.That(text, Is.EqualTo("[objective.order]"));
        }

        [Test]
        public void Resolve_CargoOnly_UsesCargoKey()
        {
            var text = GameplayObjectiveText.Resolve(11, new[]
            {
                new ObjectiveDefinition("deliver", ObjectiveType.DeliverAllCargo)
            }, key => $"[{key}]");
            Assert.That(text, Is.EqualTo("[objective.cargo]"));
        }

        [Test]
        public void Resolve_MixedObjectives_JoinsLocalizedParts()
        {
            var text = GameplayObjectiveText.Resolve(19, new[]
            {
                new ObjectiveDefinition("exit", ObjectiveType.ExitAllCarts),
                new ObjectiveDefinition("deliver", ObjectiveType.DeliverAllCargo),
                new ObjectiveDefinition("switches", ObjectiveType.ActivateAllSwitches)
            }, key => key switch
            {
                "objective.exit" => "EXIT",
                "objective.cargo" => "CARGO",
                "objective.switches" => "SWITCH",
                "objective.separator" => "|",
                _ => key
            });
            Assert.That(text, Is.EqualTo("EXIT|CARGO|SWITCH"));
        }

        [Test]
        public void KeyFor_MapsAllObjectiveTypes()
        {
            Assert.That(GameplayObjectiveText.KeyFor(ObjectiveType.ExitAllCarts), Is.EqualTo("objective.exit"));
            Assert.That(GameplayObjectiveText.KeyFor(ObjectiveType.DeliverAllCargo), Is.EqualTo("objective.cargo"));
            Assert.That(GameplayObjectiveText.KeyFor(ObjectiveType.ActivateAllSwitches), Is.EqualTo("objective.switches"));
        }
    }
}
