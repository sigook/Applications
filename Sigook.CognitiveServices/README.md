# Sigook.CognitiveServices

.NET 8 web app wrapping Azure Cognitive Services (speech-to-text / text-to-speech) for the Covenant platform. Three projects: `.Core` (services and interfaces), `.Infraestructure` (cloud provider implementations), `.UI` (API endpoints).

```bash
dotnet build Sigook.CognitiveServices.sln
dotnet run --project Sigook.CognitiveServices.UI
```

Deployed manually to the `sigook-cognitive-services` App Service (production only, F1 plan on purpose) — see [`.docs/technical/PIPELINES.md`](../.docs/technical/PIPELINES.md). Code map: [`CLAUDE.md`](CLAUDE.md).
