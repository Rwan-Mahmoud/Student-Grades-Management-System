namespace StudentGradesV7

open Avalonia
open Avalonia.Themes.Fluent
open Avalonia.Controls.ApplicationLifetimes
open StudentGradesV7.UI

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add(FluentTheme())

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktop ->
            desktop.MainWindow <- MainWindow()
        | _ -> ()