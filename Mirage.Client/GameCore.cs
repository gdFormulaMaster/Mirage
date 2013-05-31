using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.IO;
using System.Threading;

namespace Mirage.Client {
    public class GameCore : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public GameCore() : base() {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
#if DEBUG
            graphics.SynchronizeWithVerticalRetrace = false;

            IsFixedTimeStep = false;
#endif      
        }
        
        protected override void Initialize() {
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 576;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void UnloadContent() {
        }

        protected override void Update(GameTime gameTime) {
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}