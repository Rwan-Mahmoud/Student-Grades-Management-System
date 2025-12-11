namespace StudentGradesV7

open Avalonia
open Avalonia.Controls.ApplicationLifetimes

module Program =

    [<EntryPoint>]
    let main argv =
        AppBuilder
            .Configure<StudentGradesV7.App>()   
            .UsePlatformDetect()
            .WithInterFont()
            .StartWithClassicDesktopLifetime(argv)