CREATE TABLE Comments (
      Id INT AUTO_INCREMENT PRIMARY KEY,
      Content VARCHAR(1000) NOT NULL,
      CreatedAt DATETIME NOT NULL,
      TaskItemId INT NOT NULL,
      CONSTRAINT FK_Comments_TaskItems
          FOREIGN KEY (TaskItemId) REFERENCES TaskItems(Id)
              ON DELETE CASCADE
);