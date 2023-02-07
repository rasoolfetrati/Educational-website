# Learning Web Site

A website built with ASP.NET Core and SQL Server 2019 for learning purposes.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

1. [.NET Core SDK 6](https://dotnet.microsoft.com/download/dotnet-core)
2. [SQL Server 2019](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Installing

1. Clone the repository:
git clone https://github.com/rasoolfetrati/LearningWebSite/.git

2. Navigate to the project directory:

cd LearningWebSite

3. Restore the packages and build the solution:

dotnet restore
dotnet build

4. Update the connection string in the `appsettings.json` file to match your SQL Server installation.

5. Run the following command to create the database and seed the data:

dotnet ef database update

6. Finally, run the application:

dotnet run

The application should now be running on `https://localhost:5001`.

## Built With

- [ASP.NET Core 6](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore)
- [SQL Server 2019](https://docs.microsoft.com/en-us/sql/sql-server/?view=sql-server-ver15)

## Contributing

Please read [CONTRIBUTING.md](https://github.com/rasoolfetrati/LearningWebSite/blob/master/CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.



## Authors

- [Rasool Fetrati](https://github.com/<rasoolfetrati>)

## License

This project is licensed under the MIT License - see the [LICENSE.md](https://github.com/rasoolfetrati/LearningWebSite/blob/master/LICENSE) file for details.

## Acknowledgments

- [Inspiration](https://github.com/rasoolfetrati/LearningWebSite/)

