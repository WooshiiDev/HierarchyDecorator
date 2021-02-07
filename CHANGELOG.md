#### v0.6.0.0
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

#### v0.5.1.0
Readded custom component icons back to settings

 - Added functionality to icon settings switching between one and two column view if window gets too small
 - Improved visualisation of icon settings

Some of the settings still need tweaked slightly for various reasons (window size, ease of access etc.).
Will improve throughout v0.5 before jumping on to Hierarchy redesigns.
Later reworking of `ComponentTypes` is required as it's not the most flexible structure.

#### v0.5.0.0
 Settings redesign!

 - Removed single tab view due to it being a waste of space, and slow navigation.
 - Combined tabs into a single view with foldout displays.
 - Component Icon Types all display in settings as foldouts.
 - Component Icons can now all be enabled or disabled per type.
 - Prefixes can now all be expanded or hidden in settings.

#### v0.4.6.5
 - Removed Style Tab as it served very little purpose, settings will be added elsewhere
 - Fixed a bug with static references targeting the previously destroyed SerializableObject for settings

#### v0.4.6.4
 - Settings now correctly create new ScriptableObjects when one does not exist
 - Temporary defaults for Settings have been added until a more refined setup is made

#### v0.4.6.3
Fixed General Settings not saving

 - Lightened up the names of some of the classes as they were a little over the top

#### v0.4.6.2
Can now move settings asset around the project.

 - Path to current settings now saved in `EditorPrefs`. Will only create a new settings asset if one cannot be found within the project files
 - Checkboxes for objects have been hidden in the Prefab Editor to stop the overlap of the show/hide child toggles
 - Continuation of clean up, this time for the HierarchySettings class, see `AssetUtility.cs`

#### v0.4.6.1
Restructure of hierarchy decorator calls for correct two tone display

 - Seperated label GUI into it's own method
 - Darkened the selection and added a white tint when selected with two tone on
 - Fixed a bug that stopped the foldouts appearing correctly
 - Added padding to about tab
 - Tweaked some colours in default prefixes
 - Pull Request from @KreliStudio to add Enable/Disable buttons for all icon catergories

#### v0.4.6
- Fixed a bug where the bottom-most instance in the hierarchy could not be occasionally selected
- Added MonoBehaviour Icon Support. This can be toggled on/off to show all MonoBehaviour types as icons.
- Added Custom MonoBehaviour icons support. Within the Icon Tab, there is now a Custom Icon Catergory, that will allow users to select MonoBehaviours Directly. This will not only display the script if it's an existing component, 
but the custom icon for them too.

#### v0.4.5.1
- Custom GUIStyles now appear as a dropdown in the prefix style selection for easier switching

#### v0.4.5
- Setting tabs are now setup in their own individual classes
- Improved caching throughout due to this
- LayerMasks can now be changed from the Hierarchy (multi or single select)
- LayerMask display now has it's own settings catergory in the global settings tab

#### v0.4.4
- Added changelog to GitHub

#### v0.4.3
- Added null checks for components to fix console errors and hierarchy drawing
- Cleaned up light parts of the hierarchy decorator code
- Moved all global settings into `Global Settings` class
- Fixed a bug that caused errors when prefixes or styles were empty
- Fixed a bug that caused duplication of component types

#### v0.4.2 
- Finally fixed foldouts to work properly as normal

#### v0.4.1
- Did basic fixes to hierarchy now that background overlays everything
- Now draws toggles boxes using the UnityEngine style
- Redraws GameObject foldouts, but not always correctly
- Redraws default hierarchy Prefab/GameObject Icon beside GameObject names
- Added more component catergories to give easier selection, will revisit this in the future
for a more intuitive way of doing them

#### v0.4.0
- Added component icons
- Added component icon toggles
- Made prefix background widths full hierarchy size
- Added Two Tone background option
- Can now select between dark mode and light mode settings in prefix settings

#### v0.3.1 
- Created and finished general scriptable object settings design
- Overridden preferences GUI with scriptable object GUI

#### v0.3.0
- Added Options in Preferences
- Began first pass of editor for scriptable object

#### v0.2.0

- Added toggles for easier disable/enable
- Added layer visualisation in the hierarchy

#### v0.1.0:
- Prefix styles added for headers and catergorisation
