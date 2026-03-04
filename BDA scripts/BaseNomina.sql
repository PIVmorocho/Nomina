CREATE DATABASE NominaDB;
GO
USE NominaDB;
GO

-- Tabla employees
CREATE TABLE employees (
    emp_no      INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
    ci          VARCHAR(50)   NOT NULL,
    birth_date  DATE          NOT NULL,
    first_name  VARCHAR(50)   NOT NULL,
    last_name   VARCHAR(50)   NOT NULL,
    gender      CHAR(1)       NOT NULL CHECK (gender IN ('M','F')),
    hire_date   DATE          NOT NULL,
    correo      VARCHAR(100)  NULL,
    activo      BIT           NOT NULL DEFAULT 1,
    CONSTRAINT UQ_employees_ci     UNIQUE (ci),
    CONSTRAINT UQ_employees_correo UNIQUE (correo)
);

-- Tabla departments
CREATE TABLE departments (
    dept_no    INT          NOT NULL IDENTITY(1,1) PRIMARY KEY,
    dept_name  VARCHAR(50)  NOT NULL,
    activo     BIT          NOT NULL DEFAULT 1
);

-- Tabla dept_emp
CREATE TABLE dept_emp (
    emp_no     INT   NOT NULL,
    dept_no    INT   NOT NULL,
    from_date  DATE  NOT NULL,
    to_date    DATE  NULL,
    CONSTRAINT PK_dept_emp         PRIMARY KEY (emp_no, dept_no, from_date),
    CONSTRAINT FK_dept_emp_emp     FOREIGN KEY (emp_no)  REFERENCES employees(emp_no),
    CONSTRAINT FK_dept_emp_dept    FOREIGN KEY (dept_no) REFERENCES departments(dept_no)
);

-- Tabla dept_manager
CREATE TABLE dept_manager (
    emp_no     INT   NOT NULL,
    dept_no    INT   NOT NULL,
    from_date  DATE  NOT NULL,
    to_date    DATE  NULL,
    CONSTRAINT PK_dept_manager      PRIMARY KEY (emp_no, dept_no, from_date),
    CONSTRAINT FK_dept_manager_emp  FOREIGN KEY (emp_no)  REFERENCES employees(emp_no),
    CONSTRAINT FK_dept_manager_dept FOREIGN KEY (dept_no) REFERENCES departments(dept_no)
);

-- Tabla titles
CREATE TABLE titles (
    emp_no     INT          NOT NULL,
    title      VARCHAR(50)  NOT NULL,
    from_date  DATE         NOT NULL,
    to_date    DATE         NULL,
    CONSTRAINT PK_titles     PRIMARY KEY (emp_no, title, from_date),
    CONSTRAINT FK_titles_emp FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
);

-- Tabla salaries
CREATE TABLE salaries (
    emp_no     INT     NOT NULL,
    salary     BIGINT  NOT NULL,
    from_date  DATE    NOT NULL,
    to_date    DATE    NULL,
    CONSTRAINT PK_salaries     PRIMARY KEY (emp_no, from_date),
    CONSTRAINT FK_salaries_emp FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
);

-- Tabla users
CREATE TABLE users (
    usuario  VARCHAR(100)  NOT NULL PRIMARY KEY,
    emp_no   INT           NOT NULL,
    clave    VARCHAR(100)  NOT NULL,
    rol      VARCHAR(50)   NOT NULL DEFAULT 'RRHH',
    activo   BIT           NOT NULL DEFAULT 1,
    CONSTRAINT FK_users_emp FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
);

-- Tabla Log_AuditoriaSalarios
CREATE TABLE Log_AuditoriaSalarios (
    id                 INT           NOT NULL IDENTITY(1,1) PRIMARY KEY,
    usuario            VARCHAR(50)   NOT NULL,
    fechaActualizacion DATE          NOT NULL,
    DetalleCambio      VARCHAR(250)  NOT NULL,
    salario            BIGINT        NOT NULL,
    emp_no             INT           NOT NULL,
    CONSTRAINT FK_audit_emp FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
);

-- Datos iniciales: empleado y usuario administrador
INSERT INTO employees (ci, birth_date, first_name, last_name, gender, hire_date, correo)
VALUES ('0000000001', '1980-01-01', 'Admin', 'Sistema', 'M', GETDATE(), 'admin@nomina.com');

-- Contraseña: Admin123 (hash BCrypt)
INSERT INTO users (usuario, emp_no, clave, rol)
VALUES ('admin', 1, '$2a$11$K7Ei3B1v9E3pJ4Jgj4NQyuHVfOHR0.O7KD1V3qz3FwwMv5D5cQVCC', 'Administrador');

GO