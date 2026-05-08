# Smart Home Simulator

## Team Size

Work completed by a team of two.

## Overview

The Smart Home Simulator is a full-stack application that models and controls household devices such as lights, fans, thermostats, and door locks.

Users can:

- View devices grouped by location
- Control device behavior (power, brightness, speed, lock state, temperature)
- Simulate environmental conditions (ambient temperature)
- Track device command history
- Interact with a RESTful API backend

This project demonstrates:

- SOLID principles
- Clean layered architecture
- Design patterns (State, Factory, Strategy)
- RESTful API design
- JSON-based persistence
- Validation and error handling best practices

---

## Architecture

The backend follows a layered architecture:

Controller Layer → HTTP handling and validation  
Service Layer → Business logic and orchestration  
Domain Layer → Core models and state machines  
Infrastructure Layer → JSON persistence

Key patterns used:

- State Pattern → device state machines
- Factory Pattern → device creation (IDeviceFactory + type factories)
- Strategy Pattern → thermostat modes (heat/cool/auto)
- Repository Pattern → persistence abstraction

---

## Running the Application

### Prerequisites

- Docker
- Docker Compose

### Run (recommended)

```bash
docker compose up --build
```

### Stop the application

```bash
docker compose down
```

---

## Access Points

- Frontend: http://localhost:4200
- Swagger: http://localhost:5001/swagger

---

## Running Tests

```bash
dotnet test backend/tests/SmartHome.Domain.Tests
```

---

## Local Development (without Docker)

```bash
cd backend/src/SmartHome.Api
dotnet run
```

---

## API Documentation (Swagger)

Swagger UI is available at:

http://localhost:5001/swagger

It provides:

- All endpoints
- Request/response schemas
- Interactive testing

---

## API Testing (Bruno)

Bruno collections are located at:

```text
/bruno
```

To use:

1. Open Bruno
2. Open the `/bruno` folder
3. Run requests for:
   - Devices
   - Locations
   - Simulation
   - Command history

The collection includes both:

- Success cases
- Error cases

---

## Persistence

Uses JSON file storage.

File location:

```text
/data/smarthome.json
```

Features:

- State persists across restarts
- Device dehydration and rehydration
- Seed data included

---

## Core Features

### Device Types

- Light (brightness, color)
- Fan (speed)
- Thermostat (mode, temperature, state machine)
- Door Lock (locked/unlocked)

### Supported Operations

- List devices
- Get device by ID
- Register device
- Delete device
- Control device state
- Set ambient temperature
- View command history

---

## Example Endpoints

```text
GET    /api/devices
GET    /api/devices/{id}
POST   /api/devices
DELETE /api/devices/{id}
PUT    /api/devices/{id}/state

PUT    /api/locations/{location}/ambient-temperature

GET    /api/devices/{id}/history
```

---

## Repository Structure

```text
/backend
  /src
  /tests

/bruno
/data
```

---

## Demo & Architecture Walkthrough

Application Demo:  
// TODO: [INSERT LINK]

Architecture Walkthrough:  
// TODO: [INSERT LINK]

---
