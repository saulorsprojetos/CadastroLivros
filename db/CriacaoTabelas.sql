CREATE DATABASE CadastroLivros;
GO
USE CadastroLivros;
GO

CREATE TABLE dbo.Livro (
    Codl INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Livro PRIMARY KEY,
    Titulo VARCHAR(40) NOT NULL,
    Editora VARCHAR(40) NOT NULL,
    Edicao INT NOT NULL,
    AnoPublicacao VARCHAR(4) NOT NULL
);

CREATE TABLE dbo.Autor (
    CodAu INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Autor PRIMARY KEY,
    Nome VARCHAR(40) NOT NULL
);

CREATE TABLE dbo.Assunto (
    codAs INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Assunto PRIMARY KEY,
    Descricao VARCHAR(20) NOT NULL
);

CREATE TABLE dbo.Livro_Autor (
    Livro_Codl INT NOT NULL,
    Autor_CodAu INT NOT NULL,
    CONSTRAINT PK_Livro_Autor PRIMARY KEY (Livro_Codl, Autor_CodAu),
    CONSTRAINT FK_LivroAutor_Livro FOREIGN KEY (Livro_Codl) REFERENCES dbo.Livro(Codl),
    CONSTRAINT FK_LivroAutor_Autor FOREIGN KEY (Autor_CodAu) REFERENCES dbo.Autor(CodAu)
);

CREATE TABLE dbo.Livro_Assunto (
    Livro_Codl INT NOT NULL,
    Assunto_codAs INT NOT NULL,
    CONSTRAINT PK_Livro_Assunto PRIMARY KEY (Livro_Codl, Assunto_codAs),
    CONSTRAINT FK_LivroAssunto_Livro FOREIGN KEY (Livro_Codl) REFERENCES dbo.Livro(Codl),
    CONSTRAINT FK_LivroAssunto_Assunto FOREIGN KEY (Assunto_codAs) REFERENCES dbo.Assunto(codAs)
);

CREATE INDEX IX_LivroAutor_Autor ON dbo.Livro_Autor (Autor_CodAu);
CREATE INDEX IX_LivroAssunto_Assunto ON dbo.Livro_Assunto (Assunto_codAs);

CREATE TABLE dbo.FormaCompra (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_FormaCompra PRIMARY KEY,
    Nome VARCHAR(30) NOT NULL CONSTRAINT UQ_FormaCompra_Nome UNIQUE
);

CREATE TABLE dbo.Livro_Preco (
    Livro_Codl INT NOT NULL,
    FormaCompra_Id INT NOT NULL,
    Valor DECIMAL(10,2) NOT NULL,
    CONSTRAINT PK_Livro_Preco PRIMARY KEY (Livro_Codl, FormaCompra_Id),
    CONSTRAINT FK_LivroPreco_Livro FOREIGN KEY (Livro_Codl) REFERENCES dbo.Livro(Codl),
    CONSTRAINT FK_LivroPreco_FormaCompra FOREIGN KEY (FormaCompra_Id) REFERENCES dbo.FormaCompra(Id),
    CONSTRAINT CK_LivroPreco_Valor CHECK (Valor >= 0)
);

CREATE INDEX IX_LivroPreco_FormaCompra ON dbo.Livro_Preco (FormaCompra_Id);

GO
