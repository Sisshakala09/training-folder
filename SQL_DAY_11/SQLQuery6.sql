SELECT * FROM departments1;
SELECT * FROM employees1;

//*****************************//

SELECT 
    e.id AS EmployeeID,
    e.name AS EmployeeName,
    d.name AS DepartmentName
FROM 
    employees1 e
INNER JOIN 
    departments1 d ON e.deptId = d.id;

//*****************************//

SELECT 
    e.id AS EmployeeID,
    e.name AS EmployeeName,
    d.name AS DepartmentName
FROM 
    employees1 e
RIGHT JOIN 
    departments1 d ON e.deptId = d.id;


//*****************************//