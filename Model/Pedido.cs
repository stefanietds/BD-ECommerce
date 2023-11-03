namespace LojaKids.Model;

public class Pedido
{
    public int IdPedido { get; set; }
    public DateTime DataPedido { get; set; }
    public string MetodoPagamentoPedido { get; set; }
    public decimal PrecoFinalPedido { get; set; }
    public int FkCliente { get; set; }
/*
    public Pedido(string metodo_pagamento_pedido)
    {
        MetodoPagamentoPedido = metodo_pagamento_pedido;
    }
    */
    public Pedido(int id_pedido, DateTime data_pedido, string metodo_pagamento_pedido, decimal preco_final_pedido, int fk_cliente)
    {
        IdPedido = id_pedido;
        DataPedido = data_pedido;
        MetodoPagamentoPedido = metodo_pagamento_pedido;
        PrecoFinalPedido = preco_final_pedido;
        FkCliente = fk_cliente;
    }
    
    public Pedido (){}
}