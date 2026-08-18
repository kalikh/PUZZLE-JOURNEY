using System;
using System.Collections.Generic;
using CaravanSecrets.Game.Board;

namespace CaravanSecrets.Editor.LevelEditor
{
    internal static class Stage4ProductionCatalog
    {
        public static IReadOnlyList<LevelDefinition> Create() => new[]
        {
            // Desert Road 6–10: blocking order, rocks, restart, and move efficiency.
            // L6 (Phase 1): L5-style release chain plus a switch-gated rock on the horizontal route.
            L(6, "desert", 6, 5, 5, Cells(Sw(1,2), Ex(4,2), R(3,2), Ex(2,4), Ex(4,3)),
                C("a",0,2,Direction.Right), C("b",2,2,Direction.Up), C("c",2,3,Direction.Right)),
            // L7: four-cart release; switch opens the shared rock; d waits on the gated column.
            L(7, "desert", 7, 6, 5, Cells(Sw(1,2), Ex(5,2), R(4,2), Ex(2,4), Ex(5,3), Ex(4,4)),
                C("a",0,2,Direction.Right), C("b",2,2,Direction.Up), C("c",2,3,Direction.Right), C("d",4,0,Direction.Up)),
            // L8 (Phase 1): four-cart dependency; switch opens the shared rock choke for a and d.
            L(8, "desert", 8, 6, 6, Cells(Sw(1,2), Ex(5,2), R(4,2), Ex(2,5), Ex(5,3), Ex(4,5)),
                C("a",0,2,Direction.Right), C("b",2,2,Direction.Up), C("c",2,3,Direction.Right), C("d",4,0,Direction.Up)),
            // L9: blocking order plus c must open the rock before a can finish.
            L(9, "desert", 9, 5, 5, Cells(Sw(1,3), R(3,1), Ex(4,1), Ex(2,4), Ex(4,3)),
                C("a",0,1,Direction.Right), C("b",2,1,Direction.Up), C("c",0,3,Direction.Right)),
            // L10: four-cart chain; d (or c) opens the rock that seals a's exit lane.
            L(10, "desert", 10, 6, 6, Cells(Sw(1,3), R(4,1), Ex(5,1), Ex(3,5), Ex(5,3), Ex(1,5)),
                C("a",0,1,Direction.Right), C("b",3,1,Direction.Up), C("c",0,3,Direction.Right), C("d",1,0,Direction.Up)),

            // Oasis Market 11–15: cargo identity, matching destinations, mixed objectives, and limited storage.
            // L11: water opens the spices gate — wrong order blocks at the gate.
            L(11, "oasis", 1, 5, 5, Cells(), Array.Empty<CartDefinition>(),
                cargo: Cargo(Cg("spices",0,2,Direction.Right,CargoType.Spices), Cg("water",2,0,Direction.Up,CargoType.Water)),
                cargoDestinations: CargoD(Cd(4,2,CargoType.Spices), Cd(2,4,CargoType.Water)),
                objectives: Obj(O("deliver",ObjectiveType.DeliverAllCargo), O("switches",ObjectiveType.ActivateAllSwitches)),
                gates: Gates(G("gate",3,2)), switches: Switches(S("switch",2,1,"gate"))),
            // L12 (Phase 1): water opens the fabric gate; occupying the cross before water passes forces a wait/undo.
            L(12, "oasis", 2, 5, 5, Cells(), Array.Empty<CartDefinition>(),
                cargo: Cargo(Cg("fabric",0,2,Direction.Right,CargoType.Fabrics), Cg("water",2,0,Direction.Up,CargoType.Water)),
                cargoDestinations: CargoD(Cd(4,2,CargoType.Fabrics), Cd(2,4,CargoType.Water)),
                objectives: Obj(O("deliver",ObjectiveType.DeliverAllCargo), O("switches",ObjectiveType.ActivateAllSwitches)),
                gates: Gates(G("gate",3,2)), switches: Switches(S("switch",2,1,"gate"))),
            // L13: tools open the cart gate; cart must clear the tools lane before tools finish.
            L(13, "oasis", 3, 5, 5, Cells(Ex(4,2)), C("cart",0,2,Direction.Right),
                cargo: Cargo(Cg("tools",2,0,Direction.Up,CargoType.Tools)), cargoDestinations: CargoD(Cd(2,4,CargoType.Tools)),
                objectives: Obj(O("exit",ObjectiveType.ExitAllCarts), O("deliver",ObjectiveType.DeliverAllCargo), O("switches",ObjectiveType.ActivateAllSwitches)),
                gates: Gates(G("gate",3,2)), switches: Switches(S("switch",2,1,"gate"))),
            // L14: metal opens food's gate (switch off food's lane); food opens scroll's gate after its own gate.
            L(14, "oasis", 4, 6, 5, Cells(), Array.Empty<CartDefinition>(),
                cargo: Cargo(Cg("food",0,1,Direction.Right,CargoType.Food), Cg("metal",2,0,Direction.Up,CargoType.MetalParts), Cg("scroll",5,3,Direction.Left,CargoType.Scrolls)),
                cargoDestinations: CargoD(Cd(5,1,CargoType.Food), Cd(2,4,CargoType.MetalParts), Cd(0,3,CargoType.Scrolls)),
                objectives: Obj(O("deliver",ObjectiveType.DeliverAllCargo), O("switches",ObjectiveType.ActivateAllSwitches)),
                gates: Gates(G("gate_food",3,1), G("gate_scroll",2,3)),
                switches: Switches(S("switch_metal",2,2,"gate_food"), S("switch_food",4,1,"gate_scroll"))),
            // L15: cart blocks artifact's switch lane; artifact opens cart's gate after cart releases.
            L(15, "oasis", 5, 5, 5, Cells(Ex(4,2)), C("cart",1,2,Direction.Right),
                cargo: Cargo(Cg("artifact",1,0,Direction.Up,CargoType.Artifacts)), cargoDestinations: CargoD(Cd(1,4,CargoType.Artifacts)),
                objectives: Obj(O("exit",ObjectiveType.ExitAllCarts), O("deliver",ObjectiveType.DeliverAllCargo), O("switches",ObjectiveType.ActivateAllSwitches)),
                gates: Gates(G("gate",3,2)), switches: Switches(S("switch",1,3,"gate")),
                storage: Store(St("bay",2,1,1))),

            // Oasis Market 16–20: explicit linked gates, switches, exits, and cargo-triggered mechanisms.
            // L16 (Phase 1): cargo key opens the gate; cart must clear the key lane before the key can finish delivery.
            L(16, "oasis", 6, 6, 5, Cells(Ex(5,2)), C("cart",0,2,Direction.Right),
                cargo: Cargo(Cg("key",1,0,Direction.Up,CargoType.Tools)), cargoDestinations: CargoD(Cd(1,4,CargoType.Tools)),
                objectives: Obj(O("exit",ObjectiveType.ExitAllCarts), O("deliver",ObjectiveType.DeliverAllCargo), O("switches",ObjectiveType.ActivateAllSwitches)),
                gates: Gates(G("gate",3,2)), switches: Switches(S("switch",1,1,"gate"))),
            // L17: main occupies helper's first cell — main must release, helper opens gate, main finishes.
            L(17, "oasis", 7, 6, 5, Cells(Ex(5,1),Ex(1,4)), Carts(C("main",1,1,Direction.Right), C("helper",1,0,Direction.Up)),
                gates: Gates(G("gate",3,1)), switches: Switches(S("switch",1,2,"gate")),
                objectives: Obj(O("exit",ObjectiveType.ExitAllCarts), O("switches",ObjectiveType.ActivateAllSwitches))),
            // L18: cross-wired keys; main_a blocks key_a until it advances toward its gate.
            L(18, "oasis", 8, 6, 5, Cells(Ex(5,1),Ex(1,4),Ex(5,3),Ex(2,0)),
                Carts(C("main_a",1,1,Direction.Right), C("key_a",1,0,Direction.Up), C("main_b",0,3,Direction.Right), C("key_b",2,4,Direction.Down)),
                gates: Gates(G("gate_a",3,1),G("gate_b",3,3)),
                switches: Switches(S("switch_a",1,2,"gate_b"),S("switch_b",2,2,"gate_a")),
                objectives: Obj(O("exit",ObjectiveType.ExitAllCarts), O("switches",ObjectiveType.ActivateAllSwitches))),
            // L19: shared switch opens both gates; amber/blue must clear for the key, then finish after.
            L(19, "oasis", 9, 6, 5, Cells(Ex(5,1),Ex(5,3),Ex(1,4)), Carts(C("amber",0,1,Direction.Right), C("blue",0,3,Direction.Right), C("key",1,0,Direction.Up)),
                destinations: Dest(D("amber",5,1),D("blue",5,3),D("key",1,4)), gates: Gates(G("gate_a",3,1),G("gate_b",3,3)),
                switches: Switches(S("switch",1,2,"gate_a","gate_b")), objectives: Obj(O("exit",ObjectiveType.ExitAllCarts),O("switches",ObjectiveType.ActivateAllSwitches))),
            // L20: water opens cart's gate; cart must not seal the water lane.
            L(20, "oasis", 10, 6, 5, Cells(Ex(5,1)), C("cart",0,1,Direction.Right),
                cargo: Cargo(Cg("water",0,3,Direction.Right,CargoType.Water)), cargoDestinations: CargoD(Cd(5,3,CargoType.Water)),
                objectives: Obj(O("exit",ObjectiveType.ExitAllCarts),O("deliver",ObjectiveType.DeliverAllCargo),O("switches",ObjectiveType.ActivateAllSwitches)),
                gates: Gates(G("gate",3,1)), switches: Switches(S("cargo_switch",1,3,"gate"))),

            // Forgotten City 21–25: direction changes, linked mechanisms, and multi-object objectives.
            // L21 (Phase 1): turn into a gated column; side key must open the gate before the turned cart can exit.
            L(21, "city", 1, 6, 5, Cells(Ex(2,4), Ex(0,3)), Carts(C("cart",0,1,Direction.Right), C("key",5,3,Direction.Left)),
                gates: Gates(G("gate",2,3)), switches: Switches(S("switch",3,3,"gate")),
                objectives: Obj(O("exit",ObjectiveType.ExitAllCarts), O("switches",ObjectiveType.ActivateAllSwitches)),
                directionTiles: Turns(T("turn_up",2,1,Direction.Up))),
            // L22: mutual gates — scroll opens key's exit after turning; key opens scroll's delivery gate.
            L(22, "city", 2, 6, 5, Cells(Ex(0,3)), Carts(C("key",5,3,Direction.Left)),
                cargo: Cargo(Cg("scroll",0,1,Direction.Right,CargoType.Scrolls)),
                cargoDestinations: CargoD(Cd(2,4,CargoType.Scrolls)),
                objectives: Obj(O("exit",ObjectiveType.ExitAllCarts), O("deliver",ObjectiveType.DeliverAllCargo), O("switches",ObjectiveType.ActivateAllSwitches)),
                gates: Gates(G("gate_scroll",2,3), G("gate_key",1,3)),
                switches: Switches(S("sw_key",3,3,"gate_scroll"), S("sw_scroll",2,2,"gate_key")),
                directionTiles: Turns(T("turn_up",2,1,Direction.Up))),
            // L23: double turn path; gate on the final approach; key opens it from a side lane.
            L(23, "city", 3, 6, 6, Cells(Ex(0,4), Ex(0,3)), Carts(C("cart",0,1,Direction.Right), C("key",5,3,Direction.Left)),
                gates: Gates(G("gate",1,4)), switches: Switches(S("switch",4,3,"gate")),
                objectives: Obj(O("exit",ObjectiveType.ExitAllCarts), O("switches",ObjectiveType.ActivateAllSwitches)),
                directionTiles: Turns(T("up",3,1,Direction.Up),T("left",3,4,Direction.Left))),
            // L24: turn into gated column; separate key (cart cannot self-open).
            L(24, "city", 4, 6, 6, Cells(Ex(2,5), Ex(0,3)), Carts(C("cart",0,1,Direction.Right), C("key",5,3,Direction.Left)),
                gates: Gates(G("gate",2,3)), switches: Switches(S("switch",3,3,"gate")),
                directionTiles: Turns(T("up",2,1,Direction.Up)),
                objectives: Obj(O("exit",ObjectiveType.ExitAllCarts),O("switches",ObjectiveType.ActivateAllSwitches))),
            // L25: cart turns into a gated column; parts open the gate from the side lane.
            L(25, "city", 5, 6, 6, Cells(Ex(3,5)), C("cart",0,1,Direction.Right),
                cargo: Cargo(Cg("parts",5,3,Direction.Left,CargoType.MetalParts)), cargoDestinations: CargoD(Cd(0,3,CargoType.MetalParts)),
                objectives: Obj(O("exit",ObjectiveType.ExitAllCarts),O("deliver",ObjectiveType.DeliverAllCargo),O("switches",ObjectiveType.ActivateAllSwitches)),
                gates: Gates(G("gate",3,3)), switches: Switches(S("switch",4,3,"gate")),
                directionTiles: Turns(T("cart_up",3,1,Direction.Up))),

            // Forgotten City 26–30: combined, readable dependency chains.
            // L26: key_cargo opens the vertical gate; cart must turn then wait.
            L(26, "city", 6, 6, 6, Cells(Ex(3,5)), C("cart",0,1,Direction.Right),
                cargo: Cargo(Cg("key_cargo",0,3,Direction.Right,CargoType.Artifacts)), cargoDestinations: CargoD(Cd(5,3,CargoType.Artifacts)),
                objectives: Obj(O("exit",ObjectiveType.ExitAllCarts),O("deliver",ObjectiveType.DeliverAllCargo),O("switches",ObjectiveType.ActivateAllSwitches)),
                gates: Gates(G("gate",3,3)), switches: Switches(S("switch",1,3,"gate")), directionTiles: Turns(T("cart_up",3,1,Direction.Up))),
            // L27: turner opens straight's gate; straight blocks the turner's exit if moved too early.
            L(27, "city", 7, 7, 6, Cells(Ex(3,5),Ex(6,3)), Carts(C("turner",0,1,Direction.Right),C("straight",0,3,Direction.Right)),
                gates: Gates(G("gate",4,3)), switches: Switches(S("switch",1,1,"gate")), directionTiles: Turns(T("up",3,1,Direction.Up)),
                objectives: Obj(O("exit",ObjectiveType.ExitAllCarts),O("switches",ObjectiveType.ActivateAllSwitches))),
            // L28: capacity-1 storage on water's path; water opens cart gate after parking pressure.
            L(28, "city", 8, 6, 6, Cells(Ex(5,1)), C("cart",0,1,Direction.Right),
                cargo: Cargo(Cg("water",2,0,Direction.Up,CargoType.Water)), cargoDestinations: CargoD(Cd(2,5,CargoType.Water)),
                objectives: Obj(O("exit",ObjectiveType.ExitAllCarts),O("deliver",ObjectiveType.DeliverAllCargo),O("switches",ObjectiveType.ActivateAllSwitches)),
                gates: Gates(G("gate",4,1)), switches: Switches(S("switch",2,3,"gate")), storage: Store(St("store",2,1,1))),
            // L29: tools open runner's gate; turner must clear before tools finish the column.
            L(29, "city", 9, 7, 7, Cells(Ex(4,6),Ex(6,3)), Carts(C("turner",0,1,Direction.Right),C("runner",0,3,Direction.Right)),
                cargo: Cargo(Cg("tools",2,0,Direction.Up,CargoType.Tools)), cargoDestinations: CargoD(Cd(2,6,CargoType.Tools)),
                objectives: Obj(O("exit",ObjectiveType.ExitAllCarts),O("deliver",ObjectiveType.DeliverAllCargo),O("switches",ObjectiveType.ActivateAllSwitches)),
                gates: Gates(G("gate",4,3)), switches: Switches(S("switch",2,4,"gate")), directionTiles: Turns(T("up",4,1,Direction.Up))),
            // L30: caravan starts on artifact's column (must release); artifact opens caravan gate; guide opens scroll gate.
            L(30, "city", 10, 7, 7, Cells(Ex(4,6),Ex(6,3)), Carts(C("guide",0,1,Direction.Right),C("caravan",2,3,Direction.Right)),
                cargo: Cargo(Cg("artifact",2,0,Direction.Up,CargoType.Artifacts),Cg("scroll",6,5,Direction.Left,CargoType.Scrolls)),
                cargoDestinations: CargoD(Cd(2,6,CargoType.Artifacts),Cd(0,5,CargoType.Scrolls)),
                objectives: Obj(O("exit",ObjectiveType.ExitAllCarts),O("deliver",ObjectiveType.DeliverAllCargo),O("switches",ObjectiveType.ActivateAllSwitches)),
                gates: Gates(G("gate_caravan",4,3), G("gate_scroll",3,5)),
                switches: Switches(S("sw_artifact",2,4,"gate_caravan"), S("sw_guide",4,2,"gate_scroll")),
                storage: Store(St("store",3,3,1)),
                directionTiles: Turns(T("guide_up",4,1,Direction.Up)))
        };

        private static LevelDefinition L(int global, string region, int regional, int width, int height,
            IReadOnlyDictionary<GridPosition, CellType> cells, params CartDefinition[] carts) =>
            L(global, region, regional, width, height, cells, carts, null, null, null, null, null, null, null, null);

        private static LevelDefinition L(int global, string region, int regional, int width, int height,
            IReadOnlyDictionary<GridPosition, CellType> cells, CartDefinition cart,
            IReadOnlyDictionary<string, GridPosition> destinations = null, IReadOnlyList<CargoDefinition> cargo = null,
            IReadOnlyDictionary<GridPosition, CargoType> cargoDestinations = null, IReadOnlyList<ObjectiveDefinition> objectives = null,
            IReadOnlyList<GateDefinition> gates = null, IReadOnlyList<SwitchDefinition> switches = null,
            IReadOnlyList<StorageDefinition> storage = null, IReadOnlyList<DirectionTileDefinition> directionTiles = null) =>
            L(global, region, regional, width, height, cells, new[] { cart }, destinations, cargo, cargoDestinations,
                objectives, gates, switches, storage, directionTiles);

        private static LevelDefinition L(int global, string region, int regional, int width, int height,
            IReadOnlyDictionary<GridPosition, CellType> cells, IReadOnlyList<CartDefinition> carts,
            IReadOnlyDictionary<string, GridPosition> destinations = null, IReadOnlyList<CargoDefinition> cargo = null,
            IReadOnlyDictionary<GridPosition, CargoType> cargoDestinations = null, IReadOnlyList<ObjectiveDefinition> objectives = null,
            IReadOnlyList<GateDefinition> gates = null, IReadOnlyList<SwitchDefinition> switches = null,
            IReadOnlyList<StorageDefinition> storage = null, IReadOnlyList<DirectionTileDefinition> directionTiles = null) =>
            new($"desert_{global:00}", width, height, cells, carts, destinations, region, regional, 0, 10 + global * 2,
                cargo, cargoDestinations, objectives, gates, switches, storage, directionTiles);

        private static Dictionary<GridPosition, CellType> Cells(params KeyValuePair<GridPosition, CellType>[] values)
        { var result = new Dictionary<GridPosition, CellType>(); foreach (var value in values) result[value.Key] = value.Value; return result; }
        private static KeyValuePair<GridPosition, CellType> Ex(int x,int y) => new(new GridPosition(x,y),CellType.Exit);
        private static KeyValuePair<GridPosition, CellType> R(int x,int y) => new(new GridPosition(x,y),CellType.Rock);
        private static KeyValuePair<GridPosition, CellType> Sw(int x,int y) => new(new GridPosition(x,y),CellType.Switch);
        private static CartDefinition C(string id,int x,int y,Direction d) => new(id,new GridPosition(x,y),d);
        private static IReadOnlyList<CartDefinition> Carts(params CartDefinition[] values) => values;
        private static CargoDefinition Cg(string id,int x,int y,Direction d,CargoType t) => new(id,new GridPosition(x,y),d,t);
        private static GateDefinition G(string id,int x,int y) => new(id,new GridPosition(x,y));
        private static SwitchDefinition S(string id,int x,int y,params string[] gates) => new(id,new GridPosition(x,y),gates);
        private static StorageDefinition St(string id,int x,int y,int capacity) => new(id,new GridPosition(x,y),capacity);
        private static DirectionTileDefinition T(string id,int x,int y,Direction d) => new(id,new GridPosition(x,y),d);
        private static ObjectiveDefinition O(string id,ObjectiveType type) => new(id,type);
        private static KeyValuePair<string,GridPosition> D(string id,int x,int y) => new(id,new GridPosition(x,y));
        private static KeyValuePair<GridPosition,CargoType> Cd(int x,int y,CargoType type) => new(new GridPosition(x,y),type);
        private static Dictionary<string,GridPosition> Dest(params KeyValuePair<string,GridPosition>[] values)
        { var result=new Dictionary<string,GridPosition>(); foreach(var value in values) result[value.Key]=value.Value; return result; }
        private static Dictionary<GridPosition,CargoType> CargoD(params KeyValuePair<GridPosition,CargoType>[] values)
        { var result=new Dictionary<GridPosition,CargoType>(); foreach(var value in values) result[value.Key]=value.Value; return result; }
        private static IReadOnlyList<CargoDefinition> Cargo(params CargoDefinition[] values) => values;
        private static IReadOnlyList<ObjectiveDefinition> Obj(params ObjectiveDefinition[] values) => values;
        private static IReadOnlyList<GateDefinition> Gates(params GateDefinition[] values) => values;
        private static IReadOnlyList<SwitchDefinition> Switches(params SwitchDefinition[] values) => values;
        private static IReadOnlyList<StorageDefinition> Store(params StorageDefinition[] values) => values;
        private static IReadOnlyList<DirectionTileDefinition> Turns(params DirectionTileDefinition[] values) => values;
    }
}
