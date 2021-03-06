﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.IO;

namespace LevelGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D blockTexture1;
        Texture2D blockTexture2;

        Texture2D idleTexture;
        Texture2D runTexture;
        Texture2D jumpTexture;

        AnimatedSprite hero;

        public int Width;
        public int Height;

        List<Block> blocks;

        static int ScrollX;
        int levelLength;

        int currentLevel;
        KeyboardState oldState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Width = graphics.PreferredBackBufferWidth = 400;
            Height = graphics.PreferredBackBufferHeight = 400;
        }

        public bool CollidesWithLevel(Rectangle rect)
        {
            foreach (Block block in blocks)
            {
                if (block.rect.Intersects(rect))
                    return true;
            }
            return false;
        }

        public static Rectangle GetScreenRect(Rectangle rect)
        {
            Rectangle screenRect = rect;
            screenRect.Offset(-ScrollX, 0);

            return screenRect;
        }
        public void Scroll(int dx)
        {
            if (ScrollX + dx >= 0 && ScrollX + dx <= levelLength - 400)
                ScrollX += dx;
        }
        public void CreateLevel()
        {
            currentLevel++;
            if (currentLevel > 3)
                currentLevel = 1;
            blocks = new List<Block>();
            string[] s = File.ReadAllLines("content/levels/level" + currentLevel + ".txt");

            levelLength = 40 * s[0].Length;
            int x = 0;
            int y = 0;
            foreach (string str in s)
            {
                foreach (char c in str)
                {
                    Rectangle rect = new Rectangle(x, y, 40, 40);
                    if (c == 'X')
                    {
                        
                        Block block = new Block(rect, blockTexture1);
                        blocks.Add(block);
                    }
                    if (c == 'Y')
                    {
                        Block block = new Block(rect, blockTexture2);
                        blocks.Add(block);
                    }
                    x += 40;
                }
                x = 0;
                y += 40;
            }
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            blockTexture1 = Content.Load<Texture2D>("Textures/block");
            blockTexture2 = Content.Load<Texture2D>("Textures/block2");

            idleTexture = Content.Load<Texture2D>("Textures/idle");
            runTexture = Content.Load<Texture2D>("Textures/run");
            jumpTexture = Content.Load<Texture2D>("Textures/jump");

            Rectangle rect = new Rectangle(0, Height - idleTexture.Height - 40, 60, 60);
            hero = new AnimatedSprite(rect, idleTexture, runTexture, jumpTexture, this);

            CreateLevel();
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
                CreateLevel();

            if (keyState.IsKeyDown(Keys.Left))
                hero.StartRun(false);
            else if (keyState.IsKeyDown(Keys.Right))
                hero.StartRun(true);
            else hero.Stop();

            if (keyState.IsKeyDown(Keys.Up))
                hero.Jump();

            Rectangle heroScreenRect = GetScreenRect(hero.rect);

            if (heroScreenRect.Left < Width / 2)
                Scroll(-3*gameTime.ElapsedGameTime.Milliseconds/10);
            if (heroScreenRect.Left > Width / 2)
                Scroll(3 * gameTime.ElapsedGameTime.Milliseconds / 10);

            oldState = keyState;

            hero.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            foreach (Block block in blocks)
            {
                block.Draw(spriteBatch);
            }
            spriteBatch.End();

            hero.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
