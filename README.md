# DG Digital Commerce – membership Service

The **DG membership Service** is part of the **DG Digital Commerce** ecosystem.  
It provides APIs for managing membershipservice

---

## 🚀 Getting Started

Follow the steps below to set up and run the **membership Service** locally.

### 1. Clone the Repository

Use SSH to clone the repository:

```bash
https://github.com/DG-DigitalCommerce-Dev/MembershipService.git
```

---

## 🧰 Prerequisites

Make sure the following tools are installed:

- **Visual Studio 2022** (recommended)
- **.NET 8.0 SDK**

> 💡 During Visual Studio installation, select the **ASP.NET and Web Development** workload.  
> This will automatically include the .NET SDK and required web components.

---

## 🧑‍💻 Running the Project Locally

1. Open the solution file `dg-membership-service.sln` in **Visual Studio**.  
2. Allow Visual Studio to restore the **NuGet packages** automatically.  
3. Set the startup project (if needed) and press **Ctrl + F5** to run the application.  
4. The API will start locally — you can access Swagger UI in your browser at:

```
https://localhost:<port>/swagger
```

---

## 📦 Dependencies

The following NuGet packages are used in this project:

| Package | Version |
|----------|----------|
| Microsoft.AspNet.WebApi.Core | 5.3.0 |
| NLog | 6.0.5 |
| Swashbuckle.AspNetCore | 6.6.2 |
| Swashbuckle.AspNetCore.Annotations | 6.6.2 |
| Swashbuckle.AspNetCore.Filters | 8.0.2 |

---

## 🧾 Notes

- Ensure your **SSH key** is configured with GitHub to clone using SSH.  
- The project targets **.NET 8.0**, so make sure you have the correct SDK installed.  
- All dependencies are managed automatically via NuGet.  

---

## 🏗️ Project Structure

```
dg-membership-service/
│
├── Controllers/
├── Models/
├── Services/
├── Properties/
├── appsettings.json
├── Program.cs
├── Startup.cs
└── dg-membership-service.sln
```

---

## 📖 License

This project is part of the **DG Digital Commerce** internal suite and is intended for authorized use only.
