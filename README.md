🏨 Hotel Management System (HMS) – Backend API

A production-ready Hotel Management System (HMS) backend built using ASP.NET Core, following Clean Architecture and Agile module-based development principles.

The system manages hotel operations such as room management, bookings, services, authentication, feedback moderation, logging, and deployment readiness.


🚀 Tech Stack

ASP.NET Core Web API

Entity Framework Core

SQL Server

ASP.NET Identity & JWT Authentication

AutoMapper

Serilog (Structured Logging)

Clean Architecture

Agile (Module-based development)


🧱 Architecture

The project follows Clean Architecture with clear separation of concerns:
HMS
│
├── Core (Domain)
│   ├── Entities
│   ├── Enums
│   └── Interfaces
│
├── Application
│   ├── DTOs
│   ├── Services
│   └── Mappings
│
├── Infrastructure
│   ├── Data
│   ├── Repositories
│   └── Identity
│
└── API
    ├── Controllers
    ├── Middleware
    └── Configuration



📦 Implemented Modules
✅ Module 1: Authentication & Security

User registration & login

JWT authentication

Role-based authorization (Admin, Staff, Guest)

Secure endpoints using [Authorize]

✅ Module 2: Room Management

Manage hotel rooms

Room images support

Filtering, sorting, and pagination

Admin-only access for management

✅ Module 3: Booking Management

Guest room bookings

Booking lifecycle management

Availability validation

✅ Module 4: Feedback Management

Guest feedback submission

AI moderation integration (OpenAI Moderation API)

Safe fallback handling when AI service is unavailable

✅ Module 5: Services Management

Admin manages hotel services

Guests request hotel services

Admin assigns staff to service requests

Staff updates service request status


🔐 Security Features

JWT-based authentication

Role-based access control

Secure handling of secrets using environment variables

Global error handling without exposing internal details


🧪 Testing

API tested using Postman

Role-based test scenarios for:

Admin

Staff

Guest

Clear request/response validation

Clear role separation and business rules 


🛠 Logging & Monitoring

Centralized error logging

Daily rolling log files

Slow request detection middleware

Clean and structured logs for production debugging
