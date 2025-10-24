![](http://images.restapi.co.za/pvt/Noroff-64.png)

# Noroff

# Back-end Development Year 2

## 🏥 Clinic Appointment Booking System

This project is a full-stack web application for a medical clinic group that allows patients to book appointments with doctors **without logging in**.

---

## 📌 Project Description

This repository contains the **back-end API** built with:

- ASP.NET Core (C#)
- Entity Framework Core
- MySQL
- Swagger for documentation

The back-end provides CRUD operations and validation logic for managing appointments, doctors, patients, clinics, and specialties.

---

## 🚀 Technologies Used

| Component     | Technology            |
| ------------- | --------------------- |
| Back-End      | ASP.NET Core (.NET 9) |
| ORM           | Entity Framework Core |
| Database      | MySQL                 |
| Documentation | Swagger (Swashbuckle) |

---

## 📂 Folder Structure (Back-End)

- `Controllers/` – All API endpoint logic
- `Models/` – EF entity classes and navigation
- `DTOs/` – DTOs used for cleaner request/response schemas
- `Filters/` – Swagger example filters for documentation
- `Data/ClinicContext.cs` – EF Core configuration and seed logic
- `Program.cs` – Application entry and middleware

---

## 🔌 API Documentation

Swagger UI is available at:  
📍 http://localhost:5221/doc

All endpoints are documented with:

- Example requests and responses
- Filters showing realistic data (e.g., `name`, `id`, valid `birthDate`, and `appointmentDateTime`)
- All timestamps are stored in UTC and rendered in human-readable format

---

## 🧪 Endpoints

### 🗓 Appointments

- `GET /api/appointments` – Get all appointments
- `POST /api/appointments` – Create a new appointment (validates time & clash)
  ✅ **Validations:**

  - Time must be in `"yyyy-MM-ddTHH:mm"` format (24-hours Oslo time)
  - Appointment must be at least **30 minutes in advance**
  - Appointments must be booked on weekdays (Monday–Friday) between 09:00 and 16:00 (Oslo time)
  - Appointment **must not extend beyond 16:00**
    🟢 `status` can be `"Pending"` or `"Confirmed"`.

    - Defaults to `"Pending"` if not provided.
    - Use `PUT` to update status later or use a custom PATCH if needed.

  - Cannot clash with:
    - another appointment at the **same time** for the **same patient**
    - any **overlapping time** slot for the **same doctor**

- `GET /api/appointments/{id}` – Get a specific appointment by ID
- `PUT /api/appointments/{id}` – Update an appointment
  ✅ **Validations:**
  - Time must be in `"yyyy-MM-ddTHH:mm"` format (Oslo time)
  - Appointment must be at least **30 minutes in advance**
  - Appointments must be booked on weekdays (Monday–Friday) between 09:00 and 16:00 (Oslo time)
  - Appointment **must not extend beyond 16:00**
  - Cannot clash with:
    - another appointment at the **same time** for the **same patient**
    - any **overlapping time** slot for the **same doctor**
- `DELETE /api/appointments/{id}` – Delete an appointment
- `GET /api/appointments/search?date=yyyy-MM-dd` – 🔍 Search appointments by date

### 🏥 Clinics

- `GET /api/clinics` – Get all clinics
- `POST /api/clinics` – Create new clinic (checks for duplicates)
- `GET /api/clinics/{id}` – Get clinic by ID
- `PUT /api/clinics/{id}` – Update clinic
- `DELETE /api/clinics/{id}` – Delete clinic (if no doctors/appointments)

### 🩺 Doctors

- `GET /api/doctors` – Get all doctors
- `POST /api/doctors` – Create a new doctor (validates duplicates)
- `GET /api/doctors/{id}` – Get a doctor by ID
- `PUT /api/doctors/{id}` – Update doctor
- `DELETE /api/doctors/{id}` – Delete doctor (if no appointments)
- `GET /api/doctors/search?name=...` – Search doctor by first or last name (returns full name, clinic, speciality)
- `GET /api/doctors/{id}/availability?date=yyyy-MM-dd` – ⏰ Get 30-minute availability slots for a doctor

### 👤 Patients

- `GET /api/patients` – Get all patients
- `POST /api/patients` – Create a new patient (validates uniqueness)
- `GET /api/patients/{id}` – Get a specific patient
- `PUT /api/patients/{id}` – Update an existing patient
- `DELETE /api/patients/{id}` – Delete patient if no appointments

### 🔬 Specialties

- `GET /api/specialities` – Get all specialities
- `POST /api/specialities` – Create new speciality (validates name uniqueness)
- `GET /api/specialities/{id}` – Get a speciality by ID
- `PUT /api/specialities/{id}` – Update an existing speciality
- `DELETE /api/specialities/{id}` – Delete if no doctors assigned

---

## 📋 Swagger Data Filters (Examples)

### ✅ AppointmentDTO

```json
{
  "id": 0,
  "appointmentDateTime": "2025-05-14T14:30:00", // 🕒 This value is dynamically generated to reflect local (Oslo) time in Swagger
  "durationMinutes": 30,
  "patientId": 0,
  "doctorId": 0,
  "clinicId": 0,
  "status": "Pending"
}
```

### ✅ ClinicDTO

```json
{
  "id": 0,
  "name": "Oslo Clinic",
  "address": "Karl Johans gate 1, 0154 Oslo"
}
```

### ✅ DoctorDTO

```json
{
  "id": 0,
  "firstName": "Ola",
  "lastName": "Nordmann",
  "clinicId": 1,
  "specialityId": 2
}
```

### ✅ PatientDTO

```json
{
  "id": 0,
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "birthDate": "1990-05-14",
  "gender": "Male",
  "taxNumber": "123456789",
  "religion": "None"
}
```

### ✅ SpecialityDTO

```json
{
  "id": 0,
  "name": "Cardiology"
}
```

---

## ▶️ Getting Started

### 1. Apply database migrations

```bash
cd Backend/ClinicBookingSystem
dotnet ef database update
```

### 2. Start the application

```bash
dotnet run
```

### 3. Open Swagger

📍 http://localhost:5221/doc

### 🔄 Reseed the Database (If seed Data Is Missing)

```bash
1. Run `dotnet ef database drop --force`
2. Then run `dotnet ef database update`
3. Then start the app with `dotnet run`
```

If you're running the app for the first time or updated seed logic in `OnModelCreating`, do the following:

```bash
dotnet ef migrations add SeedInitialData
dotnet ef database update
```

---

## Sample Data Seeding

- 📅 Appointments: One sample appointment at 10:00 on 2025-07-21
- 🏥 Clinics: Oslo Clinic, Bergen Clinic
- 🩺 Doctors: Ola Nordmann (Cardiology), Kari Nordmann (Dermatology)
- 👤 Patients: John Doe (`john.doe@example.com`, 1990-05-14)
- 🔬 Specialities: Cardiology, Dermatology

  This allows testing without needing to create entries manually.

---

## 🔒 Privacy

Only non-sensitive PII is stored (First Name, Last Name, Email, Birth Date, Gender, Tax Number, Religion).

## 📚 References

### Core Docs

- [.NET Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [ASP.NET Core Docs](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [Pomelo EF Core MySQL Provider](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)

### Swagger & OpenAPI

- [Swashbuckle Docs](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [OpenAPI Specification](https://swagger.io/specification/)
- [Customizing Swagger in .NET](https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle)

### ASP.NET Core Patterns

- [Middleware in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/)
- [Dependency Injection in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection)
- [Web API Best Practices](https://learn.microsoft.com/en-us/aspnet/core/web-api/)
- [Error Handling in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/web-api/handle-errors)

### Validation & Security

- [Fluent Validation](https://docs.fluentvalidation.net/en/latest/)
- [Data Annotations](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations)
- [EF Core Security](https://learn.microsoft.com/en-us/ef/core/security/sql-injection)

### Logging & Debugging

- [ASP.NET Core Logging](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/)
- [Visual Studio Debugging Guide](https://learn.microsoft.com/en-us/visualstudio/debugger/debugger-feature-tour)

### Tools

- [NuGet Package Explorer](https://github.com/NuGetPackageExplorer/NuGetPackageExplorer)
- [C# Extension for VS Code](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)

### Noroff LMS Materials

- [Database Module 4](https://lms.noroff.no/mod/book/view.php?id=118298&chapterid=24143)
- [HTTP Methods Module 5](https://lms.noroff.no/mod/book/view.php?id=118299&chapterid=24162)

### General Acknowledgments

- [Used OpenAI for guidance](https://chatgpt.com/)
- [Inspired by First-Year Project Exam]
- Emoji icons referenced via:
  - [Unicode Consortium](https://home.unicode.org/emoji/emoji-frequency/)
  - [Emojipedia](https://emojipedia.org/)
