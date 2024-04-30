# CsharpToTypeScriptRequestCommandConverter

This is part of a target project I have set to enable communication between an Api interface and a client that is being developed with TypeScript.
The basis for the data storage is a command. The name of the command is a specific action that should be executed.
When executing a command on the client side, the complexity of the data transfer protocols should not be visible at all.
should not be visible at all. The result is purely readable code. And because the names
of the commands should have the domain language, it is immediately understandable what it is about when reading the code.

It is therefore important to ensure that the server-side and client-side syntax of the commands is the same.
Modern IDEs support this with the syntax check as well as with the Intellisense.
Changing the syntax of the existing command on the server side leads to an immediate error message and simple correction with the Intellisense on the client side.


Many other good practices can be implemented using these commands.
From the domain language to some principles of SOLID. A sequence of some commands with a time stamp can also be 
persisted well in order to be replayed later in a given chronological time order. (DDD, EventSourcing, CQRS, CQS)...

But this is not the place to write down the entire spectrum of good software development practices.


### Example:

#### C# source
```csharp
/// <summary>
/// Base interface for handler search and execution.
/// </summary>
public interface ICommand<T>;

/// <summary>
/// Interface for request command identification model binding.
/// </summary>
public interface IRequestCommand;

/// <summary>
/// Command example
/// </summary>
public class ChangeUserRoleRequestCommand: IRequestCommand, ICommand<bool>{
    public string UserId { get; set; }
    public UserRoles NewRole { get; set; }
}

/// <summary>
/// Used enum in command example
/// </summary>
public enum UserRoles
{
    User,
    Admin
}
```
#### Result in TypeScript
```Typescript
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 8.0.4
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

"use strict";
// defined interface for all request commands
export interface ICommand<T>{ _?: T}

// all request commands as classes

/**
 * @returns {boolean}
 */
export class ChangeUserRoleRequestCommand implements ICommand<boolean> {
    private readonly $type? = ".ChangeUserRole, CsharpToTypeScriptConverter.Tests";
    public _?: boolean;
    public userId?: string;
    public newRole?: UserRoles;
}


// Enums

export enum UserRoles {
    User = 0,
    Admin = 1,
}

```

#### Execution code
```csharp
// interface type to find all request commands.
var requestCommandType = typeof(IRequestCommand);

// intern defined command with generic return type. 
var returnType = typeof(ICommand<>);
var usedTypes = new Dictionary<string, Type>();

TypesScriptGenerator.Settings.RequestCommandInterfaceName = "ICommand";
            
var typesMetadata = MetadataHelper.GetGeneratorTypesMetadata(typeof(UserRoles).Assembly.ExportedTypes, requestCommandType, returnType, usedTypes);

// types that generator not generated, because they are deeper in definition
var notGeneratedTypes = usedTypes.Where(ut => typesMetadata.All(tm => tm.Name != ut.Key)).ToDictionary();

// generate it, when some are there. (this can be looped)
typesMetadata.AddRange(MetadataHelper.GetGeneratorTypesForUsedTypes(notGeneratedTypes));

var typesGenerator = new TypesScriptGenerator { GeneratorTypes = typesMetadata.ToArray() };
var transformedText = typesGenerator.TransformText().Trim();

var fileName = "./typeScriptTypes.ts";
File.WriteAllText(fileName, transformedText);
           
```

[NuGet-Package](https://www.nuget.org/packages/TypeScriptRequestCommandsGenerator/0.8.0)
