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


-- ? Insert a new employee into employees3 with all required columns
INSERT INTO employees3 (id, name, salary, department_id)
VALUES (106, 'AAA', 50000, 1);  -- Make sure 106 is a unique ID

-- ? Now display the table sorted by department_id (descending) and name (ascending)
SELECT * FROM employees3
ORDER BY department_id DESC, name;


-- ? Just insert other values and let SQL generate the id
INSERT INTO employees3 (name, salary, department_id)
VALUES ('Meera', 57000, 2);


-- ?? Use only when necessary
SET IDENTITY_INSERT employees3 ON;

INSERT INTO employees3 (id, name, salary, department_id)
VALUES (110, 'Meera', 57000, 2);

SET IDENTITY_INSERT employees3 OFF;




SELECT * FROM employees3 order by department_id desc,[name]
insert into employees3 ([name],department_id) values ('BBB',2)
SELECT * FROM employees3 where 1=1



