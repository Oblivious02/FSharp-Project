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


let updateStudentList (): unit =
    studentList.Items.Clear()
    students
    |> List.iter (fun s -> 
        studentList.Items.Add(sprintf "ID: %d, Name: %s, Grades: %s" s.ID s.Name (String.Join(", ", s.Grades))) |> ignore
    )
    updateClassStats ()


let searchStudents (query: string): unit =
    let results: Student list = 
        students 
        |> List.filter (fun s -> 
            s.Name.ToLower().Contains(query.ToLower()) || s.ID.ToString().Contains(query)
        )
    studentList.Items.Clear()
    results
    |> List.iter (fun s -> 
        studentList.Items.Add(sprintf "ID: %d, Name: %s, Grades: %s" s.ID s.Name (String.Join(", ", s.Grades))) |> ignore
    )

// Function to delete a student
let deleteStudent (): unit =
    if studentList.SelectedIndex >= 0 then
        // Get the selected student
        let selectedStudent: Student = students.[studentList.SelectedIndex]

        // Remove the student from the list
        students <- students |> List.filter (fun s -> s <> selectedStudent)

        updateStudentList ()
        MessageBox.Show("Student deleted successfully.") |> ignore
    else
        MessageBox.Show("Please select a student to delete!") |> ignore

// Validate grades (ensure valid input and length)
let validateGrades (gradesText: string): float list option =
    try
        Some (gradesText.Split(',') |> Array.map float |> Array.toList)
    with
        | _ -> None

// Function to update a student
let updateStudent (): unit =
    if studentList.SelectedIndex >= 0 then
        let selectedStudent: Student = students.[studentList.SelectedIndex]

        // Populate textboxes with selected student's details
        nameBox.Text <- selectedStudent.Name
        idBox.Text <- selectedStudent.ID.ToString()
        gradesBox.Text <- String.Join(",", selectedStudent.Grades)

    else
        MessageBox.Show("Please select a student to update!") |> ignore

// Function to save updated student
let saveUpdatedStudent (): unit =
    if studentList.SelectedIndex >= 0 then
        let updatedStudent: Student = {
            ID = int idBox.Text
            Name = nameBox.Text
            Grades = validateGrades gradesBox.Text |> Option.defaultValue []
        }

        if updatedStudent.Grades.IsEmpty then
            MessageBox.Show("Please enter valid grades!") |> ignore
        else
            students <- 
                students
                |> List.mapi (fun i s -> if i = studentList.SelectedIndex then updatedStudent else s)

            updateStudentList ()
            MessageBox.Show("Student updated successfully.") |> ignore

            nameBox.Clear()
            idBox.Clear()
            gradesBox.Clear()
    else
        MessageBox.Show("Please select a student to save changes!") |> ignore

// Ensure only digits and up to 10 digits for ID field
idBox.KeyPress.Add(fun e ->
    if not (Char.IsDigit(e.KeyChar) || e.KeyChar = '\b') then
        e.Handled <- true
)





