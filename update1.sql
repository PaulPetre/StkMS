BEGIN TRANSACTION
GO
CREATE TABLE dbo.Customers
	(
	CustomerId int NOT NULL,
	Name nvarchar(MAX) NOT NULL,
	Description nvarchar(MAX) NULL,
	Address nvarchar(MAX) NOT NULL,
	City nvarchar(MAX) NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Customers ADD CONSTRAINT
	PK_Customers PRIMARY KEY CLUSTERED 
	(
	CustomerId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Customers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Customers', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Customers', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Customers', 'Object', 'CONTROL') as Contr_Per 
