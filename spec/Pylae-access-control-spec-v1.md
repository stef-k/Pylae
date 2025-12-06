# Pylae – Access Control Module (v1 Specification)

## 1. Module Overview

**Pylae Access Control** extends the core Pylae system with physical door access management, integrating badge readers, electric strikes, and relay controllers.

Key goals:

- Manage **unlimited doors** across one or more sites.
- Support **badge-based access** using 125 kHz EM4100 cards (Wiegand-26 protocol).
- Provide **manual unlock** capability from the Pylae UI for manned reception/gate scenarios.
- Implement **flexible access control**:
  - **Universal doors**: accessible by all active members.
  - **Group-based access**: doors accessible by members assigned to specific access levels.
  - **Individual access**: fine-grained per-member door assignments.
- Support **time-based schedules**:
  - 24/7 access.
  - Daily time windows (e.g., 07:00–15:00).
  - Weekly patterns (e.g., Mon–Fri 10:00–18:00).
  - Per-door, per-access-level, or per-member schedule overrides.
- Log all **access events** with full audit trail.
- Maintain **mechanical fallback**: physical keys always work regardless of system state.

---

## 2. Hardware Architecture

### 2.1 Per-Door Components

| Component | Description |
|-----------|-------------|
| Electric Strike | Fail-secure 12V DC; remains locked on power loss; key always works |
| RFID Reader | 125 kHz EM4100; Wiegand-26 output |
| Exit Button | Normally-open; local egress independent of software |
| Door Contact | Magnetic sensor; detects open/closed/held/forced states |

### 2.2 Controller Bank

| Component | Description |
|-----------|-------------|
| Sonoff 4CH Pro R3 | WiFi relay module; 4 channels per unit; HTTP/eWeLink API |
| 12V DC PSU | 5A–10A; powers strikes and readers |
| Terminal Blocks | Structured wiring in cabinet |

### 2.3 Communication Flow

```
[Badge Reader] --Wiegand-26--> [Wiegand-to-TCP Bridge] --TCP/HTTP--> [Pylae]
                                                                        |
[Door Strike] <--Relay-- [Sonoff 4CH] <----------HTTP Command-----------+
```

**Wiegand Bridge Options**:
- Dedicated Wiegand-to-Ethernet converter (commercial).
- ESP32/Arduino bridge with custom firmware (DIY).
- Sonoff with Wiegand input support (requires compatible firmware).

### 2.4 Fail-Safe Behavior

| Scenario | Behavior |
|----------|----------|
| Power failure | Strike locked; key works |
| Network failure | Badge access fails; key and exit button work |
| Pylae offline | No badge access; key works |
| Controller fault | Affects only that controller's doors; key works |

No single point of failure removes all access—**mechanical lock always works**.

---

## 3. Data Model – master.db (Access Tables)

### 3.1 Controllers

Represents a Sonoff 4CH Pro R3 or compatible relay controller.

- `Id` – `INTEGER` PK AUTOINCREMENT.
- `Code` – `TEXT`, UNIQUE, NOT NULL (e.g., `ctrl-lobby`, `ctrl-warehouse`).
- `DisplayName` – `TEXT`, NOT NULL.
- `Description` – `TEXT`.
- `Host` – `TEXT`, NOT NULL (IP address or hostname).
- `Port` – `INTEGER`, NOT NULL (default: 8081 for eWeLink LAN).
- `ApiKey` – `TEXT` (if required by controller firmware).
- `ControllerType` – `TEXT`, NOT NULL (`Sonoff4CH`, `Sonoff4CHProR3`, `Generic`).
- `ChannelCount` – `INTEGER`, NOT NULL (default: 4).
- `IsEnabled` – `INTEGER` (0/1).
- `LastSeenUtc` – `TEXT` (last successful communication).
- `Status` – `TEXT` (`Online`, `Offline`, `Unknown`).
- `CreatedAtUtc` – `TEXT`.
- `UpdatedAtUtc` – `TEXT`.

### 3.2 Doors

Represents a physical door with access control hardware.

- `Id` – `INTEGER` PK AUTOINCREMENT.
- `Code` – `TEXT`, UNIQUE, NOT NULL (e.g., `door-main-entrance`).
- `DisplayName` – `TEXT`, NOT NULL.
- `Description` – `TEXT`.
- `Location` – `TEXT` (building, floor, area).
- `ControllerId` – `INTEGER`, FK → Controllers.Id.
- `RelayChannel` – `INTEGER`, NOT NULL (1–4 for Sonoff 4CH).
- `ReaderId` – `TEXT` (identifier for the associated reader, if tracked).
- `StrikeDurationMs` – `INTEGER`, NOT NULL (default: 3000; how long relay stays on).
- `AccessMode` – `TEXT`, NOT NULL:
  - `Universal` – All active members can access (subject to door schedule).
  - `Controlled` – Only members with assigned access levels can access.
  - `Disabled` – Door cannot be unlocked via badge (manual/key only).
- `ScheduleId` – `INTEGER`, FK → Schedules.Id (nullable; door-level time restriction).
- `HasContactSensor` – `INTEGER` (0/1).
- `HasExitButton` – `INTEGER` (0/1).
- `IsEnabled` – `INTEGER` (0/1).
- `DisplayOrder` – `INTEGER`.
- `CreatedAtUtc` – `TEXT`.
- `UpdatedAtUtc` – `TEXT`.

### 3.3 Schedules

Defines when access is permitted. Reusable across doors, access levels, and member assignments.

- `Id` – `INTEGER` PK AUTOINCREMENT.
- `Code` – `TEXT`, UNIQUE, NOT NULL (e.g., `always`, `business-hours`, `weekdays-morning`).
- `DisplayName` – `TEXT`, NOT NULL.
- `Description` – `TEXT`.
- `ScheduleType` – `TEXT`, NOT NULL:
  - `Always` – 24/7, no restrictions.
  - `Daily` – Same time window every day.
  - `Weekly` – Different windows per day of week.
  - `Custom` – Complex patterns (future extension).
- `IsActive` – `INTEGER` (0/1).
- `CreatedAtUtc` – `TEXT`.
- `UpdatedAtUtc` – `TEXT`.

### 3.4 ScheduleWindows

Time windows for a schedule. Multiple windows per schedule supported.

- `Id` – `INTEGER` PK AUTOINCREMENT.
- `ScheduleId` – `INTEGER`, FK → Schedules.Id, NOT NULL.
- `DayOfWeek` – `INTEGER` (0=Sunday, 1=Monday, ..., 6=Saturday; NULL for Daily type).
- `StartTime` – `TEXT`, NOT NULL (`HH:mm` format, e.g., `07:00`).
- `EndTime` – `TEXT`, NOT NULL (`HH:mm` format, e.g., `15:00`).

**Examples**:

| Schedule | Type | Windows |
|----------|------|---------|
| 24/7 | Always | (no windows needed) |
| Business Hours | Daily | 09:00–17:00 |
| Weekdays Only | Weekly | Mon 09:00–17:00, Tue 09:00–17:00, ... Fri 09:00–17:00 |
| Extended Weekdays | Weekly | Mon–Thu 07:00–20:00, Fri 07:00–18:00 |
| Weekend Security | Weekly | Sat 10:00–14:00, Sun 10:00–14:00 |

### 3.5 AccessLevels

Groups of doors with shared access policy. Members are assigned to access levels.

- `Id` – `INTEGER` PK AUTOINCREMENT.
- `Code` – `TEXT`, UNIQUE, NOT NULL (e.g., `all-doors`, `warehouse-staff`, `executive`).
- `DisplayName` – `TEXT`, NOT NULL.
- `Description` – `TEXT`.
- `Priority` – `INTEGER`, NOT NULL (higher = evaluated first; for conflict resolution).
- `DefaultScheduleId` – `INTEGER`, FK → Schedules.Id (nullable; default schedule for this level).
- `IsActive` – `INTEGER` (0/1).
- `DisplayOrder` – `INTEGER`.
- `CreatedAtUtc` – `TEXT`.
- `UpdatedAtUtc` – `TEXT`.

### 3.6 AccessLevelDoors

Junction table: which doors belong to which access levels, with optional schedule override.

- `Id` – `INTEGER` PK AUTOINCREMENT.
- `AccessLevelId` – `INTEGER`, FK → AccessLevels.Id, NOT NULL.
- `DoorId` – `INTEGER`, FK → Doors.Id, NOT NULL.
- `ScheduleId` – `INTEGER`, FK → Schedules.Id (nullable; overrides AccessLevel.DefaultScheduleId for this door).
- UNIQUE constraint on (`AccessLevelId`, `DoorId`).

### 3.7 MemberAccessLevels

Junction table: which members have which access levels, with optional overrides.

- `Id` – `INTEGER` PK AUTOINCREMENT.
- `MemberId` – `TEXT`, FK → Members.Id, NOT NULL.
- `AccessLevelId` – `INTEGER`, FK → AccessLevels.Id, NOT NULL.
- `ScheduleId` – `INTEGER`, FK → Schedules.Id (nullable; overrides for this member's use of this level).
- `ValidFrom` – `TEXT` (`YYYY-MM-DD`, nullable; access level assignment start date).
- `ValidTo` – `TEXT` (`YYYY-MM-DD`, nullable; access level assignment end date).
- `GrantedByUserId` – `INTEGER`, FK → Users.Id.
- `GrantedAtUtc` – `TEXT`.
- `Notes` – `TEXT`.
- UNIQUE constraint on (`MemberId`, `AccessLevelId`).

### 3.8 BadgeCredentials

Maps physical badge card numbers to members.

- `Id` – `INTEGER` PK AUTOINCREMENT.
- `MemberId` – `TEXT`, FK → Members.Id, NOT NULL.
- `CardNumber` – `TEXT`, UNIQUE, NOT NULL (EM4100 card ID, typically 8–10 hex digits).
- `CardFormat` – `TEXT`, NOT NULL (`EM4100`, `HID`, `Mifare`, `Other`).
- `FacilityCode` – `TEXT` (if applicable to card format).
- `DisplayLabel` – `TEXT` (friendly name, e.g., "Blue Badge #42").
- `IsActive` – `INTEGER` (0/1).
- `ValidFrom` – `TEXT` (`YYYY-MM-DD`, nullable).
- `ValidTo` – `TEXT` (`YYYY-MM-DD`, nullable).
- `IssuedByUserId` – `INTEGER`, FK → Users.Id.
- `IssuedAtUtc` – `TEXT`.
- `RevokedByUserId` – `INTEGER`, FK → Users.Id (nullable).
- `RevokedAtUtc` – `TEXT` (nullable).
- `Notes` – `TEXT`.
- `CreatedAtUtc` – `TEXT`.
- `UpdatedAtUtc` – `TEXT`.

### 3.9 AccessEvents

Immutable log of all access attempts and door events.

- `Id` – `INTEGER` PK AUTOINCREMENT.
- `EventGuid` – `TEXT` (for sync/deduplication).
- `TimestampUtc` – `TEXT`, NOT NULL.
- `TimestampLocal` – `TEXT`, NOT NULL.
- `SiteCode` – `TEXT`, NOT NULL.
- `DoorId` – `INTEGER`, FK → Doors.Id.
- `DoorCode` – `TEXT` (snapshot).
- `DoorName` – `TEXT` (snapshot).
- `ControllerId` – `INTEGER` (snapshot).
- `ControllerName` – `TEXT` (snapshot).
- `EventType` – `TEXT`, NOT NULL:
  - `AccessGranted` – Badge accepted, door unlocked.
  - `AccessDenied` – Badge rejected.
  - `ManualUnlock` – Operator unlocked via UI.
  - `ExitButton` – Exit button pressed.
  - `DoorOpened` – Contact sensor: door opened.
  - `DoorClosed` – Contact sensor: door closed.
  - `DoorHeldOpen` – Door open too long.
  - `DoorForcedOpen` – Door opened without unlock event.
  - `ControllerOffline` – Communication lost.
  - `ControllerOnline` – Communication restored.
- `Method` – `TEXT` (`Badge`, `Manual`, `ExitButton`, `System`).
- `CardNumber` – `TEXT` (nullable; the card presented).
- `MemberId` – `TEXT` (nullable; resolved member, if known).
- `MemberNumber` – `INTEGER` (snapshot).
- `MemberFirstName` – `TEXT` (snapshot).
- `MemberLastName` – `TEXT` (snapshot).
- `AccessLevelId` – `INTEGER` (which level granted access, if applicable).
- `AccessLevelName` – `TEXT` (snapshot).
- `DenyReason` – `TEXT` (if denied):
  - `UnknownCard` – Card not in system.
  - `InactiveCard` – Card is disabled.
  - `ExpiredCard` – Card outside valid dates.
  - `InactiveMember` – Member is inactive.
  - `ExpiredMember` – Member badge expired.
  - `NoAccess` – No access level grants this door.
  - `OutsideSchedule` – Access level exists but outside permitted hours.
  - `DoorDisabled` – Door access mode is Disabled.
  - `ControllerOffline` – Cannot communicate with controller.
- `UserId` – `INTEGER` (operator who performed manual unlock, if applicable).
- `Username` – `TEXT` (snapshot).
- `Notes` – `TEXT`.

**Indexes**:
- `idx_AccessEvents_Timestamp`.
- `idx_AccessEvents_DoorId_Timestamp`.
- `idx_AccessEvents_MemberId_Timestamp`.
- `idx_AccessEvents_CardNumber`.

---

## 4. Access Control Logic

### 4.1 Access Check Flow (Badge Presentation)

```
1. Badge presented at reader
   ↓
2. Wiegand bridge sends CardNumber to Pylae
   ↓
3. Lookup BadgeCredential by CardNumber
   ├─ Not found → DENIED (UnknownCard)
   ↓
4. Check BadgeCredential status
   ├─ IsActive = 0 → DENIED (InactiveCard)
   ├─ ValidFrom > Today → DENIED (ExpiredCard)
   ├─ ValidTo < Today → DENIED (ExpiredCard)
   ↓
5. Get Member from BadgeCredential
   ├─ Member.IsActive = 0 → DENIED (InactiveMember)
   ├─ Member.BadgeExpiryDate < Today → DENIED (ExpiredMember)
   ↓
6. Get Door
   ├─ Door.IsEnabled = 0 → DENIED (DoorDisabled)
   ├─ Door.AccessMode = 'Disabled' → DENIED (DoorDisabled)
   ↓
7. Check Door.AccessMode
   ├─ 'Universal' → Check door schedule → GRANTED or DENIED (OutsideSchedule)
   ├─ 'Controlled' → Continue to step 8
   ↓
8. Get MemberAccessLevels for this Member
   ↓
9. For each AccessLevel (ordered by Priority DESC):
   │
   ├─ Check AccessLevel.IsActive
   ├─ Check MemberAccessLevel.ValidFrom/ValidTo
   │
   ├─ Check if Door is in AccessLevel (via AccessLevelDoors)
   │   └─ Not found → Try next AccessLevel
   │
   ├─ Determine effective schedule:
   │   │ Priority: MemberAccessLevel.ScheduleId
   │   │         > AccessLevelDoors.ScheduleId
   │   │         > AccessLevel.DefaultScheduleId
   │   │         > Door.ScheduleId
   │   │         > Always (if all null)
   │   ↓
   ├─ Check if current time is within schedule
   │   ├─ Yes → GRANTED (log AccessLevelId)
   │   └─ No → Try next AccessLevel
   ↓
10. No matching AccessLevel found → DENIED (NoAccess)
```

### 4.2 Schedule Evaluation

```
Given: Schedule, CurrentDateTime (local)

If Schedule is null → ALLOW (no restriction)

If ScheduleType = 'Always' → ALLOW

If ScheduleType = 'Daily':
   For each ScheduleWindow:
      If CurrentTime >= StartTime AND CurrentTime < EndTime:
         → ALLOW
   → DENY

If ScheduleType = 'Weekly':
   CurrentDayOfWeek = DayOfWeek(CurrentDateTime)
   For each ScheduleWindow WHERE DayOfWeek = CurrentDayOfWeek:
      If CurrentTime >= StartTime AND CurrentTime < EndTime:
         → ALLOW
   → DENY
```

### 4.3 Schedule Override Hierarchy

When determining which schedule applies:

1. **MemberAccessLevel.ScheduleId** – Per-member override for this access level.
2. **AccessLevelDoors.ScheduleId** – Per-door override within this access level.
3. **AccessLevel.DefaultScheduleId** – Default schedule for the access level.
4. **Door.ScheduleId** – Door-level default schedule.
5. **null** – No restriction (Always allowed).

This allows scenarios like:
- "Warehouse staff can access warehouse doors Mon–Fri 06:00–22:00"
- "But John Smith has 24/7 access to all warehouse doors"
- "And the loading dock has extended hours Sat 08:00–12:00 for deliveries"

### 4.4 Manual Unlock (Manned Gate)

```
1. Operator selects door in UI
   ↓
2. Operator clicks "Unlock"
   ↓
3. Pylae sends unlock command to Controller
   ↓
4. Log AccessEvent (EventType = 'ManualUnlock', UserId = operator)
   ↓
5. Relay activates for StrikeDurationMs
   ↓
6. Door unlocks, visitor enters
```

No schedule or access level check—operator has discretion.

---

## 5. Controller Communication

### 5.1 Sonoff 4CH Pro R3 (eWeLink LAN Mode)

**Requirements**:
- Sonoff device in LAN mode (DIY mode or eWeLink LAN).
- Device and Pylae on same network segment.

**API Endpoints** (eWeLink LAN Protocol):

| Action | Method | Endpoint | Payload |
|--------|--------|----------|---------|
| Get Info | GET | `http://{host}:8081/zeroconf/info` | – |
| Turn On | POST | `http://{host}:8081/zeroconf/switch` | `{"deviceid":"...","data":{"switch":"on","outlet":N}}` |
| Turn Off | POST | `http://{host}:8081/zeroconf/switch` | `{"deviceid":"...","data":{"switch":"off","outlet":N}}` |
| Pulse | POST | `http://{host}:8081/zeroconf/pulse` | `{"deviceid":"...","data":{"pulse":"on","pulseWidth":3000,"outlet":N}}` |

**Unlock Sequence**:
```
POST /zeroconf/pulse
{
  "deviceid": "{controller.DeviceId}",
  "data": {
    "pulse": "on",
    "pulseWidth": {door.StrikeDurationMs},
    "outlet": {door.RelayChannel - 1}  // 0-indexed
  }
}
```

### 5.2 Health Monitoring

- Poll each controller every 30 seconds via `/zeroconf/info`.
- Update `Controller.LastSeenUtc` and `Controller.Status`.
- Log `ControllerOffline` / `ControllerOnline` events on status change.
- Show status indicators in UI (green/red/yellow).

### 5.3 Alternative Controllers (Future)

The architecture supports other controller types:
- **Generic HTTP Relay** – Simple GET/POST to trigger relay.
- **MQTT-based** – Publish to topic for unlock.
- **Tasmota firmware** – HTTP API similar to Sonoff.

`ControllerType` field determines which communication driver to use.

---

## 6. Wiegand Reader Integration

### 6.1 Wiegand-26 Protocol

Standard output from EM4100 readers:
- Two data lines: D0 (Data 0) and D1 (Data 1).
- 26 bits total: 1 parity + 8 facility code + 16 card ID + 1 parity.
- Transmitted on card read.

### 6.2 Bridge Options

**Option A: Commercial Wiegand-to-TCP Converter**
- Dedicated device (e.g., Wiegand to Ethernet module).
- Sends card data via TCP/UDP to Pylae.
- Configuration: IP, port, format.

**Option B: ESP32/Arduino Bridge**
- Custom firmware reads Wiegand, sends HTTP POST to Pylae.
- Low cost, flexible.
- Example endpoint: `POST /api/access/card-read`

**Option C: Integrated Controller**
- Some controllers accept Wiegand input directly.
- Requires controller firmware that forwards to Pylae or processes locally.

### 6.3 Card Read Endpoint

```
POST /api/access/card-read
X-Api-Key: {NetworkApiKey}
Content-Type: application/json

{
  "readerId": "reader-main-entrance",
  "cardNumber": "0012345678",
  "facilityCode": "123",        // optional
  "timestamp": "2025-12-06T10:30:00Z"
}
```

**Response**:
```json
{
  "action": "grant",           // or "deny"
  "doorCode": "door-main-entrance",
  "memberId": "abc-123-...",
  "memberName": "John Smith",
  "reason": null               // or denial reason
}
```

Pylae processes the access check and sends unlock command if granted.

---

## 7. UI Requirements

### 7.1 Controllers Management (Admin)

- List all controllers with status indicators.
- Add/edit controller: Code, DisplayName, Host, Port, Type, ChannelCount.
- Test connection button.
- View last seen timestamp.
- Enable/disable controller.

### 7.2 Doors Management (Admin)

- List all doors with status (enabled, controller status).
- Add/edit door:
  - Code, DisplayName, Location.
  - Controller selection (dropdown).
  - Relay channel (1–4).
  - Access mode (Universal/Controlled/Disabled).
  - Strike duration.
  - Schedule (optional).
- Test unlock button.
- View recent access events for door.

### 7.3 Schedules Management (Admin)

- List all schedules.
- Add/edit schedule:
  - Code, DisplayName, Type.
  - For Daily/Weekly: configure time windows.
  - Visual preview of schedule (timeline view).
- Predefined templates: "24/7", "Business Hours", "Weekdays Only".

### 7.4 Access Levels Management (Admin)

- List all access levels with door count, member count.
- Add/edit access level:
  - Code, DisplayName, Description.
  - Priority.
  - Default schedule.
  - Door assignments (multi-select with optional per-door schedule override).
- View members assigned to this level.

### 7.5 Member Access Assignment (Admin)

Integrated into existing Member Editor or separate panel:
- View member's current access levels.
- Add/remove access level assignments.
- Set validity dates (temporary access).
- Set per-member schedule override.
- View member's badge credentials.

### 7.6 Badge Credentials Management (Admin)

Integrated into Member Editor:
- List member's badge credentials.
- Add credential:
  - Card number (manual entry or "scan to assign" mode).
  - Card format.
  - Validity dates.
- Revoke credential (with reason).
- View credential history.

### 7.7 Access Dashboard (Admin/User)

Real-time monitoring panel:
- Door status grid (locked/unlocked, online/offline).
- Recent access events feed (auto-refresh).
- Quick unlock buttons (for operators).
- Filter by door, member, event type.
- Alert indicators for:
  - Controller offline.
  - Door held open.
  - Door forced open.
  - Access denied (configurable alert threshold).

### 7.8 Access Events Log (Admin)

- Searchable/filterable grid.
- Filters: date range, door, member, event type, grant/deny.
- Export to Excel/JSON.
- Detail view showing full event data.

---

## 8. Audit & Logging

### 8.1 AuditLog Integration

The following actions are logged to the existing AuditLog table:

| ActionType | TargetType | Description |
|------------|------------|-------------|
| `ControllerCreated` | Controller | New controller added |
| `ControllerUpdated` | Controller | Controller settings changed |
| `ControllerDeleted` | Controller | Controller removed |
| `DoorCreated` | Door | New door added |
| `DoorUpdated` | Door | Door settings changed |
| `DoorDeleted` | Door | Door removed |
| `ScheduleCreated` | Schedule | New schedule added |
| `ScheduleUpdated` | Schedule | Schedule changed |
| `ScheduleDeleted` | Schedule | Schedule removed |
| `AccessLevelCreated` | AccessLevel | New access level added |
| `AccessLevelUpdated` | AccessLevel | Access level changed |
| `AccessLevelDeleted` | AccessLevel | Access level removed |
| `AccessLevelAssigned` | MemberAccessLevel | Member assigned to level |
| `AccessLevelRevoked` | MemberAccessLevel | Member removed from level |
| `BadgeCredentialIssued` | BadgeCredential | Badge assigned to member |
| `BadgeCredentialRevoked` | BadgeCredential | Badge revoked |
| `ManualUnlock` | Door | Operator unlocked door via UI |

### 8.2 AccessEvents Retention

- Default retention: Same as `VisitRetentionYears` (3 years).
- Separate setting possible: `AccessEventRetentionYears`.
- Cleanup via scheduled service (similar to VisitCleanupService).

---

## 9. Settings Keys

New settings for access control module:

### 9.1 Module Settings

- `AccessControlEnabled` – `0`/`1` (master enable/disable for module).
- `AccessEventRetentionYears` – Default: `3`.

### 9.2 Controller Defaults

- `DefaultStrikeDurationMs` – Default: `3000`.
- `ControllerHealthCheckIntervalSeconds` – Default: `30`.
- `ControllerOfflineThresholdSeconds` – Default: `90`.

### 9.3 Alert Settings

- `AlertOnControllerOffline` – `0`/`1`.
- `AlertOnDoorHeldOpen` – `0`/`1`.
- `DoorHeldOpenThresholdSeconds` – Default: `30`.
- `AlertOnDoorForcedOpen` – `0`/`1`.
- `AlertOnAccessDenied` – `0`/`1`.

---

## 10. HTTP API Endpoints

### 10.1 Card Read (Wiegand Bridge → Pylae)

```
POST /api/access/card-read
X-Api-Key: {NetworkApiKey}

Request:
{
  "readerId": "string",
  "cardNumber": "string",
  "facilityCode": "string",  // optional
  "timestamp": "ISO8601"     // optional, defaults to server time
}

Response (200 OK):
{
  "action": "grant" | "deny",
  "doorId": 123,
  "doorCode": "string",
  "memberId": "guid",
  "memberNumber": 12345,
  "memberName": "string",
  "accessLevelId": 456,
  "accessLevelName": "string",
  "reason": "string"         // denial reason if denied
}
```

### 10.2 Manual Unlock (UI → Pylae → Controller)

```
POST /api/access/doors/{doorId}/unlock
X-Api-Key: {NetworkApiKey}

Request:
{
  "durationMs": 3000,        // optional, overrides door default
  "notes": "string"          // optional, logged with event
}

Response (200 OK):
{
  "success": true,
  "eventId": 789
}
```

### 10.3 Door Status

```
GET /api/access/doors/{doorId}/status
X-Api-Key: {NetworkApiKey}

Response:
{
  "doorId": 123,
  "doorCode": "string",
  "isEnabled": true,
  "accessMode": "Controlled",
  "controllerStatus": "Online",
  "lastEvent": {
    "eventType": "AccessGranted",
    "timestamp": "ISO8601",
    "memberName": "string"
  }
}
```

### 10.4 Access Events Query

```
GET /api/access/events?from=YYYY-MM-DD&to=YYYY-MM-DD&doorId=123&eventType=AccessDenied
X-Api-Key: {NetworkApiKey}

Response:
{
  "events": [...],
  "totalCount": 150,
  "pageSize": 100,
  "page": 1
}
```

---

## 11. Project Structure

### 11.1 New Projects

```
Pylae.sln
├── Pylae.Core                      (existing)
├── Pylae.Data                      (existing)
├── Pylae.Sync                      (existing)
├── Pylae.Desktop                   (existing)
│
├── Pylae.Access.Core               (NEW)
│   ├── Models/
│   │   ├── Controller.cs
│   │   ├── Door.cs
│   │   ├── Schedule.cs
│   │   ├── ScheduleWindow.cs
│   │   ├── AccessLevel.cs
│   │   ├── AccessLevelDoor.cs
│   │   ├── MemberAccessLevel.cs
│   │   ├── BadgeCredential.cs
│   │   └── AccessEvent.cs
│   ├── Interfaces/
│   │   ├── IControllerService.cs
│   │   ├── IDoorService.cs
│   │   ├── IScheduleService.cs
│   │   ├── IAccessLevelService.cs
│   │   ├── IBadgeCredentialService.cs
│   │   ├── IAccessControlService.cs
│   │   └── IAccessEventService.cs
│   ├── Enums/
│   │   ├── AccessMode.cs
│   │   ├── ScheduleType.cs
│   │   ├── AccessEventType.cs
│   │   ├── DenyReason.cs
│   │   ├── ControllerType.cs
│   │   └── ControllerStatus.cs
│   └── Constants/
│       └── AccessSettingKeys.cs
│
├── Pylae.Access.Data               (NEW)
│   ├── Context/
│   │   └── AccessDbContext.cs      (extends master.db or separate)
│   ├── Configurations/
│   │   ├── ControllerConfiguration.cs
│   │   ├── DoorConfiguration.cs
│   │   └── ...
│   ├── Services/
│   │   ├── ControllerService.cs
│   │   ├── DoorService.cs
│   │   ├── ScheduleService.cs
│   │   ├── AccessLevelService.cs
│   │   ├── BadgeCredentialService.cs
│   │   ├── AccessControlService.cs
│   │   └── AccessEventService.cs
│   └── Migrations/
│       └── ...
│
├── Pylae.Access.Hardware           (NEW)
│   ├── Controllers/
│   │   ├── IRelayController.cs
│   │   ├── SonoffController.cs
│   │   └── GenericHttpController.cs
│   ├── Readers/
│   │   ├── ICardReader.cs
│   │   └── WiegandBridgeListener.cs
│   └── Services/
│       ├── ControllerHealthService.cs
│       └── DoorMonitorService.cs
│
└── Pylae.Access.Api                (NEW, optional)
    └── Endpoints/
        ├── CardReadEndpoint.cs
        ├── DoorEndpoints.cs
        └── AccessEventEndpoints.cs
```

### 11.2 Integration Points

- **Pylae.Desktop** references `Pylae.Access.*` projects.
- Access forms added to MainForm navigation.
- Member editor extended with access level/credential panels.
- Background services registered in Program.cs.
- API endpoints added to SyncServer (or separate Access API host).

---

## 12. Example Scenarios

### 12.1 Scenario: Simple Office with Two Doors

**Setup**:
- Door 1: Main Entrance (Universal, 24/7).
- Door 2: Server Room (Controlled, IT Staff only).

**Configuration**:
1. Create Controller: `ctrl-office` → Sonoff at 192.168.1.100.
2. Create Doors:
   - `door-main`: Controller=ctrl-office, Channel=1, Mode=Universal.
   - `door-server`: Controller=ctrl-office, Channel=2, Mode=Controlled.
3. Create Access Level: `it-staff` → includes door-server.
4. Assign IT team members to `it-staff` access level.
5. Issue badge credentials to all members.

**Result**:
- All members badge into main entrance.
- Only IT staff can badge into server room.

### 12.2 Scenario: Warehouse with Time Restrictions

**Setup**:
- Warehouse doors accessible Mon–Fri 06:00–22:00.
- Supervisor has 24/7 access.
- Saturday deliveries need 08:00–12:00 access.

**Configuration**:
1. Create Schedules:
   - `weekday-warehouse`: Weekly, Mon–Fri 06:00–22:00.
   - `saturday-morning`: Weekly, Sat 08:00–12:00.
   - `always`: Always.
2. Create Access Level: `warehouse-staff`, DefaultSchedule=weekday-warehouse.
3. Create Access Level: `warehouse-saturday`, DefaultSchedule=saturday-morning.
4. Assign warehouse workers to `warehouse-staff`.
5. Assign delivery crew to `warehouse-saturday`.
6. Assign supervisor to `warehouse-staff` with MemberAccessLevel.ScheduleId=always (override).

**Result**:
- Workers access Mon–Fri 06:00–22:00.
- Delivery crew access Sat 08:00–12:00.
- Supervisor accesses 24/7.

### 12.3 Scenario: Multi-Site with Central Door

**Setup**:
- Building has main gate (all employees) and department doors (department staff only).

**Configuration**:
1. Door: `door-main-gate`, Mode=Universal.
2. Door: `door-dept-a`, Mode=Controlled.
3. Door: `door-dept-b`, Mode=Controlled.
4. Access Level: `dept-a-staff` → includes door-dept-a.
5. Access Level: `dept-b-staff` → includes door-dept-b.
6. Assign members to their department's access level.

**Result**:
- Everyone badges through main gate.
- Only Dept A staff enter Dept A area.
- Only Dept B staff enter Dept B area.

---

## 13. Non-Goals for v1

- **Elevator control** – Floor access via relay banks (complex, deferred).
- **Anti-passback** – Prevent tailgating by tracking in/out sequence (deferred).
- **Two-person rule** – Require two authorized badges for high-security doors (deferred).
- **Biometric integration** – Fingerprint/face readers (different protocols).
- **Mobile credentials** – Bluetooth/NFC phone-based access (deferred).
- **Visitor self-service kiosk** – Separate UI for visitor check-in (deferred).
- **Real-time video integration** – Camera feed on access events (deferred).

---

## 14. Implementation Phases

### Phase 1: Foundation (Controllers, Doors, Manual Unlock)
- Entities: Controller, Door.
- Services: ControllerService, DoorService, basic SonoffController.
- UI: Controllers form, Doors form, manual unlock in dashboard.
- **Delivers**: Admin can configure hardware, operators can unlock doors manually.

### Phase 2: Access Levels & Schedules
- Entities: Schedule, ScheduleWindow, AccessLevel, AccessLevelDoor.
- Services: ScheduleService, AccessLevelService.
- UI: Schedules form, Access Levels form.
- **Delivers**: Admin can define access policies with time restrictions.

### Phase 3: Badge Credentials & Access Control
- Entities: BadgeCredential, MemberAccessLevel, AccessEvent.
- Services: BadgeCredentialService, AccessControlService, AccessEventService.
- UI: Badge management in Member editor, access assignment panel.
- Integration: Wiegand bridge endpoint, access check flow.
- **Delivers**: Full badge-based access control.

### Phase 4: Monitoring & Alerts
- Services: ControllerHealthService, DoorMonitorService.
- UI: Access Dashboard with real-time status, alerts.
- **Delivers**: Operational monitoring and alerting.

---

## 15. Libraries (Open Source)

- **HTTP Client**: `System.Net.Http` (built-in).
- **JSON**: `System.Text.Json` (built-in).
- **Scheduling**: Existing timer-based pattern from Pylae.Desktop.
- **Wiegand parsing**: Custom implementation (26-bit parsing is straightforward).

No additional third-party libraries required beyond existing Pylae dependencies.

---

*Specification Version: 1.0*
*Date: 2025-12-06*
*Status: Draft*
