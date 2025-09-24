# UltimateRunningPlanner - Technical Architecture Documentation

## Table of Contents
1. [Project Overview](#project-overview)
2. [SOLID Principles Implementation](#solid-principles-implementation)
3. [Design Patterns Analysis](#design-patterns-analysis)
4. [Object-Oriented Programming Principles](#object-oriented-programming-principles)
5. [Architecture Overview](#architecture-overview)
6. [Advanced C# Features](#advanced-c-features)
7. [Dependency Injection and Inversion of Control](#dependency-injection-and-inversion-of-control)
8. [Code Quality and Best Practices](#code-quality-and-best-practices)

## Project Overview

The **UltimateRunningPlanner** is a sophisticated Blazor Server application designed as a learning project to demonstrate advanced software engineering principles, design patterns, and clean architecture practices. The application manages workout planning for runners, showcasing real-world implementations of SOLID principles, various design patterns, and modern C# idioms.

### Technology Stack
- **.NET 9.0** with C# 13 language features
- **Blazor Server** for interactive web UI
- **MudBlazor** UI component library
- **Dependency Injection** container
- **FluentValidation** for validation logic
- **Value Objects** and **Domain Models**

## SOLID Principles Implementation

The project serves as an exemplary implementation of SOLID principles, demonstrating how each principle contributes to maintainable, testable, and extensible code.

### Single Responsibility Principle (SRP)

**Theoretical Foundation:**
The Single Responsibility Principle states that a class should have only one reason to change, meaning it should have only one job or responsibility.

**Implementation Examples:**

#### 1. Pace Value Object
```csharp
// WebUI/Models/Pace.cs
public readonly struct Pace : IEquatable<Pace>, IComparable<Pace>, IParsable<Pace>
{
    private readonly int _totalSeconds;
    
    // Single responsibility: Represent and manipulate pace values
    public Pace(int totalSeconds) { /* ... */ }
    public double ToMinutesDouble() => _totalSeconds / 60d;
    public decimal ToMeterPerSeconds() => _totalSeconds == 0 ? 0m : 1000m / _totalSeconds;
}
```

**Analysis:** The `Pace` struct has a single, well-defined responsibility: representing pace values and providing conversions between different pace representations (seconds, minutes, meters per second). It doesn't handle persistence, validation, or business logic.

#### 2. PaceJsonConverter
```csharp
// WebUI/Converters/PaceJsonConverter.cs
public sealed class PaceJsonConverter : JsonConverter<Pace>
{
    // Single responsibility: JSON serialization/deserialization of Pace objects
    public override Pace Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    public override void Write(Utf8JsonWriter writer, Pace value, JsonSerializerOptions options)
}
```

**Analysis:** The converter has the sole responsibility of handling JSON serialization for `Pace` objects, separated from the `Pace` type itself.

#### 3. AthleteSession
```csharp
// WebUI/Services/AthleteSession.cs
public sealed class AthleteSession : IAthleteSession
{
    // Single responsibility: Manage athlete state within user session
    public void Set(Athlete? athlete)
    public void Clear()
    public async Task<Athlete?> GetAndSetAsync()
    public async Task StoreAsync(Athlete? athlete)
}
```

**Analysis:** `AthleteSession` is responsible only for managing athlete session state, delegating persistence concerns to `ILocalStorageService`.

### Open/Closed Principle (OCP)

**Theoretical Foundation:**
The Open/Closed Principle states that software entities should be open for extension but closed for modification. This means you can add new functionality without changing existing code.

**Implementation Examples:**

#### 1. Abstract Factory Pattern with Concrete Creators
```csharp
// Base creator abstraction
public abstract class AbstractPlannedWorkoutCreator : IPlannedWorkoutCreator
{
    public abstract bool CanCreate(RunType runType);
    protected abstract PlannedWorkout CreateInstance(); // Template method
    
    // Common logic - closed for modification
    public PlannedWorkout Create(CustomWorkout workout, Athlete athlete, DateOnly date)
    {
        var instance = CreateInstance();
        PopulateCommon(instance, workout, athlete, date);
        return instance;
    }
}

// Extension through concrete implementations - open for extension
public class TempoPlannedWorkoutCreator : AbstractPlannedWorkoutCreator
{
    public override bool CanCreate(RunType runType) => runType == RunType.Tempo;
    protected override PlannedWorkout CreateInstance() => new TempoWorkout();
}

public class IntervalPlannedWorkoutCreator : AbstractPlannedWorkoutCreator
{
    public override bool CanCreate(RunType runType) => runType == RunType.Intervals;
    protected override PlannedWorkout CreateInstance() => new IntervalWorkout();
}
```

**Analysis:** New workout types can be added by creating new concrete creators without modifying existing code. The base class provides common functionality while derived classes extend behavior.

#### 2. Workout Hierarchy
```csharp
// Base class - closed for modification
public abstract class PlannedWorkout
{
    public abstract string Name { get; }
    
    // Template methods for extension points
    protected virtual decimal GetEffortSpeed(Athlete athlete)
    protected virtual double CalculateWorkoutDistanceCore(decimal easySpeed, decimal effortSpeed)
    protected virtual int CalculateWorkoutDurationCore()
}

// Extensions - open for extension
public abstract class StructuredWorkout : PlannedWorkout, IStructuredWorkout
{
    // Extends behavior for interval-based workouts
    protected override decimal GetEffortSpeed(Athlete athlete) { /* specific logic */ }
    protected override double CalculateWorkoutDistanceCore(decimal easySpeed, decimal effortSpeed) { /* specific logic */ }
}

public sealed class IntervalWorkout : StructuredWorkout
{
    public override string Name => $"W{WeekNumber} {Repetitions}x{IntervalDuration}\" @ {FormatPace()}min/km";
    protected override Color CalendarColor => Color.Warning;
}
```

**Analysis:** New workout types can be added without modifying existing implementations. The hierarchy provides extension points through virtual methods and abstract properties.

### Liskov Substitution Principle (LSP)

**Theoretical Foundation:**
The Liskov Substitution Principle states that objects of a superclass should be replaceable with objects of its subclasses without breaking the application's functionality.

**Implementation Examples:**

#### 1. IPlannedWorkoutCreator Implementations
```csharp
public interface IPlannedWorkoutCreator
{
    bool CanCreate(RunType runType);
    PlannedWorkout Create(CustomWorkout workout, Athlete athlete, DateOnly date);
}

// All implementations honor the contract
public class TempoPlannedWorkoutCreator : AbstractPlannedWorkoutCreator
{
    public override bool CanCreate(RunType runType) => runType == RunType.Tempo;
    protected override PlannedWorkout CreateInstance() => new TempoWorkout();
}

public class IntervalPlannedWorkoutCreator : AbstractPlannedWorkoutCreator
{
    public override bool CanCreate(RunType runType) => runType == RunType.Intervals;
    protected override PlannedWorkout CreateInstance() => new IntervalWorkout();
}
```

**Analysis:** Any `IPlannedWorkoutCreator` implementation can be substituted for another without breaking the `PlannedWorkoutFactory` that depends on the interface.

#### 2. Workout Substitutability
```csharp
// Client code can work with any PlannedWorkout implementation
public Planning BuildPlanning(DateOnly startDate, List<CustomWorkout> workouts, Athlete athlete)
{
    foreach (var workout in workouts)
    {
        // Any PlannedWorkout subtype works here
        var planned = _workoutCreator.Create(workout, athlete, date);
        planning.Workouts.Add(planned); // LSP satisfied
    }
}
```

**Analysis:** All `PlannedWorkout` subclasses can be used interchangeably wherever a `PlannedWorkout` is expected.

### Interface Segregation Principle (ISP)

**Theoretical Foundation:**
The Interface Segregation Principle states that clients should not be forced to depend upon interfaces they don't use. It's better to have many specific interfaces than one general-purpose interface.

**Implementation Examples:**

#### 1. Segregated Interfaces
```csharp
// Specific interface for structured workouts
public interface IStructuredWorkout
{
    int Repetitions { get; set; }
    int IntervalDuration { get; set; }
    int RecoveryDuration { get; set; }
    bool IsEmpty { get; }
}

// Separate interface for athlete session management
public interface IAthleteSession
{
    Athlete? Current { get; }
    bool HasValue { get; }
    void Set(Athlete? athlete);
    void Clear();
    Task<Athlete?> GetAndSetAsync();
    Task StoreAsync(Athlete? athlete);
    Task RemoveAsync();
}

// Factory interface focused on creation
public interface IPlannedWorkoutFactory
{
    PlannedWorkout Create(CustomWorkout workout, Athlete athlete, DateOnly date);
}

// Builder interface focused on planning assembly
public interface IPlanningBuilder
{
    Planning BuildPlanning(DateOnly startDate, List<CustomWorkout> workouts, Athlete athlete, List<TrainingTemplate>? trainingTemplates = null);
}
```

**Analysis:** Each interface has a focused responsibility. Classes implement only the interfaces they need, avoiding unnecessary dependencies.

#### 2. Conditional Interface Implementation
```csharp
// Not all workouts need to be structured
public abstract class PlannedWorkout { /* base functionality */ }

// Only interval-based workouts implement IStructuredWorkout
public abstract class StructuredWorkout : PlannedWorkout, IStructuredWorkout
{
    // Implements structured workout properties
}

// Simple workouts don't implement IStructuredWorkout
public sealed class EasyWorkout : PlannedWorkout
{
    // No repetitions, intervals, or recovery concerns
}
```

### Dependency Inversion Principle (DIP)

**Theoretical Foundation:**
The Dependency Inversion Principle states that high-level modules should not depend on low-level modules. Both should depend on abstractions. Abstractions should not depend on details; details should depend on abstractions.

**Implementation Examples:**

#### 1. High-Level Planning Logic
```csharp
// High-level module depends on abstractions
public class PlanningBuilder : IPlanningBuilder
{
    private readonly IPlannedWorkoutFactory _workoutCreator; // Abstraction

    public PlanningBuilder(IPlannedWorkoutFactory workoutCreator) // DI
    {
        _workoutCreator = workoutCreator;
    }

    public Planning BuildPlanning(DateOnly startDate, List<CustomWorkout> workouts, Athlete athlete, List<TrainingTemplate>? trainingTemplates = null)
    {
        // Depends on abstraction, not concrete implementation
        var planned = _workoutCreator.Create(workout, athlete, date);
    }
}
```

#### 2. Factory Implementation Depending on Abstractions
```csharp
public class PlannedWorkoutFactory : IPlannedWorkoutFactory
{
    private readonly IDictionary<RunType, IPlannedWorkoutCreator> _creators;

    // Depends on abstraction (IPlannedWorkoutCreator), not concrete creators
    public PlannedWorkoutFactory(IEnumerable<IPlannedWorkoutCreator> creators)
    {
        var runtypes = Enum.GetValues<RunType>();
        _creators = creators.ToDictionary(rt => runtypes.First(x => rt.CanCreate(x)));
    }
}
```

#### 3. Session Management
```csharp
public sealed class AthleteSession : IAthleteSession
{
    private readonly ILocalStorageService _localStorageService; // Abstraction

    public AthleteSession(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService; // Injected dependency
    }
}
```

**Analysis:** High-level components depend on interfaces rather than concrete implementations, making the system flexible and testable.

## Design Patterns Analysis

### Abstract Factory Pattern

**Theoretical Foundation:**
The Abstract Factory pattern provides an interface for creating families of related or dependent objects without specifying their concrete classes.

**Implementation:**

#### Factory Interface
```csharp
// Abstract factory interface
public interface IPlannedWorkoutFactory
{
    PlannedWorkout Create(CustomWorkout workout, Athlete athlete, DateOnly date);
}
```

#### Concrete Factory
```csharp
// Concrete factory implementation
public class PlannedWorkoutFactory : IPlannedWorkoutFactory
{
    private readonly IDictionary<RunType, IPlannedWorkoutCreator> _creators;

    public PlannedWorkoutFactory(IEnumerable<IPlannedWorkoutCreator> creators)
    {
        // Registry of concrete creators
        var runtypes = Enum.GetValues<RunType>();
        _creators = creators.ToDictionary(rt => runtypes.First(x => rt.CanCreate(x)));
        _creators[RunType.Recovery] = _creators[RunType.Easy]; // Aliasing
    }

    public PlannedWorkout Create(CustomWorkout workout, Athlete athlete, DateOnly date)
    {
        if (!_creators.TryGetValue(workout.RunType, out var creator) || creator is null)
        {
            throw new InvalidOperationException("No PlannedWorkout creator registered for RunType " + workout.RunType);
        }

        return creator.Create(workout, athlete, date);
    }
}
```

#### Product Creators
```csharp
// Abstract creator
public interface IPlannedWorkoutCreator
{
    bool CanCreate(RunType runType);
    PlannedWorkout Create(CustomWorkout workout, Athlete athlete, DateOnly date);
}

// Concrete creators for each product family
public class TempoPlannedWorkoutCreator : AbstractPlannedWorkoutCreator
{
    public override bool CanCreate(RunType runType) => runType == RunType.Tempo;
    protected override PlannedWorkout CreateInstance() => new TempoWorkout();
}

public class IntervalPlannedWorkoutCreator : AbstractPlannedWorkoutCreator
{
    public override bool CanCreate(RunType runType) => runType == RunType.Intervals;
    protected override PlannedWorkout CreateInstance() => new IntervalWorkout();
}
```

**Benefits Demonstrated:**
- **Flexibility:** New workout types can be added without modifying existing code
- **Consistency:** All workouts are created through a consistent interface
- **Testability:** Factory can be easily mocked for unit testing
- **Configuration:** Creators are registered via dependency injection

### Template Method Pattern

**Theoretical Foundation:**
The Template Method pattern defines the skeleton of an algorithm in the superclass but lets subclasses override specific steps of the algorithm without changing its structure.

**Implementation:**

#### Base Template
```csharp
public abstract class PlannedWorkout
{
    // Template method - defines the algorithm structure
    public int CalculateEstimatedDistance(Athlete athlete)
    {
        ArgumentNullException.ThrowIfNull(athlete);

        var easySpeed = athlete.EasyPace.ToMeterPerSeconds();
        var effortSpeed = GetEffortSpeed(athlete); // Hook method
        
        double distance = CalculateWorkoutDistanceCore(easySpeed, effortSpeed); // Hook method
        
        return (int)Math.Ceiling(distance / 100) * 100;
    }

    // Template method for duration calculation
    public int CalculateEstimatedDuration()
    {
        int warmUpDuration = 15 * 60;
        int coolDownDuration = 10 * 60;
        
        int workoutSpecificDuration = CalculateWorkoutDurationCore(); // Hook method
        
        return warmUpDuration + workoutSpecificDuration + coolDownDuration;
    }

    // Hook methods - can be overridden by subclasses
    protected virtual decimal GetEffortSpeed(Athlete athlete)
    {
        return athlete.EasyPace.ToMeterPerSeconds();
    }
    
    protected virtual double CalculateWorkoutDistanceCore(decimal easySpeed, decimal effortSpeed)
    {
        return TotalDuration * (double)easySpeed;
    }
    
    protected virtual int CalculateWorkoutDurationCore()
    {
        return TotalDuration;
    }
}
```

#### Concrete Implementations
```csharp
public abstract class StructuredWorkout : PlannedWorkout, IStructuredWorkout
{
    // Override hook methods for interval-specific calculations
    protected override decimal GetEffortSpeed(Athlete athlete)
    {
        return RunType switch
        {
            RunType.Tempo => athlete.SemiMarathonPace.ToMeterPerSeconds(),
            RunType.Intervals => athlete.VmaPace.ToMeterPerSeconds(),
            _ => Pace.ToMeterPerSeconds()
        };
    }
    
    protected override double CalculateWorkoutDistanceCore(decimal easySpeed, decimal effortSpeed)
    {
        if (IsEmpty)
        {
            return TotalDuration * (double)easySpeed;
        }
        
        // Interval-specific calculation
        double effortDistance = (double)effortSpeed * IntervalDuration * Repetitions;
        double recoveryDistance = (double)easySpeed * RecoveryDuration * Repetitions;
        
        // Additional logic for warm-up/cool-down based on run type
        if (RunType is RunType.Tempo or RunType.Intervals)
        {
            double warmUpDistance = 15 * 60 * (double)easySpeed;
            double coolDownDistance = 10 * 60 * (double)easySpeed;
            return effortDistance + recoveryDistance + warmUpDistance + coolDownDistance;
        }
        
        // ... more logic
        return effortDistance + recoveryDistance + warmUpDistance;
    }
    
    protected override int CalculateWorkoutDurationCore()
    {
        return (IntervalDuration + RecoveryDuration) * Repetitions;
    }
}
```

**Benefits Demonstrated:**
- **Code Reuse:** Common algorithm structure is shared
- **Customization:** Subclasses can customize specific steps
- **Consistency:** All calculations follow the same pattern
- **Maintainability:** Algorithm changes are centralized

### Strategy Pattern (Implicit)

**Implementation in Validation:**
```csharp
// FluentValidation implements Strategy pattern
public class CustomWorkoutValidator : AbstractValidator<CustomWorkout>
{
    public CustomWorkoutValidator()
    {
        // Different validation strategies for different properties
        RuleFor(x => x.WeekNumber).GreaterThan(0);
        RuleFor(x => x.RunType).IsInEnum();
        RuleFor(x => x.TotalDuration).GreaterThan(0);
        RuleFor(x => x.Pace).GreaterThanOrEqualTo(0);
    }
}
```

### Builder Pattern (Implicit)

**Implementation in Planning Construction:**
```csharp
public class PlanningBuilder : IPlanningBuilder
{
    private readonly IPlannedWorkoutFactory _workoutCreator;

    public Planning BuildPlanning(DateOnly startDate, List<CustomWorkout> workouts, 
                                 Athlete athlete, List<TrainingTemplate>? trainingTemplates = null)
    {
        var planning = new Planning
        {
            StartDate = startDate.GetMonday(),
            BaseWorkouts = workouts,
            Athlete = athlete
        };

        // Step-by-step construction
        var mondayOfWeek = planning.StartDate;

        foreach (var weekGroup in planning.BaseWorkouts.GroupBy(w => w.WeekNumber).OrderBy(g => g.Key))
        {
            // Determine template
            int numberOfTrainingDays = weekGroup.Count();
            var template = trainingTemplates?.FirstOrDefault(t => t.TrainingDaysCount == numberOfTrainingDays)
                ?? ResolveDefaultTemplate(numberOfTrainingDays);

            planning.Template.TryAdd(template);

            // Build workouts for the week
            int idSeed = (weekGroup.Key + 9) * 100;
            foreach (var (workout, day) in template.ScheduleWeek(weekGroup))
            {
                workout.Id = idSeed++;
                var date = day == DayOfWeek.Sunday
                    ? mondayOfWeek.AddDays(6)
                    : mondayOfWeek.AddDays((int)day - 1);

                var planned = _workoutCreator.Create(workout, athlete, date);
                planning.Workouts.Add(planned);
            }

            mondayOfWeek = mondayOfWeek.AddDays(7);
        }

        return planning;
    }
}
```

### Value Object Pattern

**Theoretical Foundation:**
Value Objects are objects that have no conceptual identity and are defined by their attributes. They are immutable and equality is based on all attributes.

**Implementation:**
```csharp
[JsonConverter(typeof(PaceJsonConverter))]
public readonly struct Pace :
    IEquatable<Pace>,
    IComparable<Pace>,
    IParsable<Pace>,
    ISpanFormattable,
    IAdditionOperators<Pace, Pace, Pace>,
    ISubtractionOperators<Pace, Pace, Pace>,
    IMultiplyOperators<Pace, double, Pace>,
    IDivisionOperators<Pace, double, Pace>
{
    private readonly int _totalSeconds;

    public Pace(int totalSeconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(totalSeconds);
        _totalSeconds = totalSeconds;
    }

    // Value equality
    public bool Equals(Pace other) => _totalSeconds == other._totalSeconds;
    public override bool Equals(object? obj) => obj is Pace other && Equals(other);
    public override int GetHashCode() => _totalSeconds.GetHashCode();

    // Immutable operations
    public static Pace operator +(Pace left, Pace right) => new(checked(left._totalSeconds + right._totalSeconds));
    public static Pace operator -(Pace left, Pace right) => new(Math.Max(0, left._totalSeconds - right._totalSeconds));
    
    // Rich behavior
    public double ToMinutesDouble() => _totalSeconds / 60d;
    public decimal ToMeterPerSeconds() => _totalSeconds == 0 ? 0m : 1000m / _totalSeconds;
}
```

**Benefits:**
- **Immutability:** Once created, cannot be changed
- **Value Equality:** Two paces with the same seconds are equal
- **Rich API:** Provides meaningful operations and conversions
- **Type Safety:** Prevents mixing up pace with other numeric values

## Object-Oriented Programming Principles

### Encapsulation

**Implementation Examples:**

#### 1. Property Validation in StructuredWorkout
```csharp
public abstract class StructuredWorkout : PlannedWorkout, IStructuredWorkout
{
    private int _repetitions;
    private int _intervalDuration;
    private int _recoveryDuration;

    public int Repetitions
    {
        get => _repetitions;
        set
        {
            if (value < 0) 
                throw new ArgumentOutOfRangeException(nameof(value), "Repetitions cannot be negative.");
            _repetitions = value;
        }
    }
    
    // Similar validation for other properties
}
```

**Analysis:** Internal state is protected and validated through property setters, preventing invalid states.

#### 2. Session State Management
```csharp
public sealed class AthleteSession : IAthleteSession
{
    private Athlete? _athlete; // Private state

    public Athlete? Current => _athlete; // Controlled access
    public bool HasValue => _athlete is not null;

    public void Set(Athlete? athlete) => _athlete = athlete ?? throw new ArgumentNullException(nameof(athlete));
}
```

### Inheritance

**Implementation Examples:**

#### 1. Workout Hierarchy
```csharp
// Base class with common behavior
public abstract class PlannedWorkout
{
    // Common properties and methods
    public abstract string Name { get; }
    protected abstract Color CalendarColor { get; }
    
    // Template methods
    public int CalculateEstimatedDistance(Athlete athlete) { /* ... */ }
}

// Intermediate class for structured workouts
public abstract class StructuredWorkout : PlannedWorkout, IStructuredWorkout
{
    // Additional properties and behavior for interval-based workouts
    public int Repetitions { get; set; }
    public int IntervalDuration { get; set; }
    
    // Overridden template methods
    protected override decimal GetEffortSpeed(Athlete athlete) { /* specific logic */ }
}

// Concrete implementations
public sealed class IntervalWorkout : StructuredWorkout
{
    public override string Name => $"W{WeekNumber} {Repetitions}x{IntervalDuration}\" @ {FormatPace()}min/km";
    protected override Color CalendarColor => Color.Warning;
}

public sealed class EasyWorkout : PlannedWorkout
{
    public override string Name => $"W{WeekNumber} Easy {EstimatedDistance / 1000.0:F1}km @ {FormatPace()}min/km";
    protected override Color CalendarColor => Color.Success;
}
```

**Analysis:** 
- **Is-a Relationships:** `IntervalWorkout` is a `StructuredWorkout` is a `PlannedWorkout`
- **Behavioral Inheritance:** Common calculation logic inherited from base classes
- **Specialized Behavior:** Each concrete class provides specific implementation details

### Polymorphism

**Implementation Examples:**

#### 1. Runtime Polymorphism
```csharp
// Client code works with base type
public Planning BuildPlanning(DateOnly startDate, List<CustomWorkout> workouts, Athlete athlete)
{
    foreach (var workout in workouts)
    {
        // Factory returns specific workout type, but client treats as base type
        PlannedWorkout planned = _workoutCreator.Create(workout, athlete, date);
        
        // Polymorphic method calls
        planned.CalculateEstimatedDistance(athlete); // Calls overridden implementation
        planned.CalculateEstimatedDuration(); // Calls overridden implementation
        
        planning.Workouts.Add(planned);
    }
}
```

#### 2. Interface Polymorphism
```csharp
// Different implementations of the same interface
public class PlannedWorkoutFactory : IPlannedWorkoutFactory
{
    private readonly IDictionary<RunType, IPlannedWorkoutCreator> _creators;

    public PlannedWorkout Create(CustomWorkout workout, Athlete athlete, DateOnly date)
    {
        // Polymorphic call - actual implementation depends on RunType
        return creator.Create(workout, athlete, date);
    }
}
```

### Abstraction

**Implementation Examples:**

#### 1. Interface Abstractions
```csharp
// High-level abstraction
public interface IPlannedWorkoutFactory
{
    PlannedWorkout Create(CustomWorkout workout, Athlete athlete, DateOnly date);
}

// Client depends on abstraction, not implementation
public class PlanningBuilder : IPlanningBuilder
{
    private readonly IPlannedWorkoutFactory _workoutCreator; // Abstraction

    // Usage focuses on what, not how
    var planned = _workoutCreator.Create(workout, athlete, date);
}
```

#### 2. Abstract Classes
```csharp
// Abstract base defines contract and common behavior
public abstract class AbstractPlannedWorkoutCreator : IPlannedWorkoutCreator
{
    public abstract bool CanCreate(RunType runType); // Must be implemented
    protected abstract PlannedWorkout CreateInstance(); // Must be implemented

    // Common algorithm - concrete method
    public PlannedWorkout Create(CustomWorkout workout, Athlete athlete, DateOnly date)
    {
        ArgumentNullException.ThrowIfNull(workout);
        ArgumentNullException.ThrowIfNull(athlete);

        var instance = CreateInstance(); // Delegated to concrete implementation
        PopulateCommon(instance, workout, athlete, date); // Shared logic
        return instance;
    }
}
```

## Architecture Overview

### Layered Architecture

The application follows a layered architecture with clear separation of concerns:

```
┌─────────────────────────────────────┐
│           Presentation Layer         │ (Blazor Components)
│  - Components/                      │
│  - Pages/                           │
│  - wwwroot/                         │
└─────────────────────────────────────┘
                     │
┌─────────────────────────────────────┐
│          Application Layer          │ (Services & Orchestration)
│  - Services/                        │
│  - Validators/                      │
│  - DI/                              │
└─────────────────────────────────────┘
                     │
┌─────────────────────────────────────┐
│            Domain Layer             │ (Business Logic)
│  - Models/                          │
│  - Creators/                        │
│  - Converters/                      │
└─────────────────────────────────────┘
```

### Dependency Flow

```
Program.cs (Composition Root)
    │
    ├── Registers Services (DI Container)
    │   ├── IPlannedWorkoutFactory → PlannedWorkoutFactory
    │   ├── IPlannedWorkoutCreator → [Concrete Creators]
    │   ├── IPlanningBuilder → PlanningBuilder
    │   └── IAthleteSession → AthleteSession
    │
    └── Configures Pipeline
        ├── Blazor Server
        ├── MudBlazor UI
        └── Local Storage
```

### Design Principles Applied

#### 1. Separation of Concerns
- **Models:** Pure data structures and value objects
- **Services:** Business logic and orchestration
- **Creators:** Object creation responsibilities
- **Validators:** Input validation logic
- **Converters:** Serialization concerns

#### 2. Inversion of Control
```csharp
// High-level modules don't depend on low-level modules
public class PlanningBuilder : IPlanningBuilder
{
    private readonly IPlannedWorkoutFactory _workoutCreator; // Abstraction

    public PlanningBuilder(IPlannedWorkoutFactory workoutCreator) // Injected
    {
        _workoutCreator = workoutCreator;
    }
}

// Composition root wires dependencies
public static IServiceCollection AddPlannedWorkoutFactories(this IServiceCollection services)
{
    services.AddScoped<IPlannedWorkoutCreator, TempoPlannedWorkoutCreator>();
    services.AddScoped<IPlannedWorkoutCreator, IntervalPlannedWorkoutCreator>();
    // ... more creators
    services.AddScoped<IPlannedWorkoutFactory, PlannedWorkoutFactory>();
    return services;
}
```

#### 3. Composition over Inheritance
```csharp
// Favor composition through interfaces
public class PlannedWorkoutFactory : IPlannedWorkoutFactory
{
    private readonly IDictionary<RunType, IPlannedWorkoutCreator> _creators; // Composition

    public PlannedWorkoutFactory(IEnumerable<IPlannedWorkoutCreator> creators)
    {
        // Compose behavior from multiple creators
        _creators = creators.ToDictionary(/* ... */);
    }
}
```

## Advanced C# Features

### Modern C# Language Features

#### 1. Nullable Reference Types
```csharp
public sealed class AthleteSession : IAthleteSession
{
    private Athlete? _athlete; // Explicitly nullable
    
    public Athlete? Current => _athlete; // Nullable return
    public bool HasValue => _athlete is not null; // Pattern matching
    
    public void Set(Athlete? athlete) => _athlete = athlete ?? throw new ArgumentNullException(nameof(athlete));
}
```

#### 2. Pattern Matching and Switch Expressions
```csharp
protected override decimal GetEffortSpeed(Athlete athlete)
{
    return RunType switch
    {
        RunType.Tempo => athlete.SemiMarathonPace.ToMeterPerSeconds(),
        RunType.Intervals => athlete.VmaPace.ToMeterPerSeconds(),
        _ => Pace.ToMeterPerSeconds()
    };
}

private static TrainingTemplate ResolveDefaultTemplate(int days) =>
    days switch
    {
        <= 4 => TrainingTemplate.Default4(),
        >= 5 => TrainingTemplate.Default5(),
    };
```

#### 3. Records and Value Types
```csharp
// Value object as readonly struct
public readonly struct Pace : 
    IEquatable<Pace>, 
    IComparable<Pace>,
    IAdditionOperators<Pace, Pace, Pace>,
    ISubtractionOperators<Pace, Pace, Pace>
{
    private readonly int _totalSeconds;
    
    // Value semantics with operator overloading
    public static Pace operator +(Pace left, Pace right) => new(checked(left._totalSeconds + right._totalSeconds));
    public static Pace operator -(Pace left, Pace right) => new(Math.Max(0, left._totalSeconds - right._totalSeconds));
}
```

#### 4. Generic Math and Static Abstract Members
```csharp
public readonly struct Pace :
    IAdditionOperators<Pace, Pace, Pace>,
    ISubtractionOperators<Pace, Pace, Pace>,
    IMultiplyOperators<Pace, double, Pace>,
    IDivisionOperators<Pace, double, Pace>
{
    // Static abstract interface implementations
    public static Pace operator +(Pace left, Pace right) => /* ... */;
    public static Pace operator -(Pace left, Pace right) => /* ... */;
    public static Pace operator *(Pace left, double right) => /* ... */;
    public static Pace operator /(Pace left, double right) => /* ... */;
}
```

#### 5. Target-Typed New and Collection Expressions
```csharp
public class Athlete
{
    public HashSet<TrainingTemplate> TrainingTemplates { get; set; } = []; // Collection expression
}

// Target-typed new
var planning = new Planning
{
    StartDate = startDate.GetMonday(),
    BaseWorkouts = workouts,
    Athlete = athlete
};
```

### Performance Optimizations

#### 1. Readonly Structs for Value Objects
```csharp
// Readonly struct prevents defensive copying
public readonly struct Pace
{
    private readonly int _totalSeconds;
    
    // All members are readonly, ensuring immutability
    public int TotalSeconds => _totalSeconds;
    public double ToMinutesDouble() => _totalSeconds / 60d;
}
```

#### 2. Span<T> for String Parsing
```csharp
// ISpanFormattable for efficient string formatting
public readonly struct Pace : ISpanFormattable
{
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Efficient span-based formatting without allocations
        // ... implementation
    }
}
```

#### 3. Collection Initialization Optimizations
```csharp
// Dictionary initialization in factory
_creators = creators.ToDictionary(rt => runtypes.First(x => rt.CanCreate(x)));
```

## Dependency Injection and Inversion of Control

### Container Configuration

#### 1. Service Registration
```csharp
// Program.cs - Composition Root
var builder = WebApplication.CreateBuilder(args);

// Infrastructure services
builder.Services.AddBlazoredLocalStorage(config => { /* ... */ });
builder.Services.AddMudServices();

// Application services
builder.Services.AddWorkoutServices(); // External library
builder.Services.AddScoped<IPlanningLoaderService, PlanningLoaderService>();
builder.Services.AddScoped<IAthleteSession, AthleteSession>();
builder.Services.AddScoped<IPlanningBuilder, PlanningBuilder>();

// Factory pattern services
builder.Services.AddPlannedWorkoutFactories(); // Extension method
```

#### 2. Extension Method for Clean Registration
```csharp
public static class PlannedWorkoutServiceCollectionExtensions
{
    public static IServiceCollection AddPlannedWorkoutFactories(this IServiceCollection services)
    {
        // Register all concrete creators
        services.AddScoped<IPlannedWorkoutCreator, EasyPlannedWorkoutCreator>();
        services.AddScoped<IPlannedWorkoutCreator, SteadyPlannedWorkoutCreator>();
        services.AddScoped<IPlannedWorkoutCreator, IntervalPlannedWorkoutCreator>();
        services.AddScoped<IPlannedWorkoutCreator, TempoPlannedWorkoutCreator>();
        services.AddScoped<IPlannedWorkoutCreator, LongRunPlannedWorkoutCreator>();
        services.AddScoped<IPlannedWorkoutCreator, RacePlannedWorkoutCreator>();
        services.AddScoped<IPlannedWorkoutCreator, DefaultPlannedWorkoutCreator>();
        
        // Register the factory that uses all creators
        services.AddScoped<IPlannedWorkoutFactory, PlannedWorkoutFactory>();
        
        return services;
    }
}
```

### Lifetime Management

#### 1. Scoped Services
```csharp
// Scoped per request/circuit
builder.Services.AddScoped<IAthleteSession, AthleteSession>();
builder.Services.AddScoped<IPlanningBuilder, PlanningBuilder>();
```

**Analysis:** Session and planning services are scoped to the user's browser connection, maintaining state throughout the session.

#### 2. Transient vs Scoped Trade-offs
```csharp
// Creators could be transient (stateless)
services.AddTransient<IPlannedWorkoutCreator, TempoPlannedWorkoutCreator>();

// But registered as scoped for consistency and potential future state needs
services.AddScoped<IPlannedWorkoutCreator, TempoPlannedWorkoutCreator>();
```

### Constructor Injection Patterns

#### 1. Simple Injection
```csharp
public class PlanningBuilder : IPlanningBuilder
{
    private readonly IPlannedWorkoutFactory _workoutCreator;

    public PlanningBuilder(IPlannedWorkoutFactory workoutCreator)
    {
        _workoutCreator = workoutCreator;
    }
}
```

#### 2. Collection Injection
```csharp
public class PlannedWorkoutFactory : IPlannedWorkoutFactory
{
    private readonly IDictionary<RunType, IPlannedWorkoutCreator> _creators;

    // DI container injects all registered IPlannedWorkoutCreator implementations
    public PlannedWorkoutFactory(IEnumerable<IPlannedWorkoutCreator> creators)
    {
        var runtypes = Enum.GetValues<RunType>();
        _creators = creators.ToDictionary(rt => runtypes.First(x => rt.CanCreate(x)));
        _creators[RunType.Recovery] = _creators[RunType.Easy]; // Business rule
    }
}
```

**Analysis:** The factory receives all registered creators and builds a lookup dictionary, demonstrating how DI enables the Open/Closed Principle.

## Code Quality and Best Practices

### Validation Strategy

#### 1. FluentValidation Integration
```csharp
public class CustomWorkoutValidator : AbstractValidator<CustomWorkout>
{
    public CustomWorkoutValidator()
    {
        RuleFor(x => x.WeekNumber)
            .GreaterThan(0)
            .WithMessage("Week number must be greater than 0.");
            
        RuleFor(x => x.RunType)
            .IsInEnum()
            .WithMessage("Run type is not valid.");
            
        RuleFor(x => x.TotalDuration)
            .GreaterThan(0)
            .WithMessage("Total duration must be greater than 0.");
            
        RuleFor(x => x.Pace)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Pace must be greater than or equal to 0.");
            
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description must not be empty.");
    }
}
```

#### 2. Domain-Level Validation
```csharp
public abstract class StructuredWorkout : PlannedWorkout, IStructuredWorkout
{
    public int Repetitions
    {
        get => _repetitions;
        set
        {
            if (value < 0) 
                throw new ArgumentOutOfRangeException(nameof(value), "Repetitions cannot be negative.");
            _repetitions = value;
        }
    }
    
    public bool IsEmpty => Repetitions == 0 || IntervalDuration <= 0;
}
```

### Error Handling Patterns

#### 1. Argument Validation
```csharp
public PlannedWorkout Create(CustomWorkout workout, Athlete athlete, DateOnly date)
{
    ArgumentNullException.ThrowIfNull(workout);
    ArgumentNullException.ThrowIfNull(athlete);
    
    // ... implementation
}
```

#### 2. Business Rule Validation
```csharp
public PlannedWorkout Create(CustomWorkout workout, Athlete athlete, DateOnly date)
{
    if (!_creators.TryGetValue(workout.RunType, out var creator) || creator is null)
    {
        throw new InvalidOperationException("No PlannedWorkout creator registered for RunType " + workout.RunType);
    }
    
    return creator.Create(workout, athlete, date);
}
```

### Immutability Patterns

#### 1. Value Objects
```csharp
// Immutable struct with value semantics
public readonly struct Pace
{
    private readonly int _totalSeconds;
    
    public Pace(int totalSeconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(totalSeconds);
        _totalSeconds = totalSeconds;
    }
    
    // No mutating operations - all operations return new instances
    public static Pace operator +(Pace left, Pace right) => new(checked(left._totalSeconds + right._totalSeconds));
}
```

#### 2. Protected State
```csharp
public sealed class AthleteSession : IAthleteSession
{
    private Athlete? _athlete; // Private, controlled mutation
    
    public Athlete? Current => _athlete; // Read-only access
    
    public void Set(Athlete? athlete) => _athlete = athlete ?? throw new ArgumentNullException(nameof(athlete));
}
```

### Testing Considerations

The architecture enables comprehensive testing through:

#### 1. Dependency Injection
```csharp
// Easy to mock dependencies
public class PlanningBuilderTests
{
    [Test]
    public void BuildPlanning_Should_CreateCorrectWorkouts()
    {
        var mockFactory = new Mock<IPlannedWorkoutFactory>();
        var builder = new PlanningBuilder(mockFactory.Object);
        
        // Test with mocked dependencies
    }
}
```

#### 2. Pure Functions
```csharp
// Template methods are easily testable
[Test]
public void CalculateEstimatedDistance_Should_ReturnCorrectValue()
{
    var workout = new TempoWorkout();
    var athlete = new Athlete { /* test data */ };
    
    var distance = workout.CalculateEstimatedDistance(athlete);
    
    // Assert expectations
}
```

#### 3. Interface Segregation
```csharp
// Small, focused interfaces are easy to mock
var mockSession = new Mock<IAthleteSession>();
mockSession.Setup(s => s.Current).Returns(testAthlete);
mockSession.Setup(s => s.HasValue).Returns(true);
```

## Conclusion

The **UltimateRunningPlanner** project demonstrates a sophisticated implementation of software engineering principles and design patterns in a real-world context. The architecture showcases:

### SOLID Principles Application
- **Single Responsibility:** Each class has a focused, well-defined purpose
- **Open/Closed:** New workout types and creators can be added without modification
- **Liskov Substitution:** All implementations honor their contracts
- **Interface Segregation:** Focused interfaces prevent unnecessary dependencies
- **Dependency Inversion:** High-level modules depend on abstractions

### Design Pattern Implementation
- **Abstract Factory:** Flexible workout creation with runtime type selection
- **Template Method:** Consistent algorithms with customizable steps
- **Value Object:** Immutable, behavior-rich domain values
- **Builder:** Complex object assembly with step-by-step construction

### Modern C# Features
- **Nullable Reference Types:** Explicit null-safety
- **Pattern Matching:** Expressive conditional logic
- **Generic Math:** Type-safe mathematical operations
- **Performance Optimizations:** Efficient value types and span operations

### Architecture Quality
- **Layered Design:** Clear separation of concerns
- **Dependency Injection:** Flexible, testable composition
- **Immutability:** Reduced complexity and side effects
- **Validation Strategy:** Multiple layers of input validation

This project serves as an excellent reference for implementing clean, maintainable, and extensible C# applications that embody professional software engineering practices.