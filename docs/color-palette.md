# UI Color System (Accessible Design)

This project uses a color palette designed for:
- Readability (WCAG contrast compliant)
- Color vision deficiency support (deuteranopia-friendly)
- Clear UI state separation (no reliance on red/green)

---

## Palette Preview

<img src="palette.png" width="600"/>

---

## Color Roles

| Purpose        | Color     | Usage |
|---------------|----------|------|
| Background     | `#FFFFFF` | App background |
| Card Surface   | `#D1D5DB` | Cards, panels |
| Primary Text   | `#343D4C` | Headings, body text |
| Action (Primary) | `#134ECD` | Buttons, toggles, links |
| State / Accent | `#A1683A` | Active states, thermostat, emphasis |

---

## Design Principles

### 1. Do not rely on color alone
All states must include:
- Text labels (e.g., **ON / OFF**)
- Icons (💡 🔒 🌡)

---

### 2. Single Responsibility per Color

- **Blue (`#134ECD`)** → interaction (clickable elements)
- **Brown (`#A1683A`)** → system state (active / heating)
- **Gray / White** → layout & structure
- **Dark Text (`#343D4C`)** → readability

---

### 3. Avoid Problematic Colors

The following are intentionally avoided:
- Green (collapses in deuteranopia)
- Red/Green combinations
- Yellow/Lime accents (shift unpredictably)



