# Maze Solver

> First version, single-threaded, written in 2023 during my first year of programming.
> A year later I redid the same problem in Java with parallel exploration:
> [mazeSolver-multiThread](https://github.com/anItalianGeek/mazeSolver-multiThread).
>
> Kept as a learning archive, without active development or maintenance.

A C# console application that reads a maze from a file and solves it using a backtracking algorithm.
I built this project around april-may in 2023, still being my first year into computer science and coding.
I hope you will appreciate my efforts! Feel free to optimize my code and suggest changes!
(Of course, if i started building this project more recently i would have used more efficent data structures and maybe recursion as well)

## Features

- Reads maze dimensions and structure from an external text file
- Explores possible directions step-by-step
- Console visualization showing the maze and real-time movement
- Manages intersections using an explicit crossroad matrix acting as a stack

## Backtracking Algorithm

This project uses an iterative backtracking approach implemented with 2D array stacks rather than recursion:

1. **Direction Checking**: For each position in the maze, `CheckMovements` checks up, down, right, and left boundaries and walls (`'0'`).
2. **Crossroad Stack**: When three or more directions are available (`movementsAvailable >= 3`), `CheckIfThreeWayChoice` pushes the coordinates, available choices, and history index onto an `indexCrossroads` matrix (acting as a LIFO stack).
3. **Movement & Trail Recording**: `Move` marks the previous position with `'1'`, advances to the chosen tile, marks it with `'x'`, and appends coordinates to the `history` log.
4. **Dead End Recovery**: When no open directions remain, the algorithm pops the last recorded crossroad from `indexCrossroads`, rewinds the trail in `history`, and tries the next available alternative.
5. **Exit Condition**: The search loops until the bottom-right corner of the maze matrix is marked with `'x'`.

## Maze File Specification

The program expects a file named exactly `maze.txt` formatted as follows:

- **Line 1**: Number of rows (`dimensionRows`, integer).
- **Line 2**: Number of columns (`dimensionColumns`, integer).
- **Remaining lines**: The maze grid, where `'0'` represents a wall and `'1'` represents a walkable path.

A valid sample maze is provided in the repository as [`maze.txt`](maze.txt) (27 rows by 22 columns).

> **Important**: In [`maze-solver.cs`](maze-solver.cs) (lines 399–403), the file is loaded via:
> ```csharp
> try {
>     fileReader = new StreamReader(@"maze.txt");
> } catch (Exception e) {
>     return;
> }
> ```
> The program searches for `maze.txt` in the **current working directory** of the process (`Directory.GetCurrentDirectory()`). If `maze.txt` is missing from that directory, the exception is caught silently and the program exits immediately without error output.
>
> **Adapting custom mazes without changing source code**: Do not modify the path in `maze-solver.cs`. Instead, ensure your custom maze is saved or copied as `maze.txt` inside the working directory from which you run the executable.

## Building and Running

The repository contains a standalone C# source file ([`maze-solver.cs`](maze-solver.cs)) and does not include a pre-generated `.csproj` or solution file. You can run it with any standard C# toolchain:

### Option A: Using the .NET SDK (`dotnet`)

1. Clone the repository:
   ```sh
   git clone https://github.com/anItalianGeek/maze-solver.git
   cd maze-solver
   ```
2. Initialize a temporary console wrapper, copy the files, and run:
   ```sh
   dotnet new console -o runner
   cp maze-solver.cs runner/Program.cs
   cp maze.txt runner/
   cd runner
   dotnet run
   ```

### Option B: Using the C# Compiler (`csc`)

From a Developer Command Prompt or environment where `csc` is available:
```sh
csc maze-solver.cs
./maze-solver.exe
```
*(Make sure `maze.txt` is present in the directory where `maze-solver.exe` is executed).*

### Option C: Using Visual Studio

1. Create a new **C# Console Application**.
2. Replace `Program.cs` with the contents of [`maze-solver.cs`](maze-solver.cs).
3. Add [`maze.txt`](maze.txt) to the project and in its File Properties set **Copy to Output Directory** to **Copy if newer** (or **Copy always**).
4. Run the project (`F5` or `Ctrl+F5`).

## Contributing

Contributions are welcome! Please open an issue or submit a pull request for any changes.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
