CREATE DATABASE SchoolManager;
GO

USE SchoolManager;
GO

-- Table for employees with distinct roles
CREATE TABLE Employees(
    EmployeeID INT IDENTITY (1,1) PRIMARY KEY,
    EmployeeRole NVARCHAR(20) NOT NULL,
    EmployeeFirstName NVARCHAR(20) NOT NULL,
    EmployeeLastName NVARCHAR(20) NOT NULL,
	Salary INT NOT NULL,
	EmployeeStart DATE NOT NULL
);
GO
	
-- Table for students
CREATE TABLE Students(
    StudentID INT IDENTITY (1,1) PRIMARY KEY,
    StudentFirstName NVARCHAR(20) NOT NULL,
    StudentLastName NVARCHAR(20) NOT NULL,
    StudentPersonNumber NVARCHAR(20) NOT NULL UNIQUE,
    ClassID INT NOT NULL -- Foreign key to Classes table
);
GO

-- Table for classes
CREATE TABLE Classes(
    ClassID INT IDENTITY (1,1) PRIMARY KEY,
    ClassName NVARCHAR(30) NOT NULL --
);
GO

-- Table for courses
CREATE TABLE Courses(
    CourseID INT IDENTITY (1,1) PRIMARY KEY,
    CourseName NVARCHAR(50) NOT NULL,
    EmployeeID INT NOT NULL,
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID)
);
GO

-- Table for grades assigned to students for specific courses
CREATE TABLE Grades(
    GradeID INT IDENTITY (1,1) PRIMARY KEY,
    StudentID INT NOT NULL, -- Foreign key to Students table
    EmployeeID INT NOT NULL, -- Foreign key to Employees table (teacher who assigned the grade)
    CourseID INT NOT NULL, -- Foreign key to Courses table
    Grade NVARCHAR(2) NOT NULL,
    AssignedDate DATE NOT NULL,
    FOREIGN KEY (StudentID) REFERENCES Students(StudentID),
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID),
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);
GO

-- Adding foreign key constraints
ALTER TABLE Students
ADD CONSTRAINT FK_Students_Classes FOREIGN KEY (ClassID) REFERENCES Classes(ClassID);
GO

-- Insert test data into Employees table
INSERT INTO Employees (EmployeeRole, EmployeeFirstName, EmployeeLastName, Salary, EmployeeStart) VALUES
('Teacher', 'Anna', 'Andersson', 40000, '2015-08-01'),
('Teacher', 'Anders', 'Ang', 42000, '2015-09-01'),
('Teacher', 'Albert', 'Angström', 43000, '2014-10-01'),
('Teacher', 'Bertil', 'Berg', 41000, '2016-11-01'),
('Cleaner', 'Tom', 'Tanga', 25000, '2015-01-01'),
('Cleaner', 'Bodil', 'Berg', 26000, '2015-02-01'),
('Cook', 'Nina', 'Nellson', 30000, '2015-03-01'),
('Cook', 'Nadia', 'Nagel', 31000, '2015-04-01'),
('IT', 'Sam', 'Sammers', 50000, '2015-05-01'),
('Administrator', 'Cecilia', 'Carlsson', 45000, '2015-06-01'),
('Principal', 'David', 'Dahl', 60000, '2015-07-01');
GO

-- Insert test data into Classes table
INSERT INTO Classes (ClassName) VALUES
('Class A'),
('Class B'),
('Class C'),
('Class D');
GO

-- Insert test data into Courses table
INSERT INTO Courses (CourseName, EmployeeID) VALUES
('Math', 1),
('English', 2),
('Programming', 3),
('Swedish', 4);
GO

-- Insert test data into Students table
INSERT INTO Students (StudentFirstName, StudentLastName, StudentPersonNumber, ClassID) VALUES
('Eva', 'Eriksson', '20000101-1234', 1),
('Fredrik', 'Fors', '20000102-2345', 1),
('Greta', 'Gustafsson', '20000201-3456', 2),
('Hanna', 'Hansen', '20000202-4567', 2),
('Isak', 'Isaksson', '20000301-5678', 3),
('Julia', 'Johansson', '20000302-6789', 3),
('Kevin', 'Karlsson', '20000401-7890', 4),
('Lina', 'Larsson', '20000402-8901', 4),
('Martin', 'Magnusson', '20010101-9012', 1),
('Nina', 'Nilsson', '20010102-0123', 1),
('Oscar', 'Olofsson', '20010201-1234', 2),
('Petra', 'Persson', '20010202-2345', 2),
('Quentin', 'Qvist', '20010301-3456', 3),
('Rebecca', 'Rasmussen', '20010302-4567', 3),
('Simon', 'Svensson', '20010401-5678', 4),
('Tina', 'Thomasson', '20010402-6789', 4),
('Ulf', 'Ulriksson', '20020101-7890', 1),
('Vera', 'Viktorsson', '20020102-8901', 1),
('William', 'Wikström', '20020201-9012', 2),
('Xenia', 'Xandersson', '20020202-0123', 2);
GO

-- Assign each student to every course
INSERT INTO Grades (StudentID, EmployeeID, CourseID, Grade, AssignedDate)
SELECT s.StudentID, c.EmployeeID, c.CourseID, 'N/A', GETDATE()
FROM Students s
CROSS JOIN Courses c;
GO

-- Query to fetch all teachers
SELECT * FROM Employees
WHERE EmployeeRole = 'Teacher';
GO

-- Query to fetch all students in alphabetical order by last name
SELECT * FROM Students
ORDER BY StudentLastName;
GO

-- Query to fetch all students in a specific class
SELECT s.* FROM Students s
JOIN Classes c ON s.ClassID = c.ClassID
WHERE c.ClassName = 'Class A';
GO

-- Query to fetch all grades assigned in the last month
SELECT g.* FROM Grades g
WHERE AssignedDate >= DATEADD(MONTH, -1, GETDATE());
GO

CREATE PROCEDURE GetStudentInfo
    @StudentID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        s.StudentID,
        s.StudentFirstName,
        s.StudentLastName,
        s.StudentPersonNumber,
        s.ClassID,
        c.ClassName
    FROM 
        Students s
    LEFT JOIN 
        Classes c ON s.ClassID = c.ClassID
    WHERE 
        s.StudentID = @StudentID;
END;
GO
