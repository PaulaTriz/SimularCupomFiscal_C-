using System;

namespace SimuladorCupomFiscal
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("===========================================");
            Console.WriteLine("    SIMULADOR DE CUPOM FISCAL DE VENDAS    ");
            Console.WriteLine("===========================================\n");

            // Pergunta quantos produtos o utilizador deseja inserir
            int quantidadeProdutos = 0;
            while (true)
            {
                Console.Write("Quantos produtos deseja adicionar ao cupom (1 a 10)? ");
                if (int.TryParse(Console.ReadLine(), out quantidadeProdutos) && quantidadeProdutos >= 1 && quantidadeProdutos <= 10)
                    break;
                Console.WriteLine("Quantidade inválida! Insira um número entre 1 e 10.");
            }

            // Declaração dos arrays para armazenar os dados sem usar Listas
            string[] nomes = new string[quantidadeProdutos];
            decimal[] precosOriginais = new decimal[quantidadeProdutos];
            decimal[] percentuaisDesconto = new decimal[quantidadeProdutos];
            decimal[] valoresDesconto = new decimal[quantidadeProdutos];
            decimal[] precosComDesconto = new decimal[quantidadeProdutos];

            decimal subtotalOriginal = 0;
            decimal totalDescontoProdutos = 0;

            // 1. Entrada de Dados para cada produto
            for (int i = 0; i < quantidadeProdutos; i++)
            {
                Console.WriteLine($"\n--- Produto #{i + 1} ---");

                // Nome do produto
                do
                {
                    Console.Write("Nome do produto: ");
                    nomes[i] = Console.ReadLine()?.Trim();
                } while (string.IsNullOrWhiteSpace(nomes[i]));

                // Preço do produto com validação
                while (true)
                {
                    Console.Write("Preço do produto (R$): ");
                    if (decimal.TryParse(Console.ReadLine(), out precosOriginais[i]) && precosOriginais[i] >= 0)
                        break;
                    Console.WriteLine("Entrada inválida! Insira um valor numérico positivo.");
                }

                // Desconto em percentual com validação
                while (true)
                {
                    Console.Write("Desconto do produto (%): ");
                    if (decimal.TryParse(Console.ReadLine(), out percentuaisDesconto[i]) && percentuaisDesconto[i] >= 0 && percentuaisDesconto[i] <= 100)
                        break;
                    Console.WriteLine("Entrada inválida! Insira um valor percentual entre 0 e 100.");
                }

                // Cálculos individuais
                valoresDesconto[i] = precosOriginais[i] * (percentuaisDesconto[i] / 100m);
                precosComDesconto[i] = precosOriginais[i] - valoresDesconto[i];

                subtotalOriginal += precosOriginais[i];
                totalDescontoProdutos += valoresDesconto[i];
            }

            decimal subtotalComDesconto = subtotalOriginal - totalDescontoProdutos;

            // 2. Cliente VIP
            Console.Write("\nO cliente é VIP? (S/N): ");
            bool isVip = Console.ReadLine()?.Trim().ToLower() == "s";
            decimal descontoVip = isVip ? subtotalComDesconto * 0.05m : 0m;

            // 3. Cupom de Desconto Promocional
            Console.Write("Possui cupom de desconto? Digite o código (ou pressione Enter para pular): ");
            string codigoCupom = Console.ReadLine()?.Trim().ToUpper();
            decimal descontoCupom = 0m;

            if (codigoCupom == "PROMO10")
            {
                descontoCupom = (subtotalComDesconto - descontoVip) * 0.10m;
                Console.WriteLine(">> Cupom 'PROMO10' aplicado com sucesso! (10% OFF extra)");
            }

            decimal totalAposDescontos = subtotalComDesconto - descontoVip - descontoCupom;

            // 4. Taxa de Imposto
            decimal taxaImposto;
            while (true)
            {
                Console.Write("Digite a taxa de imposto (%): ");
                if (decimal.TryParse(Console.ReadLine(), out taxaImposto) && taxaImposto >= 0)
                    break;
                Console.WriteLine("Entrada inválida! Insira uma taxa percentual válida.");
            }

            decimal valorImposto = totalAposDescontos * (taxaImposto / 100m);
            decimal valorTotalFinal = totalAposDescontos + valorImposto;

            // 5. Opção de Pagamento em Parcelas
            Console.Write("\nForma de pagamento (1 - À vista | 2 - Parcelado): ");
            string opcaoPagamento = Console.ReadLine()?.Trim();
            int parcelas = 1;

            if (opcaoPagamento == "2")
            {
                while (true)
                {
                    Console.Write("Quantidade de parcelas (1 a 12): ");
                    if (int.TryParse(Console.ReadLine(), out parcelas) && parcelas >= 1 && parcelas <= 12)
                        break;
                    Console.WriteLine("Quantidade de parcelas inválida!");
                }
            }

            // =======================================================
            // EXIBIÇÃO DO CUPOM FISCAL
            // =======================================================
            Console.Clear();
            Console.WriteLine("==================================================");
            Console.WriteLine("               CUPOM FISCAL DE VENDA              ");
            Console.WriteLine("==================================================");
            Console.WriteLine($" Data/Hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine(string.Format("{0,-20} {1,10} {2,8} {3,8}", "Item", "Orig.", "Desc.", "Final"));
            Console.WriteLine("--------------------------------------------------");

            for (int i = 0; i < quantidadeProdutos; i++)
            {
                Console.WriteLine(string.Format("{0,-20} {1,10:C2} {2,7}% {3,8:C2}",
                    TruncarTexto(nomes[i], 20),
                    precosOriginais[i],
                    percentuaisDesconto[i],
                    precosComDesconto[i]));
            }

            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"Subtotal dos Produtos:             {subtotalOriginal,12:C2}");
            Console.WriteLine($"Desconto Total nos Produtos:       {-totalDescontoProdutos,12:C2}");

            if (isVip)
            {
                Console.WriteLine($"Desconto Cliente VIP (5%):          {-descontoVip,12:C2}");
            }
            if (descontoCupom > 0)
            {
                Console.WriteLine($"Desconto Cupom PROMO10 (10%):       {-descontoCupom,12:C2}");
            }

            Console.WriteLine($"Impostos ({taxaImposto}%):                     {valorImposto,12:C2}");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"VALOR TOTAL FINAL:                 {valorTotalFinal,12:C2}");
            Console.WriteLine("--------------------------------------------------");

            if (parcelas > 1)
            {
                decimal valorParcela = valorTotalFinal / parcelas;
                Console.WriteLine($"Pagamento: Parcelado em {parcelas}x de {valorParcela:C2}");
            }
            else
            {
                Console.WriteLine("Pagamento: À vista");
            }

            Console.WriteLine("==================================================");
            Console.WriteLine("          OBRIGADO E VOLTE SEMPRE!                ");
            Console.WriteLine("==================================================");

            Console.WriteLine("\nPressione qualquer tecla para sair...");
            Console.ReadKey();
        }

        // Função auxiliar para truncar textos longos
        static string TruncarTexto(string texto, int tamanhoMaximo)
        {
            if (string.IsNullOrEmpty(texto)) return string.Empty;
            return texto.Length <= tamanhoMaximo ? texto : texto.Substring(0, tamanhoMaximo - 3) + "...";
        }
    }
}
