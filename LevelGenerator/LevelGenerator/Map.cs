using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace LevelGenerator
{
    class Map
    {
        List<Point> mapPoints = new List<Point>();
        List<Point> endPoints = new List<Point>();
        Point lastRoom;
        Point workingRoom; //  This is the room we are currently working off of and 'extending' from
        Point newRoom; // The room that will be attempted to be added

        int[,] floorLayout;
        public int maxHeight = 8;
        public int maxWidth = 12;

        int roomCount;

        int ySeed;
        int xSeed;

        int roomSeed;

        int totalAddedRooms;
        

        Random rng = new Random();

        public Map(int roomCount)
        {
            this.roomCount = roomCount;
        }
        public void generateFloor()
        {
            floorLayout = new int[maxWidth, maxHeight];

            ySeed = rng.Next(4, 5);
            xSeed = rng.Next(6, 7);

            floorLayout[xSeed, ySeed] = 1;

            lastRoom.X = xSeed;
            lastRoom.Y = ySeed;

            for (int i = 1; i < roomCount; i++)
            {        
                mapPoints.Add(lastRoom);
                workingRoom = lastRoom;
                if(!addRoom())
                    i--;
                lastRoom = newRoom;
            }
        }
        public void generateFloorAlt()
        {
            int i = 1;
            floorLayout = new int[maxWidth, maxHeight];

            //TODO:
            xSeed = rng.Next(4, 5);
            ySeed = rng.Next(6, 7);

            floorLayout[xSeed, ySeed] = 1;

            lastRoom.X = xSeed;
            lastRoom.Y = ySeed;

            while (i < roomCount)
            {
                mapPoints.Add(lastRoom);
                endPoints.Add(lastRoom);

                workingRoom = lastRoom;

                if(addRoomAlt())
                {
                    i++;
                }
                else if (!addRoomAlt())
                {
                     continue;
                }
                Console.WriteLine("contin");

                lastRoom = newRoom;
            }
            //while there are still rooms to be added, add a room
            //Count the number of rooms added
            //Check to make sure a room was added
        }
        // Not used at the moment.
        private Point getRandomRoom(int[,] currentLayout)
        {
            return mapPoints[rng.Next(0, mapPoints.Count - 1)];  
        }
        private bool addRoom()
        {
            int loopCount = 0;
            roomSeed = rng.Next(0, 4);
            
            while (true)
            {
                switch (roomSeed)
                {
                    case 0: // Add room above current room;
                        newRoom.X = workingRoom.X;
                        newRoom.Y = workingRoom.Y - 1;
                        break;
                    case 1: // Add room to the right of current room;
                        newRoom.X = workingRoom.X + 1;
                        newRoom.Y = workingRoom.Y;
                        break;
                    case 2: // Add below it
                        newRoom.X = workingRoom.X;
                        newRoom.Y = workingRoom.Y + 1;
                        break;
                    case 3: // Add to the left
                        newRoom.X = workingRoom.X - 1;
                        newRoom.Y = workingRoom.Y;
                        break;
                }

                if (newRoom.X < maxWidth && newRoom.Y < maxHeight && newRoom.Y >= 0 && newRoom.X >= 0 && floorLayout[newRoom.X, newRoom.Y] == 0)
                {
                    floorLayout[newRoom.X, newRoom.Y] = 1;
                    return true;
                }
                else
                {
                    roomSeed++;
                    roomSeed = roomSeed % 4;
                    loopCount++;

                    if (loopCount > 3)
                    {
                        return false;
                    }
                }
            }
        }
        private bool addRoomAlt()
        {
            //TODO:
            int doorSeed = rng.Next(0, 101);
            int doorCount;

            if (doorSeed <= 70)
            {
                doorCount = 1;
            }
            else if (doorSeed <= 90)
            {
                doorCount = 2;
            }
            else if (doorSeed <= 100)
            {
                doorCount = 3;
            }
            else
            {
                doorCount = 0;
            }

            for (int i = 0; i < doorCount; i++)
            {
                totalAddedRooms = rng.Next(1, 3);
            }                          
            return true;
        }
        public int[,] getMap()
        {
            return floorLayout;
        }
        public bool attachRoom(int direction)
        {
            int loopCount = 0;
            while (true)
            {
                switch (direction)
                {
                    case 0: // Add room above current room;
                        newRoom.X = workingRoom.X;
                        newRoom.Y = workingRoom.Y - 1;
                        break;
                    case 1: // Add room to the right of current room;
                        newRoom.X = workingRoom.X + 1;
                        newRoom.Y = workingRoom.Y;
                        break;
                    case 2: // Add below it
                        newRoom.X = workingRoom.X;
                        newRoom.Y = workingRoom.Y + 1;
                        break;
                    case 3: // Add to the left
                        newRoom.X = workingRoom.X - 1;
                        newRoom.Y = workingRoom.Y;
                        break;
                }

                if (newRoom.X < maxWidth && newRoom.Y < maxHeight && newRoom.Y >= 0 && newRoom.X >= 0 && floorLayout[newRoom.X, newRoom.Y] == 0)
                {
                    floorLayout[newRoom.X, newRoom.Y] = 1;
                    return true;
                }
                else
                {
                    roomSeed++;
                    roomSeed = roomSeed % 4;
                    loopCount++;

                    if (loopCount > 3)
                    {
                        return false;
                    }
                }
            }
        }
    }
}
