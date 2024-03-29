﻿using System;
using System.Collections.Generic;
using System.Linq;
using Maze.Constants;
using Maze.Models;
using Maze.Utils;

namespace Maze.Service
{
    static class MazeService
    {
        private static readonly Random Random = new Random();

        public static CellModel[,] GenerateMaze(CellModel[,] maze,
                                                int width,
                                                int height,
                                                bool isCustomStart = false,
                                                bool isCustomFinish = false,
                                                int[] customStart = null,
                                                int[] customFinish = null)
        {
            Stack<CellModel> stack = new Stack<CellModel>();

            var currentCell = maze.GetStartCell();

            if (isCustomStart) {
                maze[currentCell.Y, currentCell.X].IsStart = false;
                maze[customStart[1], customStart[0]].IsStart = true;
            }

            do
            {
                var neighbours = ObtainNeighbours(maze, width, height, currentCell);

                if (neighbours.Size() != 0)
                {
                    var neighbourCell = GetRandom(neighbours);
                    stack.Push(neighbourCell);
                    maze = RemoveWall(maze, currentCell, neighbourCell);
                    currentCell = neighbourCell;
                    maze = MarkAsVisited(maze, currentCell);
                }
                else if (stack.Count > 0)
                {
                    currentCell = stack.Pop();
                }
                else
                {
                    currentCell = GetUnvisitedCell(maze);
                    maze = MarkAsVisited(maze, currentCell);
                }
            } while (UnvisitedCount(maze) > 0);

            if (isCustomFinish) {
                maze[customFinish[1], customFinish[0]].IsFinish = true;
                return maze;
            }

            currentCell.IsFinish = true;
            return maze;
        }

        private static RowModel ObtainNeighbours(CellModel[,] maze, int width, int height, CellModel cell)
        {
            int x = cell.X;
            int y = cell.Y;

            var up = new CellModel(x, y - MazeConstants.Distance);
            var rt = new CellModel(x + MazeConstants.Distance, y);
            var dw = new CellModel(x, y + MazeConstants.Distance);
            var lt = new CellModel(x - MazeConstants.Distance, y);

            CellModel[] directions = {dw, rt, up, lt};
            var row = new RowModel(MazeConstants.NeighbourSize);

            for (int i = 0; i < MazeConstants.NeighbourSize; i++)
            {
                var current = directions[i];
                if (!current.In(width, height)) continue;
                var currentMaze = maze[current.Y, current.X];
                if (!currentMaze.IsWall() && !currentMaze.IsVisited())
                {
                    row.Add(currentMaze);
                }
            }

            return row;
        }

        private static CellModel[,] RemoveWall(CellModel[,] maze, CellModel src, CellModel dest)
        {
            int deltaX = dest.X - src.X;
            int deltaY = dest.Y - src.Y;

            var targetCell = new CellModel();

            var addX = (deltaX != 0) ? (deltaX/Math.Abs(deltaX)) : 0;
            var addY = (deltaY != 0) ? (deltaY/Math.Abs(deltaY)) : 0;

            targetCell.X = src.X + addX;
            targetCell.Y = src.Y + addY;

            maze[targetCell.Y, targetCell.X].Type = CellConstants.Cell;
            maze[targetCell.Y, targetCell.X].VisitState = CellConstants.Visited;
            return maze;
        }

        private static CellModel GetRandom(RowModel row)
        {
            int position = Random.Next(0, row.Size());
            return row.Get(position);
        }

        private static CellModel[,] MarkAsVisited(CellModel[,] maze, CellModel src)
        {
            maze[src.Y, src.X].VisitState = CellConstants.Visited;
            return maze;
        }

        private static CellModel GetUnvisitedCell(CellModel[,] maze)
        {
            var unvisited = maze.Cast<CellModel>().Where(cell => !cell.IsWall() && !cell.IsVisited()).ToList();
            return unvisited[Random.Next(0, unvisited.Count)];
        }

        private static int UnvisitedCount(CellModel[,] maze)
        {
            int count = maze.Cast<CellModel>().Where(cell => !cell.IsWall() && !cell.IsVisited()).ToList().Count;
            return count;
        }
    }
}
