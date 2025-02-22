# 3D Level Builder - Unity

The Level Builder is a custom tool designed as part of the development of the Android game *SpaceRoads*. It facilitates the creation and management of game levels within the Unity engine, streamlining the process of designing, editing, and testing game environments. This tool enhances workflow efficiency and maintains organized, scalable code architecture, making it a valuable asset for game development teams.

## Features

- **Intuitive Level Design Interface**: Provides a user-friendly editor within Unity for placing and manipulating game objects.
- **Prefab Management**: Utilizes Unity's prefab system to allow for reusable and consistent game object templates.
- **Event Handling System**: Incorporates a robust event system to manage in-game interactions and triggers.
- **Data Persistence**: Supports saving and loading of level data to facilitate iterative development and testing.
- **Level Templates & Random Level Generation**: Allows developers to create level templates using Scriptable Objects, enabling the rapid generation of unlimited random levels directly from the editor. Templates can be saved and used to generate new levels with a single click.

## Code Architecture

The Level Builder is structured to promote modularity and scalability, adhering to object-oriented programming (OOP) principles. The primary components include:

### 1. Editor Scripts

Custom editor scripts extend Unity's functionality, providing specialized inspectors and windows for level design. These scripts enhance the user experience by offering tailored interfaces for manipulating level components.

- **LevelBuilderEditor.cs**: Manages the main editor window, offering tools for placing and configuring game objects.
- **PrefabSelector.cs**: Provides a selection interface for available prefabs, enabling quick placement within the level.

### 2. Core Components

These scripts define the fundamental behaviors and properties of level elements.

- **LevelBuilder.cs**: Serves as the central controller, handling the initialization and organization of level components.
- **LevelSelectionManager.cs**: Manages the selection and placement of prefabs within the editor environment.
- **FinalContainerController.cs**: Oversees the final arrangement and grouping of game objects before runtime integration.

### 3. Scriptable Objects

Utilizes Unity's ScriptableObject class to store and manage data assets, promoting a clean separation between data and behavior.

- **LevelsSO.cs**: Defines the structure for storing level data, including metadata and references to prefabs.
- **LevelTemplatesSO.cs**: Stores predefined level templates that can be used to generate new levels with one click, facilitating procedural level creation.

## Object-Oriented Design Principles

The Level Builder's architecture emphasizes key OOP principles:

- **Encapsulation**: Each class manages its own data and behavior, exposing only what is necessary through public interfaces.
- **Inheritance**: Base classes define common functionality, which is extended by derived classes to create specialized behaviors.
- **Polymorphism**: Interfaces and abstract classes allow for flexible and interchangeable components, facilitating scalability and maintenance.

## Event Handling System

The tool incorporates a comprehensive event handling system to manage interactions within the level editor and the game environment. This system decouples event publishers and subscribers, promoting a modular and maintainable codebase.

- **EventManager.cs**: A singleton class that maintains a dictionary of events and their corresponding listeners, providing methods to subscribe, unsubscribe, and invoke events.
- **Example Usage**:
  ```csharp
  // Subscribing to an event
  EventManager.StartListening("OnLevelLoaded", OnLevelLoadedHandler);

  // Unsubscribing from an event
  EventManager.StopListening("OnLevelLoaded", OnLevelLoadedHandler);

  // Invoking an event
  EventManager.TriggerEvent("OnLevelLoaded");
  ```

This event-driven approach ensures that components remain loosely coupled, enhancing the flexibility and testability of the system.

## Prefab Management

Prefabs serve as reusable templates for game objects, ensuring consistency and efficiency in level design. The Level Builder provides tools to manage these prefabs effectively:

- **Prefab Library**: A centralized repository within the editor that lists all available prefabs, categorized for easy navigation.
- **Drag-and-Drop Functionality**: Allows designers to quickly place prefabs into the level by dragging them from the library into the scene.
- **Prefab Variants**: Supports the creation of prefab variants to accommodate slight modifications without altering the original template.

## Data Persistence

To support iterative development, the Level Builder includes robust data persistence mechanisms:

- **Saving Levels**: Serializes level data into JSON format, storing information about object positions, properties, and relationships.
- **Loading Levels**: Deserializes JSON data to reconstruct the level within the editor or at runtime, ensuring a seamless transition between editing and testing phases.

This approach allows developers to save their progress, share level data across teams, and maintain version control over level designs.

## Conclusion

The Level Builder was developed as a critical part of *SpaceRoads*, an Android game, to streamline the level design process and enhance game development efficiency. Its integration within the Unity editor provides a robust tool for creating, managing, and testing game environments while maintaining scalable and maintainable code. This tool demonstrates strong OOP design, event-driven architecture, and efficient prefab management, making it a valuable asset for any game development team.

For a detailed walkthrough and additional documentation, please refer to the [How the Level Builder Works](https://github.com/MrNiceGameMaker/Level-Builder/blob/main/How%20the%20level%20builder%20works.pdf) guide.

