﻿using System;
using Microsoft.Data.SqlClient; // 使用 Microsoft.Data.SqlClient 库

class SqlTest
{
    static void Test()
    {
        // SQL Server的连接字符串（连接到默认数据库master）
        string masterConnectionString = "Server=localhost;Database=master;Integrated Security=true;TrustServerCertificate=True;";

        // 要创建的新数据库名称
        string databaseName = "DemoDatabase";

        // 连接到master数据库并创建新数据库
        using (SqlConnection masterConnection = new SqlConnection(masterConnectionString))
        {
            try
            {
                masterConnection.Open();
                Console.WriteLine("已连接到SQL Server (master 数据库)。");

                // 创建数据库的SQL查询
                string createDatabaseQuery = $"CREATE DATABASE {databaseName};";

                using (SqlCommand command = new SqlCommand(createDatabaseQuery, masterConnection))
                {
                    // 执行创建数据库的查询
                    command.ExecuteNonQuery();
                    Console.WriteLine($"数据库 '{databaseName}' 创建成功！");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建数据库时出错: {ex.Message}");
                return;
            }
        }

        // 新创建的数据库的连接字符串
        string demoDbConnectionString = $"Server=localhost;Database={databaseName};Integrated Security=true;TrustServerCertificate=True;";

        // 连接到新创建的数据库并创建表和插入数据
        using (SqlConnection demoDbConnection = new SqlConnection(demoDbConnectionString))
        {
            try
            {
                demoDbConnection.Open();
                Console.WriteLine($"已连接到新数据库 '{databaseName}'。");

                //表格需要s结尾，这样User类才能和Users表格对应起来
                // 创建表格SQL查询
                string createTableQuery = @"
-- 创建用户表
CREATE TABLE [Users] (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) NOT NULL,
    Password VARCHAR(255) NOT NULL,
    Role VARCHAR(50) NOT NULL,
    Power VARCHAR(50) NOT NULL,
    JobNumber VARCHAR(50) NOT NULL,
    Name VARCHAR(50) NULL
);

-- 创建申请表单
CREATE TABLE ApplicationForms (
    ApplicationFormID INT IDENTITY(1,1) PRIMARY KEY,
    ProjectLeader VARCHAR(50) NOT NULL,
    ContactWay VARCHAR(50) NULL,
    Department VARCHAR(50) NULL,
    ProjectName VARCHAR(150) NOT NULL,
    ProjectCategory VARCHAR(50) NOT NULL,
    ProjectLevel VARCHAR(50) NOT NULL,
    AwardLevel VARCHAR(50) NOT NULL,
    ParticipationForm VARCHAR(50) NOT NULL,
    ApprovalFileName VARCHAR(500) NOT NULL,
    ApprovalFileNumber VARCHAR(500) NOT NULL,
    ItemDescription VARCHAR(8000) NOT NULL,
    ProjectOutcome VARCHAR(8000) NOT NULL,
    Decision Int NOT NULL,
    AuditDepartment VARCHAR(50) NULL,
    Comments VARCHAR(8000) NULL,
    RecognitionLevel VARCHAR(50) NULL,
    DeemedAmount DECIMAL(10, 2) NULL,
    Remarks VARCHAR(8000) NULL,
    UserID INT NOT NULL,
    States INT NOT NULL,
    ApprovalDate SMALLDATETIME NOT NULL,
    FOREIGN KEY (UserID) REFERENCES [Users](UserID)
);

-- 创建审批记录表
CREATE TABLE ApprovalRecords (
    ApprovalRecordID INT IDENTITY(1,1) PRIMARY KEY,
    ApplicationFormID INT NOT NULL,
    UserID INT NOT NULL,
    ApprovalDate SMALLDATETIME NOT NULL,
    Decision INT NOT NULL,
    Comments VARCHAR(8000) NULL,
    FOREIGN KEY (ApplicationFormID) REFERENCES ApplicationForms(ApplicationFormID),
    FOREIGN KEY (UserID) REFERENCES [Users](UserID)
);

-- 创建审批表汇总
CREATE TABLE TableSummaries (
    TableSummaryID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    ApplicationFormID INT NOT NULL,
    Decision INT NOT NULL,
    FOREIGN KEY (UserID) REFERENCES [Users](UserID),
    FOREIGN KEY (ApplicationFormID) REFERENCES ApplicationForms(ApplicationFormID)
);
";

                using (SqlCommand command = new SqlCommand(createTableQuery, demoDbConnection))
                {
                    // 执行创建表的查询
                    command.ExecuteNonQuery();
                    Console.WriteLine("表格 'Employees' 创建成功！");
                }

                // // 插入数据SQL查询
                // string insertDataQuery = @"
                //     INSERT INTO Employees (Name, Age, Position) VALUES ('Alice', 30, 'Developer');
                //     INSERT INTO Employees (Name, Age, Position) VALUES ('Bob', 25, 'Designer');
                //     INSERT INTO Employees (Name, Age, Position) VALUES ('Charlie', 35, 'Manager');";
                //
                // using (SqlCommand command = new SqlCommand(insertDataQuery, demoDbConnection))
                // {
                //     // 执行插入数据的查询
                //     command.ExecuteNonQuery();
                //     Console.WriteLine("数据插入成功！");
                // }
                //
                // // 查询数据SQL查询
                // string selectQuery = "SELECT * FROM Employees";
                //
                // using (SqlCommand command = new SqlCommand(selectQuery, demoDbConnection))
                // {
                //     using (SqlDataReader reader = command.ExecuteReader())
                //     {
                //         Console.WriteLine("读取数据：");
                //         while (reader.Read())
                //         {
                //             Console.WriteLine($"Id: {reader["Id"]}, Name: {reader["Name"]}, Age: {reader["Age"]}, Position: {reader["Position"]}");
                //         }
                //     }
                // }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"操作失败: {ex.Message}");
            }
        }
    }
}
