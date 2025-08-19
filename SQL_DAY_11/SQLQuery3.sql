create table departments1(
id int identity(1,1) primary key,
[name] nvarchar(50) not null,
)


create table employees1(
id int identity(1,1) primary key,
[name] nvarchar(50) not null,
deptId int not null,
foreign key (deptId) references departments1(id),
)


//*********************************************//

DROP TABLE IF EXISTS employees3;
DROP TABLE IF EXISTS departments3;


CREATE TABLE departments3 (
    id INT PRIMARY KEY,
    name VARCHAR(50)
);


INSERT INTO departments3 (id, name) VALUES
(1, 'IT'),
(2, 'HR'),
(3, 'Sales'),
(4, 'Marketing');


CREATE TABLE employees3 (
    id INT PRIMARY KEY,
    name VARCHAR(50),
    salary DECIMAL(10, 2),
    department_id INT,
    FOREIGN KEY (department_id) REFERENCES departments3(id)
);

INSERT INTO employees3 (id, name, salary, department_id) VALUES
(101, 'Alice', 60000, 1),
(102, 'Bob', 45000, 2),
(103, 'Charlie', 70000, 1),
(104, 'David', 55000, 3),
(105, 'Eve', 48000, 4);


SELECT name
FROM employees3
WHERE department_id = (
    SELECT id FROM departments3 WHERE name = 'IT'
);


SELECT 
    name,
    (SELECT name FROM departments3 WHERE id = employees3.department_id) AS department_name
FROM employees3;


SELECT department_id, AVG(salary) AS avg_salary
FROM (
    SELECT department_id, salary FROM employees3
) AS dept_salaries
GROUP BY department_id;


SELECT name, salary, department_id
FROM employees3 e
WHERE salary > (
    SELECT AVG(salary)
    FROM employees3
    WHERE department_id = e.department_id
);


SELECT name
FROM employees3
WHERE department_id IN (
    SELECT id FROM departments3 WHERE name IN ('Sales', 'Marketing')
);


SELECT name
FROM departments3 d
WHERE EXISTS (
    SELECT 1 FROM employees3 e WHERE e.department_id = d.id
);

//*********************************************//

Basic View
CREATE VIEW vw_EmployeeNames AS
SELECT FirstName, LastName
FROM Employees;

Where clause
CREATE VIEW vw_ActiveCustomers AS
SELECT CustomerID, Name, Email
FROM Customers
WHERE IsActive = 1;

View with join
CREATE VIEW vw_EmployeeDepartments AS
SELECT e.EmployeeID, e.FirstName, e.LastName, d.DepartmentName
FROM Employees e
JOIN Departments d ON e.DepartmentId = d.DepartmentId;

Aggregate view
CREATE VIEW vw_TotalOrdersPerCustomer AS
SELECT CustomerID, COUNT(OrderID) AS TotalOrders
FROM Orders
GROUP BY CustomerID;

Computed columns
CREATE VIEW vw_ProductStockStatus AS
SELECT ProductID, ProductName, Quantity,
       CASE 
           WHEN Quantity = 0 THEN 'Out of Stock'
           WHEN Quantity < 10 THEN 'Low Stock'
           ELSE 'In Stock'
       END AS StockStatus
FROM Products;

View from a table, hiding some columns
CREATE VIEW vw_PublicEmployeeInfo AS

SELECT EmployeeID, FirstName, LastName, Department
FROM Employees;
-- Note: Salary, SSN, and other sensitive fields are not exposed.

//*****************************************//


CREATE TRIGGER trg_AfterDelete_Orders
ON employees
AFTER DELETE
AS
BEGIN
    INSERT INTO employeesbkp
    SELECT * FROM deleted;
END;



CREATE TRIGGER trg_PreventVIPDelete
ON Customers
INSTEAD OF DELETE
AS
BEGIN
    IF EXISTS (SELECT 1 FROM deleted WHERE IsVIP = 1)
    BEGIN
        RAISERROR ('Cannot delete VIP customers', 16, 1);
    END
    ELSE
    BEGIN
        DELETE FROM Customers
        WHERE CustomerID IN (SELECT CustomerID FROM deleted);
    END
END;




