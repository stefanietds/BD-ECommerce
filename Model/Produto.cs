using System.ComponentModel.DataAnnotations.Schema;

namespace LojaKids.Model;

public class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public int Tamanho { get; set; }
    public string Imagem { get; set; }
    public decimal QuantidadeEstoque { get; set; }
    public decimal Preco { get; set; }
    public string Cor { get; set; }
    public string Estilo { get; set; }
    public int FornecedorId { get; set; }
    public string NomeMarca { get; set; }

    public Produto(int id_produto, string nome_produto, int tamanho_produto, string imagem_produto, decimal quantidade_estoque_produto, decimal preco_produto, string cor_produto, 
        string estilo_produto, int fk_fornecedor, string nome_marca)
    {
        Id = id_produto;
        Nome = nome_produto;
        Tamanho = tamanho_produto;
        Imagem = imagem_produto;
        QuantidadeEstoque = quantidade_estoque_produto;
        Preco = preco_produto;
        Cor = cor_produto;
        Estilo = estilo_produto;
        FornecedorId = fk_fornecedor;
        NomeMarca = nome_marca;
    }
}