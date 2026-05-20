# Secure Database App

## Project Overview

This ASP.NET Core MVC project demonstrates database security and secure development practices.

The application stores user data securely using password hashing, encryption, HMAC validation, input validation, and Entity Framework Core.

## Technologies Used

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- C#
- AES Encryption
- HMAC SHA256
- ASP.NET Core PasswordHasher

## Features

- Secure user registration
- Password hashing instead of plain text password storage
- Encryption of sensitive financial information
- HMAC validation for data integrity
- Input validation using Data Annotations
- SQL Injection prevention using Entity Framework Core
- CSRF protection using AntiForgeryToken
- HTTPS redirection
- Security headers
- Safe logging without sensitive data exposure

## Security Concepts Implemented

### 1. Password Hashing

Passwords are stored using ASP.NET Core PasswordHasher. Plain text passwords are never saved in the database.

### 2. Sensitive Data Encryption

Financial information is encrypted before being stored in SQL Server.

### 3. HMAC for Integrity

HMAC SHA256 is used to verify that encrypted data has not been modified.

### 4. SQL Injection Prevention

Entity Framework Core is used for database operations, which helps prevent SQL Injection by using parameterized queries.

### 5. CSRF Protection

Forms use AntiForgeryToken and controller POST actions use ValidateAntiForgeryToken.

### 6. Input Validation

User inputs are validated using Required, EmailAddress, StringLength, and MinLength attributes.

### 7. Secure Logging

Sensitive data such as passwords and financial information are not logged.

## How to Run

1. Open the project in Visual Studio.
2. Update the SQL Server connection string in `appsettings.json`.
3. Open Package Manager Console.
4. Run:

```powershell
Add-Migration InitialCreate
Update-Database
