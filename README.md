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
  - [Get Organization](#get-organization)
  - [Update Organization](#update-organization)
  - [Delete Organization](#delete-organization)
  - [Add Member](#add-member)
- [3. Hall APIs](#3-hall-apis)
  - [Create Hall](#create-hall)
  - [Get All Halls](#get-all-halls)
  - [Get Hall By ID](#get-hall-by-id)
  - [Update Hall](#update-hall)
  - [Delete Hall](#delete-hall)
- [4. Session APIs](#4-session-apis)
  - [Create Session](#create-session)
  - [Get Session By ID](#get-session-by-id)
  - [Get Sessions By Hall ID](#get-sessions-by-hall-id)
  - [Update Session](#update-session)
  - [Delete Session](#delete-session)
- [5. User APIs](#5-user-apis)
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
- **Request Body**:
  ```json
  {
    "fullName": "Abdulrahman Awwad",       // Required, letters and spaces only
    "userName": "johndoe",        // Required, letters, numbers, underscores
    "email": "Abdulrahman@example.com",  // Required, Valid Email
    "confirmEmail": "Abdulrahman@example.com", // Must match email
    "phoneNumber": "1234567890",  // Required
    "password": "Password123!",   // Required
    "confirmPassword": "Password123!", // Must match password
    "role": "Admin"               // Required (Values: "Admin", "User")
  }
  ```
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "User Registered Successfully",
      "data": "User Registered Successfully",
      "errors": []
    }
    ```
  - `400 Bad Request`: Validation errors or email already exists.

### Login
Authenticates a user and returns a JWT token.

- **URL**: `/Login`
- **Method**: `POST`
- **Auth**: None
- **Request Body**:
  ```json
  {
    "email": "Abdulrahman@example.com",  // Required
    "password": "Password123!"    // Required
  }
  ```
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "Login Successful",
      "data": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "errors": []
    }
    ```
  - `400 Bad Request`: Invalid credentials.

### Forgot Password
Initiates the password reset process by sending an OTP to the user's email.

- **URL**: `/Forgot-Password`
- **Method**: `POST`
- **Auth**: None
- **Rate Limit**: `OtpPolicy`
- **Request Body**:
  ```json
  {
    "email": "Abdulrahman@example.com" // Required
  }
  ```
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "If your email exists, an OTP will be sent.",
      "data": null,
      "errors": []
    }
    ```

### Verify Reset Password OTP
Verifies the OTP and resets the user's password.

- **URL**: `/verify-rest-password-otp`
- **Method**: `POST`
- **Auth**: None
- **Rate Limit**: `OtpPolicy`
- **Request Body**:
  ```json
  {
    "email": "Abdulrahman@example.com",
    "otp": "123456",
    "newPassword": "NewPassword123!"
  }
  ```
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "Password Reset Successfully",
      "data": null,
      "errors": []
    }
    ```
  - `400 Bad Request`: Invalid OTP or User not found.

---

## 2. Organization APIs
Base Path: `/api/Organization`

### Create Organization
Creates a new organization. Only Admins can create organizations.

- **URL**: `/create-organization`
- **Method**: `POST`
- **Auth**: Required (Role: `Admin`)
- **Request Body**:
  ```json
  {
    "organizationName": "My Org",      // Required
    "organizationType": "Tech",        // Required
    "conatactEmail": "contact@org.com" // Required
  }
  ```
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "Organization Created Successfully",
      "data": {
        "organizationId": 1,
        "organizationName": "My Org",
        "organizationType": "Tech",
        "conatactEmail": "contact@org.com",
        "organizationCode": 1234,
        "createdAt": "2023-10-27T10:00:00Z"
      },
      "errors": []
    }
    ```
  - `401 Unauthorized`: User is not Admin.

### Get Organization
Retrieves organization details by ID.

- **URL**: `/{id}`
- **Method**: `GET`
- **Auth**: Required (Role: `Admin`)
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "Organization Retrieved Successfully",
      "data": {
        "organizationId": 1,
        "organizationName": "My Org",
        "organizationType": "Tech",
        "conatactEmail": "contact@org.com",
        "organizationCode": 1234,
        "createdAt": "2023-10-27T10:00:00Z"
      },
      "errors": []
    }
    ```
  - `404 Not Found`: Organization not found.

### Update Organization
Updates an organization's details.

- **URL**: `/{id}`
- **Method**: `PUT`
- **Auth**: Required (Role: `Admin`)
- **Request Body**:
  ```json
  {
    "organizationName": "Updated Org Name",      // Required
    "organizationType": "Updated Type",         // Required
    "conatactEmail": "updated@org.com"         // Required
  }
  ```
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "Organization Updated Successfully",
      "data": {
        "organizationId": 1,
        "organizationName": "Updated Org Name",
        "organizationType": "Updated Type",
        "conatactEmail": "updated@org.com",
        "organizationCode": 1234,
        "createdAt": "2023-10-27T10:00:00Z"
      },
      "errors": []
    }
    ```
  - `400 Bad Request`: Validation errors.

### Delete Organization
Deletes an organization by ID.

- **URL**: `/{id}`
- **Method**: `DELETE`
- **Auth**: Required (Role: `Admin`)
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "Organization deleted successfully",
      "data": null,
      "errors": []
    }
    ```
  - `400 Bad Request`: Error occurred.

### Add Member
Adds an existing user to an organization.

- **URL**: `/add-member`
- **Method**: `POST`
- **Auth**: Required (Role: `Admin`)
- **Request Body**:
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
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "Member added successfully",
      "data": null,
      "errors": []
    }
    ```
  - `400 Bad Request`: User already exists or validation failed.
  - `404 Not Found`: Organization not found.

---

## 3. Hall APIs
Base Path: `/api/Hall`

### Create Hall
Creates a new hall in an organization.

- **URL**: `/create-hall`
- **Method**: `POST`
- **Auth**: Required (Role: `Admin`)
- **Request Body**:
  ```json
  {
    "hallName": "Main Hall",
    "capacity": 100,
    "hallArea": 50.5,
    "organizationId": 1
  }
  ```
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "Hall created successfully",
      "data": {
        "id": 1,
        "hallName": "Main Hall",
        "capacity": 100,
        "hallArea": 50.5,
        "organizationId": 1
      },
      "errors": []
    }
    ```

### Get All Halls
Retrieves all halls for a specific organization.

- **URL**: `/get-all-halls`
- **Method**: `GET`
- **Auth**: Required (Role: `Admin`)
- **Query Parameters**:
  - `organizationId`: `1` (Required)
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "Halls retrieved successfully",
      "data": [
        {
          "id": 1,
          "hallName": "Main Hall",
          "capacity": 100,
          "hallArea": 50.5,
          "organizationId": 1
        }
      ],
      "errors": []
    }
    ```

### Get Hall By ID
Retrieves a specific hall by ID.

- **URL**: `/{hallId}`
- **Method**: `GET`
- **Auth**: Required (Role: `Admin`)
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "Hall retrieved successfully",
      "data": {
        "id": 1,
        "hallName": "Main Hall",
        "capacity": 100,
        "hallArea": 50.5,
        "organizationId": 1
      },
      "errors": []
    }
    ```
  - `404 Not Found`: Hall not found.

### Update Hall
Updates a hall's details.

- **URL**: `/{hallId}`
- **Method**: `PUT`
- **Auth**: Required (Role: `Admin`)
- **Request Body**:
  ```json
  {
    "hallName": "Updated Hall Name",
    "capacity": 150,
    "hallArea": 75.0
  }
  ```
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "Hall updated successfully",
      "data": {
        "id": 1,
        "hallName": "Updated Hall Name",
        "capacity": 150,
        "hallArea": 75.0,
        "organizationId": 1
      },
      "errors": []
    }
    ```

### Delete Hall
Deletes a hall by ID.

- **URL**: `/{hallId}`
- **Method**: `DELETE`
- **Auth**: Required (Role: `Admin`)
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "Hall deleted successfully",
      "data": null,
      "errors": []
    }
    ```

---

## 4. Session APIs
Base Path: `/api/Session`

### Create Session
Creates a new attendance session.

- **URL**: `/Create-Session`
- **Method**: `POST`
- **Auth**: Required (Role: `Admin`)
- **Request Body**:
  ```json
  {
    "organizationId": 1,
    "createdBy": "userid-guid",
    "sessionName": "Morning Session",
    "connectionType": 0, // Enum Value
    "longitude": 0,
    "latitude": 0,
    "allowedRadius": 50,
    "networkSSID": "WifiName",
    "networkBSSID": "MacAddress",
    "startAt": "2023-10-27T10:00:00Z",
    "endAt": "2023-10-27T12:00:00Z",
    "hallId": 1
  }
  ```
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "Session Created Successfully",
      "data": null,
      "errors": []
    }
    ```

### Get Session By ID
Retrieves a session by its ID.

- **URL**: `/{id}`
- **Method**: `GET`
- **Auth**: Required (Role: `Admin`)
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "Session Retrieved Successfully",
      "data": {
        "sessionId": 1,
        "organizationId": 1,
        "createdBy": "userid-guid",
        "sessionName": "Morning Session",
        "createdAt": "2023-10-27T09:00:00Z",
        "connectionType": 0,
        "longitude": 0,
        "latitude": 0,
        "allowedRadius": 50,
        "networkSSID": "WifiName",
        "networkBSSID": "MacAddress",
        "startAt": "2023-10-27T10:00:00Z",
        "endAt": "2023-10-27T12:00:00Z",
        "hallId": 1
      },
      "errors": []
    }
    ```
  - `404 Not Found`: Session not found.

### Get Sessions By Hall ID
Retrieves all sessions associated with a specific hall.

- **URL**: `/hall/{hallId}`
- **Method**: `GET`
- **Auth**: Required (Role: `Admin`)
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "Sessions Retrieved Successfully",
      "data": [
        {
          "sessionId": 1,
          "organizationId": 1,
          "createdBy": "userid-guid",
          "sessionName": "Morning Session",
          "createdAt": "2023-10-27T09:00:00Z",
          "connectionType": 0,
          "longitude": 0,
          "latitude": 0,
          "allowedRadius": 50,
          "networkSSID": "WifiName",
          "networkBSSID": "MacAddress",
          "startAt": "2023-10-27T10:00:00Z",
          "endAt": "2023-10-27T12:00:00Z",
          "hallId": 1
        }
      ],
      "errors": []
    }
    ```

### Update Session
Updates a session's details.

- **URL**: `/{id}`
- **Method**: `PUT`
- **Auth**: Required (Role: `Admin`)
- **Request Body**:
  ```json
  {
    "sessionName": "Updated Session",
    "connectionType": 0,
    "longitude": 0,
    "latitude": 0,
    "allowedRadius": 50,
    "networkSSID": "WifiName",
    "networkBSSID": "MacAddress",
    "startAt": "2023-10-27T10:00:00Z",
    "endAt": "2023-10-27T12:00:00Z",
    "hallId": 1
  }
  ```
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "Session updated successfully",
      "data": {
        "sessionId": 1,
        "organizationId": 1,
        "createdBy": "userid-guid",
        "sessionName": "Updated Session",
        "createdAt": "2023-10-27T09:00:00Z",
        "connectionType": 0,
        "longitude": 0,
        "latitude": 0,
        "allowedRadius": 50,
        "networkSSID": "WifiName",
        "networkBSSID": "MacAddress",
        "startAt": "2023-10-27T10:00:00Z",
        "endAt": "2023-10-27T12:00:00Z",
        "hallId": 1
      },
      "errors": []
    }
    ```

### Delete Session
Deletes a session by ID.

- **URL**: `/{id}`
- **Method**: `DELETE`
- **Auth**: Required (Role: `Admin`)
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "Session deleted successfully",
      "data": null,
      "errors": []
    }
    ```

---

## 5. User APIs
Base Path: `/api/User`

### Get User Details
Retrieves the role of a user within a specific organization.

- **URL**: `/get-user`
- **Method**: `POST`
- **Auth**: None
- **Request Parameters** :
```json
{
  "OrgainzatinCode": "1234",
  "Email": "Abdulrahman@example.com",
  "Password": "Password123!"
}
```
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "User Found",
      "data": {
        "id": "e98...",
        "fullName": "John Doe",
        "userName": "johndoe",
        "email": "john@example.com",
        "phoneNumber": "123456",
        "role": "User",
        "createdAt": "2023-01-01T00:00:00Z",
        "updatedAt": "2023-01-01T00:00:00Z"
      },
      "errors": []
    }
    ```

### Get User Role
Retrieves just the role of a user within a specific organization.

- **URL**: `/get-user-role`
- **Method**: `POST`
- **Auth**: None
- **Request Parameters**:
```json
{
  "OrgainzatinCode": "1234",
  "Email": "Abdulrahman@example.com",
  "Password": "Password123!"
}
```
- **Response**:
  - `200 OK`:
    ```json
    {
      "success": true,
      "message": "User is retrieved successfully",
      "data": "User",
      "errors": []
    }
    ```
