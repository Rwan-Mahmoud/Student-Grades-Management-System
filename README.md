
# 🎓 Student Grades Management System

A comprehensive student information system built with F# and Avalonia UI, featuring grade management, statistics calculation, role-based access control, and SQL Server persistence.


## 📋 Project Overview

This system allows administrators to manage student records, calculate averages, track pass/fail status, and generate class-wide statistics. It features a modern desktop UI with role-based authentication.


## ✨ Key Features

- ✅ Add/Edit/Delete student records with multiple subjects
- ✅ Automatic grade calculation with pass/fail determination (≥50% = Pass)
- ✅ Class statistics: Pass rate, highest/lowest averages
- ✅ Role-based access control: Admin and Viewer roles
- ✅ SQL Server persistence with Dapper ORM
- ✅ Modern Avalonia UI with responsive design
- ✅ Transaction support for data integrity
- ✅ Comprehensive unit tests with xUnit and FsUnit


## 👥 Team Members & Contributions

| Member    | Role                               | Responsibilities                                  | Files                                              |
|-----------|-------------------------------------|----------------------------------------------------|-----------------------------------------------------|
| Gehad Mohammed | Models Designer & Grade Calculator  | Domain types & data structures + Grade calculations | Models/Student.fs, Services/GradeCalculator.fs      |
|Habiba Yasser | CRUD Developer                      | Add/Edit/Delete operations                         | Services/CrudService.fs                             |
| Salem Elsayed  | Statistician                        | Class-wide metrics                                 | Services/Statistics.fs                              |
| Ahmed Ibrahim  | Role Manager                        | Authorization & authentication                     | Services/Authorization.fs, Services/RoleService.fs  |
| Rawan Mahmoud  | Persistence Dev                     | SQL Server & Dapper integration                    | Persistence/SqlDataService.fs                       |
| Maha Elsayed | UI Developer                        | Avalonia desktop interface                         | UI/MainWindow.fs, App.fs, Program.fs                |
| Mariam Ibrahim | Testing Lead                        | Unit & integration tests                           | Tests/*.fs                                          |




## 🏗️ Project Structure

```
Student-Grades-Management-System/
│
├── StudentGradesV7/                    
│   ├── Models/
│   │   └── Student.fs                 
│   │
│   ├── Services/
│   │   ├── Authorization.fs            
│   │   ├── RoleService.fs             
│   │   ├── GradeCalculator.fs         
│   │   ├── Statistics.fs               
│   │   └── CrudService.fs              
│   │
│   ├── Persistence/
│   │   └── SqlDataService.fs          
│   │
│   ├── UI/
│   │   └── MainWindow.fs               
│   │
│   ├── State.fs                       
│   ├── App.fs                        
│   ├── Program.fs                    
│   └── StudentGradesV7.fsproj        
│
├── StudentGradesV7.Tests/           
│   ├── GradeCalculatorTests.fs         
│   ├── StatisticsTests.fs             
│   └── StudentGradesV7.Tests.fsproj
│
├── README.md                                             
└── .gitignore                          
```


## 🎮 Usage

**Login Credentials**
- **Admin Password:**   
- **Viewer Role:** No password required (default)

 **Admin Features**
-  Add new students with subjects and grades  
-  Edit existing student information  
-  Delete student records  
-  View class statistics  

👀 **Viewer Features**
-  View all students and their grades  
- View class statistics


## 📦 NuGet Packages

 **Main Project**
- **Avalonia** 11.3.9 – Cross-platform UI framework  
- **Avalonia.Desktop** 11.3.9 – Desktop support  
- **Avalonia.Themes.Fluent** 11.3.9 – Modern theme  
- **Avalonia.FuncUI** 1.5.2 – Functional UI for F#  
- **Microsoft.Data.SqlClient** 5.2.2 – SQL Server connectivity  
- **Dapper** 2.1.35 – Micro ORM  

**Test Project**
- **xUnit** 2.9.0 – Testing framework  
- **FsUnit.xUnit** 6.0.0 – F#-friendly assertions  
- **Microsoft.NET.Test.Sdk** 17.11.0 – Test runner  



## 📐 Architecture

**Design Principles**
- **Functional-first** approach with F#  
- **Separation of concerns** (Models, Services, Persistence, UI)  
- **Immutable data structures** (F# records)  
- **Pure functions** for calculations  
- **Result types** for error handling  
- **Transactions** for data integrity

**Data Flow**

```
UI (MainWindow.fs)
    ↓
AppState (State.fs)
    ↓
Services (CrudService, GradeCalculator, Statistics)
    ↓
Persistence (SqlDataService)
    ↓
SQL Server Database
```

