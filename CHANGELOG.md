## v0.9.1 Hotfix - Duplicate component types

This is to fix a use case of types being named the same that may exist in the same project.

Unfortunately, as usual, you will need to delete your settings instance.
 - Looking to find a solution to this.
 
 Cheers
~ Wooshii

## v0.9.0 | Expanded settings, breadcrumbs, new icon settings & further API additions

### New Features

**Active Swiping**
 Toggles now have active swiping! Automatically toggle instances by click dragging over their check boxes. Active swiping also has depth settings, and can be limited to just your current selection @Razenpok @medallyon 
 
 **Breadcrumbs**
Breadcrumbs have been added to have some extra visual indication of the hierarchy tree. Change colors, single or full depth breadcrumbs, with different styles.

**Two Tone**
Two-tone alternate colors can be changed for both light and dark mode.

**Style case**
 You can now set the case of styles choosing from Upper, Lower & No Change @SanielX

### Icons
 - Overhaul to the entire tab and functionality.
 - Enable all of Unity's built in components and custom ones with direct toggles. @SiarheiPilat @nankink
 - Optional toggle to show a single script icon for MonoBehaviours. @SiarheiPilat @nankink
 - You can now search for components!
 - Custom scripts can be grouped and dragged over from the project view.
 - A list of components to exclude has been added to disable them even if show all is enabled.
 
 ### Fixes
  - Fixed performance degradation with components as their icons were not being cached. 
  - Fixed a bug where component data was not being cached correctly.
  - Fixed a bug where alternating background colors could flicker.
  - Fixed a bug where foldouts would rarely begin flickering.

### API Additions
 - An [Attribute](https://github.com/WooshiiDev/HierarchyDecorator/blob/v0.9.0/HierarchyDecorator/Scripts/Editor/Attributes/RegisterTabAttribute.cs) have been added to handle the order of SettingTabs.
 - Using custom [GUIDrawers ](https://github.com/WooshiiDev/HierarchyDecorator/blob/v0.9.0/HierarchyDecorator/Scripts/Editor/GUI/GUIDrawer.cs)to automatically layout the settings window with ease.
 - ComponentTypes are grouped for categories using [ComponentGroup.cs](https://github.com/WooshiiDev/HierarchyDecorator/blob/v0.9.0/HierarchyDecorator/Scripts/Editor/Data/Types/ComponentGroup.cs)
 
 ### Future Changes
 
This should be the final pre-1.0 version. 
 Next the main focuses will include any missing features that have not been added thus far, and getting the API finalized. It would be nice if users and teams could use this tool to create their own additions easily.
 
 Thank you for the patience and time it has taken to release this, I appreciate everyone who takes the time to support this 💪 

Please take note, issues may exist as this is still in development, so please create an [issue](https://github.com/WooshiiDev/HierarchyDecorator/issues) if anything persists.

Thanks
 -- Wooshii 



## 0.8.9

### Bug Fixes
 - Fixed a bug where built in Unity MonoBehaviour's would not display icons when toggled on.
 
Cheers
~ Wooshii

## 0.8.8

### Bug Fixes
 - Fixed a bug where custom components would not update when changing the target script.
 
Cheers
~ Wooshii

## 0.8.7

### Changes
 - Added missing namespace to [GUIHelper.cs](https://github.com/WooshiiDev/HierarchyDecorator/blob/master/HierarchyDecorator/Scripts/Editor/Util/GUIHelper.cs) to avoid conflicts with other packages and/or scripts.
 
Cheers
~ Wooshii

## 0.8.6

### Bug Fixes
 - @AtaTrkgl | Updated the SceneManagement API for 2021.2 or newer.

## 0.8.5

This release provides fixes to custom info overlapping instance labels when the hierarchy gets small, and also provides some serialization issues with settings.

Please note this release may reset your settings but does not change or add features. 
It is recommended to copy your custom settings and replace them after using this release. 

### Bug Fixes
 - EditorPref PREF_GUID for quick loading settings is now project specific. (f96723a446b91d6e4eb5054ec87bce841a4b533b)
 - Fixed a bug where settings attempted to be cached before serialization (43e12763e6b858a748c8441c164a7eb94bcae5e7)
 - Fixed a bug where custom GUIStyles had no names assigned causing look up errors. (9ccd2102347d922688cc1e70cf21bb9ed9a945c2)
 
### Changes
 - Added OnDrawInit to setup data before drawing HierarchyInfo GUI. (0a05a8bd945f816dead8504c81bdd8942b504bdd) (07873266418b00d26b36ef69dac0a383c72d53bb)
 - HierarchyInfo (Layers, Component Icons) will now disappear when overlapping with instance labels in the hierarchy. (f6a3121345ee675a31c2e8857c289cba6a34b362)

Cheers
~ Wooshii

## 0.8.4 

This is the another update for v0.8 to round off some final changes and fixes required.

### Fixes
 - Fixed older version issues with icons or GUI rects appearing in the settings.
  - Unity versions using the old UI will not use Toggle Mixed GUIStyles. This is because of Unity having no dark mode version of that style in the old UI.
  
### Changes
 - Version checks now happen in `OnEnable` for Settings. This is primarily for version support, but also moves some irrelevant code from `ISerializationCallback` methods.
 - Renamed PrefixTab file to StyleTab for correct naming.
 - Small adjustments to GUI for Unity's old UI.
 
Cheers
~ Wooshii

## 0.8.3 | 2021.X Support, Foldout Flicker Fix

This release fixes a flickering bug that was seen across multiple versions of Hierarchy Decorator when expanding or closing instances if they had children. 

Unity 2021.1 and higher now has support with some fixes to the settings GUI.

### Fixes

 - StyleDrawer now stores cache for foldouts and will draw them without flickering.
 - Unity 2021.1 and higher now draws styles correctly in the settings style tab
 - Fixed a bug in Unity 2021.1 and higher where Hierarchy Decorator could not add new styles.

### Changes
 
  - Clean up of the Rects used in the reorderable for the style tab

Cheers,
~Wooshii

## v0.8.2 | Unity 2020 Foldout GUI support

This small release is a hotfix for toggles in Unity 2020 and higher. There were some interaction issues with foldouts when attempting to show or hide children. This release should fix that issue. 

Currently, an issue exists where foldouts will flicker for a short period when expanding or closing them. This is because of the manual draw but it is known. It is on the priority list to be fixed and is being worked on currently. 

Cheers,
~Wooshii

## v0.8.1 | Missing Script Warning

Please note, due to changes you will need to update your HierarchyDecorator version

### Changes
- Added new setting of showing a warning when missing components are detected
- Darkened the inactive colour of instances in the hierarchy when two-tone is enabled.
- Layer drawer label is now brighter 

## v0.8.0 | Settings changes, style previews & public API
Please Note, due to API changes you will need to update your HierarchyDecorator version completely.

### Improvements
 - Styles are now previewed in the "Style" tab of settings.
 - Styles are now reorderable and have a cleaner, functional GUI.
 - Styles now have toggles specifically for styled instances for displaying layers and icons.
 - HierarchyDecorator has an improved check for unity component types, not just checking component counts, but version changes. If the Unity version has changed, it will update all components again.

### Changes
 - The settings class has been fully changed with settings all contained within data classes. This allows easier reference and manipulation of settings, while also containing them correctly.
 - All internal classes have been made public. This change has been added just in case anyone wants to access Hierarchy Decorator for any reason whatsoever. This will change throughout development however, so caution is advised.
 - Icons will now disable toggles on icon categories if "Show All Icons" is enabled. This is to stop confusion between enabling all icons and individual icons being accessible at the same time.

## 0.7.0.0: Fixed persisting foldout issues

## v0.6.0.0
 Hierarchy Core Improvements & Redesign

 Additions Changes
 - Hierarchy now has a new feature system, with easy toggles on options. Will be reflected in the settings in the near future.
 - Hierarchy features/info displayed will now reposition based on what is hidden or displayed keeping space clean.
 - Hovering over component icons will now display what component they are. 
 - The normal hierarchy data now draws under the custom features. This will be provided as an option in the settings in the future.
 - Can now toggle on/off full width styling for the two tone background and styles. If turned off, this will draw custom styles within the normal rect for each instance.

 Fixes/Bugs
 - Fixed a bug where the ScriptableObject will not be created from the git repository.
 - Removed the Settings ScriptableObject from the project. The settings will still exist within the package.
 - Fixed a bug where the settings would revert after editing other setting tabs.
 - Fixed bugs related to saving and updating where settings would revert if AssetDatabase.SaveAssets() was not called. All settings are now handled through serialized properties.
 - General optimisation and clean up of `HierarchyDecorator.cs` has been done.

## v0.5.1.0
Readded custom component icons back to settings

 - Added functionality to icon settings switching between one and two column view if window gets too small
 - Improved visualisation of icon settings

Some of the settings still need tweaked slightly for various reasons (window size, ease of access etc.).
Will improve throughout v0.5 before jumping on to Hierarchy redesigns.
Later reworking of `ComponentTypes` is required as it's not the most flexible structure.

## v0.5.0.0
 Settings redesign!

 - Removed single tab view due to it being a waste of space, and slow navigation.
 - Combined tabs into a single view with foldout displays.
 - Component Icon Types all display in settings as foldouts.
 - Component Icons can now all be enabled or disabled per type.
 - Prefixes can now all be expanded or hidden in settings.

## v0.4.6.5
 - Removed Style Tab as it served very little purpose, settings will be added elsewhere
 - Fixed a bug with static references targeting the previously destroyed SerializableObject for settings

## v0.4.6.4
 - Settings now correctly create new ScriptableObjects when one does not exist
 - Temporary defaults for Settings have been added until a more refined setup is made

## v0.4.6.3
Fixed General Settings not saving

 - Lightened up the names of some of the classes as they were a little over the top

## v0.4.6.2
Can now move settings asset around the project.

 - Path to current settings now saved in `EditorPrefs`. Will only create a new settings asset if one cannot be found within the project files
 - Checkboxes for objects have been hidden in the Prefab Editor to stop the overlap of the show/hide child toggles
 - Continuation of clean up, this time for the HierarchySettings class, see `AssetUtility.cs`

## v0.4.6.1
Restructure of hierarchy decorator calls for correct two tone display

 - Seperated label GUI into it's own method
 - Darkened the selection and added a white tint when selected with two tone on
 - Fixed a bug that stopped the foldouts appearing correctly
 - Added padding to about tab
 - Tweaked some colours in default prefixes
 - Pull Request from @KreliStudio to add Enable/Disable buttons for all icon catergories

## v0.4.6
- Fixed a bug where the bottom-most instance in the hierarchy could not be occasionally selected
- Added MonoBehaviour Icon Support. This can be toggled on/off to show all MonoBehaviour types as icons.
- Added Custom MonoBehaviour icons support. Within the Icon Tab, there is now a Custom Icon Catergory, that will allow users to select MonoBehaviours Directly. This will not only display the script if it's an existing component, 
but the custom icon for them too.

## v0.4.5.1
- Custom GUIStyles now appear as a dropdown in the prefix style selection for easier switching

## v0.4.5
- Setting tabs are now setup in their own individual classes
- Improved caching throughout due to this
- LayerMasks can now be changed from the Hierarchy (multi or single select)
- LayerMask display now has it's own settings catergory in the global settings tab

## v0.4.4
- Added changelog to GitHub

## v0.4.3
- Added null checks for components to fix console errors and hierarchy drawing
- Cleaned up light parts of the hierarchy decorator code
- Moved all global settings into `Global Settings` class
- Fixed a bug that caused errors when prefixes or styles were empty
- Fixed a bug that caused duplication of component types

## v0.4.2 
- Finally fixed foldouts to work properly as normal

## v0.4.1
- Did basic fixes to hierarchy now that background overlays everything
- Now draws toggles boxes using the UnityEngine style
- Redraws GameObject foldouts, but not always correctly
- Redraws default hierarchy Prefab/GameObject Icon beside GameObject names
- Added more component catergories to give easier selection, will revisit this in the future
for a more intuitive way of doing them

## v0.4.0
- Added component icons
- Added component icon toggles
- Made prefix background widths full hierarchy size
- Added Two Tone background option
- Can now select between dark mode and light mode settings in prefix settings

## v0.3.1 
- Created and finished general scriptable object settings design
- Overridden preferences GUI with scriptable object GUI

## v0.3.0
- Added Options in Preferences
- Began first pass of editor for scriptable object

## v0.2.0

- Added toggles for easier disable/enable
- Added layer visualisation in the hierarchy

## v0.1.0:
- Prefix styles added for headers and catergorisation
