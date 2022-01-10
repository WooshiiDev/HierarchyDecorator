<h2 align="center">
Please note this is still in development! Check <a href="https://github.com/WooshiiDev/HierarchyDecorator/issues">Issues</a> for any current support issues or bugs that may exist!
</h2>

<h1 align="center">  
 <img src="https://wooshii.dev/img/work/hierarchydecorator.png">
 Hierarchy Decorator
</h1>

<h4 align="center"> Unity Editor Plugin turning the Hierarchy into what it should be. <br><br>
 
 Fully Customisable. <br>
 Completely Editable.</h4>

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


Hierarchy Decorator is an extension for Unity 2018.4 and higher to support custom drawing within the Hierarchy window, and add extra information functionality. This provides easier and helpful structure to the Hierarchy without getting overly noisy or messy. 

<p align="center">
<img src="https://i.imgur.com/YCiU6zn.png" alt="Unity Download Link">
</p>

## Installation
<p align="center">
  <a href="https://github.com/WooshiiDev/HierarchyDecorator/releases">Releases</a> • <a href="https://github.com/WooshiiDev/HierarchyDecorator/releases/download/v0.8.4/HierarchyDecorator.v0.8.4.unitypackage">Unity Package</a> • <a href="https://github.com/WooshiiDev/HierarchyDecorator/archive/0.5.1.0.zip">Zip</a> 
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
Net 4.0 is required to use this. This can be turned on in the **Player Settings**.

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
| Rule-based Styling         |         Planned     |        ❌        |
| External Package Support   |         Planned     |        ❌        |
| Script Error/Warning Popup |         Planned     |        ❌        |
| Undeletable Instances      |         Planned     |        ❌        |
| Selectable Editor Flags    |         Planned     |        ❌        |
| Hierarchy Comment Popups   |         Planned     |        ❌        |

## Settings
<p align="center">
 <img src="https://i.imgur.com/FeZaopz.png" alt="Settings">
</p>

There is a scriptable object that is required for hierarchy decorator to run. If it is deleted, another will be created in the base of your asset folder in `Assets/HierarchyDecorator/`. These settings are also accessible from `Preferences`.

All settings enabled and disabled will reflect the Hierarchy instantly.
<p>
 <h3>General Settings</h3>

This section will contain all the general enabling/disabling of features and systems. In the future this may be redesigned or structured differently depending on the catergories required.
 </p>
</p>
<p align="center">
 <img src="https://i.imgur.com/6J0Pw9a.png" alt="Global settings">
</p>

<p>
 <h3>Hierarchy Styles</h3>
 
  The Style tab controls the design of the headers and seperators for the hierarchy. Colours are individual for light and dark mode providing accessibility.
 </p>
</p>
<p align="center">
 <img src="https://i.imgur.com/6zfMz61.png" alt="Style Settings">
</p>
 
<p>
 <h3>Icons</h3>
 
 To provide selection and custom choice over what components will display in the inspector, here all icons that exist in your **current version** will appear in their specified catergories. Toggling the icons on or off will display them in the hierarchy on the right side, and you can also add your own MonoBehaviours for custom scripts.
</p>
</p>
<p align="center">
 <img src="https://i.imgur.com/Itr5KXX.png" alt="Component/Icon Settings">
</p>
<p>
Icons for custom Monobehaviours or Non-UnityEngine types have their own tab and toggles so they can be added. 
</p>
<p align="center">
 <img src="https://i.imgur.com/P3ftsD0.png" alt="Component/Icon Settings">
</p>



## Support
Please submit any queries, bugs or issues, to the [Issues](https://github.com/WooshiiDev/HierarchyDecorator/issues) page on this repository. All feedback is appreciated as it not just helps myself find problems I didn't otherwise see, but also helps improve the project. 

A GitHub Project [Board](https://github.com/WooshiiDev/HierarchyDecorator/projects/1) for this project also exists showing all current progress and backlog information.

Reach out to me or see my other work through:

 - Website: https://wooshii.dev/
 - Email: wooshiidev@gmail.com;

## Donate
HierarchyDecorator will be and always has been developed in my free time, and there are many more features I'd like to include. If you would to support me, you can do so below:

[![PayPal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://paypal.me/Wooshii?locale.x=en_GB)

Development will be continued with this and will forever stay public and free.

Copyright (c) 2020-2021 Damian Slocombe
