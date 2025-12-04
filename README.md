# Project X API Documentation

This document provides a detailed reference for the backend APIs available in Project X.

## Table of Contents
- [Authentication](#authentication)
- [1. Account APIs](#1-account-apis)
  - [Register User](#register-user)
  - [Login](#login)
  - [Forgot Password](#forgot-password)
  - [Verify Reset Password OTP](#verify-reset-password-otp)
- [2. Organization APIs](#2-organization-apis)
  - [Create Organization](#create-organization)
  - [Add Member](#add-member)
- [3. User APIs](#3-user-apis)
  - [Get User Role](#get-user-role)

## Base URL
`http://localhost:7180/api` (or your configured port)

## Authentication
Most endpoints require authentication via JWT Bearer Token.
- **Header**: `Authorization: Bearer <token>`

---

## 1. Account APIs
Base Path: `/api/Account`

### Register User
Creates a new user account.

- **URL**: `/Register`
- **Method**: `POST`
- **Auth**: None
- **Request Body**: `UserRegisterDTO`
  ```json
  {
    "fullName": "John Doe",       // Required, letters and spaces only
    "userName": "johndoe",        // Required, letters, numbers, underscores
    "email": "john@example.com",  // Required, Valid Email
    "confirmEmail": "john@example.com", // Must match email
    "phoneNumber": "1234567890",  // Required
    "password": "Password123!",   // Required
    "confirmPassword": "Password123!", // Must match password
    "role": "Admin"               // Required (Values: "Admin", "User")
  }
  ```
- **Response**:
  - `200 OK`: Registration Successful
  - `400 Bad Request`: Validation errors or email already exists.

### Login
Authenticates a user and returns a JWT token.

- **URL**: `/Login`
- **Method**: `POST`
- **Auth**: None
- **Request Body**: `LoginDTO`
  ```json
  {
    "email": "john@example.com",  // Required
    "password": "Password123!"    // Required
  }
  ```
- **Response**:
  - `200 OK`: Returns JWT Token.
  - `400 Bad Request`: Invalid credentials.

### Forgot Password
Initiates the password reset process by sending an OTP to the user's email.

- **URL**: `/Forgot-Password`
- **Method**: `POST`
- **Auth**: None
- **Rate Limit**: `OtpPolicy`
- **Request Body**: `ForgotPasswordDTO`
  ```json
  {
    "email": "john@example.com" // Required
  }
  ```
- **Response**:
  - `200 OK`: "If your email exists, an OTP will be sent."

### Verify Reset Password OTP
Verifies the OTP and resets the user's password.

- **URL**: `/verify-rest-password-otp`
- **Method**: `POST`
- **Auth**: None
- **Rate Limit**: `OtpPolicy`
- **Request Body**: `VerifyOtpResetPasswordDto`
  ```json
  {
    "email": "john@example.com",
    "otp": "123456",
    "newPassword": "NewPassword123!"
  }
  ```
- **Response**:
  - `200 OK`: Password Reset Successfully.
  - `400 Bad Request`: Invalid OTP or User not found.

---

## 2. Organization APIs
Base Path: `/api/Organization`

### Create Organization
Creates a new organization. Only Admins can create organizations.

- **URL**: `/create-organization`
- **Method**: `POST`
- **Auth**: Required (Role: `Admin`)
- **Request Body**: `CreateOrganizationDTO`
  ```json
  {
    "organizationName": "My Org",      // Required
    "organizationType": "Tech",        // Required
    "conatactEmail": "contact@org.com" // Required
  }
  ```
- **Response**:
  - `200 OK`: Organization Created Successfully. Returns `OrganizationResponseDTO`.
    ```json
    {
      "organizationId": 1,
      "organizationName": "My Org",
      "organizationType": "Tech",
      "conatactEmail": "contact@org.com",
      "organizationCode": 1234,
      "createdAt": "2023-10-27T10:00:00Z"
    }
    ```
  - `401 Unauthorized`: User is not Admin.

### Add Member
Adds an existing user to an organization.

- **URL**: `/add-member`
- **Method**: `POST`
- **Auth**: Required (Role: `Admin`)
- **Request Body**: `AddMemberDTO`
  ```json
  {
    "organizationId": 1,          // Required
    "email": "newmember@example.com", // Required, Valid Email
    "fullName": "New Member",     // Required
    "userName": "newmember",      // Required
    "password": "Password123!",   // Required
    "confirmPassword": "Password123!", // Required
    "role": "User"                     // Required (Values: "Admin", "User")
  }
  ```
- **Response**:
  - `200 OK`: Member added successfully.
  - `400 Bad Request`: User already exists or validation failed.
  - `404 Not Found`: Organization not found.

---

## 3. User APIs
Base Path: `/api/User`

### Get User Role
Retrieves the role of a user within a specific organization.

- **URL**: `/get-user-role`
- **Method**: `GET`
- **Auth**: None (Public access to check role?) *Note: Code does not specify [Authorize]*
- **Request Parameters** (Query String):
  - `OrgainzatinCode`: `1234` (Required)
  - `Email`: `john@example.com` (Required)
  - `Password`: `Password123!` (Required)
- **Response**:
  - `200 OK`: Returns User Role (e.g., "Admin" or "User").
  - `400 Bad Request`: Invalid credentials or user not a member.
  - `404 Not Found`: Organization not found.
