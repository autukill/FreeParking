# FreeParkingMaui

An Android smart parking monitoring application built with .NET MAUI for real-time parking order tracking and email notifications.

## Features

- **User Login**: Support for phone number + verification code login
- **Area Selection**: Support for multiple parking area selection
- **Real-time Monitoring**: Automatically query latest parking orders every 10 minutes
- **Email Notifications**: Automatic email alerts when new orders are detected
- **Background Operation**: Keep screen on to ensure uninterrupted monitoring
- **Email Configuration**: Dynamic SMTP email configuration support

## Tech Stack

- **Framework**: .NET MAUI (.NET 10)
- **Architecture Pattern**: MVVM (Model-View-ViewModel)
- **MVVM Toolkit**: CommunityToolkit.Mvvm
- **Email Service**: MailKit
- **Data Storage**: SecureStorage (secure storage for tokens and configurations)

## Project Structure

```
FreeParkingMaui/
├── Constants.cs              # Constants configuration (API URLs, area list, SMTP config)
├── MauiProgram.cs            # Application startup configuration, dependency injection
├── Models/
│   └── Models.cs             # Data models (LoginResponse, OrderResponse, etc.)
├── Services/
│   ├── ApiService.cs         # HTTP API service
│   └── EmailService.cs       # Email sending service
├── ViewModels/
│   ├── LoginViewModel.cs     # Login page view model
│   └── MainViewModel.cs      # Main page view model
├── Views/
│   ├── LoginPage.xaml        # Login page
│   ├── LoginPage.xaml.cs
│   ├── MainPage.xaml         # Monitoring main page
│   └── MainPage.xaml.cs
├── Converters/
│   ├── BoolConverters.cs     # Boolean value converters
│   └── InvertedBoolConverter.cs
└── Platforms/                # Platform-specific code
```

## Page Descriptions

### 1. LoginPage

User login interface with the following features:
- Area selector (Picker)
- Phone number input field
- Verification code input field
- Get verification code button (60-second countdown)
- Login button

### 2. MainPage (Monitor)

Main monitoring interface with the following features:
- **Status Indicator**: Displays monitoring running status
- **User Info Card**: Shows current user phone number and area
- **Timer Card**: Displays last check time, next check time, and countdown
- **Order Card**: Shows latest parking order info (license plate, parking lot, start time, status)
- **Action Buttons**: Check now, logout, start/stop monitoring
- **Email Settings**: Click the settings button in the top right to configure SMTP email

## Configuration

### Email Configuration (QQ Mail)

1. Click the ⚙ settings button in the top right corner of the main page
2. Enter your QQ email address
3. Enter the SMTP authorization code (not your email password)
   - How to get: QQ Mail Settings → Account → Enable SMTP Service → Generate authorization code

### Supported Areas

| Area ID | Area Name |
|---------|-----------|
| c7bd547b-fb67-4554-a1e5-0842f69f4e56 | (Huaning County) |

## How to Run

### Requirements

- .NET 10 SDK
- Visual Studio 2022 or VS Code
- Android SDK (for running Android version)

### Start the Application

```bash
# Restore dependencies
dotnet restore

# Run Android version
dotnet build -t:Run -f net10.0-android

# Or launch debugging directly in Visual Studio
```

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| CommunityToolkit.Mvvm | 8.4.0 | MVVM toolkit |
| MailKit | 4.15.0 | Email sending |
| Microsoft.Maui.Controls | $(MauiVersion) | MAUI framework |

## Notes

1. **Token Expiration Handling**: When the API returns 401, the app will automatically stop monitoring and prompt to log in again
2. **Email Sending**: New order detection logic is `State != 3`, and emails are only sent when the order state changes from 3 to another state
3. **Screen Always On**: `DeviceDisplay.KeepScreenOn` is enabled when monitoring starts and automatically turned off when stopped
4. **Secure Storage**: Sensitive information such as user tokens and email configurations are encrypted using `SecureStorage`

## License

MIT License
