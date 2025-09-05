# Utility AI System for Unity  

A simple modular **Utility AI framework** for Unity that allows AI agents to make dynamic, context-based decisions using considerations, actions, and sensors.  

## Features  

- **Considerations** : Define how different parameters (e.g., health, distance, stamina) are evaluated and scored.  
- **Actions** : Encapsulate the logic of what an AI can do.  
  - Example: *Move to target*, *Attack enemy*, *Drink potion*.  
- **Decisions** : Containers that combine multiple actions and considerations.  
  - Supports **sequenced actions** (e.g., *If the light is off → go to switch → turn it on → sit down*).  
- **Brain** : Core system that evaluates all decisions, selects the best one, and executes it.  
  - Includes **AI memory** with explicitly defined types and names for entries.  
- **Sensors** : Hearing and vision systems for detecting nearby objects, players, or other agents.  
- **Global World State** : Shared state across AI agents for communication (e.g., broadcasting player position).  
- ** Some Editor Tools** : to help with debugging and testing.  
## Use Cases  

- NPCs that adapt to changing environments.  
- AI agents that coordinate using a shared world state.  
- Complex decision-making with sequenced actions and memory-driven logic.  
