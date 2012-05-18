using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LevelGame
{
    class AnimatedSprite
    {
        //Прямоугольник героя и его текстуры
        public Rectangle rect;
        Texture2D idle;
        Texture2D run;
        Texture2D jump;
        
        //Состояние героя
        bool isRunning;
        bool isSlowMode;
        bool isRunningRight;
        bool isJumping;

        //Физические величины
        float yVelocity;
        float maxYVelocity = 10;
        float g = 0.2f;

        //Размеры героя
        int frameWidth;
        int frameHeight;
        
        /// <summary>
        /// Ширина кадра
        /// </summary>
        public int Frames
        {
            get
            {
                return run.Width / frameWidth;
            }
        }

        int currentFrame;

        //Параметры времени (сколько прошло и сколько на кадр)
        int timeElapsed;
        int timeForFrame = 100;

        Game1 game;
        //Инициализация
        public AnimatedSprite(Rectangle rect, Texture2D idle, Texture2D run, Texture2D jump, Game1 game)
        {
            this.rect = rect;
            this.idle = idle;
            this.run = run;
            this.jump = jump;

            frameWidth = frameHeight = run.Height;

            this.game = game;
        }
        /// <summary>
        /// Переключение режима передвижения персонажа(нормальный/замедленный)
        /// </summary>
        public void SwitchModes()
        {
            isSlowMode = !isSlowMode;
        }
        /// <summary>
        /// Инициализация начала движения
        /// </summary>
        /// <param name="isRight"></param>
        public void StartRun(bool isRight)
        {
            if (!isRunning)
            {
                isRunning = true;
                currentFrame = 0;
                timeElapsed = 0;
            }
            isRunningRight = isRight;
        }
        /// <summary>
        /// Окончание движения
        /// </summary>
        public void Stop()
        {
            isRunning = false;
        }
        /// <summary>
        /// Прыжок
        /// </summary>
        public void Jump()
        {
            if (!isJumping && yVelocity == 0.0f)
            {
                isJumping = true;
                currentFrame = 0;
                timeElapsed = 0;
                yVelocity = maxYVelocity;
            }
        }

        public void ApplyGravity(GameTime gameTime)
        {
            yVelocity = yVelocity - g * gameTime.ElapsedGameTime.Milliseconds / 10;
            float dy = yVelocity * gameTime.ElapsedGameTime.Milliseconds / 10;

            Rectangle nextPosition = rect;
            nextPosition.Offset(0, -(int)dy);

            Rectangle boudingRect = GetBoundingRect(nextPosition);
            if (boudingRect.Top > 0 && boudingRect.Bottom < game.Height
                && !game.CollidesWithLevel(boudingRect))
                rect = nextPosition;

            bool collideOnFallDown = (game.CollidesWithLevel(boudingRect) && yVelocity < 0);

            if (boudingRect.Bottom > game.Height || collideOnFallDown)
            {
                yVelocity = 0;
                isJumping = false;
            }

        }
        public void Update(GameTime gameTime)
        {
            timeElapsed += gameTime.ElapsedGameTime.Milliseconds;
            int tempTime = timeForFrame;
            if (isSlowMode)
                tempTime *= 4;
            if (timeElapsed > tempTime)
            {
                currentFrame = (currentFrame + 1) % Frames;
                timeElapsed = 0;
            }

            if (isRunning)
            {
                int dx = 3 * gameTime.ElapsedGameTime.Milliseconds / 10;
                if (!isRunningRight)
                    dx = -dx;

                Rectangle nextPosition = rect;
                nextPosition.Offset(dx, 0);

                Rectangle boudingRect = GetBoundingRect(nextPosition);
                Rectangle screenRect = Game1.GetScreenRect(boudingRect);

                if (screenRect.Left > 0 && screenRect.Right < game.Width 
                    && !game.CollidesWithLevel(boudingRect))
                    rect = nextPosition;
            }

            ApplyGravity(gameTime);
        }

        private Rectangle GetBoundingRect(Rectangle rectangle)
        {
            int width = (int)(rectangle.Width * 0.4f);
            int x = rectangle.Left + (int)(rectangle.Width * 0.2f);

            return new Rectangle(x, rectangle.Top, width, rectangle.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle r = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
            SpriteEffects effects = SpriteEffects.None;
            if (isRunningRight)
                effects = SpriteEffects.FlipHorizontally;

            Rectangle screenRect = Game1.GetScreenRect(rect);
            spriteBatch.Begin();
            if (isJumping)
            {
                spriteBatch.Draw(jump, screenRect, r, Color.White, 0, Vector2.Zero, effects, 0);
            }
            else
            if (isRunning)
            {
                spriteBatch.Draw(run, screenRect, r, Color.White, 0, Vector2.Zero, effects, 0);
            }
            else
            {
                spriteBatch.Draw(idle, screenRect, Color.White);
            }
            spriteBatch.End();
        }
    }
}
