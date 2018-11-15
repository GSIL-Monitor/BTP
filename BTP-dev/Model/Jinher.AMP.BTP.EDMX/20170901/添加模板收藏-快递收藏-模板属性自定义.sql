   Alter Table ExpressOrderTemplateProperty
   Add [PropertyText] nvarchar(30)  NULL

   Go

   
-- Creating table 'ExpressCollection'
CREATE TABLE [dbo].[ExpressCollection] (
    [Id] uniqueidentifier  NOT NULL,
    [ExpCode] nvarchar(50)  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ExpressTemplateColletion'
CREATE TABLE [dbo].[ExpressTemplateCollection] (
    [Id] uniqueidentifier  NOT NULL,
    [TemplateId] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NULL
);
GO


-- Creating primary key on [Id] in table 'ExpressCollection'
ALTER TABLE [dbo].[ExpressCollection]
ADD CONSTRAINT [PK_ExpressCollection]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ExpressTemplateColletion'
ALTER TABLE [dbo].[ExpressTemplateCollection]
ADD CONSTRAINT [PK_ExpressTemplateCollection]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO