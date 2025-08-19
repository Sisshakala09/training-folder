/*use [WiproDB]
select count(id) from employees1
alter table employees add Salary real null
update employees1 set Salary=20000 where id=5
select * from employees1 
select deptid,sum (salary) as DeptSalary,count(id) as employees1Count from employees1 group by deptid
*/

/*
SELECT 
    e.EmpID,
    e.EmpName,
    e.Salary,
    d.DeptName
FROM 
    Employee e
JOIN 
    Department d ON e.DeptID = d.DeptID
WHERE 
    e.Salary > 50000;
    */


SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'employees1';


SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'employees1';

SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'depertment1';


emp_id
emp_name
salary
dept_id


dept_id
dept_name


SELECT 
    e.emp_id,
    e.emp_name,
    e.salary,
    d.dept_name
FROM 
    employees1 e
JOIN 
    departments1 d ON e.dept_id = d.dept_id
WHERE 
    e.salary > 50000;
