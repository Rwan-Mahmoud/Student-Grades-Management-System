namespace StudentGradesV7.Services
open StudentGradesV7.Models

module Authorization =
    let canAddStudent    (role: Role) = role = Admin
    let canEditStudent   (role: Role) = role = Admin
    let canDeleteStudent (role: Role) = role = Admin
    let canViewStudents  (_    : Role) = true
    let canViewStats     (_    : Role) = true
    let isAdmin          (role: Role) = role = Admin
