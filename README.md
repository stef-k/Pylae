# Pylae

**Gate & Reception Control System**

Pylae is a Windows desktop application for visitor/member management and access logging at gates and reception desks. It combines software-based visit tracking with optional hardware integration for automated door access control.

---

## Features

### Core Functionality
- **Member Management** - Full CRUD operations for members with photos, badges, and organizational data
- **Visit Logging** - Entry/exit tracking with timestamps, notes, and member snapshots
- **Badge Generation** - PDF badges with QR codes, member photos, and organizational branding
- **Multi-Site Support** - LAN-based HTTP sync between Pylae instances
- **Role-Based Access** - Admin and User roles with granular permissions
- **Audit Logging** - Comprehensive audit trail for all sensitive operations

### Security
- **Encrypted SQLite Databases** - All data stored in encrypted SQLite files
- **Password Strength Validation** - Enforced complexity requirements
- **QuickCode Login** - Optional 4-6 digit PIN for fast user switching
- **Idle Lock** - Configurable auto-lock after inactivity
- **API Rate Limiting** - Protection against brute force attacks
- **Windows Credential Manager** - Secure storage of database encryption passwords

### Hardware Integration (Optional)
- **RFID Badge Readers** - 125 kHz EM readers with Wiegand-26 protocol
- **Electric Door Strikes** - Fail-secure 12V DC strikes with relay control
- **Sonoff Controllers** - Wi-Fi relay modules for remote door control
- **Exit Buttons** - Hardware egress independent of software
- **Door Contacts** - Open/closed state monitoring

### Localization
- **Greek-First UI** - Primary language support for Greek
- **Localizable Resources** - All user-facing strings in .resx files
- **Dynamic Culture** - Runtime language switching via settings

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Pylae.Desktop                           │
│         (WinForms UI + ViewModels + Composition Root)       │
└─────────────────────────────────────────────────────────────┘
                              │
          ┌───────────────────┼───────────────────┐
          │                   │                   │
          ▼                   ▼                   ▼
┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
│   Pylae.Core    │  │   Pylae.Data    │  │   Pylae.Sync    │
│ (Domain Models) │  │  (EF Core +     │  │  (HTTP Sync     │
│                 │  │   SQLite)       │  │   Server/Client)│
└─────────────────┘  └─────────────────┘  └─────────────────┘
```

### Projects

| Project | Description |
|---------|-------------|
| `Pylae.Core` | Domain models, interfaces, enums, constants, validation logic |
| `Pylae.Data` | EF Core DbContexts, migrations, repositories, services |
| `Pylae.Desktop` | WinForms UI, ViewModels, resources, composition root |
| `Pylae.Sync` | LAN HTTP sync server and client for multi-site deployment |

---

## Technology Stack

- **.NET 10** - Target framework
- **WinForms** - Desktop UI framework
- **CommunityToolkit.Mvvm** - MVVM pattern implementation
- **Entity Framework Core** - ORM for data access
- **SQLite + SQLitePCLRaw.bundle_e_sqlite3mc** - Encrypted database
- **Serilog** - Structured logging
- **QuestPDF** - Badge PDF generation
- **QRCoder** - QR code generation
- **ClosedXML** - Excel export

---

## Getting Started

### Prerequisites

- Windows 10/11
- .NET 10 SDK
- Visual Studio 2022 or later

### Building

```bash
# Clone the repository
git clone https://github.com/your-org/pylae.git
cd pylae

# Restore and build
dotnet restore
dotnet build
```

### Running

```bash
dotnet run --project src/Pylae.Desktop
```

### First Run

On first launch, Pylae will prompt for:
1. **Site Code** - Unique identifier for this installation (e.g., `hq`, `gate_a`)
2. **Site Display Name** - Human-readable name for the site
3. **Admin Password** - Password for the built-in admin account
4. **Encryption Password** - Master password for database encryption

---

## Data Storage

### Location

```
C:\ProgramData\Pylae\{siteCode}\
├── Data\
│   ├── master.db      # Members, Users, Offices, Settings, AuditLog
│   └── visits.db      # Visit records (append-only)
├── Photos\
│   └── *.jpg          # Member photos
├── Logs\
│   └── *.log          # Application logs
└── Config\            # Optional configuration files
```

### Databases

| Database | Contents |
|----------|----------|
| `master.db` | Members, Users, Offices, MemberTypes, Settings, AuditLog |
| `visits.db` | Visits (immutable log with member snapshots) |

---

## Hardware Integration

Pylae supports optional hardware integration for automated door access control.

### Supported Hardware

| Component | Specification | Purpose |
|-----------|--------------|---------|
| Electric Strike | 12V DC, fail-secure | Door locking mechanism |
| RFID Reader | 125 kHz EM, Wiegand-26 | Badge reading |
| Exit Button | NO contact | Local egress |
| Door Contact | Magnetic reed switch | Open/closed detection |
| Controller | Sonoff 4CH Pro R3 | Relay control (4 doors per unit) |
| Power Supply | 12V DC, 5-10A | Powers strikes and readers |

### Access Flows

**Unmanned Door (Badge Tap)**
1. User taps badge on reader
2. Reader sends credential to Pylae via controller
3. Pylae validates access rules
4. If allowed, relay pulses to unlock strike
5. Event logged with timestamp

**Manned Door (Reception)**
1. Operator views visitor in Pylae
2. Clicks "Unlock" button
3. Pylae commands controller
4. Strike unlocks momentarily
5. Event logged with operator identity

### Fallback Behavior

| Scenario | Behavior |
|----------|----------|
| Power failure | Strike remains locked; mechanical key works |
| Network failure | Badge unlock fails; key & exit button work |
| Server offline | No badge access; key works |
| Controller fault | Only affects specific door; key works |

### Cost Estimate

- **Per door hardware**: ~57-62 €
- **Shared controller/PSU**: ~7-12 € per door
- **Total per door**: ~67-72 €
- **With professional wiring**: ~70-80 € per door

---

## Configuration

### Settings (in master.db)

| Category | Key | Description |
|----------|-----|-------------|
| **Identity** | `SiteCode` | Unique site identifier |
| | `SiteDisplayName` | Human-readable site name |
| | `PrimaryLanguage` | UI language (e.g., `el-GR`) |
| **Network** | `NetworkEnabled` | Enable HTTP sync server (0/1) |
| | `NetworkPort` | HTTP server port |
| | `NetworkApiKey` | API authentication key |
| **Security** | `IdleTimeoutMinutes` | Auto-lock timeout (0=disabled) |
| **Logging** | `LogLevel` | Minimum log level |
| | `LogRetentionDays` | Days to keep log files |
| **Badges** | `BadgeValidityMonths` | Badge expiry period (-1=disabled) |
| | `BadgeExpiryWarningDays` | Days before expiry to warn |
| **Backup** | `AutoBackupEnabled` | Enable scheduled backups (0/1) |
| | `AutoBackupIntervalHours` | Backup frequency |
| | `AutoBackupRetentionCount` | Number of backups to keep |

---

## API Endpoints (LAN Sync)

When `NetworkEnabled = 1`, Pylae exposes HTTP endpoints for LAN synchronization:

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/health` | GET | Health check (no auth required) |
| `/api/sync/info` | GET | Site metadata and stats |
| `/api/sync/visits/full` | GET | Download full visits database |
| `/api/sync/visits` | GET | Query visits by date range |
| `/api/sync/master` | POST | Upload master data and photos |

**Authentication**: All endpoints (except `/api/health`) require `X-Api-Key` header.

---

## Roles and Permissions

### Admin
- Full CRUD on Members, Users, Offices, MemberTypes
- Manage all settings including network and security
- Import/export databases and photos
- View and export Visits and AuditLog
- Reset other users' passwords and QuickCodes
- Manual door unlock (if hardware integrated)

### User (Gate/Reception)
- Perform check-in/check-out operations
- Search Members and Visits
- View member details and photos
- Add/edit Notes on Visits
- Change own password and QuickCode
- Manual door unlock (if hardware integrated)

### Special Accounts
- **System Admin** - Protected, cannot be deleted or deactivated
- **Shared Gate Account** - Optional shared login for busy gates

---

## Documentation

Detailed documentation is available in the `spec/` directory:

| Document | Description |
|----------|-------------|
| `Pylae-spec-v1.md` | Full v1 specification |
| `Pylae-project-layout.md` | Solution and project structure |
| `Pylae-coding-conventions.md` | Code style guidelines |
| `implementation-summary.md` | Enhancement implementation details |
| `smoke-checklist.md` | Testing checklist |
| `software-hardware integration/pylae-access-spec.md` | Hardware integration specification |
| `software-hardware integration/pylae-door-bom.md` | Bill of materials for door hardware |

---

## License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

---

## Contributing

[Contribution guidelines here]

---

## Support

For issues and feature requests, please use the GitHub issue tracker.
