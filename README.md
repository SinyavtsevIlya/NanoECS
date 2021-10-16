<p align="center">
    <img src="https://i.imgur.com/M5RdOCm.png" alt="NanoECS">
</p>

<!---<p align="center">
    <img src="https://discordapp.com/assets/f8389ca1a741a115313bede9ac02e2c0.svg" alt="NanoECS" width="40" height="40">
Discord IL#6472
</p>--->

# NanoECS - C#/Unity entity-component-system framework    

# Features

- Handy and easy-to-read API
- Reactive components (changing component values triggers collectors, giving you the ability to react to changes in ordered ecs manner)
- Visual debugging (you can create/change contexts, entities and components inside the editor (Optional))
- Seamless code gen (code generates on background in a standalone app, no manual actions required)
- Unique components (singleton-like accessing components via contexts) 

# Showcase

The projects below made with NanoECS and Unity:

Mobile strategy **[Save The Earth][STE]**

<a href="https://play.google.com/store/apps/details?id=com.gamefirst.free.strategy.save.the.earth">
        <img src="https://github.com/SinyavtsevIlya/ShowCaseAssets/blob/main/STE.jpg" alt="Save The Earth" height="285"></a>

*(clickable)*

Hyper Casual projects:

<p align="left">
    <a href="http://www.youtube.com/watch?v=ZAdR9D2l9MI">
        <img src="https://github.com/SinyavtsevIlya/ShowCaseAssets/blob/main/HyperRace.jpg" alt="Hyper Race 3D" height="365"></a>
    <a href="http://www.youtube.com/watch?v=ZGNpU__BdQk">
        <img src="https://github.com/SinyavtsevIlya/ShowCaseAssets/blob/main/KnifeAway.jpg" alt="Knife Away" height="365"></a>
    <a href="http://www.youtube.com/watch?v=IsTCmLSTZBU">
        <img src="https://github.com/SinyavtsevIlya/ShowCaseAssets/blob/main/Numio.png" alt="Num.io" height="365"></a>
</p>

*(clickable)*

# First look

## Entity

Create a new entity:

```csharp
var player = contexts.Core.CreateEntity()	
    .AddPosition(Vector3.zero)
    .AddHealth(100)
    .AddSkin("Butterfly");
```

## Group

Get entities with "position" and "view" components and without "movable" component:

```csharp
CoreGroup group = contexts.Core.GetGroup()
    .With.Position
    .With.View
    .Without.Movable;
```

Usage:

Handle filtered entities:

```csharp
foreach (e in group) 
{	
	Print(e.Position.Value);
	Print(e.View.Value);
	Print(e.View.IsMovable);
}
```

## Collector

Get all entities with "Speed" and "Position" **only** when the position value is changed:

```csharp
CoreCollector collector = contexts.Core.GetGroup()
    .With.Speed
    .With.Position
    .OnPositionChange();
```

Handle these entities:

```csharp
foreach (e in collector) 
{	
	Print("My position has changed! : {0}", e.Position.Value);
}
collector.Clear();
```

## Accessing component values

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

# Visual Debugging
- Reactive editing support (modifiyng component values)

<p align="left">
    <img src="https://github.com/SinyavtsevIlya/ShowCaseAssets/blob/main/NanoEcs_Editor2.gif" alt="entity editor">
</p>

- Natural auto-complete workflow for adding new components, Foldouts, Create/Destroy buttons  
<p align="left">
    <img src="https://github.com/SinyavtsevIlya/ShowCaseAssets/blob/main/NanoEcs_Editor.gif" alt="entity editor">
</p>

- GameObject linking (jump from the entity to the view game object and vice versa)
- Lists, custom types, enums, Unity Objects are supported
- Doesn't affect realease builds performance. (And can be disabled / enabled manually in the settings)

# Code Generation

<p align="left">
    <img src="https://github.com/SinyavtsevIlya/ShowCaseAssets/blob/main/NanoEcs_Console_p.jpg" alt="entity editor">
</p>

- Generation works without reflection, so you can generate <b> even if your project doesn't compile at all </b> 
- Doesn't requere any manual actions from user to trigger generation. Just write the code and see how the new API appears. (Optional)
- Customizable generation snippets. 

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
- if you prefer performance over convicience, you could disable default reactive behavior for component accessors by untiping the toggle in NanoEcs Settings. (But you still able to selectivly use it by applying [Reactive] attribute above your component.
- if use want to extend generation behavior, you can change code snippets. 

## NanoEcs Generator sources

If you for some reasons want to edit NanoEcs generator, the sources are available [here][generator-link].

# Inspiration and goals

The framework is inspired a lot by such projects as [Entitas][Entitas-link], [Actors][Actors-link], [LeoECS][LeoECS-link].

But I found that Entitas has some design problems to me:
- need of writing `Replace()` methods every time you want to change a component value and make sure reactive system "know" about it.
- `Filter` validation along with Matcher is redundant. User is forced to write both of them to make sure components are still there.
- `Matcher` declaration has a lot of boilerplate.

The goal of this project was to make API as much fluent as possible, and keep performance as well.

# Documentation

[Wiki][Wiki-link]

# How to install
- Create a new Unity Project
- Open the manifest.json file in the Packages folder inside of the Project
- Add ```"com.nanory.nanoecs": "https://github.com/SinyavtsevIlya/NanoECS.git",``` next to ```"dependencies": {```
- Go to *Packages -> NanoECS -> Install* and import ```ProjectStructure.unitypackage```

# Should I use it?

 Before making a decision, pay attention to a few points:
- if you want super performance, just take DOTS.
- support and bug fixes are active
- new features are not planned

# Feedback
If you find a bug, have some suggestions or just want to discuss, let me know: 
<!--* <b>discord</b> [![Discord](https://img.shields.io/discord/565885959598768148.svg)](https://discord.gg/u7zrtq) -->
* <b>Discord</b>  https://discordapp.com/channels/@me/IL#6472/
* <b>gmail</b> sinyavtsevilya@gmail.com

[Wiki-link]: https://github.com/SinyavtsevIlya/NanoECS/wiki
[Entitas-link]: https://github.com/sschmid/Entitas-CSharp/blob/master/README.md "Entitas"
[Actors-link]: https://github.com/dimmpixeye/ecs.unity "Actors"
[LeoECS-link]: https://github.com/Leopotam/ecs "LeoECS"
[generator-link]: https://github.com/SinyavtsevIlya/NanoECSGenerator
[STE]: https://play.google.com/store/apps/details?id=com.gamefirst.free.strategy.save.the.earth
