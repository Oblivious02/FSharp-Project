open System
open System.IO
open System.Windows.Forms
open Newtonsoft.Json
type Student = {
    ID: int
    Name: string
    Grades: float list
}

let mutable students: Student list = []
let mutable isAdmin = true

let nameBox: TextBox = new TextBox(Top = 20, Left = 110, Width = 200)
let idBox: TextBox = new TextBox(Top = 60, Left = 110, Width = 200)
let gradesBox: TextBox = new TextBox(Top = 100, Left = 110, Width = 200)
let studentList: ListBox = new ListBox(Top = 200, Left = 10, Width = 560, Height = 150)

let searchBox: TextBox = new TextBox(Top = 170, Left = 100, Width = 200)
let searchButton: Button = new Button(Text = "Search", Top = 170, Left = 310)
let deleteButton: Button = new Button(Text = "Delete Student", Top = 140, Left = 400, Width = 100)
let statsLabel: Label = new Label(Top = 370, Left = 10, Width = 560, Height = 80)

let filePath = @"C:\Users\ahmed noaman\Desktop\ConsoleApp1\students.json"

let addStudent (student: Student): unit =
    students <- student :: students

// Function to update the ListBox with students
// Function to calculate individual and class-wide statistics
let updateClassStats (): unit =
    if students.IsEmpty then
        statsLabel.Text <- "No students available."
    else
        let averages = students |> List.map (fun s -> List.average s.Grades)
        let highest = averages |> List.max
        let lowest = averages |> List.min
        let classAverage = averages |> List.average
        let passCount = averages |> List.filter (fun avg -> avg >= 50.0) |> List.length
        let failCount = averages.Length - passCount

        statsLabel.Text <- 
            sprintf "Class Average: %.2f\nHighest Average: %.2f\nLowest Average: %.2f\nPass Count: %d\nFail Count: %d" 
                    classAverage highest lowest passCount failCount

// Function to update the ListBox with students
let updateStudentList (): unit =
    studentList.Items.Clear()
    students
    |> List.iter (fun s -> 
        studentList.Items.Add(sprintf "ID: %d, Name: %s, Grades: %s" s.ID s.Name (String.Join(", ", s.Grades))) |> ignore
    )
    updateClassStats ()
