# Pylae – Access Control System Design & Integration Specification
*(v1.0 — 2-page functional & technical spec)*

## 1. Overview
Pylae provides a unified software and hardware solution for access control across manned and unmanned doors.  
The system supports:
- Badge-based access (125 kHz EM readers, Wiegand-26)
- Remote unlock via Pylae UI (manned gate/reception flows)
- Automatic access enforcement through Sonoff-based relay controllers
- Unlimited door scalability
- Mechanical fallback (existing key cylinders always work)
- Event/audit logging & per-account permissions

Pylae does not replace the mechanical lock; it adds a network-controlled, fail-secure electric strike while preserving full key functionality.

---

## 2. Hardware Architecture

### Per Door
- **Fail-secure electric strike (12V DC)** — remains locked during power loss; unlocks with key.
- **125 kHz EM RFID reader (Wiegand-26)** — transmits badge ID.
- **Exit button (NO)** — local egress, independent of software.
- **Door contact sensor** — detects door open/closed/held-forced states.

### Controller Bank (Shared)
- **Sonoff 4CH Pro R3 modules** — each provides 4 relay-controlled strike outputs.
- **12V DC PSU (5A–10A)** — powers multiple door components.
- **Terminal blocks + cabinet** — structured wiring environment.

### Scalability
The system supports an unlimited number of doors by adding additional Sonoff modules.  
Each module is a Pylae-controlled endpoint.

---

## 3. Software Architecture

### Core Entities
- **Door** — metadata + corresponding relay channel + reader ID.
- **Access Level** — groups of doors (e.g., “All Doors”, “Staff Only”, etc.).
- **Account** — user/visitor with one or more access levels.
- **Badge Credential** — EM4100 ID mapped to an account.
- **Event Log** — records all access attempts and manual unlocks.

---

## 4. Access Flows

### Unmanned Door Flow (Badge Tap)
1. User taps badge.
2. Reader → controller sends credential to Pylae.
3. Pylae checks access rules:
   - Account identity
   - Assigned doors
   - Schedule/time validity
4. If allowed → Pylae signals controller → strikes unlock.
5. Event logged.
6. Door contact confirms open/close.

**Fallback:** Mechanical key always works.

---

### Manned Door Flow (Reception/Guard)
1. Operator opens Pylae control panel.
2. Selects door → presses **Unlock**.
3. Pylae commands controller → relay pulses → strike activates.
4. Event logged with operator identity.

Used for:
- Visitor arrivals
- Deliveries
- Overrides

---

### Mixed Mode
Every door supports:
- Badge access
- Manual unlock via software
- Mechanical override with key

---

## 5. Roles

### Admin
- Full system access
- Manages doors, controllers, access levels, assignments

### Manager/Security
- Unlock doors manually
- Monitor events
- Manage badges (if permitted)

### Regular User
- Badge access according to permissions

### Visitor
- Temporary badge/QR code
- Time-limited access

---

## 6. Fallback & Safety

| Scenario | Behavior |
|---------|----------|
| Power failure | Strike remains locked; key works |
| Network failure | Badge unlock fails; key & exit button work |
| Server offline | No badge access; key works |
| Controller fault | Only affects specific door; key works |

No single point of failure removes access completely because the **mechanical lock always works**.

---

## 7. Deployment Summary
- Per-door hardware cost: **~57–62 €**
- Shared controller/PSU: **~7–12 € per door**
- Typical total: **67–72 € per door**
- Professional cabinet wiring: **70–80 € per door**

---

## 8. Benefits
- Works with existing doors (minimal modification)
- Scales from 1 to hundreds of doors
- Supports manned & unmanned environments
- Preserves full mechanical fallback
- Generates complete audit trails
- Flexible software-permissions model

---

*Generated: 2025-12-05 08:12*
