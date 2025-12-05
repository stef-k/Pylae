# Pylae – Door Access Control System  
## Bill of Materials (BOM)

This document lists all required required parts for retrofitting a door with:
- Fail-secure 12V electric strike  
- 125 kHz EM RFID reader (Wiegand-26)  
- Exit button  
- Door contact  
- Shared controller (Sonoff 4CH Pro R3)  
- Shared 12V PSU  

Prices are approximate based on the Greek market (Skroutz/Shopflix).

---

## 1. Parts List (with links)

| Part Name | Description | Typical Price (EUR) | Search Link |
|-----------|-------------|---------------------|-------------|
| **Fail-secure Electric Strike (12V DC)** | Frame-mounted “κυπρί”, compatible with existing mechanical lock; stays locked on power loss | 17–30 € | https://www.skroutz.gr/search?keyphrase=ηλεκτρικό+κυπρί+12V+DC |
| **RFID Reader (125 kHz EM, Wiegand-26, IP65)** | Cheapest and simplest reader type; 4-wire data (D0/D1) | 18–35 € | https://www.skroutz.gr/search?keyphrase=rfid+reader+125khz+wiegand |
| **Exit Button (NO contact)** | Allows egress independent of network; wired in parallel to relay | 5–15 € | https://www.skroutz.gr/search?keyphrase=μπουτόν+εξόδου |
| **Door Contact (Magnetic Reed Switch)** | Detects door open/closed state; useful for “door left open” alerts | 2–6 € | https://www.skroutz.gr/search?keyphrase=μαγνητική+επαφή+πόρτας |
| **UTP Cable (Cat5e/Cat6)** | Carries reader + contact signals | 0.30–0.60 €/m | https://www.skroutz.gr/search?keyphrase=utp+cat6 |
| **2×0.75 mm² Cable** | Power cable for electric strike | 0.50–1.00 €/m | https://www.skroutz.gr/search?keyphrase=2x0.75+καλώδιο |
| **Junction Box / Surface Box** | Small box near door for terminations | 2–5 € | https://www.skroutz.gr/search?keyphrase=κουτί+συνδέσεων |
| **Sonoff 4CH Pro R3** | 4-channel relay controller; each relay drives one strike | 25–35 € | https://www.skroutz.gr/search?keyphrase=Sonoff+4CH+Pro+R3 |
| **12V DC PSU (5A or 10A)** | Powers multiple strikes and readers | 8–15 € | https://www.skroutz.gr/search?keyphrase=τροφοδοτικό+12V+5A |
| **RFID Cards / Keyfobs (EM4100)** | Compatible cards/fobs for 125 kHz readers | 1–2 €/piece | https://www.skroutz.gr/search?keyphrase=EM4100+125khz+cards |

---

## 2. Per-Door Cost Summary

| Item | Price Range |
|------|-------------|
| Electric Strike | 17–30 € |
| RFID Reader | 18–35 € |
| Exit Button | 5–15 € |
| Door Contact | 2–6 € |
| Cabling & Junction Box | 10–15 € |

**Per-door total:** **≈ 57–62 €**

---

## 3. Shared Hardware Cost Allocation

| Shared Item | Total Cost | Cost per Door (4–8 doors) |
|-------------|------------|----------------------------|
| 12V DC PSU (5A–10A) | 8–15 € | 1–3 € |
| Sonoff 4CH Pro R3 | 25–35 € | 5–9 € |

**Shared contribution per door:** **≈ 7–12 €**

---

## 4. Final Cost Per Door

### **≈ 67–72 € per door** (typical installation)

With professional wiring cabinet:
### **≈ 70–80 € per door**

---
