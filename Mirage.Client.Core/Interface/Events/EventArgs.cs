using System;
using Microsoft.Xna.Framework.Input;

namespace Mirage.Client.Core.Interface.Events {
    [Serializable]
    public class EventArgs {
        public static readonly EventArgs Empty;

        public EventArgs(Component sender) {
            Sender = sender;
        }

        public Component Sender { get; set; }
    }

    public class ComponentEventArgs : EventArgs {
        public ComponentEventArgs(Component sender, Component component) : base(sender) {
            Component = component;
        }

        public Component Component { get; set; }
    }

    public class BoolStateEventArgs : EventArgs {
        public BoolStateEventArgs(Component sender, bool state) : base(sender) {
            State = state;
        }

        public bool State { get; set; }
    }

    public class MouseEventArgs : EventArgs {
        public MouseEventArgs(Component sender, int x, int y) : base(sender) {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }

    public class MouseButtonEventArgs : MouseEventArgs {
        public MouseButtonEventArgs(Component sender, int x, int y, MouseButton button) : base(sender, x, y) {
            Button = button;
        }

        public MouseButton Button { get; set; }
    }

    public class MouseScrollEventArgs : MouseEventArgs {
        public MouseScrollEventArgs(Component sender, int x, int y, int delta) : base(sender, x, y) {
            Delta = delta;
        }

        public int Delta { get; set; }
    }

    public class KeyEventArgs : EventArgs {
        public KeyEventArgs(Component sender, Keys key) : base(sender) {
            Key = key;
        }

        public Keys Key { get; set; }
    }
}