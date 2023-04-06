<h1 align="center">  
 <img width="824" alt="HierarchyDecoratorNew" src="https://user-images.githubusercontent.com/31889435/226486126-009081e1-44de-465c-8ff7-5641870fdcae.png">
 
 Hierarchy Decorator
</h1>

<h4 align="center"> Unity Editor plugin giving the Hierarchy a lick of paint.<br><br>
 
 Fully Customisable.<br>
 Toggle Everything.</h4>

<p align="center">
 <a href="https://unity3d.com/get-unity/download">
 <img src="https://img.shields.io/badge/unity-2018.4%2B-blue.svg" alt="Unity Download Link">
 <a href="https://github.com/WooshiiDev/HierarchyDecorator/blob/master/LICENSE">
 <img src="https://img.shields.io/badge/License-MIT-brightgreen.svg" alt="License MIT">
</p>
  

<p align="center">
  <a href="#about">About</a> •
  <a href="#installation">Installation</a> •
  <a href="#features">Features</a> •
  <a href="#support">Support</a> •
  <a href="#donate">Donate</a>
</p>

## About

Hierarchy Decorator is an extension for Unity 2018.4 and higher that extends Unity's hierarchy and takes it to the next level. With headers, component information and other features, it transforms the window into more than a plain list of objects. This can turn scene structures easier to read, understand and provide information on what is going on.

Everything is optional, and can be modified to the requirements of the project.

<p align="center">
<img width="372" alt="Unity_k2yhUSLugm" src="https://user-images.githubusercontent.com/31889435/226486583-8ad71e2b-1051-46b3-b00a-3880acffc413.png">
<img width="372" alt="Unity_myS52drEnl" src="https://user-images.githubusercontent.com/31889435/226486476-768a99ad-ae6f-4609-b8f8-8537c1f2393d.png">
</p>

## Installation
<p align="center">
  <a href="https://github.com/WooshiiDev/HierarchyDecorator/releases">Releases</a> • <a href="https://github.com/WooshiiDev/HierarchyDecorator/releases/download/v0.9.0/HierarchyDecorator.v0.9.0.unitypackage">Unity Package</a> • <a href="https://github.com/WooshiiDev/HierarchyDecorator/archive/master.zip">Zip</a> 
</p>
  

HierarchyDecorator can also be installed directly through the git url
```
https://github.com/WooshiiDev/HierarchyDecorator.git
```

You can also install it via [upm](https://openupm.com/)

```
openupm add com.wooshii.hierarchydecorator
```

You can also install this via git by adding the following to your **manifest.json**
```
"com.wooshii.hierarchydecorator" : "https://github.com/WooshiiDev/HierarchyDecorator.git"
```

## Features

Currently, this is what Hierarchy Decorator does have
|                            | Hierarchy Decorator  | Other Hierachy Extensions |
| -------------------------- | :----------------: | :-------------:   |
| Light/Dark Mode Support    |         ✔️         |        ✔️        |
| Headers/Seperators         |         ✔️         |        ✔️        |
| Toggleable Settings        |         ✔️         |        ❌        |
| Custom Header Styles       |         ✔️         |        ❌        |
| GameObject Layer Selector  |         ✔️         |        ❌        |
| Unity Icon Selection       |         ✔️         |        ❌        |
| Custom Icon Selection      |         ✔️         |        ❌        |

These are currently future additions planned

|                            | Hierarchy Decorator | Other Hierachy Extensions |
| -------------------------- | :----------------:  | :-------------:   |
| Hierarchy Breadcrumbs      |         Planned     |        ✔️        |
| Hierarchy Folders          |         Planned     |        ✔️        |
| Custom Instance Icons      |         Planned     |        ✔️        |
| External Package Support   |         Planned     |        ✔️        |
| Script Error/Warning Popup |         Planned     |        ❌        |
| Locked Instances           |         Planned     |        ❌        |
| Selectable Editor Flags    |         Planned     |        ❌        |
| Hierarchy Comment Popups   |         Planned     |        ❌        |
| <a href="https://github.com/WooshiiDev/HierarchyDecorator/issues/25">Team/Individual Settings Mode</a>   |         Planned     |        ❌        |
  
## Settings
  
<p align="center">
 <img align="center" width="929" alt="chrome_hzlst44Z1X" src="https://user-images.githubusercontent.com/31889435/226493547-3ec3db89-bcdf-4412-b000-c90aa9ee30b7.png">
</p>

There is a scriptable object that is required for hierarchy decorator to run. If it is deleted, another will be created in `Assets/HierarchyDecorator/`. These settings are also accessible from `Preferences`.

Setting design may change over time with development to support more features, or keep things looking consistent & clean.


### General
  
<details>
 <summary><b>Toggles</b></summary>
  
 <p align="center">
   <img width="915" alt="chrome_aClIcjH3wq" src="https://user-images.githubusercontent.com/31889435/226558578-78287342-711c-4b4b-acf3-18b316f3216b.gif">
   <img width="915" alt="chrome_aClIcjH3wq" src="https://user-images.githubusercontent.com/31889435/226493660-ba63cb16-a046-48f3-8d1a-c1ea0007de4a.png">
 </p>

  Toggles will simply display the state of the instance, can be clicked to toggle the instance active state.

  ```
  Show Active Toggles     Enable the toggles.
  Active Swiping          Click and drag over check boxes to toggle them.
  Swipe Same State        Only toggle the instances with the same state as the first selected.
  Swipe Selection Only    If a selection exists, only toggle the selected instances.
  Depth Mode              The accepted criteria for selecting instances when swiping.
  ```
</details>

<details>
 <summary><b>Layers</b></summary>

<p align="center">
  <img width="913" alt="chrome_szO7gPHVZ4" src="https://user-images.githubusercontent.com/31889435/226493749-b30ebd4a-bf89-4841-bde8-b78159ec6068.png">
</p>
  
 Display the current layer the instance is assigned to.
  
  ```
  Show Layers             Enable the toggles.
  Click To Select Layer   Clicking the layer label will display a layer dropdown to update it.
  Apply Child Layers      Change the child gameobjects when updating the layer above.
  ```
</details>

<details>
 <summary><b>Breadcrumbs</b></summary>

<p align="center">
   <img width="922" alt="chrome_DtNbO5Mimi" src="https://user-images.githubusercontent.com/31889435/226493794-e45fbb59-ec38-430a-a3ba-2cd137251f46.png">
</p>
  
  Breadcrumbs will show line trails in the hierarchy, between objects to help visualise the tree. 
  
  _Instance_ settings are related to breadcrumbs drwan for the instance and it's siblings.<br>
 _Hierarchy_ settings will modify how breadcrumbs are displayed for higher depths.
  
  ```
  Show                    Show the breadcrumbs.
  Color                   The colour of the drawn lines.
  Style                   The line style - Solid, Dash, Dotted.
  Display Horizontal      Draw a horizontal line, from left to right, towards the instance.
  ```
</details>
 
### Visual

<details>
 <summary><b>Background</b></summary>
 
<p align="center">
 <img width="612" alt="chrome_Y3lak6Q0Dm" src="https://user-images.githubusercontent.com/31889435/226495114-e578f1c4-60d4-473e-8b42-b2c09c0fdeb1.png">
</p>

  The background can be enabled to alternate background colour between each hierarchy row. 
    
  ```
  Alternate Background    Show the breadcrumbs.
  Color One               The first colour for the theme.
  Color Two               The second colour for the theme.
  ```
</details>

<details>
 <summary><b>Styles</b></summary>
   
  The Style tab controls the design of the headers and seperators for the hierarchy. Colours are individual for light and dark mode providing accessibility. The **prefix** is the string to specify at the start of an instance name to apply the style.
  
  Layers and icons can be specifically disables on styles instances to remove clutter and information that is not required.

 <p align="center">
  <img src="https://i.imgur.com/6zfMz61.png" alt="Style Settings">
 </p>
</details>

  ### Icons

 Icons can be displayed that represent components that exist on gameobjects. This tab will provide the flexibility to specify what components can and cannot be displayed, and also allow you to automatically show all. 

<p align="center">
 <img width="132" height="327" alt="Unity_emGzaT8YHM" src="https://user-images.githubusercontent.com/31889435/226554415-8bd0be96-6eb2-4217-8d56-e39d23dd7ffd.png"><img width="627" alt="Unity_iWzNrNwYKa" src="https://user-images.githubusercontent.com/31889435/226494920-6b78be6e-686d-42ac-a11f-629f270cb5bc.png">
</p>

<details>
 <summary><b>Settings</b></summary>
 
 - Enable Icons: Will toggle the icons on.
 - Stack Mono Behaviours - If there are any MonoBehaviour derived components, show only one script icon on the instance.
 - Show Missing Script Warning - Show an indicator if there's an invalid script or "missing" script that cannot find the source file.

</details>

<details>
 <summary><b>Icon Panel</b></summary>
 
**Show All**

Below show all are two labels - Unity & Custom. Both of these can be enabled to automatically show the respective components automatically on all instances.
Unity components refer to built in types, while custom are custom MonoBehaviour's outside of Unity's code base.

**Groups**

Unity components have been categorized into related groups to make it easier to filter through all of them that exist. The search can be used to extend this further.

Any component toggled on in Excluded will disable them completely even if show all Unity components is enabled. This is primarily to make it easier to remove types not required when **Show All** is on.

**Custom**

Custom components are for scripts created in the project, that are not a part of Unity's engine. Here scripts can be grouped together and enabled if **Show All** for custom components is not on. 

Scripts can also be dragged in from the project view and will be added to the group highlighted for easy organisation.

<p align="center">
 <img width="40%" alt="Unity_emGzaT8YHM" src="https://user-images.githubusercontent.com/31889435/226555645-85954060-c25e-4ae8-bf5b-c7313d1188ee.gif">
<img width="40%" alt="Unity_emGzaT8YHM" src="https://user-images.githubusercontent.com/31889435/226556916-73888fe0-b8fa-4365-88ab-18d786aa7c37.gif">
</p>

</details>

## Contributing
When contributing to this repository, please first discuss the change you wish to make via issue or any other method with the owners of this repository before making a change.

### Pull Request Process
1. Ensure any install or build dependencies are removed before the end of the layer when doing a build.
2. Update the README.md with details of any new features if required.
3. Increase the version numbers in any examples files and the README.md to the new version that this Pull Request would represent. The versioning scheme we use is SemVer.

## Support
Please submit any queries, bugs or issues, to the [Issues](https://github.com/WooshiiDev/HierarchyDecorator/issues) page on this repository. All feedback is appreciated as it not just helps myself find problems I didn't otherwise see, but also helps improves Hierarchy Decorator as a whole.

A GitHub Project [Board](https://github.com/users/WooshiiDev/projects/1) for this also exists showing current development goals and future features.

Reach out to me or see my other work through:

 - Website: https://wooshii.dev/
 - Email: wooshiidev@gmail.com;

## Donate
HierarchyDecorator will be and always has been developed in my free time, and there are many more features I'd like to include. If you would to support me, you can do so below:

[![PayPal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://paypal.me/Wooshii?locale.x=en_GB)
<p href="https://ko-fi.com/L3L026UOE"><img src="https://ko-fi.com/img/githubbutton_sm.svg">

Development will be continued with this and will forever stay public and free.

Copyright (c) 2020-2023 Damian Slocombe
