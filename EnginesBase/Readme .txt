База данных двигателей и их компонентов

Схема EnginesDB:

TABLE Engines (
    EngineId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255)
);

TABLE Elements (
    ElementId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255)
);

TABLE ElementLinks (
    ElementLinkId INT PRIMARY KEY IDENTITY(1,1),
    ParentId INT,
    ChildId INT,
    Quantity INT,
    FOREIGN KEY (ParentId) REFERENCES Components(ComponentId) ON DELETE CASCADE,
    FOREIGN KEY (ChildId) REFERENCES Components(ComponentId) ON DELETE CASCADE,
    CHECK (ParentId <> ChildId) 
);

TABLE EngineLinks (
    EngineLinkId INT PRIMARY KEY IDENTITY(1,1),
    EngineId INT,
    ElementId INT,
    Quantity INT,
    FOREIGN KEY (EngineId) REFERENCES Engines(EngineId) ON DELETE CASCADE,
    FOREIGN KEY (ComponentId) REFERENCES Components(ComponentId) ON DELETE CASCADE
);

