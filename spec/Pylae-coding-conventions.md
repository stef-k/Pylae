# Pylae – Coding Conventions

This document defines **coding and architectural conventions** for Pylae so human and agentic work remains consistent.

We are using:

- **.NET 10**
- **WinForms** for UI
- **Entity Framework Core** with **SQLite** (encrypted) for persistence
- **CommunityToolkit.Mvvm** for MVVM patterns
- **Serilog** for logging

---

## 1. General C# Conventions

- **Language features**
  - Target latest C# version supported by .NET 10.
  - Enable nullable reference types: `<Nullable>enable</Nullable>`.
- **Braces**
  - Always use braces `{}` for `if`, `else`, `for`, `foreach`, `while`, even for one-line bodies.
- **Namespaces**
  - Use file-scoped or block-style consistently per project. Prefer **file-scoped** for new files.
  - Namespace pattern:
    - `Pylae.Core.*`
    - `Pylae.Data.*`
    - `Pylae.Desktop.*`
    - `Pylae.Sync.*`
- **Naming**
  - Classes: `PascalCase`.
  - Methods: `PascalCase`.
  - Private fields: `_camelCase`.
  - Local variables & parameters: `camelCase`.
  - Interfaces: `I` prefix, e.g. `IMemberService`.
  - DbContext classes: `PylaeMasterDbContext`, `PylaeVisitsDbContext`.
  - Entities: singular (`Member`, `User`, `Office`).
  - DbSet properties: plural (`Members`, `Users`, `Offices`).

---

## 2. Entity Framework Core Usage

### 2.1 Contexts & Entities

- There are two DbContexts in `Pylae.Data`:
  - `PylaeMasterDbContext` (master.db)
  - `PylaeVisitsDbContext` (visits.db)
- Entities live under:
  - `Pylae.Data.Entities.Master` → Member, MemberType, Office, User, Setting, AuditEntry
  - `Pylae.Data.Entities.Visits` → Visit

### 2.2 Configuration

- Use **fluent API** in `OnModelCreating` or separate `IEntityTypeConfiguration<T>` classes under `Configurations` folders.
- Conventions:
  - Primary keys are named `Id`.
  - Use `HasKey(e => e.Id)` explicitly for clarity.
  - Column types:
    - Dates/times: store as `TEXT` ISO strings in SQLite, EF `DateTime` or `DateTimeOffset` in C#.
  - Required vs optional set via `IsRequired()`.

### 2.3 Migrations

- All migrations live in `Pylae.Data`.
- Use one migration history table per context:
  - `__EFMigrationsHistory_master`
  - `__EFMigrationsHistory_visits`
- Migration names should be descriptive:
  - `Master_InitialCreate`
  - `Visits_AddIndexesForSearch`
- Agent tasks modifying schema must:
  - Update entity classes.
  - Update configuration.
  - Add a new migration (naming consistent with above).
  - Optionally update seed data.

---

## 3. SQLite & Encryption

- Use `Microsoft.EntityFrameworkCore.Sqlite` as the provider.
- Use `SQLitePCLRaw.bundle_e_sqlite3mc` to enable encryption.
- Connection strings include `Password=` for DB encryption.
- Connection string configuration:
  - Built centrally in a `DatabaseConfig` helper in `Pylae.Data`.
  - `Pylae.Desktop` obtains DB root path and encryption key from:
    - initial setup (first run wizard), or
    - configuration + user input.
- Code trying to access the DB must **never** hardcode the password.

---

## 4. MVVM & WinForms

### 4.1 General Approach

- **Forms** are Views.
- **ViewModels** live in `Pylae.Desktop.ViewModels`.
- Use **CommunityToolkit.Mvvm**:
  - ViewModels inherit from `ObservableObject` or `[ObservableObject]`.
  - Use `[ObservableProperty]` for properties.
  - Use `[RelayCommand]` for commands.

### 4.2 Interaction Pattern

- Forms receive their ViewModel via constructor injection:

  ```csharp
  public partial class LoginForm : Form
  {
      private readonly LoginViewModel _viewModel;

      public LoginForm(LoginViewModel viewModel)
      {
          InitializeComponent();
          _viewModel = viewModel;
          // Data binding setup here
      }
  }
  ```

- ViewModels depend only on:
  - Services from `Pylae.Core` / `Pylae.Data` registered in DI.
  - Logging interfaces.
- Forms should **not**:
  - Query DbContexts directly.
  - Contain business logic (validation, rules, etc.).
- Forms **may**:
  - Perform simple UI logic (enabling/disabling controls).
  - Handle WinForms-specific concerns (dialogs, file pickers).

### 4.3 Theming & Dark Mode

- App-wide:

  - Dark mode must be enabled centrally:

    - Application.SetColorMode(SystemColorMode.System); in Program.cs.

  - No per-form calls to SetColorMode.

- Controls:

  - Avoid hardcoded BackColor, ForeColor etc. Stick to system colors where possible, so dark mode works.

  - For custom-drawn controls that should follow theming, use:

```c#
protected override CreateParams CreateParams

{
    get
    {
        SetStyle(ControlStyles.ApplyThemingImplicitly, true);
        var cp = base.CreateParams;
        // ...
        return cp;
    }
}

```

---

## 5. Dependency Injection & Composition

- Composition root is in `Pylae.Desktop` `Program.cs`.
- Use `Microsoft.Extensions.DependencyInjection`:

  ```csharp
  var services = new ServiceCollection();

  services.AddLogging(builder =>
  {
      builder.AddSerilog();
  });

  services.AddDbContext<PylaeMasterDbContext>(...);
  services.AddDbContext<PylaeVisitsDbContext>(...);

  services.AddTransient<IMemberService, MemberService>();
  services.AddTransient<IVisitService, VisitService>();
  // etc.

  services.AddTransient<LoginForm>();
  services.AddTransient<MainForm>();

  var provider = services.BuildServiceProvider();
  ```

- Always inject services via constructor injection.

---

## 6. Passwords, QuickCodes & Security

### 6.1 Password Hashing

- Use PBKDF2 via `Rfc2898DeriveBytes` or a dedicated library.
- Configuration (constants in `Pylae.Core.Security.PasswordHashingOptions`):
  - Iterations (e.g. 100,000+; adjustable).
  - Salt length.
  - Key length.
- Store:
  - Hash (base64).
  - Salt (base64).
  - Iterations (optional, per-user, if you plan to change over time).

### 6.2 QuickCode Hashing

- QuickCodes are 6-digit numeric strings.
- Must **never** be stored in plaintext.
- Hash them using the **same infrastructure** as passwords (PBKDF2), possibly with:
  - Different salt or context string (e.g. “QuickCode”).
- Validation:
  - When user enters a quick code, hash and compare to stored hash.

### 6.3 Protected System Admin

- Mark the built-in admin with `IsSystem = 1`.
- Enforce in `UserService` / `IUserService` implementation:
  - Cannot delete or deactivate `IsSystem = 1`.
  - Cannot change `Role` away from `admin`.
- Before deleting/deactivating any admin:
  - Check that at least one other active admin remains.

---

## 7. Logging

- Use **Serilog** as the primary logger.
- Configuration:
  - Static `Log.Logger` initialization in `Program.cs`.
  - Use `Serilog.Extensions.Logging` to integrate into `Microsoft.Extensions.Logging`.
- Log files:
  - Placed under `C:\ProgramData\Pylae\{siteCode}\Logs\`.
  - Rolling daily or by size (as per Settings).
- Logging style:
  - Contextual messages with structured properties, e.g.:

    ```csharp
    _logger.LogInformation("User {Username} logged in as {Role}", username, role);
    ```

- Audit logs:
  - Business events go into `AuditLog` table via `IAuditService`.
  - Optionally mirrored in Serilog logs with `AUDIT` prefix.

---

## 8. Localization & Resources

- All UI strings are defined in resx:
  - `Strings.resx` (default)
  - `Strings.el-GR.resx` (Greek)
- Example keys:
  - `App_Title` → `"Pylae"`
  - `App_Subtitle` → `"Gate & Reception Control"`
- Forms should:
  - Use resource strings, not hard-coded literals.
  - For title bar: `Text = $"{Resources.App_Title} – {Resources.App_Subtitle}";`
- Excel/PDF export headers also come from resources where appropriate.

---

## 9. Exports (Excel, PDF) & QR Codes

- Excel:
  - Use **ClosedXML**.
  - Export methods live in `Pylae.Core` or `Pylae.Desktop.Services` (depending on dependency needs).
- PDF (for badges, if implemented via PDF):
  - Use a chosen PDF library with a permissive license.
  - Encapsulate badge layout in a dedicated service, e.g. `IBadgeRenderer`.
- QR Codes:
  - Use **QRCoder**.
  - Place generation logic in a helper/service (`IQrCodeService`).
  - QR encodes `MemberNumber` (for v1).

---

## 10. Error Handling

- Application-wide:
  - Global exception handler in `Program.cs` to:
    - Log fatal exceptions.
    - Show a user-friendly message.
- ViewModels:
  - Catch expected exceptions (e.g. validation errors, DB issues) and:
    - Log them at appropriate level.
    - Expose error messages to the View via bindable properties.
- Never swallow exceptions silently.

---

## 11. Git & File Organization

- Keep documents at root:
  - `Pylae-spec-v1.md`
  - `Pylae-project-layout.md`
  - `Pylae-coding-conventions.md`
- Project folders:
  - `/src/Pylae.Core`
  - `/src/Pylae.Data`
  - `/src/Pylae.Desktop`
  - `/src/Pylae.Sync`
- Tests (if/when added):
  - `/tests/Pylae.Core.Tests`
  - `/tests/Pylae.Data.Tests`
  - `/tests/Pylae.Desktop.Tests` (where practical)

---

These conventions are the baseline. Any future agentic tasks should:

- Assume this structure.
- Follow these rules unless a later design document explicitly updates them.
