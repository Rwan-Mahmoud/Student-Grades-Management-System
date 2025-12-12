namespace StudentGradesV7.Models

type Subject = {
    Name: string
    Grade: float
}

type Student = {
    Id: int
    Name: string
    Subjects: Subject list
    IsPassed: bool
}

type Role =
    | Admin
    | Viewer

module Data =
    let adminPassword = "RM182004"
