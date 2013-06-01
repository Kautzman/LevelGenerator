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


/* Things that need to be immediately addressed:
 * Camera class is half done and needs to be filled in
 * Alternate Dungeon Generation Algorythm is completely broken
 * Need to piece together an actual drawn level with data
 * Handle input --  GameMain.Input() is available as a skeleton, but things actually need to be done there
 * Immediate goal:  Draw a complete floor based on randomly generated dungeon
 * Eventually goal: Randomly laid out drawn dungeon with working camera
 * Milestone:  With randomly selected room layout
 */ 
namespace LevelGenerator
{
    public class GameMain : Microsoft.Xna.Framework.Game
    {
        Map gameMap = new Map(15);
        GraphicsDeviceManager graphics;
        KeyboardState oldState;
        KeyboardState newState;
        SpriteBatch spriteBatch;

        Camera cam = new Camera();

        StreamReader fileReader;

        int tilesWide = 13;
        int tilesHigh = 13;

        Texture2D lightFloor;
        Texture2D darkFloor;

        Texture2D topWall, bottomWall, rightWall, leftWall;
        Texture2D topDoor, bottomDoor, rightDoor, leftDoor;
        Texture2D URWall, ULWall, BLWall, BRWall;
        Texture2D errorTile, blankTile;

        Char[,] tilesCode;
        Tile[,] tiles;
        int[,] currentMinimap;
        int[,] currentMap;

        bool debug = false;

        public GameMain()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = 416;
            graphics.PreferredBackBufferWidth = 416;
        }

        protected override void Initialize()
        {       
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            tiles = new Tile[155, 103];

            lightFloor = Content.Load<Texture2D>("tileLight");
            darkFloor = Content.Load<Texture2D>("tileDark");

            topWall = Content.Load<Texture2D>("tileWallTop");
            bottomWall = Content.Load<Texture2D>("tileWallBottom");
            rightWall = Content.Load<Texture2D>("tileWallRight");
            leftWall = Content.Load<Texture2D>("tileWallLeft");

            topDoor = Content.Load<Texture2D>("tileDoorTop");
            bottomDoor = Content.Load<Texture2D>("tileDoorBottom");
            rightDoor = Content.Load<Texture2D>("tileDoorRight");
            leftDoor = Content.Load<Texture2D>("tileDoorLeft");

            ULWall = Content.Load<Texture2D>("tileULCorner");
            URWall = Content.Load<Texture2D>("tileURCorner");
            BLWall = Content.Load<Texture2D>("tileLLCorner");
            BRWall = Content.Load<Texture2D>("tileLRCorner");

            errorTile = Content.Load<Texture2D>("errorBlock");
            blankTile = Content.Load<Texture2D>("tileBlank");

            cam.cameraPosition = new Vector2(0.0f, 0.0f);

            gameMap.generateFloor();
            currentMap = gameMap.getMap();
            parseMap();
            generateFloor();

            //GenerateRoom();
            /*for (int i = 0; i < 32; i++)
            {
                gameMap.generateFloor();
                currentMap = gameMap.getMap();
                parseMap();
                Console.WriteLine("");
            }*/

            // TODO: use this.Content to load your game content here
        }

        protected void parseMap()
        {
            int roomCount = 0;
            for (int y = 0; y < currentMap.GetLength(1); y++)
            {
                Console.WriteLine("");
                for (int x = 0; x < currentMap.GetLength(0); x++)
                {
                    if (currentMap[x, y] == 1)
                    {
                        roomCount++;
                    }
                    Console.Write(currentMap[x, y]);
                }
            }
            Console.WriteLine("\nRoom Count: " + roomCount);
        }
        protected void GenerateRoom(int xMod, int yMod)
        {
            int currentChar;
            tilesCode = new Char[tilesWide, tilesHigh];
            fileReader = new StreamReader("someLevel.txt");

            // This first loop iterates through the text file and addes it to a 2D array of characters
            // Because of special complications, and the fact that StreamReader.Read() returns an int, not a char, a lot of work is done with the ASCII coding of what is being returned.
            // ASCII Characters 13 and 10 are a carriage return and newline characters, which must be ignored.  Space is 32, which shouldn't be in the file, but they are easy to sneak in and as such,  is exlucded for now. Character 0 is simply null and is checked for for completions sake.
            // The returned value is later converted back into a character --- Convert.ToChar(someInt);
            for (int y = 0; y < tilesHigh; y++)
            {
                int i = 0;

                // this is an awkward loop.  The real iterator is actually i, but x is used for clarity purposes.  'i' iterates when something is actually added.  if something is skipped, x is deiterated
                for (int x = 0; x < tilesWide; x++)
                {
                    currentChar = fileReader.Read();

                    if (currentChar == 13 || currentChar == 10 || currentChar == 0 || currentChar == 32)
                    {
                        if (debug)
                            Console.WriteLine("Captured NewLine or Carriage Return");
                        x--; // this needs to be here, or we'll have gaps in the level
                    }
                    else
                    {
                        tilesCode[i, y] = Convert.ToChar(currentChar);
                        i++;
                    }
                }
            }

            //This second loop takes the tilesCode array (an array full of the text read from the original text file) and then draws the map based on the output
            //tiles[x, y] corrispond to tilesCode[i, y].  I could actually 'fix' the loop above, and will do at a later date, but it 'works' right now, so I'm inclined to leave it.
            for (int y = 0; y < tilesHigh; y++)
            {
                for (int x = 0; x < tilesWide; x++)
                {
                    if (debug)
                        Console.WriteLine("Ascii:" + (int)tilesCode[x, y] + " -- " + x);

                    switch (tilesCode[x, y])
                    {
                        case '#':
                            tiles[x * xMod, y * yMod] = new Tile(lightFloor, new Rectangle(x * lightFloor.Width, y * lightFloor.Height, lightFloor.Width, lightFloor.Height), Color.White, false);
                            break;
                        case '.':
                            tiles[x * xMod, y * yMod] = new Tile(darkFloor, new Rectangle(x * darkFloor.Width, y * darkFloor.Height, darkFloor.Width, darkFloor.Height), Color.White, false);
                            break;
                        case '^':
                            tiles[x * xMod, y * yMod] = new Tile(topWall, new Rectangle(x * topWall.Width, y * topWall.Height, topWall.Width, topWall.Height), Color.White, false);
                            break;
                        case 'v':
                            tiles[x * xMod, y * yMod] = new Tile(bottomWall, new Rectangle(x * bottomWall.Width, y * bottomWall.Height, bottomWall.Width, bottomWall.Height), Color.White, false);
                            break;
                        case '>':
                            tiles[x * xMod, y * yMod] = new Tile(leftWall, new Rectangle(x * bottomWall.Width, y * bottomWall.Height, bottomWall.Width, bottomWall.Height), Color.White, false);
                            break;
                        case '<':
                            tiles[x * xMod, y * yMod] = new Tile(rightWall, new Rectangle(x * bottomWall.Width, y * bottomWall.Height, bottomWall.Width, bottomWall.Height), Color.White, false);
                            break;
                        case 'Q':
                            tiles[x * xMod, y * yMod] = new Tile(ULWall, new Rectangle(x * bottomWall.Width, y * bottomWall.Height, bottomWall.Width, bottomWall.Height), Color.White, false);
                            break;
                        case 'E':
                            tiles[x * xMod, y * yMod] = new Tile(URWall, new Rectangle(x * bottomWall.Width, y * bottomWall.Height, bottomWall.Width, bottomWall.Height), Color.White, false);
                            break;
                        case 'Z':
                            tiles[x * xMod, y * yMod] = new Tile(BLWall, new Rectangle(x * bottomWall.Width, y * bottomWall.Height, bottomWall.Width, bottomWall.Height), Color.White, false);
                            break;
                        case 'C':
                            tiles[x * xMod, y * yMod] = new Tile(BRWall, new Rectangle(x * bottomWall.Width, y * bottomWall.Height, bottomWall.Width, bottomWall.Height), Color.White, false);
                            break;
                        case 'W':
                            tiles[x * xMod, y * yMod] = new Tile(topDoor, new Rectangle(x * bottomWall.Width, y * bottomWall.Height, bottomWall.Width, bottomWall.Height), Color.White, false);
                            break;
                        case 'D':
                            tiles[x * xMod, y * yMod] = new Tile(rightDoor, new Rectangle(x * bottomWall.Width, y * bottomWall.Height, bottomWall.Width, bottomWall.Height), Color.White, false);
                            break;
                        case 'X':
                            tiles[x * xMod, y * yMod] = new Tile(bottomDoor, new Rectangle(x * bottomWall.Width, y * bottomWall.Height, bottomWall.Width, bottomWall.Height), Color.White, false);
                            break;
                        case 'A':
                            tiles[x * xMod, y * yMod] = new Tile(leftDoor, new Rectangle(x * bottomWall.Width, y * bottomWall.Height, bottomWall.Width, bottomWall.Height), Color.White, false);
                            break;
                        default:
                            tiles[x * xMod, y * yMod] = new Tile(errorTile, new Rectangle(x * errorTile.Width, y * errorTile.Height, errorTile.Width, errorTile.Height), Color.Pink, false);
                            break;
                    }
                }
            }
        }
        public void generateBlankRoom(int xMod, int yMod)
        {
            for (int y = 0; y < tilesHigh; y++)
            {
                for (int x = 0; x < tilesWide; x++)
                {
                    tiles[x * xMod, y * yMod] = new Tile(errorTile, new Rectangle(x * errorTile.Width, y * errorTile.Height, errorTile.Width, errorTile.Height), Color.Pink, false);
                }
            }
        }
        // Geneartes the whole floor but calls on the generate room method to actually place the individual tiles.
        public void generateFloor()
        {
            currentMinimap = gameMap.getMap();
            for (int y = 0; y < gameMap.maxHeight; y++)
            {
                int yMod = 1;
                int xMod = 1;

                for (int x = 0; x < gameMap.maxWidth; x++)
                {
                    switch(currentMinimap[x, y])
                    {
                        case 0:
                            generateBlankRoom(xMod, yMod);
                            break;
                        case 1:
                            GenerateRoom(xMod, yMod);
                            break;
                        default:
                            // Generate Blank stuff shaded red
                            break;
                    }
                    xMod++;
                }
                yMod++;
            }
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            checkInput();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, cam.get_transformation(GraphicsDevice));
         
            foreach (Tile tile in tiles)
                    tile.Draw(spriteBatch);

            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        protected void checkInput()
        {
            newState = Keyboard.GetState();

            if (oldState.IsKeyUp(Keys.Down) && newState.IsKeyDown(Keys.Down))
                cam.Move(new Vector2(0, 50));
            if (oldState.IsKeyUp(Keys.Up) && newState.IsKeyDown(Keys.Up))
                cam.Move(new Vector2(0, -50));
            if (oldState.IsKeyUp(Keys.Right) && newState.IsKeyDown(Keys.Right))
                cam.Move(new Vector2(50, 0));
            if (oldState.IsKeyUp(Keys.Left) && newState.IsKeyDown(Keys.Left))
                cam.Move(new Vector2(-50, 0));

            oldState = newState;
        }
    }
}
