module StudentGradesV7.Tests.StatisticsTests

open Xunit
open FsUnit.Xunit
open StudentGradesV7.Models
open StudentGradesV7.Services.Statistics

[<Fact>]
let ``classStats should return None for empty student list`` () =
    // Arrange
    let students = []
    
    // Act
    let result = classStats students
    
    // Assert
    result |> should equal None

[<Fact>]
let ``classStats should calculate correct statistics for single student`` () =
    // Arrange
    let students = [
        {
            Id = 1
            Name = "Student 1"
            Subjects = [ { Name = "Math"; Grade = 80.0 } ]
            IsPassed = true
        }
    ]
    
    // Act
    let result = classStats students
    
    // Assert
    result |> should not' (equal None)
    match result with
    | Some stats ->
        stats.TotalStudents |> should equal 1
        stats.PassedCount |> should equal 1
        stats.PassRate |> should equal 100.0
        stats.HighestAverage |> should equal 80.0
        stats.LowestAverage |> should equal 80.0
    | None -> failwith "Expected Some stats"

[<Fact>]
let ``classStats should calculate correct pass rate`` () =
    // Arrange
    let students = [
        {
            Id = 1
            Name = "Student 1"
            Subjects = [ { Name = "Math"; Grade = 60.0 } ]
            IsPassed = true
        }
        {
            Id = 2
            Name = "Student 2"
            Subjects = [ { Name = "Math"; Grade = 40.0 } ]
            IsPassed = false
        }
        {
            Id = 3
            Name = "Student 3"
            Subjects = [ { Name = "Math"; Grade = 80.0 } ]
            IsPassed = true
        }
        {
            Id = 4
            Name = "Student 4"
            Subjects = [ { Name = "Math"; Grade = 30.0 } ]
            IsPassed = false
        }
    ]
    
    // Act
    let result = classStats students
    
    // Assert
    match result with
    | Some stats ->
        stats.TotalStudents |> should equal 4
        stats.PassedCount |> should equal 2
        stats.PassRate |> should equal 50.0  // 2 out of 4 = 50%
    | None -> failwith "Expected Some stats"

[<Fact>]
let ``classStats should find correct highest and lowest averages`` () =
    // Arrange
    let students = [
        {
            Id = 1
            Name = "High Achiever"
            Subjects = [
                { Name = "Math"; Grade = 95.0 }
                { Name = "Science"; Grade = 100.0 }
            ]
            IsPassed = true
        }
        {
            Id = 2
            Name = "Average Student"
            Subjects = [
                { Name = "Math"; Grade = 60.0 }
                { Name = "Science"; Grade = 70.0 }
            ]
            IsPassed = true
        }
        {
            Id = 3
            Name = "Struggling Student"
            Subjects = [
                { Name = "Math"; Grade = 30.0 }
                { Name = "Science"; Grade = 40.0 }
            ]
            IsPassed = false
        }
    ]
    
    // Act
    let result = classStats students
    
    // Assert
    match result with
    | Some stats ->
        stats.HighestAverage |> should equal 98.0  // (95+100)/2 = 97.5 rounded to 98
        stats.LowestAverage |> should equal 35.0   // (30+40)/2 = 35
    | None -> failwith "Expected Some stats"

[<Fact>]
let ``classStats should handle all students passing`` () =
    // Arrange
    let students = [
        {
            Id = 1
            Name = "Student 1"
            Subjects = [ { Name = "Math"; Grade = 80.0 } ]
            IsPassed = true
        }
        {
            Id = 2
            Name = "Student 2"
            Subjects = [ { Name = "Math"; Grade = 90.0 } ]
            IsPassed = true
        }
    ]
    
    // Act
    let result = classStats students
    
    // Assert
    match result with
    | Some stats ->
        stats.PassedCount |> should equal 2
        stats.PassRate |> should equal 100.0
    | None -> failwith "Expected Some stats"
