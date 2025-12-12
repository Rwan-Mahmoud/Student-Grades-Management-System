namespace StudentGradesV7.Services

open StudentGradesV7.Models
open StudentGradesV7.Persistence

module CrudService =

    let getAll () : Student list =
        SqlDataService.load()

    let addStudent (name: string) (subjects: Subject list) : Result<int, string> =
        try
            if subjects.IsEmpty then
                Error "Please add at least one subject"
            else
                
                let tempStudent = { Id = 0; Name = name; Subjects = subjects; IsPassed = false }
                let average = GradeCalculator.average tempStudent
                let isPassed = average >= 50.0
                
                let newId = SqlDataService.addStudent name subjects isPassed
                Ok newId
        with ex ->
            Error $"Database error: {ex.Message}"

    let updateStudent (id: int) (name: string) (subjects: Subject list) : Result<unit, string> =
        if subjects.IsEmpty then
            Error "Please add at least one subject"
        else
            
            let tempStudent = { Id = id; Name = name; Subjects = subjects; IsPassed = false }
            let average = GradeCalculator.average tempStudent
            let isPassed = average >= 50.0
            
            if SqlDataService.updateStudent id name subjects isPassed then
                Ok ()
            else
                Error "Student not found"

    let deleteStudent (id: int) : Result<unit, string> =
        if SqlDataService.deleteStudent id then
            Ok ()
        else
            Error "Student not found"
