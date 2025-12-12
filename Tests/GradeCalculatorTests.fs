module StudentGradesV7.Tests.GradeCalculatorTests

open Xunit
open FsUnit.Xunit
open StudentGradesV7.Models
open StudentGradesV7.Services.GradeCalculator

[<Fact>]
let ``average should return 0 for student with no subjects`` () =
    // Arrange
    let student = {
        Id = 1
        Name = "Test Student"
        Subjects = []
        IsPassed = false
    }
    
    // Act
    let result = average student
    
    // Assert
    result |> should equal 0.0

[<Fact>]
let ``average should calculate correct average for multiple subjects`` () =
    // Arrange
    let student = {
        Id = 1
        Name = "Test Student"
        Subjects = [
            { Name = "Math"; Grade = 80.0 }
            { Name = "Science"; Grade = 90.0 }
            { Name = "English"; Grade = 70.0 }
        ]
        IsPassed = true
    }
    
    // Act
    let result = average student
    
    // Assert
    result |> should equal 80.0  // (80 + 90 + 70) / 3 = 80

[<Fact>]
let ``average should round to nearest integer`` () =
    // Arrange
    let student = {
        Id = 1
        Name = "Test Student"
        Subjects = [
            { Name = "Math"; Grade = 85.0 }
            { Name = "Science"; Grade = 90.0 }
        ]
        IsPassed = true
    }
    
    // Act
    let result = average student
    
    // Assert
    result |> should equal 88.0  // (85 + 90) / 2 = 87.5 -> rounds to 88

[<Fact>]
let ``passed should return true when student IsPassed is true`` () =
    // Arrange
    let student = {
        Id = 1
        Name = "Passing Student"
        Subjects = [ { Name = "Math"; Grade = 80.0 } ]
        IsPassed = true
    }
    
    // Act
    let result = passed student
    
    // Assert
    result |> should equal true

[<Fact>]
let ``passed should return false when student IsPassed is false`` () =
    // Arrange
    let student = {
        Id = 1
        Name = "Failing Student"
        Subjects = [ { Name = "Math"; Grade = 40.0 } ]
        IsPassed = false
    }
    
    // Act
    let result = passed student
    
    // Assert
    result |> should equal false

[<Theory>]
[<InlineData(50.0, 60.0, 55.0)>]     // (50+60)/2 = 55
[<InlineData(100.0, 100.0, 100.0)>]  // (100+100)/2 = 100
[<InlineData(0.0, 0.0, 0.0)>]        // (0+0)/2 = 0
[<InlineData(75.5, 84.5, 80.0)>]     // (75.5+84.5)/2 = 80
let ``average should calculate correctly for various grade combinations`` (grade1: float, grade2: float, expectedAvg: float) =
    // Arrange
    let student = {
        Id = 1
        Name = "Test Student"
        Subjects = [
            { Name = "Subject1"; Grade = grade1 }
            { Name = "Subject2"; Grade = grade2 }
        ]
        IsPassed = true
    }
    
    // Act
    let result = average student
    
    // Assert
    result |> should equal expectedAvg
