# Developer Onboarding

Welcome to the project! This guide helps you set up your environment so you can build, test, and consume our libraries.

---

## 1. Prerequisites
- Install [.NET SDK 8.0+](https://dotnet.microsoft.com/download).
- Install Git and clone the repository.
- Ensure you have access to our GitHub organization.

---

## 2. NuGet Configuration

We publish packages to **NuGet.org** (stable) and **GitHub Packages** (nightly/previews).  
To restore packages locally, configure your `NuGet.config`:

### Repo Root `NuGet.config`

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <!-- Official NuGet feed -->
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />

    <!-- GitHub Packages feed -->
    <add key="github" value="https://nuget.pkg.github.com/OWNER/index.json" />
  </packageSources>

  <packageSourceCredentials>
    <github>
      <add key="Username" value="OWNER" />
      <add key="ClearTextPassword" value="YOUR_GITHUB_PAT" />
    </github>
  </packageSourceCredentials>
</configuration>
