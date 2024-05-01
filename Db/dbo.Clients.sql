CREATE TABLE [dbo].[Clients] (
    [id]         INT           IDENTITY (1, 1) NOT NULL,
    [lastName]   NVARCHAR (20) NOT NULL,
    [firstName]  NVARCHAR (20) NOT NULL,
    [middleName] NVARCHAR (20) NOT NULL,
    [phoneNumber]      VARCHAR (12)  NULL,
    [email]      NVARCHAR (40) NOT NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);

