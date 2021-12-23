# Resizing Database Images Using CLR Store Procedure

SQL code has a limitation. Initially (without using plug-in or tools), SQL Server is a data storage system, and it is unable to edit our VARBINARY(MAX) images. However, since SQL Server is written in C#, we can "talk" to her in C# too (same as Excel and VBA). The official term for this feature is called [CLR Integration](https://docs.microsoft.com/en-us/sql/relational-databases/clr-integration/clr-integration-overview?view=sql-server-ver15), and we are creating a [CLR Stored Procedure](https://docs.microsoft.com/en-us/previous-versions/sql/sql-server-2008/ms131094(v=sql.100)) in this project. To rephrase, we are building a store procedure in C#.

## Getting Started

A potential issue for most development project nowadays is installing and setting up the project environment. Usually, it occupies 40% of your total development time. Visual Studio IDE offers one-stop service for creating CLR Integration; there are tons of predefined templates. To simplify our process, We will use Visual Studio 2019 in this project. An installation link will be included in the Installing section.

### Prerequisites

Since SQL Server is from Microsoft. We will use as much of Microsoft's product as possible. Here are the prerequisites for this project:

* Windows Vista or above.
* SQL Server 2014 or above.
* SQL Server Management Studio (SSMS).
* Visual Studio IDE.

### Installing

* [SQL Server Downloads](https://www.microsoft.com/en-ca/sql-server/sql-server-downloads)
* [Download SQL Server Management Studio (SSMS)](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15)
* [Download Visual Studio 2019 for Windows](https://visualstudio.microsoft.com/downloads/)

## Running the tests

### NOTE

To enable CLR integration, you must have ALTER SETTINGS server-level permission, which is implicitly held by members of the sysadmin and serveradmin fixed server roles. Please make sure your account satisfied before to follow along.

Now we have everything we need, let's start cracking. Here is the flow of the project.

1. Configure our database for CLR Integration. Open our SSMS, use the target database. Put in the following SQL:

    ```sql
        EXEC sp_configure 'clr enabled', 1;
        RECONFIGURE;  

        EXEC sp_configure 'show advanced options', 1;
        RECONFIGURE;

        EXEC sp_configure 'clr strict security', 0;
        RECONFIGURE;

        EXEC sp_configure 'Ole Automation Procedures', 1;
        RECONFIGURE;

        CREATE ASSEMBLY SystemWeb from 'C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Drawing.dll' WITH PERMISSION_SET = UNSAFE;
        GO
    ```

    Note: If you see error on 'clr strict security', ignore it. Else if you see error on ... run the following code:

    ```sql
        add later ..
    ```

2. Now we are ready to code in Visual Studio IDE. The file **jpegResize.cs** contains the code we used for this project. You can replace the C# code they used while following the detail link down below:

   * Note: For the following link, please stop right before you reach **Publish database connection**
   * [step by step implementation to create CLRstored procedure in Visual Studio.](https://karthiktechblog.com/sql/work-with-clr-database-object-to-create-clr-stored-procedure-using-c-with-vs-2019)

3. Instead of clicking the **Publish** button, click on **Generate Script**. A sql script like the following will be generated.

   ```sql
        GO
        SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

        SET NUMERIC_ROUNDABORT OFF;

        GO
        :setvar DatabaseName "SqlCLR"
        :setvar DefaultFilePrefix "SqlCLR"
        :setvar DefaultDataPath "C:\Users\Kkannan\AppData\Local\Mic..."
        :setvar DefaultLogPath "C:\Users\Kkannan\AppData\Local\Mic..."

        GO
        :on error exit
        GO

        ...
   ```

   Please scroll down to the very bottom of the script. You will see a SQL statement like the following:

   ```sql
        add later ...
   ```

   Now, change **8000** to **MAX**. The reason we do that is simply that the default column length for CLR Stored Precedure is 8000; since our images are stored in VARBINARY(MAX) format, data exceed 8000 will be truncated. An image would only show the top 8000 pixels, which is not what we want.

4. Save your sql script, and click run. (the green triangle at the top).

5. Well Done! You deploy the CLR Store Procedure. Now, go back to your database at SSMS, and refresh your cache. The file **ImageResize.sql** contains the SQL cursor for modifying all large images. Make sure you comment our the update statement and test it.

## Deployment

We have deployed our CLR Stored Procedure to our SQL Server. However, the solution we created is not sharing by all databases. We will still need some extra work if we want to deploy it to another database(or the database in another server). However, the good news is we don't need to go back to Visual Studio IDE and do all the work again. We can export the CLR Store Procedure and its assemblies in SSMS. Here is the link for steps:

* [How to transfer or create CLR stored procedure from one database to other database in sql server](https://www.exacthelp.com/2012/02/how-to-transfer-or-create-clr-stored.html)

    ---
    **NOTE**

    You will get a file similar to **config.sql** in the current folder. However, if you are deploying to another server, you will still need to enable CLR. For your convenient, I added the code for allowing CLR to **config.sql**.

    ---

## Authors

* **Luca Zhou** - *Initial work* - [chefZau](https://github.com/chefZau)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* [Inspiration](https://stackoverflow.com/questions/10631127/change-image-size-from-150150-to-7070-in-using-sql-query)
