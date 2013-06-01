using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LevelGenerator
{
    class Tile
    {
        Texture2D tile;
        Rectangle location;
        Color tint;
        bool collision;

        public Rectangle Location
        {
            get { return location; }
        }
        // collision not yet implemented, however, the flag is active.
        public Tile(Texture2D tile, Rectangle location, Color tint, bool collision)
        {
            this.tile = tile;
            this.location = location;
            this.tint = tint;
            this.collision = collision;
        }

        public void Draw(SpriteBatch sprites)
        {
            sprites.Draw(tile, location, tint);
        }
    }
}
