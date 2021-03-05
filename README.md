Reproduction of a time-to-compile regression between Microsoft.CodeAnalysis 3.6.0 -> anything greater

```
PS ...\csharp-switch-regression> dotnet run --configuration Release --project .\CSharpSwitchRegression-3.6.0.csproj                                                                                                                                                                                                                                               Starting compilation
Completed compilation in 7940ms
```
```
PS ...\csharp-switch-regression> dotnet run --configuration Release --project .\CSharpSwitchRegression-3.9.0.csproj                                                                                                                                                                                                                                               Starting compilation
Completed compilation in 68133ms
```