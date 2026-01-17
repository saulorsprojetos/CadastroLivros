USE CadastroLivros;
GO
INSERT INTO dbo.FormaCompra (Nome) VALUES ('Balcão'), ('Self-service'), ('Internet'), ('Evento');

INSERT INTO dbo.Autor (Nome) VALUES ('Machado de Assis'), ('Clarice Lispector');
INSERT INTO dbo.Assunto (Descricao) VALUES ('Romance'), ('Contos');

INSERT INTO dbo.Livro (Titulo, Editora, Edicao, AnoPublicacao)
VALUES ('Dom Casmurro', 'Editora X', 1, '1899');

INSERT INTO dbo.Livro_Autor (Livro_Codl, Autor_CodAu) VALUES (1, 1);
INSERT INTO dbo.Livro_Assunto (Livro_Codl, Assunto_codAs) VALUES (1, 1), (1, 2);

INSERT INTO dbo.Livro_Preco (Livro_Codl, FormaCompra_Id, Valor)
VALUES (1, 1, 39.90), (1, 3, 34.90);
GO
