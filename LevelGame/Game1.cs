using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace LevelGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        Texture2D bg_image;

        //private Point bgFrameSize = new Point(400, 200);
        //private Point currPosition = new Point(0, 0);
        //private Point bgSheetSize = new Point(2, 1);
        //private int bgMillisecondsPerFrame = 150;
        //private int bgTimeSinceLastFrame = 0;


        //Создаем наше устройство
        GraphicsDeviceManager graphics;
        //Спрайт - основной элемент для отображения графики и текста 2д
        SpriteBatch spriteBatch;

        //Текстуры уровня(2 разных блока)
        Texture2D blockTexture1;
        Texture2D blockTexture2;
        Texture2D finishTexture;
        Texture2D coinTexture;

        //Текстуры движения(стоит, бежит, прыгает)
        Texture2D idleTexture;
        Texture2D runTexture;
        Texture2D jumpTexture;

        //Экземпляр нашего персонажа
        AnimatedSprite hero;
        
        //Параметры экрана
        public int Width;
        public int Height;

        /// <summary>
        /// Список блоков
        /// </summary>
        List<Block> blocks;
        List<Item> items;

        static int ScrollX;
        int levelLength;
        int blcSize = 20;

        int currentLevel;

        /// <summary>
        /// Состояние клавиатуры
        /// </summary>
        KeyboardState oldState;
        
        //Конструктор - инициализация устройства, установка папки с контентом,
        //Установка разрешения экрана
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Width = graphics.PreferredBackBufferWidth = 800;
            Height = graphics.PreferredBackBufferHeight = 20 * blcSize;
        }
        //Игровые функции

        /// <summary>
        /// Столкновение с уровнем
        /// </summary>
        /// <param name="rect">Экземпляр нашего прямоугольника</param>
        /// <returns>Столкнулся или нет</returns>
        public bool CollidesWithLevel(Rectangle rect)
        {
            foreach (Block block in blocks)
            {
                if (block.rect.Intersects(rect))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Изменяем положение прямоугольника на экране на -ScrollX только по 0Х
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rectangle GetScreenRect(Rectangle rect)
        {
            //Rectangle screenRect = rect;
            //screenRect.Offset(-ScrollX, 0);

            //return screenRect;

            rect.Offset(-ScrollX, 0);
            return rect;
        }
        public void Scroll(int dx)
        {
            if (ScrollX + dx >= 0 && ScrollX + dx <= levelLength - Height)
                ScrollX += dx;
        }
        public void CreateLevel()
        {
            currentLevel++;
            if (currentLevel > 3)
                currentLevel = 1;
            blocks = new List<Block>();
            string[] s = File.ReadAllLines("content/levels/level" + currentLevel + ".txt");

            levelLength = blcSize * s[0].Length - 400;
            int x = 0;
            int y = 0;
            foreach (string str in s)
            {
                foreach (char c in str)
                {
                    Rectangle rect = new Rectangle(x, y, blcSize, blcSize);
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
                    x += blcSize;
                }
                x = 0;
                y += blcSize;
            }

            //Пробую итемы
            items = new List<Item>();
            string[] s1 = File.ReadAllLines("content/levels/level" + currentLevel + ".txt");

            x = 0;
            y = 0;
            foreach (string str in s)
            {
                foreach (char c in str)
                {
                    Rectangle rect = new Rectangle(x, y, blcSize, blcSize);
                    if (c == 'F')
                    {
                        Item graal = new Item(rect, finishTexture);
                        items.Add(graal);
                    }
                    if (c == 'C')
                    {
                        Item graal = new Item(rect, coinTexture);
                        items.Add(graal);
                    }
                    x += blcSize;
                }
                x = 0;
                y += blcSize;
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

            bg_image = Content.Load<Texture2D>("Textures/bg2");
            finishTexture = Content.Load<Texture2D>("Textures/graal");
            coinTexture = Content.Load<Texture2D>("Textures/coin");
           // bg_image = Content.Load<Texture2D>("Textures/bg");
            
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


            //bgTimeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            //if (bgTimeSinceLastFrame > bgMillisecondsPerFrame)
            //{
            //    bgTimeSinceLastFrame -= bgMillisecondsPerFrame;
            //    ++currPosition.X;
            //    if (currPosition.X >= bgSheetSize.X)
            //    {
            //        currPosition.X = 0;
            //        ++currPosition.Y;
            //        if (currPosition.Y >= bgSheetSize.Y)
            //        {
            //            currPosition.Y = 0;
            //        }
            //    }
            //}



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
            // spriteBatch.Draw(bg_image, Vector2.Zero, new Rectangle(currPosition.X * bgFrameSize.X, currPosition.Y * bgFrameSize.Y, bgFrameSize.X, bgFrameSize.Y), Color.White);
            spriteBatch.Draw(bg_image, Vector2.Zero, new Rectangle(0, 0, 800, 600), Color.White);
            foreach (Block block in blocks)
            {
                block.Draw(spriteBatch);
            }
            foreach (Item item in items)
            {
                item.Draw(spriteBatch);
            }
            spriteBatch.End();

            hero.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
