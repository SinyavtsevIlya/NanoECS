<p align="center">
    <img src="https://i.imgur.com/M5RdOCm.png" alt="NanoECS">
</p>

<!---<p align="center">
    <img src="https://discordapp.com/assets/f8389ca1a741a115313bede9ac02e2c0.svg" alt="NanoECS" width="40" height="40">
Discord IL#6472
</p>--->

# NanoECS - C#/Unity entity-component-system framework    

## Features

- Code Generation (provides a really clean API, reducing boiler-plate code)
- Reactive Components (gives you the ability to change component values in a pure manner, still providing reactive groups-triggering)  
- Visual Debugging (you can create/change contexts, entities and components inside the editor (Optional))
- Unique components (singleton-like accessing components via contexts) 

### Showcase

The projects below made with Unity and NanoECS:

<p align="center">
    <a href="http://www.youtube.com/watch?v=fHCvZpfxc1I">
        <img src="http://img.youtube.com/vi/fHCvZpfxc1I/0.jpg" alt="Save The Earth" height="165"></a>
    <a href="http://www.youtube.com/watch?v=ZAdR9D2l9MI">
        <img src="http://img.youtube.com/vi/ZAdR9D2l9MI/0.jpg" alt="Hyper Race 3D" height="165"></a>
    <a href="http://www.youtube.com/watch?v=ZGNpU__BdQk">
        <img src="http://img.youtube.com/vi/ZGNpU__BdQk/0.jpg" alt="Knife Away" height="165"></a>
    <a href="http://www.youtube.com/watch?v=IsTCmLSTZBU">
        <img src="http://img.youtube.com/vi/IsTCmLSTZBU/0.jpg" alt="Num.io" height="165"></a>

</p>

## First look

### Entity creation
```csharp
	// Create a new entity
        var player = contexts.Core.CreateEntity()	
            .AddPosition(Vector3.zero)
            .AddHealth(100)
            .AddSkin("Butterfly");
```

### Group

creation:
```csharp
        // Get entities with "position" and "view" components and without "movable" component
        CoreGroup group = contexts.Core.GetGroup()
            .With.Position
            .With.View
            .Without.Movable;
```

usage:
```csharp
        // handle filtered entities
	foreach (e in group) 
	{	
		Print(e.Position.Value);
		Print(e.View.Value);
		Print(e.View.IsMovable);
	}
```

### Collector

creation:
```csharp
	// Get all entities with "speed" and "position" *only* when the position value is changed
        CoreCollector collector = contexts.Core.GetGroup()
	    .With.Speed
            .With.Position
            .OnPositionChange();
```

usage:
```csharp
	// handle these entities
	foreach (e in collector) 
	{	
		Print("My position has changed! : {0}", e.Position.Value);
	}
	// and clear the collector when done
	collector.Clear();
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

## Visual Debugging

<p align="left">
    <img src="https://i.gyazo.com/04ff2e81521c2c1abdc721e8f06d4508.gif" alt="entity editor" height="462">
    <img src="https://i.imgur.com/OLi4mXl.png" alt="entity editor" height="462">
</p>

- Reactive editing support (modifiyng component values gives the same result as "in code")
- GameObject linking (jump from the entity to the view game object and vice versa)
- Lists, custom types, enums, Unity Objects are supported
- Natural auto-complete workflow for adding new components, Foldouts, Create/Destroy buttons  
- Doesn't affect realease builds performance. (And can be disabled / enabled manually in the settings)

## Generation

<p align="left">
    <img src="https://i.imgur.com/5AZlEjC.png" alt="entity editor">
</p>

- Generation works without reflection, so you can generate <b> even if your project doesn't compile at all </b> 
- Doesn't requere any manual actions from user to trigger generation. Just write the code and see how the new API appears. (Optional)
- Customizable generation snippets. 

## Inspiration

The framework is inspired a lot by such projects as [Entitas][Entitas-link], [Actors][Actors-link], [LeoECS][LeoECS-link].
I like Entitas so much, but I found it's really tricky to write "Replace" and filter-methods every-time. So the goal was to reduce boiler-plate and keep performance as well.

## Documentation

[Wiki][Wiki-link]

## How to install
- Create a new Unity Project
- Open the manifest.json file in the Packages folder inside of the Project
- Add ```"com.nanory.nanoecs": "https://github.com/SinyavtsevIlya/NanoECS.git",``` next to ```"dependencies": {```
- Go to *Packages -> NanoECS -> Install* and import ```ProjectStructure.unitypackage```

## How to use generator?
- Go to *Packages -> NanoECS -> Generator* and run NanoEcsGenerator
- Insure that you have a folder named Components somewhere inside your Assets folder
- Create new component there, for example:

```csharp
class Score
{
    int value;
}
```

Generator will automatically detect changes and create API fow you.
You dont need to leave your IDE, just keep writing your code.

A few tips:

- yes, you dont need to write "public" access modifier. It's just a "blueprint" for a real component.
- use camelCase for fields (real property will be PascalCased)
- Fieldless components (e.g. `class Movable { }`) looks this way in the result: 
```entity.IsMovable = true```
- you can use namespaces
- you can use different `contexts` by adding a corresponding attribute(s). If you don't specify any context at all, the first one from `NanoECS settings` will be chosen.



## Still in progress

- It's a very early version of framework. So bear with me :)
- Performance is tested in the mobile project (middle tier) with a lot of AI, render stuff etc. and it shows ~60 fps. I'm going to optimize it further, but it's good enough for most purposes.
- If you find a bug, let me know: 
	<!--* <b>discord</b> [![Discord](https://img.shields.io/discord/565885959598768148.svg)](https://discord.gg/u7zrtq) -->
	* <b>Discord</b>  https://discordapp.com/channels/@me/IL#6472/
	* <b>gmail</b> sinyavtsevilya@gmail.com

[Wiki-link]: https://github.com/SinyavtsevIlya/NanoECS/wiki
[Entitas-link]: https://github.com/sschmid/Entitas-CSharp/blob/master/README.md "Entitas"
[Actors-link]: https://github.com/dimmpixeye/ecs.unity "Actors"
[LeoECS-link]: https://github.com/Leopotam/ecs "LeoECS"
