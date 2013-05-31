using Microsoft.Xna.Framework.Graphics;
using System;
using EventArgs = Mirage.Client.Core.Interface.Events.EventArgs;

namespace Mirage.Client.Core.Interface.Controls {
    public enum Alignment {
        TopLeft,
        TopCenter,
        TopRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        BottomLeft,
        BottomCenter,
        BottomRight,

        Left = MiddleLeft,
        Center = MiddleCenter,
        Right = MiddleRight,
    }

    [Serializable]
    public delegate void TextEventHandler(TextEventArgs args);
    public class TextEventArgs : EventArgs {
        public TextEventArgs(Component sender) : base(sender) {

        }
    }

    public class TextComponent : Component {
        private string text;
        private SpriteFont font;
        private Alignment alignment;

        public string Text {
            get { return text; }
            set {
                text = value;
                TriggerOnTextChangedEvent(this);
            }
        }

        public SpriteFont Font {
            get { return font; }
            set {
                font = value;
            }
        }

        public Alignment Alignment {
            get { return alignment; }
            set {
                alignment = value;
            }
        }

        public TextComponent(string text = "", SpriteFont font = null) : base() {
            
        }

        public event TextEventHandler OnTextChangedEvent;
        internal protected virtual void TriggerOnTextChangedEvent(Component sender) {
            if (OnTextChangedEvent != null)
                OnTextChangedEvent.Invoke(new TextEventArgs(sender));
        }
    }
}