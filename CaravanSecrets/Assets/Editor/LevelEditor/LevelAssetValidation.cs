using System.Collections.Generic;
using System.Linq;
using CaravanSecrets.Data.Levels;
using CaravanSecrets.Game.Board;

namespace CaravanSecrets.Editor.LevelEditor
{
    public static class LevelAssetValidation
    {
        public static IReadOnlyList<string> Validate(LevelAsset asset, IEnumerable<LevelAsset> allAssets = null)
        {
            var issues = new List<string>();
            if (asset == null) { issues.Add("Level asset is missing."); return issues; }
            issues.AddRange(LevelValidator.Validate(asset.ToDefinition()));

            foreach (var duplicate in asset.Cells.GroupBy(cell => cell.Position).Where(group => group.Count() > 1))
                issues.Add($"Multiple cells occupy {duplicate.Key}.");
            foreach (var duplicate in asset.Destinations.GroupBy(item => item.CartId).Where(group => group.Count() > 1))
                issues.Add($"Cart {duplicate.Key} has multiple destination links.");
            foreach (var duplicate in asset.Carts.GroupBy(cart => cart.Position).Where(group => group.Count() > 1))
                issues.Add($"Multiple carts occupy {duplicate.Key}.");
            foreach (var duplicate in asset.Cargo.GroupBy(item => item.Id).Where(group => group.Count() > 1))
                issues.Add($"Duplicate cargo ID: {duplicate.Key}.");
            foreach (var duplicate in asset.Cargo.GroupBy(item => item.Position).Where(group => group.Count() > 1))
                issues.Add($"Multiple cargo objects occupy {duplicate.Key}.");
            foreach (var duplicate in asset.CargoDestinations.GroupBy(item => item.Position).Where(group => group.Count() > 1))
                issues.Add($"Multiple cargo destinations occupy {duplicate.Key}.");
            foreach (var duplicate in asset.Objectives.GroupBy(item => item.Id).Where(group => group.Count() > 1))
                issues.Add($"Duplicate objective ID: {duplicate.Key}.");
            var mechanisms = asset.Gates.Select(item => (item.Id, item.Position))
                .Concat(asset.LinkedSwitches.Select(item => (item.Id, item.Position)))
                .Concat(asset.StorageSlots.Select(item => (item.Id, item.Position)))
                .Concat(asset.DirectionTiles.Select(item => (item.Id, item.Position)));
            foreach (var duplicate in mechanisms.GroupBy(item => item.Id).Where(group => group.Count() > 1))
                issues.Add($"Duplicate mechanism ID: {duplicate.Key}.");
            foreach (var duplicate in mechanisms.GroupBy(item => item.Position).Where(group => group.Count() > 1))
                issues.Add($"Multiple mechanisms occupy {duplicate.Key}.");

            if (allAssets != null && asset.LevelNumber > 0)
            {
                var conflict = allAssets.FirstOrDefault(other => other != null && other != asset &&
                    other.RegionId == asset.RegionId && other.LevelNumber == asset.LevelNumber);
                if (conflict != null)
                    issues.Add($"Level number {asset.LevelNumber} is already used by {conflict.LevelId} in region {asset.RegionId}.");
            }
            return issues;
        }
    }
}
