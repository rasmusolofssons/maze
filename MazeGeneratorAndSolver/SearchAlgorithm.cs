using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace MazeGeneratorAndSolver
{
    public class SearchAlgorithm
    {
        private Maze _maze;
        public SearchAlgorithm(Maze maze)
        {
            _maze = maze;
        }

        #region Breadth-First Search
        public bool BreathFirstSearch()
        {
            var que = new Queue<Cell>();
            que.Enqueue(_maze.Begin);
            _maze.Begin.IsVisited = true;
            List<Cell> path = new List<Cell>();
            while (que.Count > 0)
            {
             var currentCell = que.Dequeue();
                if (currentCell.Position == _maze.End.Position)
                {
                    while (currentCell.Position != _maze.Begin.Position)
                    {
                        path.Add(currentCell);
                        currentCell = currentCell.PreviousCell;
                    }

                    path.Reverse();
                    foreach (var cell in path)
                    {
                        _maze.FoundPath.Add(cell);
                        Thread.Sleep(20);
                    }

                    break;

                }

                Thread.Sleep(2);
                var cellNeighbours = GetCurrentCanVisitCellNeighbours(currentCell);

                foreach (var cellNeighbour in cellNeighbours)
                {
                    if (!cellNeighbour.IsVisited)
                    {

                        cellNeighbour.PreviousCell = currentCell;

                        que.Enqueue(cellNeighbour);
                        cellNeighbour.IsVisited = true;
                    }
                }


            }
            // implement Breadth-First search here

            return false;
        }

        #endregion Breadth-First Search

        #region Depth-First Search
        public void DepthFirstSearch()
        {
            _maze.End.Position = new Point(_maze.Width - 1, _maze.Height - 1);

            var stack = new Stack<Cell>();
            stack.Push(_maze.Begin);
            _maze.Begin.IsVisited = true;

            while (stack.Count > 0)
            {
                var cell = stack.Pop();

                _maze.CurrentGenerateCell = cell.Position;
                var neighbours = GetCurrentCellNeighbours(cell);
                var cellNeighbours = new List<Cell>();
                foreach (var neighbour in neighbours)
                {
                    var neighbourCell = new Cell(neighbour, neighbour);
                    cellNeighbours.Add(neighbourCell);
                }

                if (cell != null && cell.PreviousCell != null && cellNeighbours.Count > 0)
                {
                    Random random = new Random();
                    var randomNumber = random.Next(0, cellNeighbours.Count);
                    RemoveWall(cell, cellNeighbours[randomNumber]);
                    RemoveWall(cell, cell.PreviousCell);
                    

                }

                foreach (var neighbourCell in cellNeighbours)
                {
                    if (!neighbourCell.IsVisited)
                    {

                        stack.Push(neighbourCell);

                        if (stack.Count > 1)
                        {
                            var nextCell = stack.Peek();

                            nextCell.PreviousCell = cell;

                        }
                        neighbourCell.IsVisited = true;
                        System.Threading.Thread.Sleep(2);
                    }
                }
            }
            _maze.End.CellWalls[2] = false;
        }


        private void MakeMazeBeginEnd()
        {
            Point temp = new Point();
            Random random = new Random();
            temp.Y = random.Next(_maze.Height);
            temp.X = 0;
            _maze.MazeArray[temp.X, temp.Y].CellWalls[0] = false;
            _maze.Begin = _maze.MazeArray[temp.X, temp.Y];

            temp.Y = random.Next(_maze.Height);
            temp.X = _maze.Width - 1;
            _maze.MazeArray[temp.X, temp.Y].CellWalls[2] = false;
            _maze.End = _maze.MazeArray[temp.X, temp.Y];
        }


        private void RemoveWall(Cell current, Cell next)
        {
            // Next is down 
            if (current.Position.X == next.Position.X && current.Position.Y > next.Position.Y)
            {
                _maze.MazeArray[current.Position.X, current.Position.Y].CellWalls[1] = false;
                _maze.MazeArray[next.Position.X, next.Position.Y].CellWalls[3] = false;
            }
            // the next is up
            else if (current.Position.X == next.Position.X)
            {
                _maze.MazeArray[current.Position.X, current.Position.Y].CellWalls[3] = false;
                _maze.MazeArray[next.Position.X, next.Position.Y].CellWalls[1] = false;
            }
            // the next is right
            else if (current.Position.X > next.Position.X)
            {
                _maze.MazeArray[current.Position.X, current.Position.Y].CellWalls[0] = false;
                _maze.MazeArray[next.Position.X, next.Position.Y].CellWalls[2] = false;
            }
            // the next is left
            else
            {
                _maze.MazeArray[current.Position.X, current.Position.Y].CellWalls[2] = false;
                _maze.MazeArray[next.Position.X, next.Position.Y].CellWalls[0] = false;
            }
        }

        private List<Point> GetCurrentCellNeighbours(Cell current)
        {
            List<Point> neighbours = new List<Point>();

            Point tempPos = current.Position;
            // Check right neigbour cell 
            tempPos.X = current.Position.X - 1;
            if (tempPos.X >= 0 && AllWallsIntact(_maze.MazeArray[tempPos.X, tempPos.Y]))
            {
                neighbours.Add(tempPos);
            }

            // Check left neigbour cell 
            tempPos.X = current.Position.X + 1;
            if (tempPos.X < _maze.Width && AllWallsIntact(_maze.MazeArray[tempPos.X, tempPos.Y]))
            {
                neighbours.Add(tempPos);
            }

            // Check Upper neigbour cell 
            tempPos.X = current.Position.X;
            tempPos.Y = current.Position.Y - 1;
            if (tempPos.Y >= 0 && AllWallsIntact(_maze.MazeArray[tempPos.X, tempPos.Y]))
            {
                neighbours.Add(tempPos);
            }

            // Check Lower neigbour cell 
            tempPos.Y = current.Position.Y + 1;
            if (tempPos.Y < _maze.Height && AllWallsIntact(_maze.MazeArray[tempPos.X, tempPos.Y]))
            {
                neighbours.Add(tempPos);
            }

            return neighbours;
        }

        private List<Cell> GetCurrentCanVisitCellNeighbours(Cell current)
        {

            //cellwalls[3] är ner för currentCell. Om väggen ner är öppen från nuvarande cell, lägg till grann-cellen nedanför.
            //Hur hitta grancellen nedanför?
            List<Cell> neighbours = new List<Cell>();

            Point tempPos = current.Position;
            // Check left neigbour cell 
            tempPos.X = current.Position.X - 1;
            if (tempPos.X >= 0 && _maze.MazeArray[tempPos.X, tempPos.Y].CellWalls[2] == false)
            {
                neighbours.Add(_maze.MazeArray[tempPos.X, tempPos.Y]);
            }

            // Check right neigbour cell 
            tempPos.X = current.Position.X + 1;
            if (tempPos.X < _maze.Width && _maze.MazeArray[tempPos.X, tempPos.Y].CellWalls[0] == false)
            {
                neighbours.Add(_maze.MazeArray[tempPos.X, tempPos.Y]);
            }

            // Check Upper neigbour cell 
            tempPos.X = current.Position.X;
            tempPos.Y = current.Position.Y - 1;
            if (tempPos.Y >= 0 && _maze.MazeArray[tempPos.X, tempPos.Y].CellWalls[3] == false)
            {
                neighbours.Add(_maze.MazeArray[tempPos.X, tempPos.Y]);
            }

            // Check Lower neigbour cell 
            tempPos.Y = current.Position.Y + 1;
            if (tempPos.Y < _maze.Height && _maze.MazeArray[tempPos.X, tempPos.Y].CellWalls[1] == false)
            {
                neighbours.Add(_maze.MazeArray[tempPos.X, tempPos.Y]);
            }

            return neighbours;
        }

        private bool AllWallsIntact(Cell cell)
        {
            for (int i = 0; i < 4; i++)
            {
                if (!_maze.MazeArray[cell.Position.X, cell.Position.Y].CellWalls[i])
                {
                    return false;
                }
            }
            return true;
        }
        #endregion Depth-First Search

    }
}
