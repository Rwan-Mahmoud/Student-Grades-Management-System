namespace StudentGradesV7.Persistence
open System
open Microsoft.Data.SqlClient
open Dapper
open StudentGradesV7.Models

module SqlDataService =

    let private connectionString = 
        @"Server=localhost;Database=StudentGradesDb;Trusted_Connection=True;TrustServerCertificate=True;"

    let private getConnection() = new SqlConnection(connectionString)

    let private ensureDatabaseAndTable() =
        use masterConn = new SqlConnection(@"Server=localhost;Initial Catalog=master;Trusted_Connection=True;TrustServerCertificate=True;")
        masterConn.Open()
        masterConn.Execute("IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'StudentGradesDb') CREATE DATABASE StudentGradesDb") |> ignore

        use conn = getConnection()
        conn.Open()
        
      
        let sqlStudents = """
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Students')
            BEGIN
                CREATE TABLE Students (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL,
                    IsPassed BIT NOT NULL DEFAULT 0
                )
            END
            ELSE
            BEGIN
                
                IF NOT EXISTS (SELECT * FROM sys.columns 
                              WHERE object_id = OBJECT_ID('Students') 
                              AND name = 'IsPassed')
                BEGIN
                    ALTER TABLE Students ADD IsPassed BIT NOT NULL DEFAULT 0
                END
            END
        """
        conn.Execute(sqlStudents) |> ignore
        
     
        let sqlSubjects = """
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Subjects')
            BEGIN
                CREATE TABLE Subjects (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    StudentId INT NOT NULL,
                    SubjectName NVARCHAR(100) NOT NULL,
                    Grade FLOAT NOT NULL CHECK (Grade BETWEEN 0 AND 100),
                    FOREIGN KEY (StudentId) REFERENCES Students(Id) ON DELETE CASCADE
                )
            END
        """
        conn.Execute(sqlSubjects) |> ignore

    type private StudentRow = { Id: int; Name: string; IsPassed: bool }
    type private SubjectRow = { SubjectName: string; Grade: float }

    let load() : Student list =
        ensureDatabaseAndTable()
        use conn = getConnection()
        conn.Open()
        
        let students = conn.Query<StudentRow>("SELECT Id, Name, IsPassed FROM Students ORDER BY Id") |> Seq.toList
        
        students |> List.map (fun s ->
            let subjects = 
                conn.Query<SubjectRow>(
                    "SELECT SubjectName, Grade FROM Subjects WHERE StudentId = @Id ORDER BY Id",
                    {| Id = s.Id |})
                |> Seq.map (fun sub -> { Name = sub.SubjectName; Grade = sub.Grade })
                |> Seq.toList
            { Id = s.Id; Name = s.Name; Subjects = subjects; IsPassed = s.IsPassed }
        )

    let addStudent (name: string) (subjects: Subject list) (isPassed: bool) : int =
        ensureDatabaseAndTable()
        use conn = getConnection()
        conn.Open()
        use transaction = conn.BeginTransaction()
        
        try
            let studentId = conn.QuerySingle<int>(
                "INSERT INTO Students (Name, IsPassed) VALUES (@Name, @IsPassed); SELECT CAST(SCOPE_IDENTITY() AS INT)",
                {| Name = name; IsPassed = isPassed |},
                transaction)
            
            for subject in subjects do
                conn.Execute(
                    "INSERT INTO Subjects (StudentId, SubjectName, Grade) VALUES (@StudentId, @Name, @Grade)",
                    {| StudentId = studentId; Name = subject.Name; Grade = subject.Grade |},
                    transaction) |> ignore
            
            transaction.Commit()
            studentId
        with
        | ex ->
            transaction.Rollback()
            reraise()

    let updateStudent (id: int) (name: string) (subjects: Subject list) (isPassed: bool) : bool =
        use conn = getConnection()
        conn.Open()
        use transaction = conn.BeginTransaction()
        
        try
       
            let rowsAffected = conn.Execute(
                "UPDATE Students SET Name = @Name, IsPassed = @IsPassed WHERE Id = @Id",
                {| Id = id; Name = name; IsPassed = isPassed |},
                transaction)
            
            if rowsAffected > 0 then
     
                conn.Execute("DELETE FROM Subjects WHERE StudentId = @Id", {| Id = id |}, transaction) |> ignore
                

                for subject in subjects do
                    conn.Execute(
                        "INSERT INTO Subjects (StudentId, SubjectName, Grade) VALUES (@StudentId, @Name, @Grade)",
                        {| StudentId = id; Name = subject.Name; Grade = subject.Grade |},
                        transaction) |> ignore
                
                transaction.Commit()
                true
            else
                transaction.Rollback()
                false
        with
        | ex ->
            transaction.Rollback()
            reraise()

    let deleteStudent (id: int) : bool =
        use conn = getConnection()
        conn.Open()

        conn.Execute("DELETE FROM Students WHERE Id = @Id", {| Id = id |}) > 0
