![](http://images.restapi.co.za/pvt/Noroff-64.png)

# Noroff

# Back-end Development Year 2

This is a full-stack Clinic Appointment Booking System created for the final year assessment. It includes a React front-end and an ASP.NET Core REST API back-end using MySQL with Entity Framework.

### Setup Frontend

```bash
cd frontend/clinic-booking-frontend
npm install
npm run dev
```

---

## 📁 Project Structure

- **Frontend**: React, TypeScript, Vite, Axios, React Router, Zustand
- **Backend**: ASP.NET Core Web API, Entity Framework Core (Code-First), MySQL
- **Styling**: Tailwind CSS, CSS Modules, custom animations
- **API Documentation**: Swagger (http://localhost:5221/doc)

---

## 🩺 Features

### ✅ Booking Appointments

- Patients can book appointments without login/registration.
- Required info: first name, last name, email, birth date, gender, tax number, religion.
- Pick a doctor (fetched dynamically), clinic, appointment date & time, duration.
- Form validations:
  - No overlapping appointments per doctor.
  - Business hours enforced (8:00 - 16:00).
  - Real-time feedback for availability.
- Calendar displays real-time booked slots with:
  - 🔴 Red underlines on fully/partially booked dates
  - ⛔️ Unavailable times grayed or marked as taken in the time selector

### ✅ Doctor Search

- Search doctors by first or last name.
- Shows full name, clinic name, and speciality.
- Results fetched from API.

### ✅ Navigation & Layout

- Responsive layout with a reusable NavBar and footer.
- Smooth loaders, fade-in transitions.
- Custom Contact and About pages.

### ✅ Admin Features

- View list of all appointments.
- Edit, delete, or confirm appointment status.
- **Admins can only edit appointments within business hours (09:00 - 16:00 Oslo time)**.
- Friendly validation messages like:
  - `⏰ You can only edit bookings between 09:00 and 16:00 Oslo time.`
  - `❌ You can only edit up to X minutes as the clinic closes at 16:00.`
- Admin layout with protected views and visual feedback.

---

## 🔗 Routes

| Path                           | Description                  |
| ------------------------------ | ---------------------------- |
| `/`                            | Redirects to `/home`         |
| `/home`                        | Welcome page with CTA        |
| `/book`                        | Book an appointment form     |
| `/search`                      | Search doctor by name        |
| `/about`                       | About NorClinic              |
| `/contact`                     | Contact info page            |
| `/admin/appointments`          | Admin view/edit appointments |
| `/admin/appointments/edit/:id` | Admin edit form              |
| `*`                            | 404 fallback route           |

---

## ⚙️ API Endpoints (Back-End)

### AppointmentsController

| Method | Endpoint                 | Description                          |
| ------ | ------------------------ | ------------------------------------ |
| GET    | `/api/appointments`      | List all appointments                |
| GET    | `/api/appointments/{id}` | Get appointment by ID                |
| POST   | `/api/appointments`      | Create appointment (with validation) |
| PUT    | `/api/appointments/{id}` | Update appointment (with validation) |
| DELETE | `/api/appointments/{id}` | Delete appointment                   |

### DoctorsController

| Method | Endpoint                         | Description      |
| ------ | -------------------------------- | ---------------- |
| GET    | `/api/doctors`                   | List all doctors |
| GET    | `/api/doctors/{id}`              | Get doctor by ID |
| POST   | `/api/doctors`                   | Create doctor    |
| PUT    | `/api/doctors/{id}`              | Update doctor    |
| DELETE | `/api/doctors/{id}`              | Delete doctor    |
| GET    | `/api/doctors/search?name=Smith` | Search by name   |

(Same CRUD pattern for Patients, Clinics, and Specialities)

---

## 📄 Validation & Business Logic

- No duplicate records (enforced with EF Core validation).
- No conflicting appointments per doctor and time.
- Enforced working hours (08:00 - 16:00) on appointments.
- Clean error responses with useful messages.

---

## 🖥️ Localhost Usage

Once the app is running:

| Page                                | Localhost URL                                     |
| ----------------------------------- | ------------------------------------------------- |
| Home / Landing                      | `http://localhost:5173/home`                      |
| Book Appointment                    | `http://localhost:5173/book`                      |
| Search Doctor                       | `http://localhost:5173/search`                    |
| About                               | `http://localhost:5173/about`                     |
| Contact Us                          | `http://localhost:5173/contact`                   |
| Admin Appointment List              | `http://localhost:5173/admin/appointments`        |
| Admin Edit Appointment (example ID) | `http://localhost:5173/admin/appointments/edit/1` |

Make sure your **backend (ASP.NET Core)** is running on `http://localhost:5221`, and the **Swagger docs** are available at `http://localhost:5221/doc`.

---

## 📚 References

- Logo source: [Freepik](https://www.freepik.com/free-vector/doctor-office-logo-template_47639114.htm)
- Background video: [Pexels](https://www.pexels.com/video/a-female-doctor-talking-to-a-girl-and-her-mom-7579978/)
- About inspiration: [Webdew Examples](https://www.webdew.com/blog/about-us-page-examples)
- Fonts: [Google Fonts - Roboto](https://fonts.googleapis.com/css2?family=Roboto:wght@400;500;700&display=swap)
- [React Router Docs](https://react-typescript-cheatsheet.netlify.app/docs/basic/setup)
- [React + TypeScript Cheatsheets](https://react-typescript-cheatsheet.netlify.app/)
- [Axios Docs](https://axios-http.com/docs/intro)
- [TailwindCSS Docs](https://tailwindcss.com/docs)
- [React Datepicker](https://reactdatepicker.com/)
- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/?view=aspnetcore-8.0)
- [Entity Framework Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [Swagger (Swashbuckle) Docs](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [MySQL Connector for .NET](https://dev.mysql.com/downloads/connector/net/)
- [Vite Docs](https://vitejs.dev/guide/)
- [Emoji Standard - Unicode](https://home.unicode.org/emoji/emoji-frequency/)
- [Emojipedia](https://emojipedia.org/)
- [Noroff LMS - Frontend Setup](https://lms.noroff.no/mod/book/view.php?id=118302&chapterid=24347)
- [LMS - Background Video Setup](https://lms.noroff.no/mod/book/view.php?id=118301&chapterid=24271)
- [Booking Button Guide](https://lms.noroff.no/mod/book/view.php?id=118301&chapterid=24273)
- [Loading Feedback Pages](https://lms.noroff.no/mod/book/view.php?id=118301&chapterid=24277)
- [First Year Exam Inspiration]
- [Used OpenAI for guidance](https://chatgpt.com/)

- [admin background image](https://www.istockphoto.com/photo/document-management-system-dms-with-arrange-folder-and-files-icons-man-setup-storage-gm1830042746-550698800?utm_source=pexels&utm_medium=affiliate&utm_campaign=sponsored_photo&utm_content=srp_inline_media&utm_term=administration)

---

## 🚨 Note

If running frontend and backend on different ports locally, configure CORS in the backend to avoid browser errors.
