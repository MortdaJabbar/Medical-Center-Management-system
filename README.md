# 🏥 Medical Center Management System

A **comprehensive web-based management system** for medical centers built with **ASP.NET Core Web API**, **SQL Server**, and a modular **three-tier architecture**.  
The system streamlines operations for **patients, doctors, pharmacists, staff, billing, tests, and more**.

---

##  Features

**Patient Management** – Add, update, and manage patient records with images  
**Doctor & Pharmacist Management** – Manage doctors, specializations, and pharmacist licensing  
 **Appointments Module** – Schedule, update, and track patient appointments  
 **Prescriptions & Medications** – Issue prescriptions, manage medication inventory  
 **Medical Tests Module** – Order, track, and upload test results (PDF support)  
 **Billing & Invoices** – Generate invoices for tests, prescriptions, and appointments  
 **User Accounts & Roles** – Secure login with JWT, email verification, and 2FA  
 **Staff Dashboard** – Summarizes payments, unpaid services, and key statistics  

---

##  Tech Stack

- **Backend:** ASP.NET Core 8 Web API  
- **Database:** Microsoft SQL Server (ADO.NET + EF Core Hybrid)  
- **Frontend Template:** Doctris HTML Template (integrated)  
- **Authentication:** JWT, email verification, optional 2FA  
- **Architecture:**  
  - Data Access Layer (DAL) – ADO.NET with stored procedures  
  - Business Logic Layer (BLL) – Validation, hashing, rules  
  - API Layer – RESTful endpoints  

---

##  Project Structure

```
Medical-Center-Management-System/
│
├── API/                     # ASP.NET Core API project
├── BLL/                     # Business Logic Layer
├── DAL/                     # Data Access Layer
├── Database/                # SQL scripts (schema & seed data)
│   ├── MedicalCenterSchema.sql
│   ├── MedicalCenterSeed.sql
├── Frontend/                # HTML, CSS, JS (Bootstrap template)
├── README.md
└── .gitignore
```

---

## ⚙️ Installation

###  **1. Clone the Repository**
```bash
git clone https://github.com/MortdaJabbar/Medical-Center-Management-system.git
cd Medical-Center-Management-system
```

###  **2. Set Up the Database**
- Open **SQL Server Management Studio (SSMS)**.
- Run `Database/MedicalCenterSchema.sql` to create tables & stored procedures.
- Run `Database/MedicalCenterSeed.sql` to insert **dummy/fake data**.

###  **3. Configure Settings**
 `appsettings.json`.
- Add your:
  - ✅ SQL Server connection string
  - ✅ JWT key, issuer, and audience
  - ✅ SMTP credentials (for email verification)

###  **4. Run the API**
```bash
dotnet run
```
API will run at:  
👉 `https://localhost:7119` (or similar).

###  **5. Open the Frontend**
- Serve the HTML template using **VS Code Live Server** or any local server.
- Connect it to the API endpoints.

---

##  Authentication & Security

-  **JWT-based Authentication** for all API endpoints  
-  **Role-based Authorization** (patients, doctors, staff, admin)  
-  **Email Verification** via SMTP (Gmail configured)  
-  **Optional 2FA support**

---

##  Screenshots 

---

## 👨‍💻 Author

Mortda Jabbar  
mc.mortda@gmail.com
---
