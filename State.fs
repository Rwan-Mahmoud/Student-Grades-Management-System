namespace StudentGradesV7

open StudentGradesV7.Models
open StudentGradesV7.Services
open StudentGradesV7.Services.CrudService
open System

module AppState =

    let mutable private currentRole = Viewer
    let mutable private students : Student list = []
    let mutable private message = "Starting the system..."

    let role() = currentRole
    let isAdmin() = currentRole = Admin
    let setRole r = currentRole <- r
    let studentsList() = students
    let messageText() = message
    let setMessage txt = message <- txt

    let login password =
        if RoleService.isValidAdminPassword password then
            setRole Admin
            setMessage "Admin logged in successfully!"
            true
        else
            setMessage "Wrong password!"
            false

    let logout() =
        setRole Viewer
        setMessage "Logged out successfully"

    let private loadStudentsSafely() =
        try
            let data = CrudService.getAll()
            students <- data
            setMessage $"Loaded {data.Length} student(s) from database"
        with ex ->
            setMessage $"Failed to connect to database: {ex.Message.Split('\n').[0]}"
            students <- []

    let addStudent (name: string) (subjects: Subject list) (clearForm: unit -> unit) =
        let name = name.Trim()
        if String.IsNullOrWhiteSpace name then
            setMessage "Student name is required!"
            false
        elif subjects.IsEmpty then
            setMessage "Please add at least one subject!"
            false
        else
            try
                match CrudService.addStudent name subjects with
                | Ok newId ->
                    loadStudentsSafely()
                    clearForm()
                    setMessage $"Student '{name}' added successfully! (ID: {newId})"
                    true
                | Error msg ->
                    setMessage msg
                    false
            with ex ->
                setMessage $"Error adding student: {ex.Message}"
                false

    let updateStudent (id: int) (name: string) (subjects: Subject list) (closeEdit: unit -> unit) =
        try
            let name = name.Trim()
            if String.IsNullOrWhiteSpace name then
                setMessage "Student name cannot be empty!"
                false
            elif subjects.IsEmpty then
                setMessage "Please add at least one subject!"
                false
            else
                match CrudService.updateStudent id name subjects with
                | Ok () ->
                    loadStudentsSafely()
                    closeEdit()
                    setMessage "Student updated successfully!"
                    true
                | Error msg ->
                    setMessage msg
                    false
        with ex ->
            setMessage $"Error updating student: {ex.Message}"
            false

    let deleteStudent id =
        try
            match CrudService.deleteStudent id with
            | Ok () ->
                loadStudentsSafely()
                setMessage "Student deleted successfully!"
                true
            | Error msg ->
                setMessage msg
                false
        with ex ->
            setMessage $"Error deleting student: {ex.Message}"
            false

    let showStats() =
        loadStudentsSafely()
        match Statistics.classStats students with
        | Some st ->
            setMessage (sprintf "Total: %d | Passed: %d (%.1f%%) | Highest: %.1f%% | Lowest: %.1f%%"
                            st.TotalStudents st.PassedCount st.PassRate st.HighestAverage st.LowestAverage)
        | None ->
            setMessage "No students yet"

    do 
        loadStudentsSafely()
        message <- "Welcome! Login as Admin to add/edit students"
