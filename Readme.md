# Stock Simulator API 📈

A scalable ASP.NET Core Web API for simulating stock market transactions. This project allows users to register, manage their virtual balance, and securely buy or sell shares.

It was built with a strong focus on **clean architecture, concurrency handling, and comprehensive unit testing**.

## ✨ Key Features

* **Secure User Management:** Built-in registration and authentication system utilizing `ASP.NET Core Identity`.
* **Concurrency Protection:** Solves the "double-spending" race condition problem using transactions with a repeatable reads isolation level. Ensures that multiple rapid requests and the change of Stock's price during the transaction do not result in a negative account balance.
* **Portfolio & History:** Accurately tracks user balances, currently owned shares (`UserShares`), and keeps a detailed ledger of all operations (`Transactions`).
* **Deeply Tested:** High code coverage using `xUnit` and `Moq`. Tests are isolated using short-lived in-memory SQLite databases, verifying not only HTTP responses but also the actual database state.

## 🛠️ Tech Stack

* **Framework:** .NET 10 / ASP.NET Core Web API
* **Database & ORM:** Entity Framework Core, SQLite (Physical file for dev, In-Memory for tests)
* **Authentication:** ASP.NET Core Identity
* **Testing:** xUnit, Moq
* **Documentation:** Swagger / OpenAPI

## 📂 Project Structure

The repository is divided into a clean structure separating the application logic from the testing environment:

* `StockSimulator.Api/` - The main Web API project containing Controllers, DTOs, Models, and EF Core DbContext.
* `StockSimulator.Tests/` - The test project containing isolated unit tests.

## 🚀 Getting Started

### Prerequisites
* [.NET 10.0 SDK](https://dotnet.microsoft.com/download).

### Installation & Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/mateuszkuprewicz/StockSimulator.git
   cd StockSimulator
   ```

2. **Navigate to the API directory:**
   ```bash
   cd StockSimulator.Api
   ```

3. **Apply database migrations:**
   This will create the physical `app.db` SQLite file and apply the initial schema.
   ```bash
   dotnet ef database update
   ```

4. **Run the application:**
   ```bash
   dotnet run
   ```

5. **Explore the API:**
   Once running, open your browser and navigate to the Swagger UI to test the endpoints interactively:
   `http://localhost:<port>/swagger`

## 🧪 Running Tests

The project features a suite of independent unit tests that do not require a physical database to run (they utilize EF Core SQLite In-Memory).

To execute the tests, run the following command in the root directory:
```bash
dotnet test
```

## 📖 Main API Endpoints

### Authentication
* `POST /api/authentication/register` - Register a new user with a starting balance.
* `POST /api/authentication/login` - Authenticate and log in.

### Stocks
* `POST /api/stocks/buy` - Buy a specific amount of shares (requires sufficient balance).
* `POST /api/stocks/sell` - Sell a specific amount of owned shares.

## 🗺️ Roadmap (Future Enhancements)

This project is actively evolving. Planned features include:

* [ ] **Order Book System (P2P Trading):** Transitioning from direct system buy/sell actions to a real market model. Users will be able to place custom buy/sell offers, and an underlying matching engine will execute trades between different users.
* [ ] **AI Trading Bots:** Implementing automated entities (AI companies/investors) that actively participate in the market, generating organic price movements and trading volume.
* [ ] **Email Verification:** Enhancing the registration process with standard email confirmation links and password reset capabilities.
* [ ] **Containerization:** Adding Docker and `docker-compose` support for simplified deployment, ensuring environment consistency across development and production.

---
