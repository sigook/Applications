# Sigook.CognitiveServices — .NET AI/ML Services

Azure Cognitive Services integration for the Covenant platform. Currently focused on speech-to-text and text-to-speech capabilities.

## Code Navigation

```
Business services:  Sigook.CognitiveServices.Core/BussinesServices/Implementations/   (SpeechService)
Service interfaces: Sigook.CognitiveServices.Core/BussinesServices/Interfaces/         (ISpeechService)
Cloud interfaces:   Sigook.CognitiveServices.Core/Interfaces/Cloud/                    (ISpeechConverter)
Models:             Sigook.CognitiveServices.Core/Models/Speech/
Infrastructure:     Sigook.CognitiveServices.Infraestructure/                           (Cloud provider implementations)
API/UI:             Sigook.CognitiveServices.UI/                                        (API endpoints)
```

## Commands

```bash
# Build
dotnet build Sigook.CognitiveServices.sln
```
