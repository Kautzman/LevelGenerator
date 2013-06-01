using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LevelGenerator
{
    class Camera
    {
        protected float zoom;
        public Matrix transform;
        public Vector2 position;
        protected float rotation;

        public Camera()
        {
            zoom = 1.0f;
            rotation = 0f;
            position = Vector2.Zero;
        }
        public float Zoom
        {
            get { return zoom; }
            set { zoom = value; if (zoom < 0.1f) zoom = 0.1f; }
        }
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public void Move(Vector2 amount)
        {
            position += amount;
        }
        public Vector2 cameraPosition
        {
            get { return position; }
            set {position = value; }
        }
        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            transform = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                    Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));

            return transform;
        }
    }
}
