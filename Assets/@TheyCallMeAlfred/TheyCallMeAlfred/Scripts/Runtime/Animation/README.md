# Animation System

A flexible, project-agnostic animation system for Unity.

---

## Quick Start

### 1. Setup

1. Add an `Animator` component to your character (with an Animator Controller)
2. Add a `CharacterAnimator` component
3. In the Inspector, add your animations to the **Animations** list

### 2. Configure Animations

Each animation entry has:

| Field | Description |
|-------|-------------|
| **Key** | Unique identifier (e.g., "Idle", "Attack1") |
| **Animator State Name** | State name in Animator Controller (defaults to Key) |
| **Transition Duration** | Crossfade time (0.1-0.2 typical) |
| **Can Be Interrupted** | If false, this animation must complete |
| **Is Looping** | Whether the animation loops |
| **Speed Multiplier** | Playback speed (1 = normal) |
| **Lock Duration** | Prevent state changes for this duration |

### 3. Play Animations

```csharp
var anim = GetComponent<CharacterAnimator>();

// Play by key
anim.Play("Idle");
anim.Play("Attack");

// Force play (ignores locks/interrupts)
anim.Play("Death", forcePlay: true);

// Check current animation
if (anim.IsPlaying("Idle")) { }

// Locomotion helper
float speed = velocity.magnitude;
anim.SetLocomotion(speed, "Idle", "Walk", "Run");
```

---

## Folder Structure

```
Assets/_Shared/Animation/
├── Core/
│   ├── CharacterAnimator.cs        # Main component (recommended)
│   ├── AnimationStateConfig.cs     # Animation configuration class
│   ├── IAnimationController.cs     # Interface
│   ├── AnimationController.cs      # Base class with hash caching
│   ├── AnimationStateMachine.cs    # Enum-based state machine
│   ├── AnimationState.cs           # State class for advanced usage
│   └── AnimationStateMachineAdvanced.cs  # State machine with classes
└── README.md
```

---

## Components

### CharacterAnimator (Recommended)

The simplest way to control animations. String-based, Inspector-configurable.

```csharp
using AlfredWooden.Animation;

public class MyCharacter : MonoBehaviour {
    private CharacterAnimator _anim;

    void Start() {
        _anim = GetComponent<CharacterAnimator>();
    }

    void Update() {
        // Locomotion based on speed
        float speed = GetComponent<Rigidbody>().velocity.magnitude;
        _anim.SetLocomotion(speed, "Idle", "Walk", "Run",
            walkThreshold: 0.1f,
            runThreshold: 4f);
    }

    public void Attack() {
        _anim.Play("Attack");
    }
}
```

**Inspector Setup:**
```
Animations:
  [0] Key: Idle,   Transition: 0.2,  CanInterrupt: true
  [1] Key: Walk,   Transition: 0.15, CanInterrupt: true
  [2] Key: Run,    Transition: 0.15, CanInterrupt: true
  [3] Key: Attack, Transition: 0.1,  CanInterrupt: false, Lock: 0.8
  [4] Key: Death,  Transition: 0.1,  CanInterrupt: false
```

---

### AnimationController (Base Class)

Lower-level control with hash caching and events.

```csharp
using AlfredWooden.Animation;

public class MyController : MonoBehaviour {
    private AnimationController _anim;

    void Start() {
        _anim = GetComponent<AnimationController>();

        // Pre-cache hashes for performance
        _anim.CacheStateHashes("Idle", "Walk", "Run", "Attack");

        // Subscribe to events
        _anim.OnAnimationStarted += OnAnimStart;
        _anim.OnAnimationCompleted += OnAnimEnd;
    }

    void PlayIdle() {
        _anim.CrossFade("Idle", 0.2f);
    }

    void OnAnimStart(string stateName) {
        Debug.Log($"Started: {stateName}");
    }

    void OnAnimEnd(string stateName) {
        Debug.Log($"Completed: {stateName}");
    }
}
```

---

### AnimationStateMachine<TEnum> (Enum-Based)

Type-safe state machine using enums. Good when you have a fixed set of states.

```csharp
using AlfredWooden.Animation;

// 1. Define your states
public enum EnemyAnimState {
    Idle,
    Patrol,
    Chase,
    Attack,
    Death
}

// 2. Create your state machine
public class EnemyAnimator : AnimationStateMachine<EnemyAnimState> {

    protected override void InitializeStates() {
        // Register each state with its animator state name
        RegisterState(EnemyAnimState.Idle, "Idle",
            transitionDuration: 0.2f);

        RegisterState(EnemyAnimState.Patrol, "Walk",
            transitionDuration: 0.15f);

        RegisterState(EnemyAnimState.Chase, "Run",
            transitionDuration: 0.1f);

        RegisterState(EnemyAnimState.Attack, "Attack",
            transitionDuration: 0.1f,
            canBeInterrupted: false);

        RegisterState(EnemyAnimState.Death, "Death",
            transitionDuration: 0.1f,
            canBeInterrupted: false);
    }
}

// 3. Use it
public class Enemy : MonoBehaviour {
    private EnemyAnimator _anim;

    void Start() {
        _anim = GetComponent<EnemyAnimator>();
        _anim.OnStateChanged += (prev, next) => {
            Debug.Log($"{prev} -> {next}");
        };
    }

    void SetChasing() {
        _anim.RequestStateChange(EnemyAnimState.Chase);
    }
}
```

**Requirements:** Requires `AnimationController` component on same GameObject.

---

### AnimationStateMachineAdvanced<TEnum> (State Classes)

Full state machine with Enter/Update/Exit lifecycle methods. Use for complex logic.

```csharp
using AlfredWooden.Animation;

// 1. Define states enum
public enum PlayerAnimState { Idle, Walk, Attack, ComboAttack }

// 2. Create state classes
public class AttackState : AnimationState<PlayerAnimState> {
    public override string AnimationName => "Attack";
    public override float TransitionDuration => 0.1f;
    public override bool CanBeInterrupted => false;

    private float _attackDuration;

    public AttackState() : base(PlayerAnimState.Attack) { }

    public override void EnterState() {
        base.EnterState();
        _attackDuration = GetAnimationLength();
    }

    public override void UpdateState() {
        base.UpdateState();

        // Enable hitbox at 30% through animation
        if (StateTime > _attackDuration * 0.3f) {
            // EnableHitbox();
        }
    }

    public override PlayerAnimState GetNextState() {
        // Auto-transition to idle when done
        if (StateTime >= _attackDuration) {
            return PlayerAnimState.Idle;
        }
        return StateKey;
    }

    public override void ExitState() {
        base.ExitState();
        // DisableHitbox();
    }
}

// 3. Create state machine
public class PlayerAnimator : AnimationStateMachineAdvanced<PlayerAnimState> {

    protected override void InitializeStates() {
        AddState(PlayerAnimState.Idle, new IdleState());
        AddState(PlayerAnimState.Walk, new WalkState());
        AddState(PlayerAnimState.Attack, new AttackState());
    }
}
```

---

## API Reference

### CharacterAnimator

| Method | Description |
|--------|-------------|
| `Play(key)` | Play animation by key |
| `Play(key, force)` | Force play, ignoring locks |
| `PlayImmediate(key)` | Play without crossfade |
| `SetLocomotion(speed, idle, walk, run)` | Auto-select based on speed |
| `LockForDuration(seconds)` | Prevent state changes |
| `Unlock()` | Remove lock |
| `IsPlaying(key)` | Check current animation |
| `HasAnimation(key)` | Check if key exists |
| `AddAnimation(config)` | Add at runtime |
| `RemoveAnimation(key)` | Remove at runtime |

| Property | Description |
|----------|-------------|
| `Animator` | Unity Animator component |
| `CurrentAnimation` | Current animation key |
| `IsLocked` | Whether locked |

| Event | Description |
|-------|-------------|
| `OnAnimationChanged(prev, next)` | Fired on state change |

### AnimationController

| Method | Description |
|--------|-------------|
| `Play(name, layer)` | Play immediately |
| `CrossFade(name, duration, layer)` | Crossfade transition |
| `SetFloat/Bool/Trigger(param, value)` | Set animator parameters |
| `GetStateHash(name)` | Get cached hash |
| `CacheStateHashes(names)` | Pre-cache for performance |
| `IsInState(name)` | Check animator state |
| `GetNormalizedTime()` | Get animation progress (0-1) |
| `SetSpeed(speed)` | Set animator speed |

### AnimationStateMachine<T>

| Method | Description |
|--------|-------------|
| `RequestStateChange(state)` | Request transition |
| `ForceStateChange(state)` | Force transition |
| `LockForDuration(seconds)` | Lock state machine |
| `Unlock()` | Remove lock |
| `IsInState(state)` | Check current state |

| Property | Description |
|----------|-------------|
| `CurrentState` | Current enum state |
| `PreviousState` | Previous enum state |
| `IsLocked` | Whether locked |

---

## Examples

### Combat Animation with Lock

```csharp
// In Inspector, configure Attack with CanBeInterrupted: false, LockDuration: 0.5

void OnAttackPressed() {
    if (!_anim.IsLocked) {
        _anim.Play("Attack");
        // Player can't change animation for 0.5s
    }
}
```

### Animation Events

```csharp
void Start() {
    var anim = GetComponent<CharacterAnimator>();
    anim.OnAnimationChanged += (prev, next) => {
        if (next == "Attack") {
            PlayAttackSound();
        }
    };
}
```

### Runtime Animation Registration

```csharp
void UnlockNewAbility() {
    var anim = GetComponent<CharacterAnimator>();

    anim.AddAnimation(new AnimationStateConfig {
        Key = "SpecialAttack",
        AnimatorStateName = "SpecialAttack",
        TransitionDuration = 0.1f,
        CanBeInterrupted = false,
        LockDuration = 1.2f
    });
}
```

### Different Speeds Per Animation

```csharp
// In Inspector:
// Walk: SpeedMultiplier = 1.0
// Run:  SpeedMultiplier = 1.2
// Injured Walk: SpeedMultiplier = 0.7

void SetInjured(bool injured) {
    _anim.Play(injured ? "InjuredWalk" : "Walk");
}
```

---

## Best Practices

1. **Use CharacterAnimator** for most cases - it's the simplest
2. **Use enum-based StateMachine** when you need type safety
3. **Use Advanced StateMachine** only when states need custom logic
4. **Pre-configure in Inspector** rather than runtime when possible
5. **Set CanBeInterrupted = false** for attacks/actions that must complete
6. **Use LockDuration** to prevent animation spam
7. **Match Key to AnimatorStateName** for simplicity (leave AnimatorStateName empty)

---

## Namespace

All classes are in `AlfredWooden.Animation` namespace:

```csharp
using AlfredWooden.Animation;
```
