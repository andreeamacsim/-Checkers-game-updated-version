using Checkers.Helpers;
using Checkers.Models;

using Checkers.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Checkers.ViewModels
{
    public class GameViewModel: BaseNotification
    {
        
        

        private (int line, int column)? _currentCell, _newCell;

        private PlayerType _currentPlayer = PlayerType.Red;

        public PlayerType CurrentPlayer
        {
            get { return _currentPlayer; }

            set
            {
                if (_currentPlayer != value)
                {
                    _currentPlayer = value;
                    OnPropertyChanged(nameof(CurrentPlayer));  
                }
            }
        }
        

       
        private GameStatistics _statistics;
        public GameViewModel(GameStatistics statistics)
        {
            LoadStatistics();
            _statistics = statistics;
            _currentCell = null;
            _newCell = null;
            _currentPlayer = PlayerType.Red;
        }

        public bool IsMoveValid(ObservableCollection<Cell> cells)
        {
            if (_currentCell == null || _newCell == null)
                return false;
            return IsSimpleMoveValid(cells) || IsJumpValid(cells); ;
        }

        private bool IsSimpleMoveValid(ObservableCollection<Cell> cells)
        {
            if (_currentCell == null || _newCell == null)
                return false;

            int rowDifference = _newCell.Value.line - _currentCell.Value.line;
            int colDifference = Math.Abs(_currentCell.Value.column - _newCell.Value.column);
            int currentCellIndex = _currentCell.Value.line * 8 + _currentCell.Value.column;
            bool isKing = cells[currentCellIndex].Content == CheckerTypes.RedKing || cells[currentCellIndex].Content == CheckerTypes.WhiteKing;

            
            if (colDifference == 1 && Math.Abs(rowDifference) == 1)
            {
                if (isKing)
                {
                    
                    return true;
                }
                else
                {
                    
                    return (_currentPlayer == PlayerType.Red && rowDifference == -1) ||
                           (_currentPlayer == PlayerType.White && rowDifference == 1);
                }
            }

            return false;
        }

        private bool IsJumpValid(ObservableCollection<Cell> cells)
        {
            if (_currentCell == null || _newCell == null)
                return false;

            int rowDifference = _newCell.Value.line - _currentCell.Value.line;
            int colDifference = _newCell.Value.column - _currentCell.Value.column;
            int currentCellIndex = _currentCell.Value.line * 8 + _currentCell.Value.column;
            bool isKing = cells[currentCellIndex].Content == CheckerTypes.RedKing || cells[currentCellIndex].Content == CheckerTypes.WhiteKing;

            
            if (Math.Abs(rowDifference) == 2 && Math.Abs(colDifference) == 2)
            {
                int midRow = _currentCell.Value.line + rowDifference / 2;
                int midCol = _currentCell.Value.column + colDifference / 2;
                int opponentCellIndex = midRow * 8 + midCol;

                
                if (cells[opponentCellIndex].IsOccupied && Checker.GetPlayerTypeFromChecker(cells[opponentCellIndex].Content) != _currentPlayer)
                {
                    
                    if (!isKing && ((_currentPlayer == PlayerType.Red && rowDifference > 0) || (_currentPlayer == PlayerType.White && rowDifference < 0)))
                    {
                        return false;
                    }

                    
                    return !cells[_newCell.Value.line * 8 + _newCell.Value.column].IsOccupied;
                }
            }

            return false;
        }

        private void CheckForAdditionalJumps(ObservableCollection<Cell> cells, (int line, int column) cell)
        {
            _currentCell = cell;
            _newCell = null;


            foreach (var direction in new[] { (1, 1), (1, -1), (-1, 1), (-1, -1) })
            {
                var potentialNewCell = (cell.line + direction.Item1 * 2, cell.column + direction.Item2 * 2);
                if (potentialNewCell.Item1 >= 0 && potentialNewCell.Item1 < 8 && potentialNewCell.Item2 >= 0 && potentialNewCell.Item2 < 8)

                {
                    _newCell = potentialNewCell;
                    if (IsJumpValid(cells))
                    {

                        return;
                    }
                }
            }

            FinalizeTurn(cells, cell);
        }



        private void AssignCurrentCell(Cell cell)
        {
            if (Checker.GetPlayerTypeFromChecker(cell.Content) == _currentPlayer)
            {
                _currentCell = (cell.Line, cell.Column);
            }
        }

        private void AssignNewCell(Cell cell)
        {
            if (Checker.GetPlayerTypeFromChecker(cell.Content) == PlayerType.None)
            {
                _newCell = (cell.Line, cell.Column);
            }
        }

        public void AssignCheckerType(IList<Cell> cells, (int line, int column) cell, CheckerTypes checkerType)
        {
            var cellIndex = cell.line * 8 + cell.column;
            cells[cellIndex].Content = checkerType;
        }

   


        public void MovePiece(ObservableCollection<Cell> cells)
        {
            var currentCellIndex = _currentCell.Value.line * 8 + _currentCell.Value.column;
            var newCellIndex = _newCell.Value.line * 8 + _newCell.Value.column;
          

            if (IsJumpValid(cells))
            {
                PerformJump(cells, currentCellIndex, newCellIndex);
                
                CheckForKing(cells, newCellIndex);
                CheckForAdditionalJumps(cells, _newCell.Value);
            }
            else if (IsSimpleMoveValid(cells) && !cells[newCellIndex].IsOccupied)
            {
                PerformRegularMove(cells, currentCellIndex, newCellIndex);
                
                FinalizeTurn(cells, _newCell.Value);
            }
        }

        private void FinalizeTurn(ObservableCollection<Cell> cells, (int line, int column) cell)
        {
            var cellIndex = cell.line * 8 + cell.column;
            CheckForKing(cells, cellIndex); 

            int redCount = cells.Count(c => c.Content == CheckerTypes.RedPawn || c.Content == CheckerTypes.RedKing);
            int whiteCount = cells.Count(c => c.Content == CheckerTypes.WhitePawn || c.Content == CheckerTypes.WhiteKing);

            if (redCount == 0 || whiteCount == 0)
            {
                DeclareWinner(redCount, whiteCount);
            }
            else
            {
                SwitchPlayer();
            }
        }

        private void CheckForKing(ObservableCollection<Cell> cells, int cellIndex)
        {
            var cell = cells[cellIndex];
            
            if ((cell.Content == CheckerTypes.RedPawn && cell.Line == 0) || (cell.Content == CheckerTypes.WhitePawn && cell.Line == 7))
            {
                cells[cellIndex].Content = cell.Content == CheckerTypes.RedPawn ? CheckerTypes.RedKing : CheckerTypes.WhiteKing;
            }
        }

        private void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == PlayerType.Red ? PlayerType.White : PlayerType.Red;
            
            _currentCell = null;
            _newCell = null;
        }


        private void DeclareWinner(int redCount, int whiteCount)
        {
            int piecesRemaining = redCount > 0 ? redCount : whiteCount; 

            
            if (redCount == 0)
            {
                _statistics.WhiteWins++;
                MessageBox.Show("White player wins!");
            }
            else if (whiteCount == 0)
            {
                _statistics.RedWins++;
                MessageBox.Show("Red player wins!");
            }

            
            if (piecesRemaining > _statistics.MaxPiecesRemaining)
            {
                _statistics.MaxPiecesRemaining = piecesRemaining;
            }

            SaveStatistics();
            ResetGameBoard();
            MessageBox.Show("The game has ended. Starting a new game.");
        }


        private void ResetGameBoard()
        {
            _currentCell = null;
            _newCell = null;
            CurrentPlayer= PlayerType.Red; 
        }


        private void SaveStatistics()
        {
            string path = @"C:\Users\Andreea\Desktop\-Checkers-game-\Checkers\file.txt";
            string userName = SessionData.CurrentUserName;

            try
            {
                using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8))
                {
                    writer.WriteLine($"User: {userName}"); 
                    writer.WriteLine($"Date: {DateTime.Now}");
                    writer.WriteLine($"Red player wins: {_statistics.RedWins}");
                    writer.WriteLine($"White player wins: {_statistics.WhiteWins}");
                    writer.WriteLine($"Max pieces remaining for a winner: {_statistics.MaxPiecesRemaining}");
                    writer.WriteLine("----");
                    Console.WriteLine("Statistics saved successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while writing to the file: {ex.Message}");
            }
            SaveCumulativeStatistics();
        }



        private void LoadStatistics()
        {
            _statistics = new GameStatistics();
            string path = @"C:\Users\Andreea\Desktop\-Checkers-game-\Checkers\file.txt";
            string currentUser = SessionData.CurrentUserName;

            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string line;
                    bool isCurrentUserSection = false; 
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("User: "))
                        {
                            isCurrentUserSection = line.EndsWith(currentUser); 
                            continue; 
                        }

                        if (isCurrentUserSection) 
                        {
                            
                            if (line.Contains("Red player wins"))
                            {
                                int wins = int.Parse(line.Split(':')[1].Trim());
                                _statistics.RedWins += wins;
                            }
                            else if (line.Contains("White player wins"))
                            {
                                int wins = int.Parse(line.Split(':')[1].Trim());
                                _statistics.WhiteWins += wins;
                            }
                            else if (line.Contains("Max pieces remaining for a winner"))
                            {
                                int pieces = int.Parse(line.Split(':')[1].Trim());
                                if (pieces > _statistics.MaxPiecesRemaining)
                                {
                                    _statistics.MaxPiecesRemaining = pieces;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               
                _statistics = new GameStatistics(); 
            }
        }

        private void SaveCumulativeStatistics()
        {
            string path = @"C:\Users\Andreea\Desktop\-Checkers-game-\Checkers\file.txt";

            
            LoadStatistics();

            try
            {
                using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8)) 
                {
                    
                    writer.WriteLine("Cumulative Statistics:");
                    writer.WriteLine($"Total Red Wins: {_statistics.RedWins}");
                    writer.WriteLine($"Total White Wins: {_statistics.WhiteWins}");
                    writer.WriteLine($"Max Pieces Remaining: {_statistics.MaxPiecesRemaining}");
                    writer.WriteLine("----");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while appending cumulative statistics to the file: {ex.Message}");
            }
        }


        private void PerformRegularMove(ObservableCollection<Cell> cells, int currentCellIndex, int newCellIndex)
        {

            cells[newCellIndex].Content = cells[currentCellIndex].Content;
            cells[newCellIndex].IsOccupied = true;
            cells[currentCellIndex].IsOccupied = false;
            cells[currentCellIndex].Content = CheckerTypes.None;
        }

        private void PerformJump(ObservableCollection<Cell> cells, int currentCellIndex, int newCellIndex)
        {

            int opponentCellIndex = (_newCell.Value.line + _currentCell.Value.line) / 2 * 8 +
                                   (_newCell.Value.column + _currentCell.Value.column) / 2;
            if (cells[opponentCellIndex].IsOccupied &&
                cells[opponentCellIndex].Content != cells[currentCellIndex].Content)
            {
                cells[newCellIndex].Content = cells[currentCellIndex].Content;
                cells[newCellIndex].IsOccupied = true;
                cells[currentCellIndex].IsOccupied = false;
                cells[currentCellIndex].Content = CheckerTypes.None;


                cells[opponentCellIndex].Content = CheckerTypes.None;
                cells[opponentCellIndex].IsOccupied = false;
            }
            else
            {
                MessageBox.Show("Invalid move. You can't jump over an empty space or your own piece.");
                return;
            }
        }


        public void CellClicked(Cell cell)
        {
            if (cell.IsOccupied)
            {
                AssignCurrentCell(cell);
            }
            else
            {
                AssignNewCell(cell);
            }
        }


    }
}
