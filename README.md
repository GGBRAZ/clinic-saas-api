# 🏥 Clinic SaaS API

Production-ready backend API for clinic scheduling, patient management, attendance tracking, financial visibility, and SaaS-oriented clinic operations.

---

## 🚀 Overview

Clinic SaaS API is a modern backend system built with .NET and PostgreSQL, designed to support clinics, physiotherapy studios, and pilates businesses.

The project focuses on delivering a scalable, maintainable, and production-oriented architecture aligned with real-world SaaS applications. It models the operational flow of a clinic, from patient registration to appointment management, attendance tracking, and financial analytics.

---

## 🎯 Purpose

This project demonstrates:

- Senior-level backend engineering practices
- Clean Architecture and separation of concerns
- Domain-driven design fundamentals
- SaaS-ready backend structure
- Multi-tenant clinic context resolution through JWT claims
- Production-grade API design
- Business-oriented metrics such as lost revenue and no-show rate

---

## 🧱 Tech Stack

| Layer | Technology |
|---|---|
| Backend | .NET 10 / ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Database | PostgreSQL |
| Containerization | Docker |
| API Documentation | OpenAPI + NSwag UI |
| Authentication | JWT Bearer |
| Logging | Serilog |
| Validation | FluentValidation (planned) |

---

## 🏗️ Architecture

The project follows a modular layered architecture:

```text
src/
  ClinicSaaS.Api            → HTTP layer (Controllers, Auth, API documentation)
  ClinicSaaS.Application    → DTOs and application contracts
  ClinicSaaS.Domain         → Core entities, enums, and business rules
  ClinicSaaS.Infrastructure → Persistence, EF Core, DbContext

tests/
  ClinicSaaS.UnitTests
```

### Layer Responsibilities

#### Domain
- Entities
- Business rules
- Status transitions
- Core modeling

#### Application
- DTOs
- Input/output contracts
- API-facing models

#### Infrastructure
- Database access
- Entity mappings
- EF Core configuration

#### API
- Controllers
- Request/response orchestration
- JWT authentication
- OpenAPI / NSwag documentation

---

## 📦 Features

### ✅ Implemented

- Create clinic
- List clinics
- Get clinic by ID
- Create patient
- List patients by clinic context
- Get patient by ID
- Create appointment
- List appointments by clinic context
- Get appointment by ID
- Reschedule appointment
- Track appointment history
- Mark appointment as completed
- Mark appointment as no-show
- Mark appointment as canceled
- Financial dashboard
- Financial dashboard by period
- No-show rate calculation
- Clinic context resolved from JWT claim
- PostgreSQL integration via EF Core
- Initial migrations
- OpenAPI documentation with NSwag UI

### 🚧 Planned

- Role-based authorization refinement
- Audit trail improvements
- Front-end dashboard
- SaaS billing model

---

## 🧩 Domain Model

### Clinic

```text
Id
Name
Slug (unique)
Email
Phone
CreatedAt
```

### Patient

```text
Id
ClinicId
FullName
Phone
Email
BirthDate
Notes
CreatedAt
```

### Appointment

```text
Id
ClinicId
PatientId
Date
StartTime
EndTime
ExpectedAmount
Status
Notes
CreatedAt
```

### Appointment History

```text
Id
AppointmentId
Action
OldStatus
NewStatus
OldDate
NewDate
OldStartTime
NewStartTime
OldEndTime
NewEndTime
Notes
CreatedAt
```

### Appointment Status

```text
Scheduled
Completed
Canceled
NoShow
```

---

## 🧠 Authentication and Clinic Context

The API uses JWT Bearer authentication.

The authenticated token carries the clinic context through the `clinic_id` claim, which is resolved by the API at request time. This replaces the earlier manual tenant header approach and keeps authentication and tenant context aligned in a single token.

### Current JWT Claims

```text
sub
email
role
clinic_id
```

### Benefits

- No manual clinic header required
- Reduced risk of token/header mismatch
- Cleaner multi-tenant request flow
- Better production alignment for SaaS APIs

---

## ⚙️ Getting Started

### Prerequisites

- .NET SDK 10+
- Docker Desktop
- Git

### Run PostgreSQL

```bash
docker compose up -d
```

### Run the API

```bash
dotnet run --project src/ClinicSaaS.Api
```

### Apply Migrations

```bash
dotnet ef database update --project src/ClinicSaaS.Infrastructure --startup-project src/ClinicSaaS.Api
```

---

## 🐳 Docker

PostgreSQL is containerized using Docker:

```yaml
version: "3.9"

services:
  postgres:
    image: postgres:17
    container_name: clinic-saas-postgres
    environment:
      POSTGRES_DB: clinicsaasdb
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
    volumes:
      - clinicsaas_pgdata:/var/lib/postgresql/data

volumes:
  clinicsaas_pgdata:
```

---

## 📄 API Documentation

The API exposes an OpenAPI document with an interactive UI powered by NSwag.

### Development URL

```text
http://localhost:5029/swagger
```

### Authentication in the API UI

Use the authentication endpoint to obtain a JWT token:

```http
POST /api/Auth/login
```

Then authorize requests in the API UI using the returned access token.

---

## 🔌 Example Endpoints

### Login

```http
POST /api/Auth/login
```

#### Request

```json
{
  "email": "admin@clinic.com",
  "password": "123456"
}
```

#### Response

```json
{
  "accessToken": "jwt-token-here",
  "expiresAtUtc": "2026-04-15T01:00:00Z"
}
```

---

### Create Clinic

```http
POST /api/Clinics
```

#### Request

```json
{
  "name": "Studio Equilibrio Vital",
  "slug": "studio-equilibrio-vital",
  "email": "contato@equilibriovital.com",
  "phone": "+55 21 99999-9999"
}
```

---

### Get Patients

```http
GET /api/Patients
Authorization: Bearer <jwt-token>
```

#### Response

```json
[
  {
    "id": "guid",
    "clinicId": "guid",
    "fullName": "Mariana Costa",
    "phone": "+55 21 98888-1111",
    "email": "mariana.costa@email.com",
    "birthDate": "1992-08-15T00:00:00",
    "notes": "Pilates twice a week",
    "createdAt": "2026-04-11T02:55:14Z"
  }
]
```

---

### Create Appointment

```http
POST /api/Appointments
Authorization: Bearer <jwt-token>
```

#### Request

```json
{
  "patientId": "patient-guid",
  "date": "2026-04-12T00:00:00Z",
  "startTime": "09:00:00",
  "endTime": "10:00:00",
  "expectedAmount": 150.00,
  "notes": "Initial pilates evaluation"
}
```

---

### Reschedule Appointment

```http
PATCH /api/Appointments/{id}/reschedule
Authorization: Bearer <jwt-token>
```

#### Request

```json
{
  "date": "2026-04-21T00:00:00Z",
  "startTime": "14:00:00",
  "endTime": "15:00:00",
  "notes": "Patient requested a new time"
}
```

---

### Appointment History

```http
GET /api/Appointments/{id}/history
Authorization: Bearer <jwt-token>
```

---

### Financial Dashboard

```http
GET /api/dashboard/financial
Authorization: Bearer <jwt-token>
```

#### Response

```json
{
  "totalRevenue": 150.00,
  "lostRevenue": 180.00,
  "noShowCount": 1,
  "completedCount": 1
}
```

---

## 📊 Business Metrics

The project already supports business-oriented indicators such as:

- Completed appointments
- No-show appointments
- Canceled appointments
- Scheduled appointments
- Total revenue
- Lost revenue
- No-show rate
- Period-based financial analysis
- Appointment lifecycle tracking
- Rescheduling audit trail

These metrics make the project more aligned with real SaaS products rather than tutorial-style CRUD applications.

---

## 🧪 Suggested Test Flow

A practical end-to-end validation flow:

1. Authenticate and obtain a JWT token
2. Create a clinic
3. Create patients
4. Create appointments
5. Reschedule an appointment
6. Mark one appointment as completed
7. Mark one appointment as no-show
8. Mark one appointment as canceled
9. Query appointment history
10. Query the financial dashboard

This flow validates both technical correctness and business behavior.

---

## 🧭 Roadmap

### Short Term

- Authorization refinement
- Better validation rules
- Audit trail improvements
- Stronger production hardening

### Mid Term

- Operational dashboard UI
- Revenue trend analysis
- No-show forecasting
- Tenant-aware user model

### Long Term

- SaaS billing and subscription model
- Advanced reporting
- Notification workflows
- Scheduling intelligence

---

## 🧠 Design Principles

- Clean Architecture
- Separation of concerns
- Domain-first modeling
- API-first design
- Multi-tenant awareness
- Scalability and maintainability
- Production-oriented decisions
- Business-driven backend design

---

## 🔐 Security

The API uses JWT Bearer authentication with:

- issuer validation
- audience validation
- token lifetime validation
- signing key validation
- clinic context resolved from the `clinic_id` claim

This avoids manual tenant headers and keeps authentication and clinic scope consistent in the same token.

---

## 📊 Why This Project Matters

This project is not a tutorial CRUD.

It represents a real-world backend system designed with:

- scalable architecture
- business-oriented modeling
- JWT-based clinic context
- production-ready patterns
- operational and financial insights
- appointment lifecycle tracking

It demonstrates the mindset required to build SaaS products, not just APIs.

---

## 👨‍💻 Author

**Guilherme Braz**  
Senior .NET Engineer  
AI-Assisted Development | Backend Systems | SaaS

---

## 📬 Contact

Available for work involving:

- Backend development
- System modernization
- SaaS platforms
- API architecture
- Operational systems
- Multi-tenant backend design

---

## 📄 License

This project is licensed under the MIT License.
