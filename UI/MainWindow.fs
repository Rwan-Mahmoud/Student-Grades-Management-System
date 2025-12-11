namespace StudentGradesV7.UI

open System
open Avalonia
open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.Layout
open Avalonia.Media
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open StudentGradesV7.Models
open StudentGradesV7.Services
open StudentGradesV7

type MainWindow() as self =
    inherit Window()

    do
        self.Title <- "Student Grades System "
        self.Width <- 1400.0
        self.Height <- 850.0
        self.WindowStartupLocation <- WindowStartupLocation.CenterScreen

        self.Content <-
            Component (fun ctx ->
                let pwd = ctx.useState ""
                let nameInput = ctx.useState ""
                let delId = ctx.useState ""
                let forceUpdate = ctx.useState 0
                
             
                let newSubjects = ctx.useState<(string * string) list> []  
                let newSubjectName = ctx.useState ""
                let newSubjectGrade = ctx.useState ""

                DockPanel.create [
                    DockPanel.children [
                        // ========== Sidebar ==========
                        ScrollViewer.create [
                            ScrollViewer.dock Dock.Left
                            ScrollViewer.width 450
                            ScrollViewer.content (
                                StackPanel.create [
                                    StackPanel.background "#2c3e50"
                                    StackPanel.margin 20
                                    StackPanel.spacing 20
                                    StackPanel.children [
                                        TextBlock.create [ 
                                            TextBlock.fontSize 28
                                            TextBlock.foreground Brushes.White
                                            TextBlock.fontWeight FontWeight.Bold
                                            TextBlock.text "Grades System" 
                                        ]
                                        TextBlock.create [ 
                                            TextBlock.fontSize 18
                                            TextBlock.foreground Brushes.Cyan
                                            TextBlock.text (sprintf "Role: %A" (AppState.role())) 
                                        ]

                                        // ========== Login / Logout ==========
                                        if AppState.isAdmin() then
                                            Button.create [
                                                Button.content "Logout"
                                                Button.background Brushes.OrangeRed
                                                Button.foreground Brushes.White
                                                Button.onClick (fun _ ->
                                                    AppState.logout()
                                                    forceUpdate.Set(forceUpdate.Current + 1)
                                                )
                                            ]
                                        else
                                            StackPanel.create [
                                                StackPanel.spacing 10
                                                StackPanel.children [
                                                    TextBlock.create [ TextBlock.foreground Brushes.Orange; TextBlock.text "Admin Login" ]
                                                    TextBox.create [
                                                        TextBox.watermark "Enter Password"
                                                        TextBox.passwordChar '*'
                                                        TextBox.text pwd.Current
                                                        TextBox.onTextChanged pwd.Set
                                                    ]
                                                    Button.create [
                                                        Button.content "Login as Admin"
                                                        Button.background Brushes.Green
                                                        Button.foreground Brushes.White
                                                        Button.onClick (fun _ ->
                                                            let _ = AppState.login pwd.Current
                                                            pwd.Set ""
                                                            forceUpdate.Set(forceUpdate.Current + 1)
                                                        )
                                                    ]
                                                ]
                                            ]

                                        // ========== Add Student ==========
                                        if AppState.isAdmin() then
                                            StackPanel.create [
                                                StackPanel.spacing 15
                                                StackPanel.background "#34495e"
                                                StackPanel.margin 10
                                                StackPanel.children [
                                                    TextBlock.create [ 
                                                        TextBlock.foreground Brushes.Orange
                                                        TextBlock.fontSize 20
                                                        TextBlock.fontWeight FontWeight.Bold
                                                        TextBlock.text "➕ Add New Student" 
                                                    ]
                                                    
                                                    TextBox.create [
                                                        TextBox.watermark "Student Name (required)"
                                                        TextBox.text nameInput.Current
                                                        TextBox.onTextChanged nameInput.Set
                                                    ]

                                                 
                                                    Border.create [
                                                        Border.borderBrush Brushes.Gray
                                                        Border.borderThickness 1.0
                                                        Border.padding 10
                                                        Border.child (
                                                            StackPanel.create [
                                                                StackPanel.spacing 8
                                                                StackPanel.children [
                                                                    TextBlock.create [ 
                                                                        TextBlock.foreground Brushes.LightGreen
                                                                        TextBlock.text "Add Subject:" 
                                                                    ]
                                                                    
                                                                    TextBox.create [
                                                                        TextBox.watermark "Subject Name (e.g., Math)"
                                                                        TextBox.text newSubjectName.Current
                                                                        TextBox.onTextChanged newSubjectName.Set
                                                                    ]
                                                                    
                                                                    TextBox.create [
                                                                        TextBox.watermark "Grade (0-100)"
                                                                        TextBox.text newSubjectGrade.Current
                                                                        TextBox.onTextChanged newSubjectGrade.Set
                                                                    ]
                                                                    
                                                                    Button.create [
                                                                        Button.content "➕ Add Subject"
                                                                        Button.background Brushes.DarkGreen
                                                                        Button.foreground Brushes.White
                                                                        Button.onClick (fun _ ->
                                                                            let name = newSubjectName.Current.Trim()
                                                                            let gradeStr = newSubjectGrade.Current.Trim()
                                                                            if not (String.IsNullOrWhiteSpace name) && not (String.IsNullOrWhiteSpace gradeStr) then
                                                                                match Double.TryParse gradeStr with
                                                                                | true, g when g >= 0.0 && g <= 100.0 ->
                                                                                    newSubjects.Set(newSubjects.Current @ [(name, gradeStr)])
                                                                                    newSubjectName.Set ""
                                                                                    newSubjectGrade.Set ""
                                                                                | _ -> AppState.setMessage "Grade must be between 0-100"
                                                                            forceUpdate.Set(forceUpdate.Current + 1)
                                                                        )
                                                                    ]
                                                                ]
                                                            ]
                                                        )
                                                    ]

                                                    if not newSubjects.Current.IsEmpty then
                                                        Border.create [
                                                            Border.background "#1e2836"
                                                            Border.padding 10
                                                            Border.child (
                                                                StackPanel.create [
                                                                    StackPanel.spacing 5
                                                                    StackPanel.children [
                                                                        TextBlock.create [ 
                                                                            TextBlock.foreground Brushes.Yellow
                                                                            TextBlock.fontWeight FontWeight.Bold
                                                                            TextBlock.text "📚 Subjects Added:" 
                                                                        ]
                                                                        
                                                                        for idx, (subName, subGrade) in newSubjects.Current |> List.indexed do
                                                                            StackPanel.create [
                                                                                StackPanel.orientation Orientation.Horizontal
                                                                                StackPanel.spacing 10
                                                                                StackPanel.children [
                                                                                    TextBlock.create [ 
                                                                                        TextBlock.foreground Brushes.White
                                                                                        TextBlock.text $"{subName}: {subGrade}" 
                                                                                    ]
                                                                                    Button.create [
                                                                                        Button.content "❌"
                                                                                        Button.background Brushes.Red
                                                                                        Button.foreground Brushes.White
                                                                                        Button.fontSize 10
                                                                                        Button.padding 3
                                                                                        Button.onClick (fun _ ->
                                                                                            let filtered = newSubjects.Current |> List.indexed |> List.filter (fun (i, _) -> i <> idx) |> List.map snd
                                                                                            newSubjects.Set filtered
                                                                                            forceUpdate.Set(forceUpdate.Current + 1)
                                                                                        )
                                                                                    ]
                                                                                ]
                                                                            ]
                                                                    ]
                                                                ]
                                                            )
                                                        ]

                                                    Button.create [
                                                        Button.content "💾 Save Student"
                                                        Button.background Brushes.Blue
                                                        Button.foreground Brushes.White
                                                        Button.fontSize 16
                                                        Button.isEnabled (not (String.IsNullOrWhiteSpace nameInput.Current) && not newSubjects.Current.IsEmpty)
                                                        Button.onClick (fun _ ->
                                                            let subjects = 
                                                                newSubjects.Current 
                                                                |> List.map (fun (name, gradeStr) -> 
                                                                    { Name = name; Grade = Double.Parse gradeStr })
                                                            
                                                            ignore <| AppState.addStudent
                                                                nameInput.Current
                                                                subjects
                                                                (fun () ->
                                                                    nameInput.Set ""
                                                                    newSubjects.Set []
                                                                )
                                                            forceUpdate.Set(forceUpdate.Current + 1)
                                                        )
                                                    ]
                                                ]
                                            ]

                                        // ========== Delete Student ==========
                                        if AppState.isAdmin() then
                                            StackPanel.create [
                                                StackPanel.spacing 10
                                                StackPanel.children [
                                                    TextBlock.create [ TextBlock.foreground Brushes.Red; TextBlock.text "🗑️ Delete Student by ID" ]
                                                    TextBox.create [
                                                        TextBox.watermark "Enter ID"
                                                        TextBox.text delId.Current
                                                        TextBox.onTextChanged delId.Set
                                                    ]
                                                    Button.create [
                                                        Button.content "Delete"
                                                        Button.background Brushes.Red
                                                        Button.foreground Brushes.White
                                                        Button.onClick (fun _ ->
                                                            match Int32.TryParse delId.Current with
                                                            | true, id ->
                                                                ignore (AppState.deleteStudent id)
                                                                delId.Set ""
                                                            | _ -> AppState.setMessage "Invalid ID!"
                                                            forceUpdate.Set(forceUpdate.Current + 1)
                                                        )
                                                    ]
                                                ]
                                            ]

                                        // ========== Statistics ==========
                                        Button.create [
                                            Button.content "📊 Show Class Statistics"
                                            Button.background Brushes.Purple
                                            Button.foreground Brushes.White
                                            Button.onClick (fun _ ->
                                                AppState.showStats()
                                                forceUpdate.Set(forceUpdate.Current + 1)
                                            )
                                        ]

                                        // ========== Exit ==========
                                        Button.create [
                                            Button.content "🚪 Exit"
                                            Button.background Brushes.DarkRed
                                            Button.foreground Brushes.White
                                            Button.onClick (fun _ -> self.Close())
                                        ]
                                    ]
                                ]
                            )
                        ]

                        // ========== Students List ==========
                        ScrollViewer.create [
                            ScrollViewer.content (
                                StackPanel.create [
                                    StackPanel.margin 40
                                    StackPanel.spacing 25
                                    StackPanel.children [
                                        TextBlock.create [ 
                                            TextBlock.fontSize 32
                                            TextBlock.fontWeight FontWeight.Bold
                                            TextBlock.text "👥 Students List" 
                                        ]

                                        for s in AppState.studentsList() do
                                            let avg = GradeCalculator.average s
                                            let passed = GradeCalculator.passed s
                                            let color = if passed then Brushes.Green else Brushes.Red
                                            let status = if passed then "✅ PASSED" else "❌ FAILED"
                                            let editing = ctx.useState false
                                            let editName = ctx.useState s.Name
                                            let editSubjects = ctx.useState (s.Subjects |> List.map (fun sub -> (sub.Name, string sub.Grade)))
                                            let editNewSubName = ctx.useState ""
                                            let editNewSubGrade = ctx.useState ""

                                            if not editing.Current then
                                                // ========== View Mode ==========
                                                Border.create [
                                                    Border.background "#f8f9fa"
                                                    Border.borderBrush Brushes.LightGray
                                                    Border.borderThickness 2.0
                                                    Border.padding 15
                                                    Border.margin 10
                                                    Border.child (
                                                        StackPanel.create [
                                                            StackPanel.spacing 10
                                                            StackPanel.children [
                                                                StackPanel.create [
                                                                    StackPanel.orientation Orientation.Horizontal
                                                                    StackPanel.spacing 15
                                                                    StackPanel.children [
                                                                        TextBlock.create [
                                                                            TextBlock.fontSize 20
                                                                            TextBlock.foreground color
                                                                            TextBlock.fontWeight FontWeight.SemiBold
                                                                            TextBlock.text (sprintf "ID: %d | %s | Avg: %.1f%% | %s" s.Id s.Name avg status)
                                                                        ]
                                                                        if AppState.isAdmin() then
                                                                            Button.create [
                                                                                Button.content "✏️ Edit"
                                                                                Button.background Brushes.Orange
                                                                                Button.foreground Brushes.White
                                                                                Button.onClick (fun _ -> editing.Set true)
                                                                            ]
                                                                    ]
                                                                ]
                                                                
                                                             
                                                                StackPanel.create [
                                                                    StackPanel.spacing 5
                                                                    StackPanel.children [
                                                                        for sub in s.Subjects do
                                                                            TextBlock.create [
                                                                                TextBlock.foreground Brushes.DarkSlateGray
                                                                                TextBlock.fontSize 14
                                                                                TextBlock.text (sprintf "  📖 %s: %.0f" sub.Name sub.Grade)
                                                                            ]
                                                                    ]
                                                                ]
                                                            ]
                                                        ]
                                                    )
                                                ]
                                            else
                                                // ========== Edit Mode ==========
                                                Border.create [
                                                    Border.background "#fff3cd"
                                                    Border.borderBrush Brushes.Orange
                                                    Border.borderThickness 3.0
                                                    Border.padding 20
                                                    Border.margin 10
                                                    Border.child (
                                                        StackPanel.create [
                                                            StackPanel.spacing 15
                                                            StackPanel.children [
                                                                TextBlock.create [ 
                                                                    TextBlock.fontSize 18
                                                                    TextBlock.fontWeight FontWeight.Bold
                                                                    TextBlock.text (sprintf "✏️ Editing Student ID: %d" s.Id) 
                                                                ]

                                                                TextBox.create [
                                                                    TextBox.watermark "Student Name (required)"
                                                                    TextBox.text editName.Current
                                                                    TextBox.onTextChanged editName.Set
                                                                ]

                                                            
                                                                Border.create [
                                                                    Border.background "White"
                                                                    Border.padding 10
                                                                    Border.child (
                                                                        StackPanel.create [
                                                                            StackPanel.spacing 8
                                                                            StackPanel.children [
                                                                                TextBlock.create [ 
                                                                                    TextBlock.fontWeight FontWeight.Bold
                                                                                    TextBlock.text "📚 Current Subjects:" 
                                                                                ]
                                                                                
                                                                                for idx, (subName, subGrade) in editSubjects.Current |> List.indexed do
                                                                                    StackPanel.create [
                                                                                        StackPanel.orientation Orientation.Horizontal
                                                                                        StackPanel.spacing 10
                                                                                        StackPanel.children [
                                                                                            TextBox.create [
                                                                                                TextBox.width 150
                                                                                                TextBox.text subName
                                                                                                TextBox.onTextChanged (fun newName ->
                                                                                                    let updated = editSubjects.Current |> List.mapi (fun i (n, g) -> if i = idx then (newName, g) else (n, g))
                                                                                                    editSubjects.Set updated
                                                                                                )
                                                                                            ]
                                                                                            TextBox.create [
                                                                                                TextBox.width 80
                                                                                                TextBox.text subGrade
                                                                                                TextBox.onTextChanged (fun newGrade ->
                                                                                                    let updated = editSubjects.Current |> List.mapi (fun i (n, g) -> if i = idx then (n, newGrade) else (n, g))
                                                                                                    editSubjects.Set updated
                                                                                                )
                                                                                            ]
                                                                                            Button.create [
                                                                                                Button.content "❌"
                                                                                                Button.background Brushes.Red
                                                                                                Button.foreground Brushes.White
                                                                                                Button.onClick (fun _ ->
                                                                                                    let filtered = editSubjects.Current |> List.indexed |> List.filter (fun (i, _) -> i <> idx) |> List.map snd
                                                                                                    editSubjects.Set filtered
                                                                                                )
                                                                                            ]
                                                                                        ]
                                                                                    ]
                                                                            ]
                                                                        ]
                                                                    )
                                                                ]

                                                                
                                                                Border.create [
                                                                    Border.background "#e7f3ff"
                                                                    Border.padding 10
                                                                    Border.child (
                                                                        StackPanel.create [
                                                                            StackPanel.spacing 8
                                                                            StackPanel.children [
                                                                                TextBlock.create [ TextBlock.text "➕ Add New Subject:" ]
                                                                                TextBox.create [
                                                                                    TextBox.watermark "Subject Name"
                                                                                    TextBox.text editNewSubName.Current
                                                                                    TextBox.onTextChanged editNewSubName.Set
                                                                                ]
                                                                                TextBox.create [
                                                                                    TextBox.watermark "Grade (0-100)"
                                                                                    TextBox.text editNewSubGrade.Current
                                                                                    TextBox.onTextChanged editNewSubGrade.Set
                                                                                ]
                                                                                Button.create [
                                                                                    Button.content "➕ Add"
                                                                                    Button.background Brushes.Green
                                                                                    Button.foreground Brushes.White
                                                                                    Button.onClick (fun _ ->
                                                                                        let name = editNewSubName.Current.Trim()
                                                                                        let gradeStr = editNewSubGrade.Current.Trim()
                                                                                        if not (String.IsNullOrWhiteSpace name) && not (String.IsNullOrWhiteSpace gradeStr) then
                                                                                            match Double.TryParse gradeStr with
                                                                                            | true, g when g >= 0.0 && g <= 100.0 ->
                                                                                                editSubjects.Set(editSubjects.Current @ [(name, gradeStr)])
                                                                                                editNewSubName.Set ""
                                                                                                editNewSubGrade.Set ""
                                                                                            | _ -> AppState.setMessage "Grade must be between 0-100"
                                                                                    )
                                                                                ]
                                                                            ]
                                                                        ]
                                                                    )
                                                                ]

                                                                StackPanel.create [
                                                                    StackPanel.orientation Orientation.Horizontal
                                                                    StackPanel.spacing 15
                                                                    StackPanel.children [
                                                                        Button.create [
                                                                            Button.content "💾 Save Changes"
                                                                            Button.background Brushes.Green
                                                                            Button.foreground Brushes.White
                                                                            Button.isEnabled (
                                                                                not (String.IsNullOrWhiteSpace editName.Current) &&
                                                                                not editSubjects.Current.IsEmpty &&
                                                                                editSubjects.Current |> List.forall (fun (n, g) ->
                                                                                    not (String.IsNullOrWhiteSpace n) &&
                                                                                    match Double.TryParse g with
                                                                                    | true, v -> v >= 0.0 && v <= 100.0
                                                                                    | _ -> false
                                                                                )
                                                                            )
                                                                            Button.onClick (fun _ ->
                                                                                let subjects = 
                                                                                    editSubjects.Current 
                                                                                    |> List.map (fun (name, gradeStr) -> 
                                                                                        { Name = name; Grade = Double.Parse gradeStr })
                                                                                
                                                                                ignore <| AppState.updateStudent
                                                                                    s.Id
                                                                                    editName.Current
                                                                                    subjects
                                                                                    (fun () -> editing.Set false)
                                                                                forceUpdate.Set(forceUpdate.Current + 1)
                                                                            )
                                                                        ]
                                                                        Button.create [
                                                                            Button.content "❌ Cancel"
                                                                            Button.background Brushes.Gray
                                                                            Button.foreground Brushes.White
                                                                            Button.onClick (fun _ ->
                                                                                editing.Set false
                                                                                editName.Set s.Name
                                                                                editSubjects.Set (s.Subjects |> List.map (fun sub -> (sub.Name, string sub.Grade)))
                                                                            )
                                                                        ]
                                                                    ]
                                                                ]
                                                            ]
                                                        ]
                                                    )
                                                ]

                                        TextBlock.create [
                                            TextBlock.fontSize 24
                                            TextBlock.foreground Brushes.DarkOrange
                                            TextBlock.fontWeight FontWeight.Bold
                                            TextBlock.text (AppState.messageText())
                                            TextBlock.margin (Thickness(0.0, 60.0))
                                            TextBlock.horizontalAlignment HorizontalAlignment.Center
                                        ]
                                    ]
                                ]
                            )
                        ]
                    ]
                ]
            )
