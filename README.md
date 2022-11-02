# MagicCube
🧭 **`Index`**
- [**Project Hierarchy**](#project-hierarchy)
- [**Scene Hierarchy**](#scene-hierarchy)
- [**Scripts Structure**](#scipts-structure)
- [**Class Dependency**](#class-dependency)
- [**Solid Principles**](#solid-principles)
- [**Assembly Definitions**](#assembly-definitions)


# Project Hierarchy
Screenshot 2022-11-02 at 10.28.30 PM.png<img width="1392" alt="image" src="https://user-images.githubusercontent.com/16806053/199553689-222cf430-6e28-4482-b99c-0f3fded31a31.png">



Overview -
1. **`Editor`** - Unity Editor Specific Scripts
1. **`Materials`** - Materials used to represent the Magic Cube Face Colors
1. **`Models`** - Magic Cube FBX
1. **`Prefabs`** - Magic Cubes of different sizes and their respective render cameras
1. **`Scenes`** - All scenes
1. **`Scripts`** - Custom scripts for this project
1. **`Textures`** - Custom textures




# **Scene Hierarchy**
Screenshot 2022-11-02 at 10.34.09 PM.png<img width="336" alt="image" src="https://user-images.githubusercontent.com/16806053/199554635-6dd6227e-2fcb-4fba-ac0b-dc2d1dd17f9d.png">


Overview -
1. **`Core`** - Core logic scripts
1. **`UI`** - UI elements




# **Scipts Structure**
Screenshot 2022-11-02 at 10.38.38 PM.png<img width="304" alt="image" src="https://user-images.githubusercontent.com/16806053/199555460-e60ceb43-ee44-4740-9e04-8174e93a1147.png">


Overview -
1. **`Core`** - Scipts that control the controlling algorithm of the application
2. **`Data`** - Scripts that contains static data asseccible by all Core scripts
3. **`Utility`** - Utility Scripts just used to carry out repetitive tasks in the Unity Editor


# **Class Dependency**
Screenshot 2022-11-02 at 10.58.17 PM.png<img width="1259" alt="image" src="https://user-images.githubusercontent.com/16806053/199560378-c5a18295-19d2-449f-9b2c-af298a8afde7.png">


This project's architecure has been primarily inspired from the MVC. Where model contains classes which hold static data to govern parameters throughtout the game, Controller contains the core controlling classes and View contains the Visual aspect of the game.



# **Solid Principles**
Screenshot 2022-11-02 at 11.01.53 PM.png<img width="1388" alt="image" src="https://user-images.githubusercontent.com/16806053/199560657-47f3cb64-c93c-43b0-ae09-ddb6e119df86.png">


The scripts in the project follow S.O.L.I.D Principles, where every script has strictly single responsibility. To elaborate on this, the manager scripts are singletons and as shown below have a single dedicated responsility:

Managers -
1. **`GameManager.cs`** - ONLY Controls the Main Flow of the game, is the BIG DADDY.
2. **`CubeManager.cs`** - ONLY Controls every aspect related to the magic cube.
3. **`InputManager.cs`** - ONLY handles User input
4. **`UIManager.cs`** - ONLY handles UI related tasks such as show/hide menus or UI elements.





# **Assembly Definitions**
Screenshot 2022-11-02 at 11.13.52 PM.png<img width="1388" alt="image" src="https://user-images.githubusercontent.com/16806053/199563386-5141e09b-da27-4c6a-b6b2-59b81a3ce793.png">

The Custom scripts utilise the full potential of NAMESPACE and ASSEMBLY DEFINITIONS which forces the development architecture to follow good OOPs practices.



