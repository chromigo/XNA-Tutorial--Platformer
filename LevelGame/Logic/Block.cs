using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LevelGame
{   
    class Block
    {//Блок - прямоугольник с текстурой
        public Rectangle rect;
        Texture2D texture;
        //получаем текстуру и прямоугольник, с которым будем работать
        public Block(Rectangle rect, Texture2D texture)
        {
            this.rect = rect;
            this.texture = texture;
        }
        /// <summary>
        /// Изменяем положение и отрисовывает наш Блок на экране
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle screenRect = Game1.GetScreenRect(rect);
            spriteBatch.Draw(texture, screenRect, Color.White);
        }
    }

    //Новый вид - итем!
    class Item
    {//Блок - прямоугольник с текстурой
        public Rectangle rect;
        Texture2D texture;
        //получаем текстуру и прямоугольник, с которым будем работать
        public Item(Rectangle rect, Texture2D texture)
        {
            this.rect = rect;
            this.texture = texture;
        }
        /// <summary>
        /// Изменяем положение и отрисовывает наш Блок на экране
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle screenRect = Game1.GetScreenRect(rect);
            spriteBatch.Draw(texture, screenRect, Color.White);
        }
    }
}
