# Plan: FreeParkingMaui (Android)

Refactor the parking monitoring application using .NET MAUI (targeting .NET 8/9+).

## 1. Project Architecture (MVVM)
Use **CommunityToolkit.Mvvm** for efficient MVVM implementation.

### Folder Structure
- **Models**: Data transfer objects (DTOs) for API responses.
- **Services**: Business logic and infrastructure.
- **ViewModels**: State management and command logic.
- **Views**: XAML UI pages.
- **Resources/Raw**: Configuration files (optional).

## 2. Core Components

### Models
- `LoginResponse`: Token data.
- `OrderResponse`: Order status, car info, parking time.
- `Area`: ID and Name for selection.
- `SmsRequest`/`LoginRequest`.

### Services
1.  **ApiService**:
    -   Singleton `HttpClient`.
    -   Methods: `GetVerificationCode`, `Login`, `GetLatestOrder`.
    -   **Token Handling**: storage/retrieval via `SecureStorage`.
    -   **401 Handling**: Detect 401, clear token, trigger navigation to Login.
2.  **EmailService**:
    -   Implementation using `System.Net.Mail` (SmtpClient) for background email sending.
    -   Configuration: SMTP Host, Port, User, Password (to be defined in Constants).
3.  **BackgroundService**:
    -   Use `DeviceDisplay.KeepScreenOn` to keep the app running in the foreground (simplest approach for "old phone" use case).
    -   Use `IDispatcherTimer` or `PeriodicTimer` for the 10-minute polling loop.

## 3. ViewModels

### LoginViewModel
-   Properties: `PhoneNumber`, `AuthCode`, `SelectedArea`, `IsBusy`, `Countdown`.
-   Commands: `GetCodeCommand` (with 60s timer), `LoginCommand`.
-   Logic: Input validation, API calls, Navigation to MainPage upon success.

### MainViewModel (Monitor)
-   Properties: `UserPhone`, `AreaName`, `ApiStatus` (Pending/Running/Error), `NextCheckTime`, `Countdown`, `LatestOrder` info, `IsMonitoring`.
-   Commands: `StartMonitoringCommand`, `StopMonitoringCommand`, `CheckNowCommand`, `LogoutCommand`.
-   Logic:
    -   `StartMonitoring`: Sets `KeepScreenOn`, starts periodic timer (10 min).
    -   `CheckOrder`: Calls API. If new order (!= 3) detected, call `EmailService`.
    -   `Logout`: Clears token, stops timer, navigates to Login.

## 4. UI Design (XAML)

### LoginPage
-   `Picker` for Area selection.
-   `Entry` for Phone (Numeric).
-   `Entry` for Code + "Get Code" Button (Horizontal Stack).
-   "Login" Button.

### MainPage (Monitor)
-   **Header**: Status Indicator (Green/Gray).
-   **Info Card**: Current User, Area, API Status.
-   **Timer Card**: Last Check, Next Check, Countdown.
-   **Order Card**: (Visible if order exists) Status, Car Plate, Start Time.
-   **Action Bar**: "Check Now", "Logout".
-   **Log Console**: `CollectionView` for scrolling logs.

## 5. Implementation Steps
1.  **Setup**: Add NuGet packages (`CommunityToolkit.Mvvm`).
2.  **Infrastructure**: Create `Constants.cs` (API URLs, SMTP Config), `AppShell` routing.
3.  **Logic**: Implement Services and ViewModels.
4.  **UI**: Build XAML pages.
5.  **Integration**: Wire up DI (Dependency Injection) in `MauiProgram.cs`.
6.  **Android Config**: permissions (`INTERNET`), manifest settings.

## 6. Email Configuration
Since automatic email sending requires SMTP credentials, I will create a placeholder configuration in `Constants.cs` that you will need to fill with your real SMTP details (e.g., Gmail App Password).
