namespace LojaKids.Model;

public class Cliente
{
    public int IdCliente { get; set; }
    public string NomeCliente { get; set; }
    public string EmailCliente { get; set; }
    public string TelefoneCliente { get; set; }
    public string EnderecoCliente { get; set; }
    public string CpfCliente { get; set; }

    public Cliente(string nome_cliente, string email_cliente, string telefone_cliente, 
        string endereco_cliente, string cpf_cliente)
    {
        NomeCliente = nome_cliente;
        TelefoneCliente = telefone_cliente;
        EmailCliente = email_cliente;
        TelefoneCliente = telefone_cliente;
        EnderecoCliente = endereco_cliente;
        CpfCliente = cpf_cliente;
    }
    
    public Cliente(int id_cliente, string telefone_cliente)
    {
        IdCliente = id_cliente;
        TelefoneCliente = telefone_cliente;
    }
    
    public Cliente(string email_cliente, string nome_cliente)
    {
        EmailCliente = email_cliente;
        NomeCliente = nome_cliente;
    }

    public Cliente(int id_cliente, string telefone_cliente, string endereco_cliente)
    {
        IdCliente = id_cliente;
        TelefoneCliente = telefone_cliente;
        EnderecoCliente = endereco_cliente;
    }

    public Cliente()
    {
    }
}