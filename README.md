<p align="center">
    <img src="https://i.imgur.com/M5RdOCm.png" alt="NanoECS">
</p>

# NanoECS - C#/Unity entity-component-system framework    

## Features

- Code Generation (provides really clean API, reducing boiler-plate code)
- Reactive Components (gives you the ability to change component values in a pure manner, still providing reactive groups-triggering)  
- Visual Debugging (you can create/change contexts, entities and components inside the editor (Optional))

## How to Install
- Create a new Unity Project
- Open the manifest.json file in the Packages folder inside of the Project
- Add ```"com.nanory.nanoecs": "https://github.com/SinyavtsevIlya/NanoECS.git",``` 
- Go to *Packages -> NanoECS -> Install* and import ```ProjectStructure.unitypackage```

## First look

### Group request
```csharp
        GameGroup group = contexts.Game.GetGroup()
            .With.Position
            .With.View
            .Without.Movable;
```

### Entity creation
```csharp
        var player = contexts.Game.CreateEntity()
            .AddPlayerView(view);
            .AddUnityTransform(viewGO.transform);
            .AddCollider(collider);
            .AddDirectionView(directionView);
            .AddFrags(0);
            .AddDefeatsCounter(0);
            .AddSkin(string.Empty);
```

### Accessing component values
```csharp
        // example 1
        e.Position.Value += e.Direction.Value.ToVector3() * e.Speed.Value * delta;
        
        // example 2
        foreach (var player in defeatedPlayers)
        {
            player.IsDestroyed = true;
            player.DefeatsCounter.Value++;

            if (!player.IsAIDriven)
            {
                contexts.Request.CreateEntity()
                    .AddDelay(0.6f)
                    .IsGameOverRequest = true;
            }
        }
```

## Inspiration

The framework is inspired a lot by such projects as Entitas, Actors, LeoECS.
