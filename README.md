# TilePacker

A little cross platform .NET Core tileset packing tool.

At the moment TilePacker is super basic. It will pack a directory of PNG files into a single PNG texture and each tile will be the size of the largest input image.

## Usage

Clone the repository and run the command line tool using `dotnet run`

```cmd
dotnet run --input \directory\of\images --output ouput-image.png
```
