GO
USE CadastroLivros;
GO

CREATE OR ALTER VIEW dbo.vw_RelatorioLivrosPorAutor
AS
WITH Base AS
(
    SELECT
        a.CodAu,
        a.Nome AS AutorNome,
        l.Codl,
        l.Titulo,
        l.Editora,
        l.Edicao,
        l.AnoPublicacao
    FROM dbo.Autor a
    JOIN dbo.Livro_Autor la ON la.Autor_CodAu = a.CodAu
    JOIN dbo.Livro l        ON l.Codl = la.Livro_Codl
),
AssuntosAgg AS
(
    SELECT
        las.Livro_Codl AS Codl,
        STRING_AGG(s.Descricao, ', ') WITHIN GROUP (ORDER BY s.Descricao) AS Assuntos
    FROM dbo.Livro_Assunto las
    JOIN dbo.Assunto s ON s.codAs = las.Assunto_codAs
    GROUP BY las.Livro_Codl
),
PrecosAgg AS
(
    SELECT
        lp.Livro_Codl AS Codl,
        STRING_AGG(
            CONCAT(fc.Nome, ': R$ ', FORMAT(lp.Valor, 'N2', 'pt-BR')),
            ' | '
        ) WITHIN GROUP (ORDER BY fc.Nome) AS Precos
    FROM dbo.Livro_Preco lp
    JOIN dbo.FormaCompra fc ON fc.Id = lp.FormaCompra_Id
    GROUP BY lp.Livro_Codl
)
SELECT
    b.CodAu,
    b.AutorNome,
    b.Codl,
    b.Titulo,
    b.Editora,
    b.Edicao,
    b.AnoPublicacao,
    aa.Assuntos,
    pa.Precos
FROM Base b
LEFT JOIN AssuntosAgg aa ON aa.Codl = b.Codl
LEFT JOIN PrecosAgg   pa ON pa.Codl = b.Codl;
GO
