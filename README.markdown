# Battleship Game

## Overview
This C# desktop application is a faithful recreation of the classic Battleship game, featuring a user-friendly GUI and a 10x10 grid for both the player and the computer opponent. Players strategically place four ships (Carrier, Destroyer, Battleship, Submarine) and take turns attacking coordinates to sink enemy ships. The game tracks hits, misses, and ship sinkings, storing game statistics (winner names, game duration, and attempts) in a SQLite database for a dynamic leaderboard. It supports replay functionality and displays win/loss statistics, offering an engaging and interactive experience.

## Technologies
- **Language**: C# (.NET Framework)
- **GUI**: Windows Forms or WPF (depending on implementation)
- **Database**: SQLite
- **Build Tool**: Visual Studio

## Features
- Two 10x10 grids: one for the player (with visible ships) and one for the computer (hidden ships, showing hits/misses).
- Random placement of four ships: Carrier (5 cells), Destroyer (4 cells), Battleship (3 cells), Submarine (2 cells), placed horizontally or vertically.
- Interactive gameplay with hit (red 'X') and miss (green '-') tracking.
- Computer opponent avoids repeating coordinates and announces ship sinkings (e.g., "My Carrier has sunk!").
- SQLite database to store game statistics: winner names, game duration, and number of attempts.
- Leaderboard displaying top players and their stats.
- Post-game options: replay or exit, with cumulative win/loss statistics.
- User-friendly GUI for seamless coordinate selection and game interaction.

## Installation
1. Clone the repository:
   ```
   git clone https://github.com/OzzYBcc/battleship-game.git
   ```
2. Navigate to the project directory:
   ```
   cd battleship-game
   ```
3. Open the solution file (`.sln`) in Visual Studio.
4. Ensure the required NuGet packages for SQLite are installed (e.g., `System.Data.SQLite`).
5. Set up the SQLite database by running the `setup.sql` script (located in the `/db` directory) to initialize the leaderboard and stats tables.
6. Build and run the project in Visual Studio:
   ```
   dotnet build
   dotnet run
   ```

## Usage
- **Game Setup**: Start the game, and ships are randomly placed on both grids.
- **Gameplay**: Select coordinates (e.g., B-8) to attack the computer's grid. The computer responds with a random, non-repeated coordinate attack.
- **Hit/Miss Feedback**: Hits are marked with a red 'X', misses with a green '-'.
- **Ship Sinking**: When all cells of a ship are hit, a message displays (e.g., "Enemy Submarine sunk!").
- **Game End**: The first to sink all opponent ships wins. View game duration, attempts, and leaderboard.
- **Replay**: Choose to start a new game or exit, with win/loss stats displayed.
- **Example Code (Grid Attack Logic)**:
  ```csharp
  public class BattleshipGame {
      private char[,] playerGrid = new char[10, 10];
      private char[,] opponentGrid = new char[10, 10];

      public bool AttackOpponent(int row, int col) {
          if (opponentGrid[row, col] == 'S') {
              opponentGrid[row, col] = 'X'; // Hit
              CheckShipSunk(row, col);
              return true;
          } else {
              opponentGrid[row, col] = '-'; // Miss
              return false;
          }
      }
  }
  ```
- **Example Database Query (Leaderboard)**:
  ```csharp
  using System.Data.SQLite;

  public void SaveGameStats(string winner, int duration, int attempts) {
      using (var connection = new SQLiteConnection("Data Source=game.db;Version=3;")) {
          connection.Open();
          var command = new SQLiteCommand(
              "INSERT INTO Leaderboard (Winner, Duration, Attempts) VALUES (@winner, @duration, @attempts)",
              connection);
          command.Parameters.AddWithValue("@winner", winner);
          command.Parameters.AddWithValue("@duration", duration);
          command.Parameters.AddWithValue("@attempts", attempts);
          command.ExecuteNonQuery();
      }
  }
  ```

## Contributing
Contributions are welcome! To contribute:
1. Fork the repository.
2. Create a new branch (`git checkout -b feature-branch`).
3. Commit your changes (`git commit -m 'Add new feature'`).
4. Push to the branch (`git push origin feature-branch`).
5. Open a pull request.

Potential improvements include adding sound effects, enhancing the GUI with animations, or implementing smarter AI for the computer opponent.

## License
MIT License