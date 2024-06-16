# RelationalSerializer

RelationalSerializer is a tool for creating a JSON database to serialize into .NET objects. It's a .NET native version of https://github.com/ncannasse/castle that allows for some various formatting and validation as well as quick iteration.

## Technologies & Architecture

Roslyn is used to parse code directly to figure out which fields can be serialized; there's an integration with Github.

The back end is a C#/.NET ASP Core REST API and an MSSQL database. The API is designed to be ephemeral and both are designed to be run within Docker.

The front end uses Vue 3.

SignlaR enables live collaboration.

## Quickstart

Please note that there's still more work to be done and this is not ready for usage.

### Windows

After pulling down this repostiory, run the following in PowerShell.

```.\Install.ps1 -NetworkBridge "MyNetworkBridge" -DBPassword "Don'tLose7his!"```

## Example

![Example screenshot showing data](Example.png)

The above is generated from the following code:

```csharp
//...using statements
public class Ability
{
    public string guid { get; set; }
    public string Name { get; set; }
    public string CodeBehind { get; set; }
    public string ToolTip { get; set; }

//...functions
}
```

Note that the CodeBehind string is modified with the Code modifier allowing us to pick a language and get syntax highlighting.

There's support for length of strings, min and max of integers, enums as dropdowns, lists and arrays, and objects by reference.