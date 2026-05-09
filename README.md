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
- Design patterns (State, Factory, Strategy, Repository, Command)
- RESTful API design
- SQLite ORM persistence with Entity Framework Core
- Real-time synchronization with Server-Sent Events (SSE)
- Docker volume-based persistence
- Validation and error handling best practices
- Dependency Injection and DTO/Mapper architecture

---

## Architecture

The backend follows a layered architecture:

Controller Layer → HTTP handling and validation  
Service Layer → Business logic and orchestration  
Domain Layer → Core models and state machines  
Infrastructure Layer → ORM persistence and infrastructure services

### Design Pattern Catalog

| Pattern                          | Classes / Files                                                                                                                                                                       | Why It Was Used                                                                                                                                                                                                                                                                                      |
| -------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| State Pattern                    | `SmartHome.Domain/Devices/States/*`, including light, fan, thermostat, and door lock state classes                                                                                    | Each device has valid and invalid transitions. The State pattern keeps state-specific behavior out of controllers and services and makes transitions explicit. For example, a light can only change brightness while it is on, and a thermostat transitions between Off, Idle, Heating, and Cooling. |
| Factory Pattern                  | `IDeviceFactory`, `DeviceFactory`, `IDeviceTypeFactory`, `LightDeviceFactory`, `FanDeviceFactory`, `ThermostatDeviceFactory`, `DoorLockDeviceFactory`                                 | Device creation is centralized instead of scattered through controllers. This supports the Open-Closed Principle because new device types can be added through new factories rather than changing controller logic.                                                                                  |
| Strategy Pattern                 | `IThermostatModeStrategy`, `HeatModeStrategy`, `CoolModeStrategy`, `AutoModeStrategy`, `ThermostatModeStrategyFactory`                                                                | Thermostat behavior changes depending on mode. Heat, Cool, and Auto each decide differently whether the thermostat should heat, cool, or remain idle. Strategy keeps this logic interchangeable without large conditional blocks inside the thermostat.                                              |
| Repository Pattern               | `IDeviceRepository`, `ILocationRepository`, `SqliteRepository`                                                                                                                        | Persistence is hidden behind interfaces so the service layer does not directly depend on database or storage implementation details.                                                                                                                                                                 |
| Command Pattern                  | `IDeviceCommand`, `DeviceCommand`, `TogglePowerCommand`, `SetBrightnessCommand`, `SetFanSpeedCommand`, `SetThermostatModeCommand`, `SetTargetTemperatureCommand`, `ToggleLockCommand` | Device actions are represented as command objects. This makes each operation reusable, testable, and easy to log in command history. The command history stores what operation was performed and when.                                                                                               |
| Dependency Injection             | `Program.cs` service registrations                                                                                                                                                    | Services, repositories, factories, validators, and controllers receive dependencies through constructor injection. This avoids service locator behavior and keeps classes testable.                                                                                                                  |
| Global Error Handling Middleware | `GlobalExceptionHandlingMiddleware`                                                                                                                                                   | Error handling is centralized instead of repeated in every controller. Domain and validation errors are converted into consistent `application/problem+json` responses without leaking stack traces or internal details.                                                                             |
| DTO / Mapper Pattern             | API DTOs and mapper classes such as `DeviceMapper` and `CommandHistoryMapper`                                                                                                         | The API layer does not expose domain objects directly. Mappers translate domain models into response DTOs, keeping HTTP concerns separate from domain logic.                                                                                                                                         |

---

## Running the Application

### Prerequisites

- Docker
- Docker Compose

### Run in Docker (recommended)

```bash
docker compose up --build
```

### Stop the application

```bash
docker compose down
```

---

## Run Local (without Docker)

### Backend

```bash
cd backend/src/SmartHome.Api
dotnet run --launch-profile https
```

### Frontend

```bash
cd frontend/smart-home-ui
npm install
npm run build
```

### Frontend Build Verification

```bash
cd frontend/smart-home-ui
npm run build
```

## Access Points

- Frontend: http://localhost:4200
- Swagger: http://localhost:5001/swagger

---

## Running Tests

Run tests from the backend/ directory with:

### Unit Testing:

`dotnet test backend/tests/SmartHome.Domain.Tests`

### Backend Integration Testing:

`dotnet test tests/SmartHome.Api.Tests/SmartHome.Api.Tests.csproj`

## Frontend Testing:

Run frontend component and service tests from:

```bash
cd frontend/smart-home-ui
ng test
```

### Bruno:

Integration/API testing is performed through the committed Bruno collection in `/bruno/SmartHome API`.

The collection includes requests for:

- device listing, filtering, registration, removal, and control
- invalid validation cases
- command history retrieval
- ambient temperature get/set
- simulation speed changes
- simulation reset
- thermostat, light, fan, and door lock workflows

Run the backend, open the Bruno collection, select the SmartHome API environment, and execute the requests in order.

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

## API Documentation (Swagger)

Swagger UI is available at:

http://localhost:5001/swagger

It provides:

- All endpoints
- Request/response schemas
- Interactive testing

---

## Extra Credit: ORM Persistence

The application supports persistent storage through a SQLite database using Entity Framework Core ORM integration.

Database persistence is container-safe through Docker volume mapping:

```text
/data/SmartHome.db
```

### Features

- State persists across Docker restarts and rebuilds
- Seed data included for demonstration/testing
- EF Core ORM-based repository implementation
- SQLite database stored through mounted Docker volume
- Device and location state persisted across sessions

### ORM Architecture

Persistence responsibilities are isolated behind repository interfaces:

- `IDeviceRepository`
- `ILocationRepository`

The active ORM implementation uses:

- `SqliteRepository`
- `SmartHomeDbContext`
- EF Core SQLite provider

This preserves separation of concerns between:

- controllers
- business logic/services
- persistence infrastructure

while allowing the persistence layer to evolve independently from application logic.

---

## Core Features

<img src="docs/smarthomeUI.png" width="600"/>

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

## Example API Endpoints

| Method | Endpoint                                        | Description                                             |
| ------ | ----------------------------------------------- | ------------------------------------------------------- |
| GET    | `/api/devices`                                  | Retrieve all registered devices with optional filtering |
| GET    | `/api/devices/{id}`                             | Retrieve a single device by ID                          |
| POST   | `/api/devices`                                  | Register a new smart home device                        |
| DELETE | `/api/devices/{id}`                             | Remove an existing device                               |
| POST   | `/api/devices/{id}/commands`                    | Update or control a device state                        |
| PUT    | `/api/locations/{location}/ambient-temperature` | Set the ambient temperature for a location              |
| GET    | `/api/devices/{id}/history`                     | Retrieve command history for a device                   |
| PUT    | `/api/simulation/speed`                         | Change the simulation speed multiplier                  |
| POST   | `/api/simulation/reset`                         | Reset all devices and simulation settings to defaults   |

---

## Repository Structure

```text
README.md
docker-compose.yml
.gitignore

/backend
  /src
    /SmartHome.Api
    /SmartHome.Domain
    /SmartHome.Infrastructure

  /tests
    /SmartHome.Domain.Tests
    /SmartHome.Api.Tests

/frontend
  /smart-home-ui
    /src
    package.json
    angular.json

/bruno

/data
  SmartHome.db
```

---

## Demo & Architecture Walkthrough Videos

[Application Demo](https://www.loom.com/share/548398bc6e494c6aac0fa27718c3252a)

[Architecture Walkthrough](https://www.loom.com/share/580b6177a3204353a16452df96d730af)

---

## Extra Credit: Server-Sent Events (SSE) Real-Time Synchronization

As a late-stage enhancement after the primary demo videos were recorded, the application was extended with Server-Sent Events (SSE) support for real-time dashboard synchronization across connected clients.

The final implementation supports both ORM persistence and SSE-based real-time synchronization simultaneously.

### Functionality

- Connected dashboards subscribe to a shared SSE event stream.
- Device state changes are broadcast by the backend in real time.
- All connected clients automatically refresh when:
  - devices are controlled
  - devices are added or removed
  - the simulation is reset
- Synchronization works across multiple browser tabs or devices viewing the application simultaneously.

### Implementation Overview

The backend exposes an SSE endpoint:

```http
GET /api/devices/events
```

When device state changes occur, the API broadcasts update events to all connected SSE clients using a singleton event broadcaster service.

The Angular frontend subscribes to the SSE stream using the browser EventSource API and reactively reloads dashboard state when events are received.

### Notes:

This feature was implemented after the original Loom demonstrations were recorded and may not appear in the submitted demo videos.

## Known Issues

The Angular frontend may log a non-blocking change detection warning related to the simulated clock. Core functionality is unaffected.
