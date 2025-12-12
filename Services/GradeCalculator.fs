namespace StudentGradesV7.Services
open StudentGradesV7.Models
open System

module GradeCalculator =

// ! pure
    let average (student: Student) =
        if student.Subjects.IsEmpty then 0.0
        else 
            student.Subjects 
            |> List.map (fun s -> s.Grade) 
            |> List.average 
            |> Math.Round

    let passed (student: Student) = student.IsPassed
