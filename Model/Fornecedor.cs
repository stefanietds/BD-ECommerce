namespace LojaKids.Model;

public class Fornecedor
{
    public int CNPJMarca { get; set; }
    public string NomeMarca { get; set; }

    public Fornecedor(int CNPJ_marca, string nome_marca)
    {
        CNPJMarca = CNPJ_marca;
        NomeMarca = nome_marca;
    }
}