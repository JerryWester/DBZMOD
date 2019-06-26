﻿using DBZMOD.Enums;
using System;
using System.Collections.Generic;
using Terraria.GameInput;

namespace DBZMOD.Util
{
    /// <summary>
    ///     Class containing methods useful for manipulating and processing control and control state for various mod player functions.
    /// </summary>
    class ControlHelper
    {
        public const int BLOCK_PHASE_1_TICKS = 30;
        public const int BLOCK_PHASE_2_TICKS = 120;
        public const int BLOCK_PHASE_3_TICKS = 240;
        public const int LIGHT_ATTACK_HOLD_DURATION_TICKS = 60;
        public const int DASH_INPUT_WINDOW_TICKS = 15;
        public const int HELD_TIME_LIMIT_FOR_SINGLE_PRESS = 5;
        public const int FRAME_WINDOW_PADDING_FOR_DIAGONAL_DASH = 5;
        public const int DOUBLE_INPUT_COOLDOWN = 15;

        public static int GetInputTimeLimit(Controls control)
        {
            switch (control)
            {
                case Controls.Block:
                    return BLOCK_PHASE_3_TICKS;
                case Controls.LightAttack:
                    return LIGHT_ATTACK_HOLD_DURATION_TICKS;
                case Controls.HeavyAttack:
                    return 0;
                default:
                    return DASH_INPUT_WINDOW_TICKS;                  
            }
        }

        public static int GetHeldTimeLimit(Controls control)
        {
            return HELD_TIME_LIMIT_FOR_SINGLE_PRESS;
        }

        /// <summary>
        ///     Cache a list of the control types to avoid slow reflection.
        /// </summary>
        private static List<Controls> _controlTypes = new List<Controls>();
        public static List<Controls> ControlTypes {
            get
            {
                if (_controlTypes.Count == 0)
                {
                    _controlTypes = new List<Controls>();
                    foreach(Controls control in Enum.GetValues(typeof(Controls)))
                    {
                        _controlTypes.Add(control);
                    }
                }

                return  _controlTypes;
            }
        }


        /// <summary>
        ///     Dictionary property responsible for holding player control state.
        /// </summary>
        public static Dictionary<Controls, ControlStateMetadata> currentControlDictionary = null;
        public static Dictionary<Controls, ControlStateMetadata> CurrentControlState
        {
            get
            {
                return previousControlDictionary;
            }
            set
            {
                previousControlDictionary = value;
            }
        }

        /// <summary>
        ///     Dictionary property responsible for holding player's control state in the previous tick to compare with current.
        /// </summary>
        public static Dictionary<Controls, ControlStateMetadata> previousControlDictionary = null;
        public static Dictionary<Controls, ControlStateMetadata> PreviousControlState
        {
            get
            {
                return previousControlDictionary;
            }
            set
            {
                previousControlDictionary = value;
            }
        }

        public static List<Controls> GetPressedKeys(TriggersSet inputState)
        {
            List<Controls> pressedKeys = new List<Controls>();
            foreach (Controls control in ControlTypes)
            {
                // block is just the combination of two keys, skip it.
                if (control == Controls.Block)
                    continue;
                bool pressed = false;
                switch (control)
                {
                    case Controls.LightAttack:
                        pressed = inputState.MouseLeft;
                        break;
                    case Controls.HeavyAttack:
                        pressed = inputState.MouseRight;
                        break;
                    case Controls.Up:
                        pressed = inputState.Up;
                        break;
                    case Controls.Down:
                        pressed = inputState.Down;
                        break;
                    case Controls.Left:
                        pressed = inputState.Left;
                        break;
                    case Controls.Right:
                        pressed = inputState.Right;
                        break;
                }
                if (pressed)
                {
                    pressedKeys.Add(control);
                }
            }
            return pressedKeys;
        }

        public static List<Controls> GetReleasedKeys(TriggersSet inputState)
        {
            List<Controls> releasedKeys = new List<Controls>();
            foreach (Controls control in ControlTypes)
            {
                // block is just the combination of two keys, skip it.
                if (control == Controls.Block)
                    continue;
                bool pressed = false;
                switch (control)
                {
                    case Controls.LightAttack:
                        pressed = inputState.MouseLeft;
                        break;
                    case Controls.HeavyAttack:
                        pressed = inputState.MouseRight;
                        break;
                    case Controls.Up:
                        pressed = inputState.Up;
                        break;
                    case Controls.Down:
                        pressed = inputState.Down;
                        break;
                    case Controls.Left:
                        pressed = inputState.Left;
                        break;
                    case Controls.Right:
                        pressed = inputState.Right;
                        break;
                }
                if (!pressed)
                {
                    releasedKeys.Add(control);
                }
            }
            return releasedKeys;
        }

        public static ActionsToPerform ProcessInputs(TriggersSet inputState)
        {
            if (CurrentControlState == null)
            {
                CurrentControlState = SeedNewControlState();
            }

            // rotate the control state persistence, previous is now whatever Current Control State was in the previous frame (or empty if being initialized)
            PreviousControlState = CurrentControlState;

            List<Controls> pressedKeys = GetPressedKeys(inputState);
            foreach (Controls key in pressedKeys)
            {
                ProcessControlPress(key);
            }

            List<Controls> releasedKeys = GetReleasedKeys(inputState);
            foreach (Controls key in releasedKeys)
            {
                ProcessControlRelease(key);
            }

            var actionsToPerform = ProcessActionQueue();

            return actionsToPerform;
        }

        public static ActionsToPerform ProcessActionQueue()
        {
            var actionsToPerform = new ActionsToPerform();
            actionsToPerform.lightAttack = ShouldPerformLightAttack();
            actionsToPerform.heavyAttack = ShouldPerformHeavyAttack();
            actionsToPerform.blockPhase1 = ShouldPerformBlockPhase1();
            actionsToPerform.blockPhase2 = ShouldPerformBlockPhase2();
            actionsToPerform.blockPhase3 = ShouldPerformBlockPhase3();
            actionsToPerform.dashUpLeft = ShouldDashUpLeft();
            actionsToPerform.dashUpRight = ShouldDashUpRight();
            actionsToPerform.dashDownLeft = ShouldDashDownLeft();
            actionsToPerform.dashDownRight = ShouldDashDownRight();
            actionsToPerform.dashUp = ShouldDashUp();
            actionsToPerform.dashDown = ShouldDashDown();
            actionsToPerform.dashLeft = ShouldDashLeft();
            actionsToPerform.dashRight = ShouldDashRight();
            actionsToPerform.flurry = ShouldFlurry();
            return actionsToPerform;
        }

        public static bool ShouldPerformLightAttack()
        {
            // experiment with Press Once and allow it to fire instantly if this feels completely awful.
            return (CurrentControlState[Controls.LightAttack].state == ControlStates.PressedOnce
                    || CurrentControlState[Controls.LightAttack].state == ControlStates.PressedTwice)
                && CurrentControlState[Controls.HeavyAttack].state == ControlStates.Released;
        }

        public static bool ShouldFlurry()
        {
            return CurrentControlState[Controls.LightAttack].state == ControlStates.PressedAndHeld
                && CurrentControlState[Controls.LightAttack].inputTimer >= GetInputTimeLimit(Controls.LightAttack);
        }

        public static bool ShouldPerformHeavyAttack()
        {
            // experiment with Press Once and allow it to fire instantly if this feels completely awful, same as above.
            return CurrentControlState[Controls.HeavyAttack].state == ControlStates.PressedAndReleased
                && CurrentControlState[Controls.LightAttack].state == ControlStates.Released;
        }

        public static bool ShouldPerformBlockPhase1()
        {
            // capture the minimum theoretical held timer
            int actualBlockTimer = Math.Min(CurrentControlState[Controls.LightAttack].inputTimer, CurrentControlState[Controls.HeavyAttack].inputTimer);

            // experiment with Press and Hold states or come up with ways to resolve conflicts with Light and Heavy attacks if attacking first is an issue.
            return (CurrentControlState[Controls.HeavyAttack].state == ControlStates.PressedOnce
                    || CurrentControlState[Controls.HeavyAttack].state == ControlStates.PressedAndHeld)
                && (CurrentControlState[Controls.LightAttack].state == ControlStates.PressedOnce
                    || CurrentControlState[Controls.LightAttack].state == ControlStates.PressedAndHeld)
                && actualBlockTimer <= BLOCK_PHASE_1_TICKS;
        }

        public static bool ShouldPerformBlockPhase2()
        {
            // capture the minimum theoretical held timer
            int actualBlockTimer = Math.Min(CurrentControlState[Controls.LightAttack].inputTimer, CurrentControlState[Controls.HeavyAttack].inputTimer);

            // experiment with Press and Hold states or come up with ways to resolve conflicts with Light and Heavy attacks if attacking first is an issue.
            return (CurrentControlState[Controls.HeavyAttack].state == ControlStates.PressedOnce
                    || CurrentControlState[Controls.HeavyAttack].state == ControlStates.PressedAndHeld)
                && (CurrentControlState[Controls.LightAttack].state == ControlStates.PressedOnce
                    || CurrentControlState[Controls.LightAttack].state == ControlStates.PressedAndHeld)
                && actualBlockTimer > BLOCK_PHASE_1_TICKS && actualBlockTimer <= BLOCK_PHASE_2_TICKS;
        }

        public static bool ShouldPerformBlockPhase3()
        {
            // capture the minimum theoretical held timer
            int actualBlockTimer = Math.Min(CurrentControlState[Controls.LightAttack].inputTimer, CurrentControlState[Controls.HeavyAttack].inputTimer);

            // experiment with Press and Hold states or come up with ways to resolve conflicts with Light and Heavy attacks if attacking first is an issue.
            return (CurrentControlState[Controls.HeavyAttack].state == ControlStates.PressedOnce
                    || CurrentControlState[Controls.HeavyAttack].state == ControlStates.PressedAndHeld)
                && (CurrentControlState[Controls.LightAttack].state == ControlStates.PressedOnce
                    || CurrentControlState[Controls.LightAttack].state == ControlStates.PressedAndHeld)
                && actualBlockTimer > BLOCK_PHASE_2_TICKS && actualBlockTimer <= BLOCK_PHASE_3_TICKS;
        }

        public static bool DirectionsReleasedExcept(Controls control)
        {
            foreach(Controls typeOfControl in ControlTypes)
            {
                if (typeOfControl == Controls.LightAttack || typeOfControl == Controls.HeavyAttack || typeOfControl == Controls.LightAttack)
                    continue;
                if (typeOfControl == control)
                    continue;

                // if any directional control is NOT in a released state return false.
                if (CurrentControlState[typeOfControl].state != ControlStates.Released && CurrentControlState[typeOfControl].state != ControlStates.PressedAndReleased)
                    return false;
            }

            // no controls were found pressed but this one, presumably. return true
            return true;
        }

        public static bool IsReleased(Controls control)
        {
            return CurrentControlState[control].state == ControlStates.Released || CurrentControlState[control].state == ControlStates.PressedAndReleased;
        }

        public static bool IsPressed(Controls control)
        {
            return CurrentControlState[control].state == ControlStates.PressedOnce
                || (CurrentControlState[control].state == ControlStates.PressedAndHeld && CurrentControlState[control].heldTimer < GetHeldTimeLimit(control));
        }

        public static bool ShouldDashUp()
        {
            return CurrentControlState[Controls.Up].state == ControlStates.PressedTwice
                    && DirectionsReleasedExcept(Controls.Up);
        }

        public static bool ShouldDashDown()
        {
            return CurrentControlState[Controls.Down].state == ControlStates.PressedTwice
                    && DirectionsReleasedExcept(Controls.Down);
        }

        public static bool ShouldDashLeft()
        {
            return CurrentControlState[Controls.Left].state == ControlStates.PressedTwice
                    && DirectionsReleasedExcept(Controls.Left);
        }

        public static bool ShouldDashRight()
        {
            return CurrentControlState[Controls.Right].state == ControlStates.PressedTwice
                    && DirectionsReleasedExcept(Controls.Right);
        }

        public static bool ShouldDashUpLeft()
        {
            return CurrentControlState[Controls.Left].state == ControlStates.PressedTwice
                    && CurrentControlState[Controls.Up].state == ControlStates.PressedAndHeld;
        }

        public static bool ShouldDashUpRight()
        {
            return CurrentControlState[Controls.Right].state == ControlStates.PressedTwice
                    && CurrentControlState[Controls.Up].state == ControlStates.PressedAndHeld;
        }

        public static bool ShouldDashDownLeft()
        {
            return CurrentControlState[Controls.Left].state == ControlStates.PressedTwice
                    && CurrentControlState[Controls.Down].state == ControlStates.PressedAndHeld;
        }

        public static bool ShouldDashDownRight()
        {
            return CurrentControlState[Controls.Right].state == ControlStates.PressedTwice
                    && CurrentControlState[Controls.Down].state == ControlStates.PressedAndHeld;
        }

        public static void ProcessControlPress(Controls control)
        {
            ProcessControl(control, true);
        }

        public static void ProcessControlRelease(Controls control)
        {
            ProcessControl(control, false);
        }

        public static void ProcessControl(Controls control, bool isPressed)
        {
            var previousControlState = GetPreviousControlState(control);
            var metadata = SeedMetadataBasedOnPreviousState(control, previousControlState, isPressed);
            CurrentControlState[control] = metadata;
        }

        public static ControlStateMetadata SeedMetadataBasedOnPreviousState(Controls control, ControlStateMetadata previousControlState, bool isPressed)
        {
            ControlStates currentState = ControlStates.Released;
            int previousInputTimer = previousControlState.inputTimer;
            int previousHeldTimer = previousControlState.heldTimer;
            int previousDoubleInputCooldown = previousControlState.doubleInputCooldown;
            int doubleInputCooldown = Math.Max(previousDoubleInputCooldown - 1, 0);
            int inputTimer = 0;
            int heldTimer = 0;
            if (isPressed)
            {
                switch (previousControlState.state)
                {
                    case ControlStates.Released:
                        currentState = ControlStates.PressedOnce;
                        break;
                    case ControlStates.PressedTwice:
                        // prevents you from chaining dashes on third/subsequent presses
                        currentState = ControlStates.Released;
                        break;
                    case ControlStates.PressedOnce:
                    case ControlStates.PressedAndHeld:
                        currentState = ControlStates.PressedAndHeld;
                        break;
                    case ControlStates.PressedAndReleased:
                        if (previousInputTimer <= GetInputTimeLimit(control) && doubleInputCooldown == 0)
                        {
                            if (control != Controls.LightAttack)
                                doubleInputCooldown = DOUBLE_INPUT_COOLDOWN;
                            currentState = ControlStates.PressedTwice;
                        } else
                        {
                            currentState = ControlStates.PressedOnce;
                        }
                        break;
                }
            }
            else
            {
                switch (previousControlState.state)
                {
                    case ControlStates.Released:                        
                    case ControlStates.PressedTwice:                        
                        break;
                    case ControlStates.PressedOnce:
                    case ControlStates.PressedAndReleased:
                    case ControlStates.PressedAndHeld:
                        if (previousInputTimer <= GetInputTimeLimit(control))
                        {
                            // light attack must be held for flurries.
                            if (control == Controls.LightAttack)
                            {
                                inputTimer = 0;
                            }
                            if (previousControlState.state == ControlStates.PressedAndHeld && previousHeldTimer <= GetHeldTimeLimit(control))
                            {
                                currentState = ControlStates.PressedAndReleased;
                            }
                            else
                            {
                                if ((previousControlState.state == ControlStates.PressedOnce || previousControlState.state == ControlStates.PressedAndReleased)
                                    && previousInputTimer <= GetInputTimeLimit(control))
                                {
                                    currentState = ControlStates.PressedAndReleased;
                                }
                                else
                                {
                                    currentState = ControlStates.Released;
                                }                                
                            }
                        }
                        else
                        {
                            inputTimer = 0;
                            currentState = ControlStates.Released;
                        }
                        break;
                }
            }

            // if we're in a combo or held state, the input timer gets incremented.
            if (currentState == ControlStates.PressedOnce || currentState == ControlStates.PressedAndHeld || currentState == ControlStates.PressedAndReleased)
            {
                inputTimer = previousInputTimer + 1;
                if (currentState == ControlStates.PressedAndHeld)
                {
                    heldTimer = previousHeldTimer + 1;
                } else
                {
                    heldTimer = 0;
                }
            } else
            {
                inputTimer = 0;
            }

            var result = new ControlStateMetadata(currentState, inputTimer, heldTimer, doubleInputCooldown);
            return result;
        }

        public static Dictionary<Controls, ControlStateMetadata> SeedNewControlState()
        {
            var result = new Dictionary<Controls, ControlStateMetadata>();
            foreach(Controls control in ControlTypes)
            {
                result.Add(control, new ControlStateMetadata());
            }

            return result;
        }

        public static ControlStateMetadata GetPreviousControlState(Controls control)
        {
            return PreviousControlState[control];
        }

        public static void SetCurrentControlState(Controls control, ControlStates state, int inputTimer, int heldTimer, int doubleInputCooldown)
        {
            SetCurrentControlState(control, new ControlStateMetadata(state, inputTimer, heldTimer, doubleInputCooldown));
        }

        public static void SetCurrentControlState(Controls control, ControlStateMetadata metaData)
        {
            var overwriteResult = new KeyValuePair<Controls, ControlStateMetadata>(control, metaData);
            CurrentControlState[control] = metaData;
        }
    }

    /// <summary>
    ///     class of the util that handles tracking the metadata of a particular control, its state, how long it's held/been since press/release.
    /// </summary>
    class ControlStateMetadata
    {
        // enum declaring the state of a given control at any point in time.
        public ControlStates state;

        // tracks the input delay before Pressed/PressedAndReleased reset.
        public int inputTimer;

        // tracks the input length of a hold before it's considered too long to be just a normal press (and thus disqualified from things like dashing)
        public int heldTimer;

        // the amount of time after firing a double-press sequence to delay before accepting another one. Prevents chaining zanzokens.
        public int doubleInputCooldown;

        // default constructor for a control is a neutral/released state.
        public ControlStateMetadata()
        {
            this.state = ControlStates.Released;
            this.inputTimer = 0;
            this.heldTimer = 0;
            this.doubleInputCooldown = 0;
        }

        // default constructor for a control is a neutral/released state.
        public ControlStateMetadata(ControlStates initState, int initInputTimer, int initHeldTimer, int doubleInputCooldown)
        {
            this.state = initState;
            this.inputTimer = initInputTimer;
            this.heldTimer = initHeldTimer;
            this.doubleInputCooldown = doubleInputCooldown;
        }
    }

    /// <summary>
    ///     A class representing all the actions a player can take with the Ki Fist equipped, to be processed simultaneously.
    /// </summary>
    class ActionsToPerform
    {
        public bool lightAttack;
        public bool heavyAttack;
        public bool blockPhase1;
        public bool blockPhase2;
        public bool blockPhase3;
        public bool dashUp;
        public bool dashDown;
        public bool dashLeft;
        public bool dashRight;
        public bool dashUpLeft;
        public bool dashUpRight;
        public bool dashDownLeft;
        public bool dashDownRight;
        public bool flurry;
    }
}
