# Maze Solver

A C# console application that reads a maze from a file and solves it using a backtracking algorithm.
I built this project around april-may in 2023, still being my first year into computer science and coding.
I hope you will appreciate my efforts! Feel free to optimize my code and suggest changes!
(Of course, if i started building this project more recently i would have used more efficent data structures and maybe recursion as well)

## Features

- Reads maze dimensions and structure from a file
- Explores possible paths to find the exit
- Visual representation of the maze and the solution path
- Efficiently handles crossroads and dead ends

## Backtracking Algorithm

This project uses a backtracking algorithm to solve the maze. The backtracking algorithm explores all possible solutions and backtracks when it reaches a dead end. Here’s how the program implements this logic:

1. **Exploration of Directions**: For each position in the maze, the program checks possible directions for movement.
2. **Storing Crossroads**: Crossroads with more than two directions are stored so that the program can backtrack if a dead end is reached.
3. **Movement and Recording**: The program moves in the chosen direction and records each move.
4. **Dead End Detection**: When there are no valid moves, the program identifies a dead end.
5. **Returning to Crossroads**: In case of a dead end, the program backtracks to the last stored crossroad and tries a different direction.
6. **Continuing Until Success or Exhaustion of Options**: This process continues until the maze exit is found or all possible options are exhausted.

This technique allows the program to explore the maze efficiently, finding the path from start to finish without getting stuck in dead ends.

## Getting Started

### Prerequisites

- .NET 5.0 SDK or later

### Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/anItalianGeek/maze-solver.git
    ```
2. Navigate to the project directory:
    ```sh
    cd maze-solver
    ```

### Usage

1. Ensure the maze file is correctly formatted as specified:
    - First line: number of rows
    - Second line: number of columns
    - Following lines: maze layout with '0' for walls and '1' for paths
2. Update the path to the maze file in the source code if necessary.
3. Build and run the project:
    ```sh
    dotnet run
    ```

## Contributing

Contributions are welcome! Please open an issue or submit a pull request for any changes.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
