create table salaryhistory(
empid int,
oldsalary real,
newsalary real,
changedon date
)


CREATE TRIGGER trg_AfterUpdate_Salary
ON Employees
AFTER UPDATE
AS
BEGIN
    INSERT INTO SalaryHistory (empid, OldSalary, NewSalary, ChangedOn)
    SELECT 
        i.id, 
        d.salary, 
        i.EmpSalary, 
        GETDATE()
    FROM inserted i
    INNER JOIN deleted d ON i.id = d.id
    WHERE i.EmpSalary <> d.EmpSalary;
END;


create table employeeaudit(
id int not null,
[action] nvarchar(30) not null,
actiondate datetime not null
)

select * from employeeaudit
INSERT INTO Employees (EmpName, DeptId, EmpSalary)
VALUES ('Thomas', 3, 40000);

CREATE TRIGGER trg_AfterInsert_Employees
ON Employees
AFTER INSERT
AS
BEGIN
    INSERT INTO EmployeeAudit (id, [Action], ActionDate)
    SELECT id, 'INSERT', GETDATE()
    FROM inserted;
END;


select * from employeeaudit
INSERT INTO Employees (Name, deptId, salary)
VALUES ('Thomas', 3, 40000);



CREATE TRIGGER trg_AfterDelete_Orders
ON employees
AFTER DELETE
AS
BEGIN
    INSERT INTO employeesbkp
    SELECT * FROM deleted;
END;

delete from employees where id=4

SELECT * INTO employeesbkp FROM employees WHERE 1 = 0;
CREATE TRIGGER trg_AfterDelete_Orders
ON employees
AFTER DELETE
AS
BEGIN
    INSERT INTO employeesbkp
    SELECT * FROM deleted;
END;


CREATE TRIGGER trg_AfterDelete_Orders
ON employees
AFTER DELETE
AS
BEGIN
    INSERT INTO employeesbkp
    SELECT * FROM deleted;
END;
