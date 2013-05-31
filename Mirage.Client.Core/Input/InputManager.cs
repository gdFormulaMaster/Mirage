using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Mirage.Client.Core {
    /// <summary>
    /// Sealed class structure for the InputState
    /// </summary>
    public sealed class InputState {
        private KeyboardState keyboard;
        private MouseState mouse;

        /// <summary>
        /// The Keyboard state
        /// </summary>
        public KeyboardState Keyboard { get { return keyboard; } }
        
        /// <summary>
        /// The Mouse state
        /// </summary>
        public MouseState Mouse { get { return mouse; } }

        /// <summary>
        /// Private constructor for the InputState.
        /// </summary>
        /// <param name="keyboard">Keyboard state</param>
        /// <param name="mouse">Mouse state</param>
        private InputState(KeyboardState keyboard, MouseState mouse) {
            this.keyboard = keyboard;
            this.mouse = mouse;
        }

        /// <summary>
        /// Creates an instance of the current input state.
        /// </summary>
        /// <returns>An instance of the current input state</returns>
        public static InputState GetCurrentState() {
            return new InputState(Microsoft.Xna.Framework.Input.Keyboard.GetState(), Microsoft.Xna.Framework.Input.Mouse.GetState());
        }
    }

    /// <summary>
    /// Mouse Button Enumeration
    /// </summary>
    public enum MouseButton {
        Primary,
        Secondary,
        Middle,
        XButton1,
        XButton2,
    }

    /// <summary>
    /// Handles and manages input from MonoGame.
    /// </summary>
    public sealed class InputManager {
        private InputState previousInput;
        private InputState currentInput;
        private TimeSpan currentTime;
        private Dictionary<Keys, TimeSpan> nextTextInput;
        private bool capsLock;

        /// <summary>
        /// Flag for whether a Shift key is pressed.
        /// </summary>
        public bool Shift { get { return KeyDown(Keys.LeftShift) || KeyDown(Keys.RightShift); } }
        
        /// <summary>
        /// Flag for whether an Alt key is pressed.
        /// </summary>
        public bool Alt { get { return KeyDown(Keys.LeftAlt) || KeyDown(Keys.RightAlt); } }

        /// <summary>
        /// Flag for whether a Control key is pressed.
        /// </summary>
        public bool Control { get { return KeyDown(Keys.LeftControl) || KeyDown(Keys.RightControl); } }

        /// <summary>
        /// Flag for Caps Lock status. Operating system independent, when the program starts caps lock is assumed to be off.
        /// </summary>
        public bool CapsLock { get { return capsLock; } }

        /// <summary>
        /// Switches the primary and secondary (left and right) mouse buttons.
        /// </summary>
        /// <remarks>
        /// The operating system or MonoGame (at least on Windows) already switches the two if the buttons
        /// are switched through the OS mouse settings, and if LeftyMode is on, it will switch them again.
        /// </remarks>
        public bool LeftyMode { get; set; }

        /// <summary>
        /// Controls whether the user can hold down a key to repeat strokes, or whether they must manually tap the key.
        /// </summary>
        public bool EnableKeyRepeat { get; set; }

        /// <summary>
        /// The interval between each key repeat.
        /// </summary>
        public TimeSpan RepeatInterval { get; set; }

        /// <summary>
        /// The delay before the key repeat feature starts taking input.
        /// </summary>
        public TimeSpan RepeatDelay { get; set; }

        /// <summary>
        /// The current location of the mouse cursor.
        /// </summary>
        public Point Cursor { get { return new Point(currentInput.Mouse.X, currentInput.Mouse.Y); } }

        /// <summary>
        /// Constructs a basic instance of the InputManager.
        /// Key repeat and lefty mode are both OFF.
        /// </summary>
        public InputManager()
            : this(false, false) {
        }

        /// <summary>
        /// Constructs an instance of the InputManager with the specified option(s).
        /// </summary>
        /// <param name="keyRepeat">Whether key repeat should be enabled</param>
        public InputManager(bool keyRepeat)
            : this(keyRepeat, false) {
        }

        /// <summary>
        /// Constructs an instance of the InputManager with the specified option(s).
        /// </summary>
        /// <param name="keyRepeat">Whether key repeat should be enabled</param>
        /// <param name="lefty">Whether lefty mode should be enabled</param>
        public InputManager(bool keyRepeat, bool lefty)
            : this(keyRepeat, new TimeSpan(84 * TimeSpan.TicksPerMillisecond), lefty) {
        }

        /// <summary>
        /// Constructs an instance of the InputManager with the specified option(s).
        /// </summary>
        /// <param name="keyRepeat">Whether key repeat should be enabled</param>
        /// <param name="keyRepeatInterval">The time interval between key repeats</param>
        public InputManager(bool keyRepeat, TimeSpan keyRepeatInterval)
            : this(keyRepeat, keyRepeatInterval, false) {
        }

        /// <summary>
        /// Constructs an instance of the InputManager with the specified option(s).
        /// </summary>
        /// <param name="keyRepeat">Whether key repeat should be enabled</param>
        /// <param name="keyRepeatInterval">The time interval between key repeats</param>
        /// <param name="lefty">Whether lefty mode should be enabled</param>
        public InputManager(bool keyRepeat, TimeSpan keyRepeatInterval, bool lefty) {
            currentInput = previousInput = InputState.GetCurrentState();
            nextTextInput = new Dictionary<Keys, TimeSpan>();
            EnableKeyRepeat = keyRepeat;
            RepeatInterval = keyRepeatInterval;
            RepeatDelay = new TimeSpan(500 * TimeSpan.TicksPerMillisecond);
            LeftyMode = lefty;
        }

        /// <summary>
        /// Updates the state of the InputManager.
        /// </summary>
        /// <param name="time">The time of this update</param>
        public void Update(GameTime time) {
            previousInput = currentInput;
            currentInput = InputState.GetCurrentState();
            currentTime = time.TotalGameTime;

            if (EnableKeyRepeat)
                foreach (Keys key in previousInput.Keyboard.GetPressedKeys())
                    if (KeyReleased(key))
                        nextTextInput.Remove(key);

            if (KeyReleased(Keys.CapsLock))
                capsLock = !capsLock;
        }

        /// <summary>
        /// Gets the input with the option to specify the current input and the length limit of the return string.
        /// </summary>
        /// <param name="current">The current input</param>
        /// <param name="lengthLimit">The length limit of the text input</param>
        /// <returns>The processed text input</returns>
        public string GetTextInput(string current = "", int lengthLimit = 0) {
            int position = -1;
            return GetTextInput(current, lengthLimit, ref position);
        }

        /// <summary>
        /// Gets the input with the specified current input, length limit and a reference to the cursor position.
        /// </summary>
        /// <param name="current">The current input</param>
        /// <param name="lengthLimit">The length limit of the text input</param>
        /// <param name="currentPosition">The current cursor position</param>
        /// <returns>The processed text input</returns>
        public string GetTextInput(string current, int lengthLimit, ref int currentPosition) {
            bool shifted = capsLock ? !(KeyDown(Keys.LeftShift) || KeyDown(Keys.RightShift)) : (KeyDown(Keys.LeftShift) || KeyDown(Keys.RightShift));
            string input = (current == null) ? "" : current;

            foreach (Keys key in currentInput.Keyboard.GetPressedKeys()) {
                bool doProcessKey = false;
                if (EnableKeyRepeat) {
                    doProcessKey = (!nextTextInput.ContainsKey(key) || (nextTextInput[key] - currentTime) <= TimeSpan.Zero);
                    if (doProcessKey && nextTextInput.ContainsKey(key))
                        nextTextInput[key] = currentTime + RepeatInterval;
                    else if (doProcessKey)
                        nextTextInput[key] = currentTime + RepeatDelay;
                } else {
                    doProcessKey = KeyPressed(key);
                }

                if (doProcessKey) {
                    char c = TranslateKey(key, shifted);
                    if (c != '\0' && (lengthLimit < 1 || input.Length < lengthLimit)) {
                        input = input.Insert(Math.Min(Math.Max(0, currentPosition >= 0 ? currentPosition : input.Length), input.Length), c.ToString());
                        currentPosition++;
                    } else {
                        switch (key) {
                            case Keys.Delete:
                                input = input.Remove(currentPosition, 1);
                                break;
                            case Keys.Back:
                                if (input.Length > 0 && input.Length >= currentPosition) {
                                    if (currentPosition > 0) {
                                        input = input.Remove(currentPosition - 1, 1);
                                        currentPosition--;
                                    } else if (currentPosition == -1) {
                                        input = input.Substring(0, input.Length - 1);
                                    }
                                }
                                break;
                            case Keys.Left:
                                if (currentPosition > 0)
                                    currentPosition--;
                                break;
                            case Keys.Right:
                                if (currentPosition >= 0 && currentPosition < input.Length)
                                    currentPosition++;
                                break;
                            case Keys.Home:
                                if (currentPosition > 0)
                                    currentPosition = 0;
                                break;
                            case Keys.End:
                                if (currentPosition >= 0)
                                    currentPosition = input.Length;
                                break;
                        }
                    }
                }
            }

            if (input != null && input.Length > lengthLimit && lengthLimit > 0) {
                input = input.Substring(0, lengthLimit);
            } else if (input == null) {
                input = "";
            }

            return input;
        }

        /// <summary>
        /// Determines whether the specified key is being held down
        /// </summary>
        /// <param name="key">The key to get the state of</param>
        /// <returns>Key held flag</returns>
        public bool KeyHeld(Keys key) {
            return currentInput.Keyboard.IsKeyDown(key) && previousInput.Keyboard.IsKeyDown(key);
        }

        /// <summary>
        /// Determines whether the specified key was just pressed
        /// </summary>
        /// <param name="key">The key to get the state of</param>
        /// <returns>Key pressed flag</returns>
        public bool KeyPressed(Keys key) {
            return currentInput.Keyboard.IsKeyDown(key) && previousInput.Keyboard.IsKeyUp(key);
        }

        /// <summary>
        /// Determines whether the specified key was just released
        /// </summary>
        /// <param name="key">The key to get the state of</param>
        /// <returns>Key released flag</returns>
        public bool KeyReleased(Keys key) {
            return currentInput.Keyboard.IsKeyUp(key) && previousInput.Keyboard.IsKeyDown(key);
        }

        /// <summary>
        /// Determines whether the specified key is down
        /// </summary>
        /// <param name="key">The key to get the state of</param>
        /// <returns>Key down flag</returns>
        public bool KeyDown(Keys key) {
            return currentInput.Keyboard.IsKeyDown(key);
        }

        /// <summary>
        /// Determines whether the specified key is up
        /// </summary>
        /// <param name="key">The key to get the state of</param>
        /// <returns>Key up flag</returns>
        public bool KeyUp(Keys key) {
            return currentInput.Keyboard.IsKeyUp(key);
        }

        /// <summary>
        /// Determines whether the mouse was just moved
        /// </summary>
        /// <returns>Movement flag</returns>
        public bool MouseMoved() {
            return (previousInput.Mouse.X != currentInput.Mouse.X) || (previousInput.Mouse.Y != currentInput.Mouse.Y);
        }

        /// <summary>
        /// Determines whether the mouse was just scrolled
        /// </summary>
        /// <returns>Scroll flag</returns>
        public bool MouseScrolled() {
            return (previousInput.Mouse.ScrollWheelValue != currentInput.Mouse.ScrollWheelValue);
        }

        /// <summary>
        /// Gets the current mouse scroll position
        /// </summary>
        /// <returns>Mouse scroll position</returns>
        public int MouseScrollPosition() {
            return currentInput.Mouse.ScrollWheelValue;
        }

        /// <summary>
        /// Gets the current mouse scroll delta
        /// </summary>
        /// <returns>Mouse scroll delta</returns>
        public int MouseScrollDelta() {
            return (currentInput.Mouse.ScrollWheelValue - previousInput.Mouse.ScrollWheelValue);
        }

        /// <summary>
        /// Determines whether the specified mouse button is being held down
        /// </summary>
        /// <param name="button">The button to get the state of</param>
        /// <returns>Button held flag</returns>
        public bool MouseHeld(MouseButton button) {
            switch (button) {
                case MouseButton.Primary:
                    return ((LeftyMode ? currentInput.Mouse.RightButton : currentInput.Mouse.LeftButton) == ButtonState.Pressed) && ((LeftyMode ? previousInput.Mouse.RightButton : previousInput.Mouse.LeftButton) == ButtonState.Pressed);
                case MouseButton.Secondary:
                    return ((LeftyMode ? currentInput.Mouse.LeftButton : currentInput.Mouse.RightButton) == ButtonState.Pressed) && ((LeftyMode ? previousInput.Mouse.LeftButton : previousInput.Mouse.RightButton) == ButtonState.Pressed);
                case MouseButton.Middle:
                    return (currentInput.Mouse.MiddleButton == ButtonState.Pressed) && (previousInput.Mouse.MiddleButton == ButtonState.Pressed);
                case MouseButton.XButton1:
                    return (currentInput.Mouse.XButton1 == ButtonState.Pressed) && (previousInput.Mouse.XButton1 == ButtonState.Pressed);
                case MouseButton.XButton2:
                    return (currentInput.Mouse.XButton2 == ButtonState.Pressed) && (previousInput.Mouse.XButton2 == ButtonState.Pressed);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified mouse button was just pressed
        /// </summary>
        /// <param name="button">The button to get the state of</param>
        /// <returns>Button pressed flag</returns>
        public bool MousePressed(MouseButton button) {
            switch (button) {
                case MouseButton.Primary:
                    return ((LeftyMode ? currentInput.Mouse.RightButton : currentInput.Mouse.LeftButton) == ButtonState.Pressed) && ((LeftyMode ? previousInput.Mouse.RightButton : previousInput.Mouse.LeftButton) == ButtonState.Released);
                case MouseButton.Secondary:
                    return ((LeftyMode ? currentInput.Mouse.LeftButton : currentInput.Mouse.RightButton) == ButtonState.Pressed) && ((LeftyMode ? previousInput.Mouse.LeftButton : previousInput.Mouse.RightButton) == ButtonState.Released);
                case MouseButton.Middle:
                    return (currentInput.Mouse.MiddleButton == ButtonState.Pressed) && (previousInput.Mouse.MiddleButton == ButtonState.Released);
                case MouseButton.XButton1:
                    return (currentInput.Mouse.XButton1 == ButtonState.Pressed) && (previousInput.Mouse.XButton1 == ButtonState.Released);
                case MouseButton.XButton2:
                    return (currentInput.Mouse.XButton2 == ButtonState.Pressed) && (previousInput.Mouse.XButton2 == ButtonState.Released);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified mouse button was just released
        /// </summary>
        /// <param name="button">The button to get the state of</param>
        /// <returns>Button released flag</returns>
        public bool MouseReleased(MouseButton button) {
            switch (button) {
                case MouseButton.Primary:
                    return ((LeftyMode ? currentInput.Mouse.RightButton : currentInput.Mouse.LeftButton) == ButtonState.Released) && ((LeftyMode ? previousInput.Mouse.RightButton : previousInput.Mouse.LeftButton) == ButtonState.Pressed);
                case MouseButton.Secondary:
                    return ((LeftyMode ? currentInput.Mouse.LeftButton : currentInput.Mouse.RightButton) == ButtonState.Released) && ((LeftyMode ? previousInput.Mouse.LeftButton : previousInput.Mouse.RightButton) == ButtonState.Pressed);
                case MouseButton.Middle:
                    return (currentInput.Mouse.MiddleButton == ButtonState.Released) && (previousInput.Mouse.MiddleButton == ButtonState.Pressed);
                case MouseButton.XButton1:
                    return (currentInput.Mouse.XButton1 == ButtonState.Released) && (previousInput.Mouse.XButton1 == ButtonState.Pressed);
                case MouseButton.XButton2:
                    return (currentInput.Mouse.XButton2 == ButtonState.Released) && (previousInput.Mouse.XButton2 == ButtonState.Pressed);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified mouse button is down
        /// </summary>
        /// <param name="button">The button to get the state of</param>
        /// <returns>Button down flag</returns>
        public bool MouseDown(MouseButton button) {
            switch (button) {
                case MouseButton.Primary:
                    return (LeftyMode ? currentInput.Mouse.RightButton : currentInput.Mouse.LeftButton) == ButtonState.Pressed;
                case MouseButton.Secondary:
                    return (LeftyMode ? currentInput.Mouse.LeftButton : currentInput.Mouse.RightButton) == ButtonState.Pressed;
                case MouseButton.Middle:
                    return currentInput.Mouse.MiddleButton == ButtonState.Pressed;
                case MouseButton.XButton1:
                    return currentInput.Mouse.XButton1 == ButtonState.Pressed;
                case MouseButton.XButton2:
                    return currentInput.Mouse.XButton2 == ButtonState.Pressed;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified mouse button is up
        /// </summary>
        /// <param name="button">The button to get the state of</param>
        /// <returns>Button up flag</returns>
        public bool MouseUp(MouseButton button) {
            switch (button) {
                case MouseButton.Primary:
                    return (LeftyMode ? currentInput.Mouse.RightButton : currentInput.Mouse.LeftButton) == ButtonState.Released;
                case MouseButton.Secondary:
                    return (LeftyMode ? currentInput.Mouse.LeftButton : currentInput.Mouse.RightButton) == ButtonState.Released;
                case MouseButton.Middle:
                    return currentInput.Mouse.MiddleButton == ButtonState.Released;
                case MouseButton.XButton1:
                    return currentInput.Mouse.XButton1 == ButtonState.Released;
                case MouseButton.XButton2:
                    return currentInput.Mouse.XButton2 == ButtonState.Released;
            }

            return false;
        }

        /// <summary>
        /// Translates XNA Keys to a char based on specified modifiers.
        /// </summary>
        /// <param name="key">The XNA Key to be translated.</param>
        /// <param name="shiftModifier">True for upper case, false for lower case.</param>
        /// <returns></returns>
        public static char TranslateKey(Keys key, bool shiftModifier) {
            switch (key) {
                case Keys.Q: return shiftModifier ? 'Q' : 'q';
                case Keys.W: return shiftModifier ? 'W' : 'w';
                case Keys.E: return shiftModifier ? 'E' : 'e';
                case Keys.R: return shiftModifier ? 'R' : 'r';
                case Keys.T: return shiftModifier ? 'T' : 't';
                case Keys.Y: return shiftModifier ? 'Y' : 'y';
                case Keys.U: return shiftModifier ? 'U' : 'u';
                case Keys.I: return shiftModifier ? 'I' : 'i';
                case Keys.O: return shiftModifier ? 'O' : 'o';
                case Keys.P: return shiftModifier ? 'P' : 'p';
                case Keys.A: return shiftModifier ? 'A' : 'a';
                case Keys.S: return shiftModifier ? 'S' : 's';
                case Keys.D: return shiftModifier ? 'D' : 'd';
                case Keys.F: return shiftModifier ? 'F' : 'f';
                case Keys.G: return shiftModifier ? 'G' : 'g';
                case Keys.H: return shiftModifier ? 'H' : 'h';
                case Keys.J: return shiftModifier ? 'J' : 'j';
                case Keys.K: return shiftModifier ? 'K' : 'k';
                case Keys.L: return shiftModifier ? 'L' : 'l';
                case Keys.Z: return shiftModifier ? 'Z' : 'z';
                case Keys.X: return shiftModifier ? 'X' : 'x';
                case Keys.C: return shiftModifier ? 'C' : 'c';
                case Keys.V: return shiftModifier ? 'V' : 'v';
                case Keys.B: return shiftModifier ? 'B' : 'b';
                case Keys.N: return shiftModifier ? 'N' : 'n';
                case Keys.M: return shiftModifier ? 'M' : 'm';
                case Keys.D1: return shiftModifier ? '!' : '1';
                case Keys.D2: return shiftModifier ? '@' : '2';
                case Keys.D3: return shiftModifier ? '#' : '3';
                case Keys.D4: return shiftModifier ? '$' : '4';
                case Keys.D5: return shiftModifier ? '%' : '5';
                case Keys.D6: return shiftModifier ? '^' : '6';
                case Keys.D7: return shiftModifier ? '&' : '7';
                case Keys.D8: return shiftModifier ? '*' : '8';
                case Keys.D9: return shiftModifier ? '(' : '9';
                case Keys.D0: return shiftModifier ? ')' : '0';
                case Keys.OemTilde: return shiftModifier ? '~' : '`';
                case Keys.OemMinus: return shiftModifier ? '_' : '-';
                case Keys.OemPlus: return shiftModifier ? '+' : '=';
                case Keys.OemOpenBrackets: return shiftModifier ? '{' : '[';
                case Keys.OemCloseBrackets: return shiftModifier ? '}' : ']';
                case Keys.OemBackslash: return shiftModifier ? '|' : '\\';
                case Keys.OemSemicolon: return shiftModifier ? ':' : ';';
                case Keys.OemQuotes: return shiftModifier ? '"' : '\'';
                case Keys.OemComma: return shiftModifier ? '<' : ',';
                case Keys.OemPeriod: return shiftModifier ? '>' : '.';
                case Keys.OemQuestion: return shiftModifier ? '?' : '/';
                case Keys.Subtract: return '-';
                case Keys.Add: return '+';
                case Keys.Multiply: return '*';
                case Keys.Divide: return '/';
                case Keys.Decimal: return '.';
                case Keys.NumPad0: return '0';
                case Keys.NumPad1: return '1';
                case Keys.NumPad2: return '2';
                case Keys.NumPad3: return '3';
                case Keys.NumPad4: return '4';
                case Keys.NumPad5: return '5';
                case Keys.NumPad6: return '6';
                case Keys.NumPad7: return '7';
                case Keys.NumPad8: return '8';
                case Keys.NumPad9: return '9';
                case Keys.Space: return ' ';
            }

            return '\0';
        }
    }
}