namespace StudentGradesV7.Services
open StudentGradesV7.Models
open StudentGradesV7.Services.GradeCalculator

module Statistics =

    type ClassStats = {
        TotalStudents   : int
        PassedCount     : int
        PassRate        : float
        HighestAverage  : float
        LowestAverage   : float
    }

    let classStats (students: Student list) : ClassStats option =
        if students.IsEmpty then None
        else
            let averages = students |> List.map GradeCalculator.average
            let passed   = students |> List.filter (fun s -> s.IsPassed) |> List.length
            Some {
                TotalStudents   = students.Length
                PassedCount     = passed
                PassRate        = (float passed / float students.Length) * 100.0
                HighestAverage  = List.max averages
                LowestAverage   = List.min averages
            }
