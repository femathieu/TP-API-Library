
CREATE TABLE Users(
	Id int not null IDENTITY PRIMARY KEY,
	Firstname NVARCHAR(10) not null,
	Lastname NVARCHAR(10) not null,
	Login NVARCHAR(10) not null,
	Pwd NVARCHAR(10) not null,
	email NVARCHAR(10) not null UNIQUE,
	role NVARCHAR(10) not null
);

CREATE TABLE Books(
	Id int not null IDENTITY PRIMARY KEY,
	Title NVARCHAR(10) not null,
	Author NVARCHAR(10) not null
);

CREATE TABLE UsersBooks(
	Id int not null IDENTITY PRIMARY KEY,
	UsersId int not null,
	BooksId int not null,
	date smalldatetime not null,
	CONSTRAINT FK_UsersId_UsersId FOREIGN KEY (UsersId)
		REFERENCES Users (Id),
	CONSTRAINT FK_BooksId_BooksId FOREIGN KEY (BooksId)
		REFERENCES Books (Id)
);
