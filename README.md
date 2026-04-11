# 🏥 Clinic SaaS API

Production-ready backend API for clinic scheduling, patient management, attendance tracking, and SaaS-oriented clinic operations.

---

## 🚀 Overview

Clinic SaaS API is a modern backend system built with .NET and PostgreSQL, designed to support clinics, physiotherapy studios, and pilates businesses.

The project focuses on delivering a scalable, maintainable, and production-oriented architecture aligned with real-world SaaS applications. It models the operational flow of a clinic, from patient registration to appointment management and financial visibility.

---

## 🎯 Purpose

This project demonstrates:

- Senior-level backend engineering practices
- Clean Architecture and separation of concerns
- Domain-driven design fundamentals
- SaaS-ready backend structure
- Multi-tenant request isolation
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
| API Documentation | Swagger / OpenAPI |
| Logging | Serilog (planned) |
| Validation | FluentValidation (planned) |

---

## 🏗️ Architecture

The project follows a modular layered architecture:

```text
src/
  ClinicSaaS.Api            → HTTP layer (Controllers, Middleware, Swagger)
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
- Swagger
- Multi-tenant request handling

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
- Mark appointment as completed
- Mark appointment as no-show
- Mark appointment as canceled
- Financial dashboard
- Financial dashboard by period
- No-show rate calculation
- Multi-tenant request isolation via header
- PostgreSQL integration via EF Core
- Initial migrations
- Swagger documentation

### 🚧 Planned

- Reschedule rules
- Attendance history
- JWT authentication
- Role-based authorization
- Audit trail
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

### Appointment Status

```text
Scheduled
Completed
Canceled
NoShow
```

---

## 🧠 Multi-Tenant Design

The API uses a basic multi-tenant strategy based on the request header:

```http
X-Clinic-Id: <clinic-guid>
```

This means:

- the active clinic is resolved from the request context
- patient and appointment operations are isolated by clinic
- dashboard metrics are calculated for the active clinic
- tenant context no longer needs to be sent in request bodies for patient and appointment creation

### Endpoints that require `X-Clinic-Id`

- `/api/Patients`
- `/api/Appointments`
- `/api/dashboard/financial`
- `/api/dashboard/financial-by-period`

### Endpoints that do not require `X-Clinic-Id`

- `/api/Clinics`

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

After running the API, access Swagger at:

```text
https://localhost:xxxx/swagger
```

Swagger is configured to support the `X-Clinic-Id` header on multi-tenant endpoints.

---

## 🔌 Example Endpoints

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

#### Response

```json
{
  "id": "guid",
  "name": "Studio Equilibrio Vital",
  "slug": "studio-equilibrio-vital",
  "email": "contato@equilibriovital.com",
  "phone": "+55 21 99999-9999",
  "createdAt": "2026-01-01T00:00:00Z"
}
```

---

### Create Patient

```http
POST /api/Patients
X-Clinic-Id: <clinic-guid>
```

#### Request

```json
{
  "fullName": "Mariana Costa",
  "phone": "+55 21 98888-1111",
  "email": "mariana.costa@email.com",
  "birthDate": "1992-08-15T00:00:00Z",
  "notes": "Pilates twice a week"
}
```

---

### Create Appointment

```http
POST /api/Appointments
X-Clinic-Id: <clinic-guid>
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

### Mark Appointment as NoShow

```http
PATCH /api/Appointments/{id}/noshow
X-Clinic-Id: <clinic-guid>
```

---

### Financial Dashboard

```http
GET /api/dashboard/financial
X-Clinic-Id: <clinic-guid>
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

### Financial Dashboard by Period

```http
GET /api/dashboard/financial-by-period?startDate=2026-04-12&endDate=2026-04-13
X-Clinic-Id: <clinic-guid>
```

#### Response

```json
{
  "clinicId": "guid",
  "startDate": "2026-04-12T00:00:00",
  "endDate": "2026-04-13T00:00:00",
  "totalAppointments": 4,
  "completedCount": 1,
  "noShowCount": 1,
  "canceledCount": 1,
  "scheduledCount": 1,
  "totalRevenue": 150.00,
  "lostRevenue": 180.00,
  "noShowRate": 25.00
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

These metrics make the project more aligned with real SaaS products rather than tutorial-style CRUD applications.

---

## 🧪 Suggested Test Flow

A practical end-to-end validation flow:

1. Create a clinic
2. Create patients using `X-Clinic-Id`
3. Create appointments using `X-Clinic-Id`
4. Mark one appointment as completed
5. Mark one appointment as no-show
6. Mark one appointment as canceled
7. Query the financial dashboard
8. Query the period-based dashboard

This flow validates both technical correctness and business behavior.

---

## 🧭 Roadmap

### Short Term

- Reschedule endpoint
- Attendance history
- Audit trail for appointment state changes
- Better validation rules

### Mid Term

- JWT authentication
- Role-based authorization
- Tenant-aware user model
- Global error handling middleware

### Long Term

- Operational dashboard UI
- Revenue trend analysis
- No-show forecasting
- SaaS billing and subscription model

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

Current and planned security considerations:

- Tenant context isolated via request header
- DTO-based input boundaries
- Request validation
- Planned JWT authentication
- Planned role-based access control
- Planned secure configuration handling

---

## 📊 Why This Project Matters

This project is not a tutorial CRUD.

It represents a real-world backend system designed with:

- scalable architecture
- business-oriented modeling
- multi-tenant readiness
- production-ready patterns
- operational and financial insights

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
