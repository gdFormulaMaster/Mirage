using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Mirage.Client.Core.Interface;

namespace Mirage.Client.Core.Demo {
    public class Demo : Game {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont testFont;
        string testString;
        int testCursor;
        InputManager input;
        InterfaceManager ui;

        public Demo() : base() {
            ui = new InterfaceManager(this);
            graphics = ui.Graphics;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = ui.Batch;

            testFont = Content.Load<SpriteFont>("testFont");
            testString = "";
            testCursor = 0;

            input = ui.Input;

            Component c = new Component();
            c.BackgroundColor = Color.Black;
            c.Bounds = new Rectangle(0, 0, 32, 32);
            ui.Add(c);
        }

        protected override void UnloadContent() {
            
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            ui.Update(gameTime);
            input.Update(gameTime);
            testString = input.GetTextInput(testString, 0, ref testCursor);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            ui.Batch.Begin();

            ui.Draw(gameTime);
            spriteBatch.DrawString(testFont, testString.Insert(Math.Max(testCursor, 0), "|"), Vector2.Zero, Color.White);

            spriteBatch.DrawString(testFont, "Left: " + input.MouseDown(MouseButton.Primary) + "\nRight: " + input.MouseDown(MouseButton.Secondary) + "\nCursor: " + input.Cursor, Vector2.UnitY * 16, Color.White);

            ui.Batch.End();
            base.Draw(gameTime);
        }
    }
}
