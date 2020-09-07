# Hierarchy Decorator
[![Unity 2018.3+](https://img.shields.io/badge/unity-2018.4%2B-blue.svg)](https://unity3d.com/get-unity/download)
[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](https://github.com/WooshiiDev/HierarchyDecorator/blob/master/LICENSE)

Hierarchy Decorator is an extension for Unity 2019.1 and higher to support custom drawing within the Hierarchy window, and add extra information functionality. This provides easier and helpful structure to the Hierarchy without getting overly noisy or messy. 

![](https://i.imgur.com/ohTbb0t.gif)

### Installation

Please make sure you have Net 4.0 selected in the **Player Settings** to use this

1. You can install this by downloading the .unitypackage that is provided in this repository. In the UnityPackage there are preset settings provided if you would like a basic system but it can be extended with your own ones.

2. HierarchyDecorator can also be installed directly through the git url

`https://github.com/WooshiiDev/HierarchyDecorator.git`

3. You can also install it via [upm](https://openupm.com/)

`openupm add com.wooshii.hierarchydecorator`

4. You can also install this via git by adding the following to your **manifest.json**

`"com.wooshii.hierarchydecorator" : "https://github.com/WooshiiDev/HierarchyDecorator.git#upm"`

**Note (Current Issue)**

When using 2018.4 it will remove assembly references when the package is brought into your project. 
Please go to **Hierarchy Decorator** -> **Scripts** -> **Editor** -> **Wooshii.HierarchyDecorator.Editor** and add **Wooshii.HierarchyDecorator** to the assembly list

### Feedback
Please submit any queries, bugs or issues, to the issues page on this repository. All feedback is appreciated as it not just helps myself find problems I didn't otherwise see, but also helps improve the project. 

### Support
HierarchyDecorator will be and always has been developed in my free time, and there are many more features I'd like to include. If you would to support me, you can do so below:

[PayPal](https://paypal.me/Wooshii?locale.x=en_GB)

Development will be continued with this and will forever stay public and free.

# Overview 
Currently the features included are:

 - Custom hierarchy GameObject styling for better seperators and headers
 - Showing the current GameObject layer
 - Icon visualisation for components that are on GameObjects
 - Custom settings with interchangable style and icon selections
 - Light/Dark mode toggle for styles
 
The custom hierarchy styling is capable through prefixes added to the start of GameObjects. After added, they will instantly change to the styling the related prefix provides.

### Settings
![](https://i.imgur.com/E36wayq.png)

There is a scriptable object that is required for hierarchy decorator to run. If it is deleted, another will be created in the base of your asset folder in `Assets/HierarchyDecorator/`. These settings are also accessible from the `Preference` window.

There are 4 sections to the settings:

 - **Global Settings** - will show the general toggles for showing and hiding features 
 
 ![](https://i.imgur.com/TKMe0kO.png)
 
 - **Prefixes** - Showing the settings for all prefixes 
 
 ![](https://i.imgur.com/GSiP5so.png)
 
 - **Styles** - Change the `GUIStyle` settings for each style 
 
 ![](https://i.imgur.com/JqPw9Hx.png?1)
 
 - **Shown Components** - Toggle on/off the components you wish to see appear beside GameObjects if they exist on them.   
 
 ![](https://i.imgur.com/QRMhsGU.png?1)

