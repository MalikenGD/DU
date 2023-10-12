# Changelog
All notable changes to this project, [SPAM Ability Framework](https://spam.infinitevoid.games), will be documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] 2023-02-13 | Projectiles
This version reworks projectiles, adding more customisability and quality-of-life changes. In short, projectiles has now been separated from the ability so a projectile ability can spawn multiple, independent, projectiles. This unlocks the possibility to create very complex and visually pleasing projectile abilities. Also, projectiles now move independently and can be moved by either animation curve or straight towards the target, and can be set to rotate in different directions out of the box. The animation curved movement is highly configurable, and can also be somewhat randomised for less repetitive movement.

The release includes general stability fixes. Since this version is a major upgrade, you MUST remove your previous SPAM installation (delete the folder) before upgrading, or you will encounter issues. Also make sure to read through the "Changed" section as this is where the major breaking changes are specified.
If you encounter any issues while upgrading, don't hesitate to ask on Discord or by email for help.

### Added
- Abilities can now contain customisable data that makes sense for your project. Just create a class that derives from `CustomAbilitySettings` and you can assign this in the editor. You can then get your settings at runtime with `AbilityBaseSO.GetCustomAbilityData<T>()` where T is the type of your settings class.
- A preview is now displayed of the selected projectile prefab in projectile ability.
- Logging capabilities that can be turned on and off with compiler directives. This allows for better support when you need help, and easier handling of code stripping in builds. The logger is located under `SpamFramework.Core.Infrastructure.Logging.SpamLogger`, and if you wish more feedback from the framework for debugging purposes you can add the directive "SPAM_DEBUG".
- Better support for 2D/2.5D projectiles.
- Prefabs for 2D projectiles.
- AbilityTarget2D component for 2D projectiles.
- New effect: `ConditionScaledDamageEffect`. This effect applies higher damage if the target has a given condition. Remember that effects are the backbone of your game and you're encouraged to use the pre-supplied as templates/inspiration for creating your own effects.
- Projectile abilities can now spawn multiple projectiles per cast, and they can move independently from each other.
- `AbilityBase.Description` want exposed, so a public property has been added to it.
- Checkbox for "Add as child" when you add an ability to a GameObject. This will add the ability as a child GameObject (with the same name as the ability) to the selected object.
- Area of effect can now be either a sphere- or box cast. This adds support for grid-based game where the area of effect of an ability should be f.eg. 3x4 tiles, 2x2 tiles etc.
- It's now possible to visualize the direction cone (area of effect area) for directional abilities in the editor. You turn this on by checking "Visualize Aoe" under the directional ability component.
- It's also possible to visualize the Area of effect for targeted abilities. The toggle for this is located under the targeted ability component.

### Changed
Most of the changes are breaking changes, so please read through them carefully.
- Projectile prefabs have been updated to more properly reflect their intended usage.
- `AbilityEffect.ApplyTo` has been changed to include more data and allow for easier unit testing effects. This will break custom effects, but in most cases the change will be very minor to fix for your project.
- The property "Move in 3D" was moved from ability to Projectile Movement. It's tooltip is also updated to reflect its intended usage: allow projectiles to be targeted (and travel towards) a point that's at a different height than their spawn point.
- `OnHitSpawnOnHitAtImpactPoint` was renamed to `SpawnOnHitAtImpactPoint`. Only the public name has been changed so it wont break your currently saved settings, but will break any custom code that's reading it.
- Renamed `noInPool` for projectile pools to `numInPull` for consistency and clarity.
- `Projectile` is now a base class, with different kind of projectiles (2D/3D) deriving from it. This also allows you to implement your own projectiles by derving your custom script from `Projectile` and implementing the methods required by SPAM. 

### Fixed
- Custom effects where not showing up under the Effects window.
- Issues in the documentation (dead links, formatting, typos...)
- Deprecation warnings has been fixed in later Unity versions.
- Some issues with Unity 2022 fixed. Note that 2022 is not yet officially supported, but minor testing will be done in this version until it is formally released as an LTS version.
- A bug where a null reference exception was filling up the log after a domain rebuild.
- A bug where the right components where not added to the GameObject you had targeted for an ability under some circumstances.

## [1.2.0] 2022-09-22 | Conditions
This version adds a completely new system to the framework: **Conditions**. 
Conditions is a versatile and powerful system that can be used to create alot of diverse effects, like the following (non-exhaustive) list of examples:
- Add condition(s) to targets on ability hit (stunned, on fire).
- Apply additional effects when a target has or lacks a certain condition (if target is on fire, deal additional damage and remove on fire).
- Only apply effects from abilities when the target/caster meets certain pre-conditions (can only be used on targets that are frozen. Can only be used when caster is shielded).
- Add VFX to a target when it's under a certain condition (on fire can display burning VFX).
- Allow for emergent gameplay (targets/caster gets wet when standing in water, which could enable/disable the use of some abilities or change how they work).
- Add states to targets when they equip a certain item (equipping a shield grants the shielded condition).
- Add elemental groups / types to your game (human, undead, troll... which then could be used as "if caster is vampire and target is human heal caster").

Conditions can be customized in a lot of ways, and has more uses than the ones outlined above. Please see the documentation either online or supplied with the package for more information.

### Added
- The conditions system (duh).
- A "rename asset"-button to effects, which is visible when the underlying effect-asset doesn't have the same name as the effect. This makes it easier to select effects in effect-pickers.
- AbilityEffects can now be applied to caster instead of target by checking "On caster" in the "Main Effects"-list.
- An example condition to the sample game provided.

### Changed
- Adding the same ability to multiple casters created lots of pools. Abilities now tries to reuse pools when they're added so you don't get drowned in them.
- AbilityInvoker now also implements IAbilityTarget to support applying effects to the caster instead of the target.
- The menu-items for SPAM-windows under "Tools" have been grouped under "Spam Framework".

### Fixed
- Effects were applied twice per cast under some circumstances. This was not intentional and has been nerfed back to it's intended "once per cast" behaviour.
- The new input system is now supported out of the box. It's a very bare implementation but atleast it won't break your project on import.
- "On Hit Rotation" has been un-hidden from the Ability VFX settings. 
- An exception were sometimes thrown when creating assets in a directory outside of Assets/ or in a subfolder that didn't exists. This has been fixed to assets will always be created under the chosen path, but never outside the Assets folder. 

## [1.1.2] - 2022-08-21
### Fixed
- The documentation had some embrarrasssing typos, these have been corrected.
- The quick-start part of the documentation wasn't as quick as intended. It is now rewritten to let you hit the ground running!
- Context menus for SPAM were a little more "spammy" than originally planned. They have now been grouped.

## [1.1.1] - 2022-08-15
### Changed
- Abilities previously required effects to spawn their VFX and SFX, this has been changed so it's no longer a requirement. Abilities can now be seen and heard without any effect what-so-ever on the game world.
- The offline-documentation was previously dead (hidden). It has been resurrected into the root of the project. 

### Added
- Added a readme to explain how to install the sample project and where to find the documentation.

### Fixed
- An exception was thrown when removing a VFX that had been assigned to an ability. This exception is now gone.
- The timing between ability effects in the ability effects-list was not being saved under some circumstances. Now it's always saved.
- The list of ability effects did not like to be reordered. It was sent to counseling and now understands the importance of being reordered.
- The list of ability effects did also not understand the concept of "be large enough to fit all items". The concept has been explain thoroughly to it and it now understands how to fit all its children properly.
- The namespace in the sample project was way outside of world bounds, throwing compilation errors like there was no tomorrow. After adjusting the namespace the sample now sits in place, ready to serve, like a proper NPC.

## [1.1.0] - 2022-08-08
### Changed
- Project structure updated to "UPM Package"-format. This means the framework now installs to ~/Packages instead of ~/Assets, keeping you project folder clean. If you encounter any difficulties while upgrading, please remove any prior versions and reimport. If you still have issues, please send an email to: [help@infinivtevoid.games](mailto:help@infinivtevoid.games)

### Added
- This changelog.

## [1.0.1] - 2022-08-06
### Fixed
- Removed references from core framework to example project.
- Fixed bug with abilities window not loading correctly in Unity version 2021.2 or newer.
- Fixed bug with projectiles without telegraphs.
- Fixed layout of effect list on lower resolution screens.

## [1.0.0] - 2022-08-02
Initial release