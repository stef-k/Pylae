# Pylae – Project Layout

This document describes the **solution and project structure** for Pylae so that implementation (human or agentic) starts from a consistent base.

Pylae is a **WinForms .NET 10** desktop app using:

- **Entity Framework Core** with **SQLite** (encrypted) for data access.
- A layered architecture:
  - `Pylae.Core` for domain models and business logic.
  - `Pylae.Data` for EF Core DbContexts, entities, and migrations.
  - `Pylae.Desktop` for WinForms UI + ViewModels.
  - `Pylae.Sync` (optional, in v1) for LAN HTTP sync APIs and client.

---

## 1. Solution Structure

**Solution name**: `Pylae.sln`

Projects:

1. `Pylae.Core`
2. `Pylae.Data`
3. `Pylae.Desktop`
4. `Pylae.Sync` (LAN HTTP host + client; can start minimal and grow)

### 1.1 Target Frameworks

- **All projects** target:
  - `net10.0` or `net10.0-windows` (for WinForms, use the `-windows` TFM).

Recommended:

- `Pylae.Desktop` → `net10.0-windows`
- `Pylae.Core`, `Pylae.Data`, `Pylae.Sync` → `net10.0`

---

## 2. Project Responsibilities

### 2.1 Pylae.Core

**Purpose**: domain layer and reusable logic.

Contains:

- Domain models (plain C# classes) representing:
  - Member, MemberType, Office, User, Setting, AuditEntry, Visit.
- Common enums and constants (e.g. `UserRole`, `VisitDirection`, `VisitMethod`).
- Interfaces for services:
  - `IMemberService`, `IVisitService`, `ISettingsService`, `IUserService`, etc.
- Validation and business rules (e.g. badge expiry checks) where possible.
- Utility classes that are independent of UI and EF (e.g. time helpers).

**References**:

- No project references (base library).
- NuGet:
  - `CommunityToolkit.Mvvm` (for shared ViewModel base classes / observable types if needed).
  - `Microsoft.Extensions.Logging.Abstractions` (for logging interfaces, if used here).

---

### 2.2 Pylae.Data

**Purpose**: EF Core + SQLite data access for both `master.db` and `visits.db`.

Contains:

- EF Core **DbContext** classes:
  - `PylaeMasterDbContext` → maps `Members`, `Users`, `Offices`, `MemberTypes`, `Settings`, `AuditLog`.
  - `PylaeVisitsDbContext` → maps `Visits`.
- EF Core entity configurations (fluent mappings / `OnModelCreating`).
- `DbContextOptions` and factory helpers (design-time factories for migrations).
- Repository-style services (optional) or query methods:
  - `MemberRepository`, `VisitRepository`, `UserRepository`, etc. (or equivalent services).
- SQLite connection management with encryption:
  - Connection strings targeting:
    - `master.db`
    - `visits.db`
  - Configuration to set `Password=` for encrypted SQLite provider.
- EF Core Migrations:
  - Migrations for `PylaeMasterDbContext`.
  - Migrations for `PylaeVisitsDbContext`.
- Seed data (initial admin user, default settings, default member types).

**References**:

- Project references:
  - `Pylae.Core`
- NuGet:
  - `Microsoft.EntityFrameworkCore`
  - `Microsoft.EntityFrameworkCore.Sqlite`
  - `Microsoft.EntityFrameworkCore.Design`
  - `Microsoft.EntityFrameworkCore.Tools` (for migrations)
  - `SQLitePCLRaw.bundle_e_sqlite3mc` (for encryption support)
  - `Microsoft.Extensions.Logging.Abstractions`
  - `Microsoft.Extensions.Options` (if using options patterns for connection config)

---

### 2.3 Pylae.Desktop

**Purpose**: WinForms UI + ViewModels + composition root.

Contains:

- Program entry point:
  - `Program.cs` sets up:
    - `ApplicationConfiguration.Initialize()`
    - Dependency Injection (ServiceCollection)
    - Logging (Serilog → Microsoft logging)
    - EF DbContexts registration
    - Application-wide exception handling
    - Localization / culture initialization
    - High DPI init
    - Dark-mode init

        ```c#
        // Pseudo-structure for Program.cs

        ApplicationConfiguration.Initialize();
        // Follow OS theme (light/dark)
        Application.SetColorMode(SystemColorMode.System);

        var services = BuildServiceProvider();
        var mainForm = services.GetRequiredService<MainForm>();
        Application.Run(mainForm);```

- WinForms views:
  - `LoginForm`
  - `MainForm`
  - `GateForm` (or Gate tab)
  - `MembersForm`
  - `VisitsForm`
  - `SettingsForm`
  - `ExportForm`
  - `RemoteSitesForm` (for sync configuration)
- ViewModels (MVVM via CommunityToolkit.Mvvm):
  - `LoginViewModel`
  - `ShellViewModel` / `MainViewModel`
  - `GateViewModel`
  - `MembersViewModel`
  - `VisitsViewModel`
  - `SettingsViewModel`
- Resources:
  - `Strings.resx`, `Strings.el-GR.resx` (including `App_Subtitle`).
  - Icon resources if needed.
- QR code rendering helpers for badge previews.
- Excel export UI triggers calling export services.

**References**:

- Project references:
  - `Pylae.Core`
  - `Pylae.Data`
  - `Pylae.Sync` (for HTTP client/integration; can be added later)
- NuGet:
  - `CommunityToolkit.Mvvm`
  - `Serilog`
  - `Serilog.Sinks.File`
  - `Serilog.Extensions.Logging`
  - `QRCoder`
  - `ClosedXML`
  - PDF lib (e.g. QuestPDF or equivalent OSS library)
  - `Microsoft.Extensions.DependencyInjection`
  - `Microsoft.Extensions.Configuration` (if configuration files are used)
  - `Microsoft.Extensions.Logging`

---

### 2.4 Pylae.Sync

**Purpose**: HTTP sync host & client for LAN communication.

Contains:

- Minimal API or ASP.NET Core host (if separate) **OR** self-hosted HTTP server/services that can be reused by desktop.
- DTOs for sync operations:
  - `SyncInfoResponse`
  - `VisitsExportRequest/Response`
  - `MasterPackage`
- Client services:
  - `IRemoteSiteClient` for calling remote Pylae instances over HTTP.

**Implementation options**:

- For v1, Pylae.Sync can be a small **class library** referenced by `Pylae.Desktop` that:
  - Hosts Kestrel via `WebApplication` (if you embed ASP.NET Core).
  - Provides a typed `PylaeSyncClient` for Admin “Remote Sites” pull/push.

**References**:

- Project references:
  - `Pylae.Core`
  - `Pylae.Data`
- NuGet:
  - `Microsoft.AspNetCore.App` (via `<FrameworkReference>` in .csproj) or `Microsoft.AspNetCore` packages depending on hosting model.
  - `System.Net.Http.Json` (or similar) for client.
  - `Microsoft.Extensions.Logging`

---

## 3. Project Dependencies Diagram (High-Level)

- `Pylae.Core`
  - No references.
- `Pylae.Data`
  - References: `Pylae.Core`
- `Pylae.Sync`
  - References: `Pylae.Core`, `Pylae.Data`
- `Pylae.Desktop`
  - References: `Pylae.Core`, `Pylae.Data`, `Pylae.Sync`

No project should reference `Pylae.Desktop` (UI stays on top of the stack).

---

## 4. Entity Framework Core & Migrations

- `Pylae.Data` is the **migrations project**.
- We use **two DbContexts**:
  - `PylaeMasterDbContext` → `__EFMigrationsHistory_master`
  - `PylaeVisitsDbContext` → `__EFMigrationsHistory_visits`
- Design-time factories:
  - `PylaeMasterDbContextFactory` implements `IDesignTimeDbContextFactory<PylaeMasterDbContext>`
  - `PylaeVisitsDbContextFactory` implements `IDesignTimeDbContextFactory<PylaeVisitsDbContext>`
- Migrations naming:
  - `Master_InitialCreate`, `Master_AddBadgeExpiry`, …
  - `Visits_InitialCreate`, `Visits_AddIndexes`, …

---

## 5. Configuration & Settings Files

While most settings live in the `Settings` table in `master.db`, the solution can also support:

- `appsettings.json` (in `Pylae.Desktop`) for:
  - Logging defaults.
  - Initial database directories.
- On first run, `Pylae.Desktop`:
  - Ensures the required directory structure under `C:\ProgramData\Pylae\{siteCode}`.
  - Ensures DBs exist and run migrations.

---

## 6. Build & Packaging (Initial Plan)

- Single EXE deployment focused on Windows 10/11:
  - WinForms app with dependencies (EF, Serilog, etc.) deployed alongside.
- Later: a dedicated installer (e.g. Inno Setup or WiX Toolset) can be added, but is **out of scope** of this layout doc.

---

This layout is intended to be stable so that **agentic tasks** can reference project names and boundaries without guessing. All implementation tasks should assume this structure unless explicitly updated in future design documents.
