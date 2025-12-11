namespace StudentGradesV7.Services
open StudentGradesV7.Models

module RoleService =

//! isValidAdminPasswor (pure)
    let isValidAdminPassword (password: string) : bool =
        password = StudentGradesV7.Models.Data.adminPassword

    let logout () = Viewer

