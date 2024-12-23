﻿open System
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
let addButton = new Button(Text = "Add Student", Top = 140, Left = 100, Width = 100)
let updateButton = new Button(Text = "Update Student", Top = 140, Left = 200, Width = 100)
let searchButton: Button = new Button(Text = "Search", Top = 170, Left = 310)
let deleteButton: Button = new Button(Text = "Delete Student", Top = 140, Left = 400, Width = 100)
let statsLabel: Label = new Label(Top = 370, Left = 10, Width = 560, Height = 80)

let filePath = @"F:\Ahmed Saber\faculty of computers and artfitial intelligance\Fsharp_Project\students.json"

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
        let grades = gradesText.Split(',') |> Array.map float |> Array.toList
        if grades |> List.exists (fun grade -> grade < 0.0 || grade > 100.0) then
            None
        else
            Some grades
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

let checkUserRole() =
    if isAdmin then
        MessageBox.Show("You are Admin. You have full permissions.") |> ignore
    else
        MessageBox.Show("You are Viewer. You have view-only permissions.") |> ignore

// Function to switch the role
let switchRole() =
    isAdmin <- not isAdmin
    if isAdmin then
        MessageBox.Show("Switched to Admin. You now have full permissions.") |> ignore
        // Enable admin actions
        updateButton.Enabled <- true
        deleteButton.Enabled <- true
        addButton.Enabled <- true
    else
        MessageBox.Show("Switched to Viewer. You now have view-only permissions.") |> ignore
        // Disable admin actions
        updateButton.Enabled <- false
        deleteButton.Enabled <- false
        addButton.Enabled <- false

// Function to save students to a JSON file
let saveStudentsToFile (filePath: string): unit =
    try
        let json = JsonConvert.SerializeObject(students, Formatting.Indented)
        File.WriteAllText(filePath, json)
        MessageBox.Show("Students saved to file.") |> ignore
    with
    | ex -> MessageBox.Show($"Error saving data: {ex.Message}") |> ignore

// Function to load students from a JSON file
let loadStudentsFromFile (filePath: string): unit =
    try
        if File.Exists(filePath) then
            let json = File.ReadAllText(filePath)
            students <- JsonConvert.DeserializeObject<Student list>(json)
            updateStudentList ()
          //  MessageBox.Show("Students loaded from file.") |> ignore
       // else
          //  MessageBox.Show("No saved students found.") |> ignore
    with
    | ex -> MessageBox.Show($"Error loading data: {ex.Message}") |> ignore

let form: Form = new Form(Text = "Student Grades Management System", Width = 800, Height = 500)

// Create other UI elements
let nameLabel: Label = new Label(Text = "Name:", Top = 20, Left = 10)
let idLabel: Label = new Label(Text = "ID:", Top = 60, Left = 10)
let gradesLabel: Label = new Label(Text = "Grades:", Top = 100, Left = 10)
let saveButton: Button = new Button(Text = "Save Changes", Top = 140, Left = 300, Width = 100)
let checkRoleButton: Button = new Button(Text = "Check Role", Top = 140, Left = 400, Width = 100)
let switchRoleButton: Button = new Button(Text = "Switch Role", Top = 140, Left = 500, Width = 100)
let saveToFileButton: Button = new Button(Text = "Save to File", Top = 140, Left = 600, Width = 100)

// Add event handlers
addButton.Click.Add (fun _ ->
    let name: string = nameBox.Text
    let id: int = int idBox.Text
    let grades = validateGrades gradesBox.Text
    match grades with
    | Some validGrades ->
        addStudent { ID = id; Name = name; Grades = validGrades }
        updateStudentList ()
        MessageBox.Show($"Student added: {name}") |> ignore
        nameBox.Clear()
        idBox.Clear()
        gradesBox.Clear()
    | None -> MessageBox.Show("Please enter valid grades. Grades must be between 0 and 100.") |> ignore
)




updateButton.Click.Add (fun _ -> updateStudent ())
saveButton.Click.Add (fun _ -> saveUpdatedStudent ())
checkRoleButton.Click.Add (fun _ -> checkUserRole())
switchRoleButton.Click.Add (fun _ -> switchRole())
saveToFileButton.Click.Add(fun _ -> saveStudentsToFile filePath)

searchButton.Click.Add (fun _ ->
    let query: string = searchBox.Text
    if query <> "" then
        searchStudents query
    else
        MessageBox.Show("Please enter a search term.") |> ignore
)

deleteButton.Click.Add (fun _ -> deleteStudent ())

// Add elements to the form
form.Controls.Add(nameLabel)
form.Controls.Add(nameBox)
form.Controls.Add(idLabel)
form.Controls.Add(idBox)
form.Controls.Add(gradesLabel)
form.Controls.Add(gradesBox)
form.Controls.Add(addButton)
form.Controls.Add(updateButton)
form.Controls.Add(saveButton)
form.Controls.Add(studentList)
form.Controls.Add(searchBox)
form.Controls.Add(searchButton)
form.Controls.Add(deleteButton)
form.Controls.Add(statsLabel)
form.Controls.Add(checkRoleButton)
form.Controls.Add(switchRoleButton)
form.Controls.Add(saveToFileButton)

// Load students from file when the app starts
// Load students from file and update the list
loadStudentsFromFile filePath
updateStudentList ()

// Run the form
[<STAThread>]
do Application.Run(form)