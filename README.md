# 🏥 Clinic SaaS API

Production-ready backend API for clinic scheduling, patient management, and SaaS-oriented healthcare operations.

---

## 🚀 Overview

Clinic SaaS API is a modern backend system built with .NET and PostgreSQL, designed to support clinics, physiotherapy studios, and wellness businesses.

The project focuses on delivering a scalable, maintainable, and production-oriented architecture aligned with real-world SaaS applications.

---

## 🎯 Purpose

This project demonstrates:

- Senior-level backend engineering practices
- Clean Architecture and separation of concerns
- Domain-driven design fundamentals
- SaaS-ready backend structure
- Production-grade API design

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
  ClinicSaaS.Api            → HTTP layer (Controllers, Middleware)
  ClinicSaaS.Application    → Use cases, DTOs, orchestration
  ClinicSaaS.Domain         → Core entities and business rules
  ClinicSaaS.Infrastructure → Persistence, EF Core, external services

tests/
  ClinicSaaS.UnitTests
```

### Layer Responsibilities

- **Domain**
  - Entities
  - Business rules
  - Core logic

- **Application**
  - Use cases
  - DTOs
  - Validation

- **Infrastructure**
  - Database access (EF Core)
  - External integrations

- **API**
  - Controllers
  - Request/response handling
  - Swagger documentation

---

## 📦 Features

### Implemented

- Create clinic
- List clinics
- Get clinic by ID
- PostgreSQL integration via EF Core
- Initial database migration
- Swagger documentation

### In Progress

- Patient management
- Appointment scheduling

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

---

## ⚙️ Getting Started

### Prerequisites

- .NET SDK (10+)
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

### Apply migrations

```bash
dotnet ef database update --project src/ClinicSaaS.Infrastructure --startup-project src/ClinicSaaS.Api
```

---

## 🐳 Docker

PostgreSQL is containerized using Docker:

```yaml
services:
  postgres:
    image: postgres:17
    container_name: clinic-saas-postgres
    ports:
      - "5432:5432"
```

---

## 📄 API Documentation

After running the API, access Swagger:

```text
https://localhost:xxxx/swagger
```

---

## 🔌 Example Endpoints

### Create Clinic

```http
POST /api/clinics
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

## 🧭 Roadmap

### Short Term

- Patient CRUD
- Appointment CRUD
- Entity relationships (Clinic → Patients → Appointments)

### Mid Term

- JWT Authentication
- Role-based authorization
- Multi-tenant support (ClinicId isolation)

### Long Term

- Operational dashboard
- No-show tracking
- Financial analytics (expected vs lost revenue)
- SaaS billing model

---

## 🧠 Design Principles

- Clean Architecture
- Separation of concerns
- Domain-first modeling
- API-first design
- Scalability and maintainability
- Production-oriented decisions

---

## 🔐 Security

Planned security features:

- JWT-based authentication
- HTTPS enforcement
- Input validation
- Secure configuration handling

---

## 📊 Why This Project Matters

This project represents a real-world backend system designed with:

- scalable architecture
- production-ready patterns
- SaaS-oriented modeling
- maintainable and extensible design

It goes beyond tutorial-level CRUD and reflects real backend engineering practices.

---

## 👨‍💻 Author

Guilherme Braz  
Senior .NET Engineer  
AI-Assisted Development | Backend Systems | SaaS

---

## 📬 Contact

Available for:

- Backend development
- System modernization
- SaaS platforms
- API architecture

---

## 📄 License

This project is licensed under the MIT License.
