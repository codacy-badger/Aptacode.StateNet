### `StateNetwork`

The data structure containing each state, input and connection between them.

| Public methods | Return Type   | Description  |
| -------------- | ------------- | ----- |
| IsValid        | bool | Returns true if the Network has a start state |
| UpdateStateName       | bool      |   XXX |
| GetState       | State      |   Return the state with the given name |
| CreateState       | State      |   Create new state only if missing, return named state |
| RemoveState       | void      |   Remove the state which matches the given name and all of its connections |
| GetOrderedStates       | IEnumerable<State>      |   XXX |
| GetEndStates       | IEnumerable<State>      |   XXX |
| GetStates       | IEnumerable<State>      |   XXX |
| etc...       |       |    |

#### `IStateNetwork`

> Consider adding a description of the Interface... may be too verbose

### `StateNetworkEditor`

Allows a user to modify the StateNetwork by Adding / Removing States, Inputs and Connections

### `Input`

Applied to the StateNetEngine to cause a transition from the current state to a target state.

### `State`

A node in the network, each state has a list of output connections.

### `Connection`

A edge in the network. Each connection has a Source State, Input, Target State and Connection Weight.
When an input is applied to the StateNetEngine each connection whose source state and input matches the
Engines Current state and applied input will have their ConnectionWeight evaluated and collected in a ConnectionDistribution

### `ConnectionWeight`

An expression which searches the EngineHistory at runtime to determine the weight of a connection.
The expression can be a static weight such as '0' or '1' or a dynamic weight such as 'StateVisitCount("D2") >= 2 ? 1 : 0'

### `ConnectionDistribution`

A collection of Connections and their evaluated weights

### `ConnectionChooser`

Takes a ConnectionDistribution and randomly chooses a connection influenced by its weight.

### `StateNetEngine`

Traverses the StateNetwork at runtime based on the inputs the user apply

### `EngineHistory`

Records each transition for the StateNetwork