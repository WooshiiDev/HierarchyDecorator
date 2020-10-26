<h1 align="center">  
 <br>
 <img src="https://wooshii.studio/v2/img/projects/hierarchydecorator.png">
</h1>

<h4 align="center">  Unity editor extension adding requirements the hierarchy is missing.<br>Fully Customizable. Fully Editable.</h4>

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
  <a href="#donate">Donate</a> •
  <a href="#license">License</a>
</p>

## About

Hierarchy Decorator is an extension for Unity 2019.1 and higher to support custom drawing within the Hierarchy window, and add extra information functionality. This provides easier and helpful structure to the Hierarchy without getting overly noisy or messy. 

![](https://i.imgur.com/ATzBDoO.gif)

## Installation
<p align="center">
  <a href="https://github.com/WooshiiDev/HierarchyDecorator/releases/tag/v0.4.6">Recent Release (v0.4.6)</a> • <a href="https://github.com/WooshiiDev/HierarchyDecorator/releases/download/v0.4.6/HierarchyDecorator.unitypackage">Unity Package</a> • <a href="https://github.com/WooshiiDev/HierarchyDecorator/archive/v0.4.6.zip">Zip</a> 
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

|                            | Wooshii Hierarchy Decorator  | Other Hierachy Extensions |
| -------------------------- | :----------------: | :-------------: |
| GameObject Layer Selector  |         ✔️         |        ✔️        |
| Light/Dark Mode Support    |         ✔️         |        ✔️        |
| Custom Headers/Seperators  |         ✔️         |        ❌        |
| Component Icon Display     |         ✔️         |        ❌        |
| Toggleable Settings        |         ✔️         |        ❌        |

## Settings
![](https://i.imgur.com/Uop5ZEv.png)

There is a scriptable object that is required for hierarchy decorator to run. If it is deleted, another will be created in the base of your asset folder in `Assets/HierarchyDecorator/`. These settings are also accessible from the `Preference` window.

<p>
 <h3>Global</h3>
 
 All settings that generally control what the hierarchy looks like will be here. Optional settings, toggles and modes will also be shown here. This tab will go through
 many design changes as development goes on, just to keep everything tidy and structured.
 </p>
</p>
<p align="center">
 <img src="https://i.imgur.com/9zldtdE.gif" alt="Global settings">
</p>

<p>
 <h3>Prefixes</h3>
 
 The Prefix tab controls the design of the headers and seperators for the hierarchy. Changing or updating the settings will instantly show in the hierarchy.
 </p>
</p>
<p align="center">
 <img src="https://i.imgur.com/mLpfHcK.gif" alt="Prefix Settings">
</p>

<p>
 <h3>GUIStyles</h3>
 
 The GUIStyle tab controls the drawing of each header and style, on top of the prefix tab. Currently, this doesn't provide much and will most likely be removed in the future,
 with replacement settings added to the prefix tab. This however, can still be changed without any restrictions.
 </p>
</p>
<p align="center">
 <img src="https://i.imgur.com/JqPw9Hx.png?1" alt="GUIStyle Settings">
</p>
 
<p>
 <h3>Component Icons</h3>
 
 To provide selection and custom choice over what components will display in the inspector, here all icons that exist in your **current version** will appear in their specified catergories. Toggling the icons on or off will display them in the hierarchy on the right side, and you can also add your own MonoBehaviours for custom scripts.
 </p>
</p>
<p align="center">
 <img src="https://i.imgur.com/93LhHx9.gif" alt="Component/Icon Settings">
</p>

## Support
Please submit any queries, bugs or issues, to the [Issues](https://github.com/WooshiiDev/HierarchyDecorator/issues) page on this repository. All feedback is appreciated as it not just helps myself find problems I didn't otherwise see, but also helps improve the project. 

A GitHub Project [Board](https://github.com/WooshiiDev/HierarchyDecorator/projects/1) for this project also exists showing all current progress and backlog information.

Reach out to me or see my other work through:

 - Website: https://wooshii.studio/
 - Email: wooshiidev@gmail.com;

## Donate
HierarchyDecorator will be and always has been developed in my free time, and there are many more features I'd like to include. If you would to support me, you can do so below:

[![PayPal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://paypal.me/Wooshii?locale.x=en_GB)
[![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/L3L026UOE)

Development will be continued with this and will forever stay public and free.

## License
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Copyright (c) 2020 Damian Slocombe
