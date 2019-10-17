# AptacodeStateNet

## A small .Net Standard library used to model simple Finite State Machines

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/bbdf96f5e1304d679e6addf01b2618a1)](https://www.codacy.com/manual/Timmoth/AptacodeStateNet?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Timmoth/AptacodeStateNet&amp;utm_campaign=Badge_Grade)

### Overview

The state machine is configured using two generic type parameters: 'States' & 'Actions' both are user defined enums.


-  Each possible state that the machine can be in at a given time is defined in the 'States' enum.

-  All of the actions that can an be applied to the state machine to induce a transitions between states are defined in the 'Actions' enum.

The state machine takes its initial state as a constructor parameter.

Once initialised all possible transitions between states must be defined.
A transition is triggered based on which state the machine is currently in and the action which was applied.
Depending on the type of transition the user can define their own logic and result for the transition which is used to
determine which state the state machine will enter.

There are three types of transition (*Note the user can define their own.):

-  InvalidTransition when an action cannot be applied to a state.

-  UnaryTransition when an action being applied to a state results the state machine moving to exactly one state.

-  BinaryTransition when an action being applied to a state can cause the state machine to move to multiple states depending 

on some user defined criteria.

### Written Example

A state machine of a video playback application is in 'States.Playing'.

There is a BinaryTransition defined for 'States.Playing' with a trigger of 'Actions.Pause' and destinations: 'States.Paused' & 'States.Stopped'. 

The user defined a function which returns either BinaryChoice.Left or BinaryChoice.Right depending on if the application could pause the video.

when a user presses the pause button 'Actions.Pause' will be applied. 

Since the current state is 'States.Playing' the above transition will be applied, if the application could not pause, the funciton returns BinaryChoice.Right hence moving into 'States.Stopped'.

### Usage

```csharp
//Define all possible states and actions
public enum States { NotReady, Ready, Running, Paused };
public enum Actions { Setup, Start, Pause, Resume, Stop };

//Create an instance set to an initial state
StateMachine stateMachine = new StateMachine<States, Actions>(States.NotReady);

//Define all possible transitions

//Invalid Transition
stateMachine.Define(
      new InvalidTransition<States, Actions>(
            States.NotReady, 
            Actions.Start, 
            "Must be Ready to Start"));
            
//Unary Transition
stateMachine.Define(
      new UnaryTransition<States, Actions>(
            States.NotReady, 
            Actions.Setup, 
            States.Ready,
            new Func<UnaryTransitionAcceptanceResult>(() => {
                  return new UnaryTransitionAcceptanceResult("Setup successful");
            }),
            "Setup"));

//Binary Transition
stateMachine.Define(
      new BinaryTransition<States, Actions>(
            States.Running, 
            Actions.Pause, 
            States.Paused, 
            States.NotReady, 
            new Func<BinaryTransitionAcceptanceResult>(() => {
                  if(canPause)
                        return new BinaryTransitionAcceptanceResult(BinaryChoice.Left, "Pause successful");
                  else
                        return new BinaryTransitionAcceptanceResult(BinaryChoice.Right, "Could not Pause");
            }),
            "Pause"));

//When a transition is applied
stateMachine.OnTransition += (s, e) => 
{ 
      status = string.Format("Old State: {0} Acton: {1} New State: {2}", e.OldState, e.Action, e.NewState);
};


//Apply actions to cause a transition
stateMachine.Apply(Actions.Setup);
stateMachine.Apply(Actions.Start);


```

## License

MIT License
