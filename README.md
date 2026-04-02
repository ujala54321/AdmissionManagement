📘 Admission Management & CRM System
🚀 Overview

The Admission Management & CRM System is a web-based application designed to streamline the student admission process for colleges and institutions.

It enables administrators and admission officers to:

Configure programs and quotas
Manage applicants efficiently
Allocate seats without violations
Track documents and fees
Monitor admissions through dashboards
🎯 Objective

To build a simple and reliable system that ensures:

✅ No seat overbooking
✅ Quota-wise seat control
✅ Smooth and structured admission workflow
🧩 Features
🔹 1. Master Setup

Admin can configure:

Institution
Campus
Department
Program / Branch
Academic Year
Course Type (UG / PG)
Entry Type (Regular / Lateral)
Admission Mode (Government / Management)
🔹 2. Seat Matrix & Quota Management
Define total intake (e.g., 100 seats)
Configure quotas:
KCET
COMEDK
Management
System Rules:
Total quota = Intake
Real-time seat tracking
Prevent over-allocation
Optional:
Supernumerary seats
Institution-level caps (e.g., J&K quota)
🔹 3. Applicant Management
Create applicant with basic details (max 15 fields)
Assign:
Category (GM/SC/ST)
Entry Type
Quota Type
Track:
Marks / qualification
Document status:
Pending
Submitted
Verified
🔹 4. Admission Allocation
📌 Government Quota Flow
Enter allotment number
Select quota
System checks availability
Seat gets locked
📌 Management Quota Flow
Create applicant manually
Select program & quota
Allocate seat if available
🔹 5. Admission Confirmation
Generate unique admission number:
Format: INST/2026/UG/CSE/KCET/0001
Rules:
Unique and immutable
Generated only once
🔹 6. Fee Management
Status:
Pending
Paid

⚠️ Admission is confirmed only when fee is paid

🔹 7. Dashboard (Basic Analytics)
Total intake vs admitted
Quota-wise filled seats
Remaining seats
Pending documents
Fee pending list
👥 User Roles
🛠️ Admin
Setup master data
Configure quotas
🎓 Admission Officer
Create applicants
Allocate seats
Verify documents
Confirm admissions
📊 Management (View Only)
View dashboards
Track admission progress
🧑‍💻 User Stories
🔹 Setup
Admin can create programs and define quotas
🔹 Applicant Management
Admission Officer can create and manage applicants
Track document verification
🔹 Seat Allocation
System prevents allocation if quota is full
Officer can view available seats before allocation
🔹 Admission Confirmation
Generate admission number
Confirm only after fee payment
🔹 Dashboard
Management can monitor admission status
🔄 User Journeys
🧭 1. System Setup

Admin →
Institution → Campus → Department → Program → Intake → Quotas

🧭 2. Government Admission Flow

Admission Officer →
Create Applicant →
Enter Allotment →
Select Quota →
Check Availability →
Seat Locked →
Documents Verified →
Fee Paid →
Admission Confirmed

🧭 3. Management Admission Flow

Admission Officer →
Create Applicant →
Select Program & Quota →
Check Availability →
Allocate Seat →
Verify Documents →
Fee Paid →
Admission Confirmed

🧭 4. Monitoring

Management →
View Dashboard →

Filled seats
Remaining quota
Pending fees/documents
⚙️ Key System Rules
🚫 Quota seats must not exceed intake
🚫 No allocation if quota is full
🔒 Admission number generated only once
💰 Admission confirmed only after fee payment
🔄 Seat counters update in real-time
🛠️ Tech Stack (You can modify this)
ASP.NET Core MVC
Entity Framework Core
SQL Server
Bootstrap / HTML / CSS
📌 Future Enhancements
Payment gateway integration
Document upload system
Advanced reporting
Notifications (Email/SMS)
🤝 Contribution

Feel free to fork this repository and improve the system.
