<p align="center">
    <img src="https://i.imgur.com/M5RdOCm.png" alt="NanoECS">
</p>

# NanoECS - C#/Unity entity-component-system framework    

## Features

- Code Generation (provides a really clean API, reducing boiler-plate code)
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
example 1:
```csharp
        e.Position.Value += e.Direction.Value.ToVector3() * e.Speed.Value * delta;
```
example 2:
```csharp
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

## Visual Debugging

<p align="left">
    <img src="https://i.imgur.com/BIlFenW.png?1" alt="entity editor" height="462">
    <img src="https://i.imgur.com/OLi4mXl.png" alt="entity editor" height="462">
</p>

- Reactive editing support (modifiyng component values gives the same result as "in code")
- GameObject linking (jump from the entity to the view game object and vice versa)
- Lists, custom types, enums, Unity Objects are supported
- Natural auto-complete workflow for adding new components, Foldouts, Create/Destroy buttons  

## Generation

<p align="left">
    <img src="https://i.imgur.com/x4w7o0q.png" alt="entity editor">
</p>

- Generation works without reflection, so you can generate <b> even if you project doesn't compile at all </b> 
- The generator is a standalone application, decoupled from Unity. So you don't need to leave your IDE
- One key press to generate everything in milliseconds
- You can edit the generation by changing the snippets. Soon it will be possible to add custom snippets, and write your own generation

## Reactive components

Here's how it works - you only create a template (or blueprint) of a component that looks like this:

```csharp
class Score
{
    int value;
}
```

The generator parses this class, and creates a completely 
 new component, in which instead of the fields there are reactive properties:

```csharp
public partial class ScoreComponent : ComponentEcs 
{
	int value;
	
	public System.Action<int, int>OnValueChange;

	public int Value 
	{
		get {  return value; }
		set 
		{
			if (this.value == value) return;
			
			var cached = this.value;
			this.value = value;
			if (_InternalOnValueChange != null) 
			{	
				_InternalOnValueChange(0);
			}
			
			if (OnValueChange != null) 
			{
				OnValueChange(cached, value);
			}
		}
	}
}
```

When changing the ```value``` property, related entity is added to all ```.OnScoreChange``` collectors.
