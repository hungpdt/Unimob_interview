# Farm Tycoon

A small fruit-farm tycoon built in Unity 2021.3 LTS. You build crops on a
grid and upgrade them, a delivery worker walks out, harvests, carries the fruit back to the market,
and sells it to a waiting customer. Money is shown as big numbers (1K / 1M / 1B) and feeds back into
an upgrade panel that buffs profit.

I focused on the four things the brief calls out: a state machine for the characters, a stat buff
system that can stack from several independent sources, `double`-based currency for large numbers,
and Unity NavMesh for movement. All balance numbers live in ScriptableObjects so nothing is
hard-coded in scripts.

## Features

| Action | What happens | Main classes |
|---|---|---|
| Build | Click an empty plot → popup → pay `BuildCost` → box opens (anim + countdown) → crop appears and a worker spawns | `ConstructionController`, `ConstructionBuildViewController` |
| Upgrade crop | Click a built crop → popup shows cost/payout → pay tier cost → +level, adds an additive profit bonus (+10%, +20%…) | `ConstructionController.TryUpgrade`, `ConstructionUpgradeViewController` |
| Harvest | Worker paths to the crop, waits `HarvestDuration`, then locks the sale value at that moment | `HarvestState`, `ConstructionController.Harvest` |
| Deliver & sell | Worker carries the value to a free dock, hands the fruit to the customer, credits the money | `MoveToMarketState`, `DeliverState`, `MarketController`, `DockController` |
| Customer | Spawns, queues, is assigned a dock, waits, receives, leaves (starts with 1) | `CustomerController`, `CustomerStates` |
| Manage panel | Buy ×N profit for one crop, ×N profit for all crops, or +N customers | `UpgradeManager`, `UpgradeViewController`, `UpgradeConfig` |
| Big numbers | All money is `double`, rendered as 1K / 1M / 1B / 1T | `CurrencyManager`, `NumberFormatter` |

## Architecture

### State machine

Characters use a small generic `StateMachine<T>` with one class per state (`BaseState<T>` with
`Enter` / `Tick` / `Exit`). I chose this over an `enum` + `switch` so each state keeps its own local
data (e.g. `HarvestState._elapsed`) and adding a state doesn't touch existing ones. The state
instances are created once in `Initialize` and cached as fields, so transitions don't allocate.

```
Delivery:  Idle → MoveToConstruction → Harvest → WaitForCustomer → MoveToMarket → Deliver → Leave
Customer:  Idle → MoveToDock → Wait → Receive → Leave
```

`DeliveryController` and `CustomerController` share `CharacterBaseController`, which wraps the
`NavMeshAgent` and the fruit-carry logic. I pulled that base out once the two controllers started
duplicating the same movement code.

### Profit and multi-source buffs

Profit isn't computed by multiplying inline. Every buff is a `StatModifier` stored in a
`StatCollection`, which evaluates one formula and caches it:

```
Final = Base × (1 + Σ Additive) × Π Multiplicative
```

- **Additive** — level upgrades. +10% and +20% sum to ×1.30 (not ×1.21), which matches how players
  read "percent bonuses."
- **Multiplicative** — manage-panel "×2" upgrades. Each is a separate factor, so buying ×2 twice
  gives ×4.

### Currency

`CurrencyManager` keeps the balance as `double`, not `int`/`long`. Tycoon economies blow past
`long`'s range quickly, and once numbers are in the millions the lost integer precision doesn't
matter because everything is shown rounded. Balance changes fire `OnCoinChanged`; the HUD
subscribes and updates immediately. `NumberFormatter` adds K/M/B/T/… suffixes and guards
`NaN`/`Infinity`.

### Pathfinding

Movement is Unity NavMesh, baked into the Gameplay scene. Each worker/customer has a
`NavMeshAgent`; `CharacterBaseController.HasArrived` checks `pathPending`, `stoppingDistance`, and
residual velocity before reporting arrival. States only say *where* to go — the agent handles the
shortest path and obstacle avoidance.

### Layering

- **`GameManager`** is the composition root: it constructs the plain-C# services
  (`CurrencyManager`, `StatService`, `UpgradeManager`) and injects them, and finds the scene
  managers in `Awake`.
- **`EventBus`** is a static type-keyed pub/sub with struct payloads (`HarvestCompletedEvent`,
  `SaleCompletedEvent`, …) so publishers and the UI don't reference each other directly.
- **ScriptableObjects** hold every tunable number.
- **`PrefabPool`** recycles workers and customers instead of `Instantiate`/`Destroy` each cycle.

## Project structure

```
Assets/Scripts/
├── Core/        GameManager, CurrencyManager, UpgradeManager, EventBus, Events
├── Stats/       StatService, StatCollection, StatModifier (Final = Base × (1+Σadd) × Πmult)
├── Characters/  StateMachine, CharacterBaseController, Delivery/Customer controllers + states
├── Gameplay/    ConstructionController/Manager, MarketController, DockController
├── Data/        GameConfig + Construction/Upgrade/Delivery/Customer configs (ScriptableObjects)
├── UI/          UIManager, view controllers, info billboard, build timer
└── Utils/       NumberFormatter, EffectSpawner, PrefabPool
```

## How to run

1. Open in Unity 2021.3.x LTS.
2. Open `Assets/Scenes/Gameplay.unity`.
3. Press Play.

Click a box to build, click a built crop to upgrade, use the HUD button for the manage panel.

## Build settings

Android, portrait, 1920×1080, Min API 24. Final/submission build uses IL2CPP + ARM64. Make sure `Gameplay.unity` is in Build Settings.

## Notes / next steps

- No save system — coins, plots, and purchased upgrades reset each run.
- Add cheat
