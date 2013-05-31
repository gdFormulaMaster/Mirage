using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Mirage.Client.Core.Interface {
    public sealed class InterfaceManager {
        internal class RootComponent : Component {
            private readonly InterfaceManager manager;
            private Component focusedComponent;

            public RootComponent(InterfaceManager manager) : base() {
                if (manager == null)
                    throw new ArgumentNullException();

                this.manager = manager;
                //BackgroundColor = Color.Black;
            }

            protected override Component FocusedComponent() {
                return focusedComponent;
            }

            protected override void FocusComponent(Component component) {
                if (focusedComponent != null)
                    TriggerOnFocusLost(focusedComponent);

                focusedComponent = component;
                if (focusedComponent != null)
                    TriggerOnFocusGained(focusedComponent);
            }
        }

        private readonly Game game;
        private readonly GraphicsDeviceManager graphics;
        private readonly SpriteBatch batch;
        private readonly InputManager input;
        private readonly RootComponent rootComponent;

        public Game Game {
            get { return game; }
        }

        public ContentManager Content {
            get { return game.Content; }
        }

        public GraphicsDevice GraphicsDevice {
            get { return game.GraphicsDevice; }
        }

        public SpriteBatch Batch {
            get { return batch; }
        }

        public GraphicsDeviceManager Graphics {
            get { return graphics; }
        }

        public InputManager Input {
            get { return input; }
        }

        public InterfaceManager(Game game) {
            if (game == null)
                throw new ArgumentNullException("The InterfaceManager class must be created with a non-null Game reference.");

            this.game = game;
            this.graphics = new GraphicsDeviceManager(this.game);
            this.batch = new SpriteBatch(GraphicsDevice);
            this.input = new InputManager();
            this.rootComponent = new RootComponent(this);
            this.rootComponent.Bounds = new Rectangle(0, 0, this.graphics.PreferredBackBufferWidth, this.graphics.PreferredBackBufferHeight);
        }

        public void Add(Component component) {
            rootComponent.Add(component);
        }

        public void Remove(Component component) {
            rootComponent.Remove(component);
        }

        public bool Contains(Component component) {
            return rootComponent.Contains(component);
        }

        public void Update(GameTime time) {
            rootComponent.Update(this, time);
        }

        public void Draw(GameTime time) {
            rootComponent.Draw(this, time);
        }
    }
}