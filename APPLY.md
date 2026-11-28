# Badil.Api logging / correlation ID / CORS patch

This patch is based on the VS2AI project export supplied on 2026-11-28. Copy the files under `src/Badil.Api/` into the matching project paths.

## Included changes

- Adds Serilog console and daily rolling file sinks (`logs/badil-.log`).
- Adds a Correlation ID middleware that reuses a non-empty `X-Correlation-Id` request header or generates a GUID, returns it in the response header, and pushes it into Serilog `LogContext`.
- Adds Serilog request-completion logging so each HTTP request generates a log event containing the correlation ID.
- Adds a CORS allow-list read from `Cors:AllowedOrigins` in `appsettings.json`. The list is intentionally empty because the React development origin was not provided. Add the exact origin later (scheme + host + port, with no path), for example `http://localhost:5173` only if that is confirmed to be the actual React origin.
- Adds exactly one executable line to `Program.cs`; all middleware pipeline registration is performed by an `IStartupFilter` registered from the extension method.
- Adds NuGet references only to `Badil.Api.csproj`.

## Apply on the local repository

1. Fetch and update `main`, then create `feat/api-logging` from that fresh main:

   ```powershell
   git fetch origin
   git switch main
   git pull --ff-only origin main
   git switch -c feat/api-logging
   ```

2. Copy the included `src/Badil.Api` files into your repository, preserving the directory structure. Review local modifications first; this patch was generated from the supplied export, not from a live Git working tree.
3. Restore and build:

   ```powershell
   dotnet restore .\Badil-Backend.sln
   dotnet build .\Badil-Backend.sln --no-restore
   ```

4. Start the API, then test with a name because the current Ping validator requires one:

   ```powershell
   curl.exe -i "http://localhost:5214/api/ping?name=Ahmed"
   curl.exe -i "http://localhost:5214/api/ping?name=Ahmed"
   curl.exe -i -H "X-Correlation-Id: test-123" "http://localhost:5214/api/ping?name=Ahmed"
   curl.exe -i -H "X-Correlation-Id: test-123" "http://localhost:5214/api/ping?name=Ahmed"
   ```

   The first two responses should contain different generated IDs. The last two should both contain `test-123`. Each request should also produce a request-completion log in the console and daily log file.

5. Once the actual React dev origin is known, add it to `Cors:AllowedOrigins`, for example:

   ```json
   "Cors": {
     "AllowedOrigins": [ "http://localhost:<confirmed-port>" ]
   }
   ```

6. Only after build and the runtime checks pass, commit, push, and open a PR from `feat/api-logging` to `main`.

## Validation limitation

The supplied attachment is a source export, not a working checkout with `.git`, package restore state, or a .NET SDK. This patch has therefore not been built or run here, and no branch, commit, push, or PR has been created. Run the commands above in the actual repository and inspect any compile/runtime output before opening the PR.
