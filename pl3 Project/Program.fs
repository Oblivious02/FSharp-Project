open System
open System.Windows.Forms

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