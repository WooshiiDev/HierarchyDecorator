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
