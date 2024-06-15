using System;
using System.Data.SqlTypes;
using Npgsql;

namespace tolyID
{
    class Program
    {
        static void Main(string[] args)
        {
            
                string cs = "Host=localhost;Username=postgres;Password=admin;Database=TolyID";

            using var con = new NpgsqlConnection(cs);
            con.Open();
            
            bool sair = false;

            while (sair == false)
            {
                CreateTables(con);
                Console.WriteLine("============= Bem vindo ao TolyID =============");
                Console.WriteLine("Escolha uma opção");
                Console.WriteLine("Opção 1 = Inserir dados da ficha de campo");
                Console.WriteLine("Opção 2 = Consultar dados pelo identificação do animal");
                Console.WriteLine("Opção 3 = Editar dados do animal (todos os dados exeto o número de identificação terão de ser preenchidos novamente)");
                Console.WriteLine("Opção 4 = Excluir animal do sistema");
                Console.WriteLine("Opção 5 = Sair");
                int opcaoMenu = Convert.ToInt32(Console.ReadLine());

                switch (opcaoMenu)
                {
                    case 1:
                        InsertFichaDeCampo(con);
                        break;
                    case 2:
                        BuscarTatuPeloID(con);
                        break;
                    case 3:
                        EditarTatuPeloID(con);
                        break;
                    case 4:
                        ExcluirTatuPeloID(con);
                        break;
                    case 5:
                        sair = true;
                        Console.WriteLine("Aplicação encerrada");
                        break;
                }
            }
            
        }

        static void CreateTables(NpgsqlConnection con)
        {
            string stm = @"
                CREATE TABLE IF NOT EXISTS Ficha_de_Campo (
                    Identificacao_do_animal VARCHAR PRIMARY KEY,
                    Equipe_responsavel VARCHAR,
                    Peso DECIMAL,
                    Numero_do_microchip INTEGER,
                    Horario_da_captura TIME,
                    Local_de_captura VARCHAR,
                    Contato_do_responsavel VARCHAR,
                    Numero_de_identificacao INTEGER,
                    Data_de_captura DATE,
                    Observacoes VARCHAR,
                    Instituicao VARCHAR,
                    UNIQUE (Numero_do_microchip, Numero_de_identificacao)
                );

                CREATE TABLE IF NOT EXISTS Biometria (
                    Identificacao_do_animal VARCHAR UNIQUE,
                    Comprimento_pe_sem_unha DECIMAL,
                    Semicircunferencia_escudo_pelvico DECIMAL,
                    Comprimento_da_cabeca DECIMAL,
                    Largura_escudo_cefalico DECIMAL,
                    Comprimento_escudo_cefalico DECIMAL,
                    Numero_de_cintas INTEGER,
                    Semicircunferencia_escudo_escapular DECIMAL,
                    Largura_cabeca DECIMAL,
                    Comprimento_do_clitoris DECIMAL,
                    Comprimento_escudo_pelvico DECIMAL,
                    Comprimento_total DECIMAL,
                    Comprimento_da_orelha DECIMAL,
                    Comprimento_da_cauda DECIMAL,
                    Padrao_escudo_cefalico DECIMAL,
                    Largura_cauda DECIMAL,
                    Largura_base_do_penis DECIMAL,
                    Comprimento_mao_sem_unha DECIMAL,
                    Comprimento_unha_da_mao DECIMAL,
                    Comprimento_do_penis DECIMAL,
                    Comprimento_unha_do_pe DECIMAL,
                    Largura_inter_orbital DECIMAL,
                    Largura_inter_lacrimal DECIMAL,
                    Comprimento_escudo_escapular DECIMAL,
                    largura_segunda_cinta DECIMAL,
                    Observacoes VARCHAR,
                    FOREIGN KEY (Identificacao_do_animal) REFERENCES Ficha_de_Campo (Identificacao_do_animal)
                );

                CREATE TABLE IF NOT EXISTS Amostras (
                    Identificacao_do_animal VARCHAR UNIQUE,
                    Fezes BOOLEAN,
                    Sawb BOOLEAN,
                    Pelo BOOLEAN,
                    Local BOOLEAN,
                    FOREIGN KEY (Identificacao_do_animal) REFERENCES Ficha_de_Campo (Identificacao_do_animal)
                );

                CREATE TABLE IF NOT EXISTS Parametros_fisiologicos (
                    Identificacao_do_animal VARCHAR UNIQUE,
                    FC VARCHAR,
                    FR VARCHAR,
                    Oximetria VARCHAR,
                    Temperatura VARCHAR,
                    FOREIGN KEY (Identificacao_do_animal) REFERENCES Ficha_anestesica (Identificacao_do_animal)
                );

                CREATE TABLE IF NOT EXISTS Ficha_anestesica (
                    Identificacao_do_animal VARCHAR UNIQUE,
                    Tempo_de_anestesia VARCHAR,
                    Tipo_do_anestesico_dose VARCHAR PRIMARY KEY,
                    Inducao VARCHAR,
                    Aplicacao VARCHAR,
                    Via_de_administracao VARCHAR,
                    Retorno VARCHAR,
                    Observacoes VARCHAR,
                    FOREIGN KEY (Identificacao_do_animal) REFERENCES Ficha_de_Campo (Identificacao_do_animal)
                );

                CREATE TABLE IF NOT EXISTS Equipe_Responsavel (
                    Identificacao_do_animal VARCHAR UNIQUE,
                    Cientistas VARCHAR,
                    Instituicao VARCHAR,
                    FOREIGN KEY (Identificacao_do_animal) REFERENCES Ficha_de_Campo (Identificacao_do_animal)
                );
            ";

            using var cmd = new NpgsqlCommand(stm, con);
            cmd.ExecuteNonQuery();
        }

        static void InsertFichaDeCampo(NpgsqlConnection con)
        {
            Console.WriteLine("Inserindo dados na Ficha de Campo");
            Console.Write("Identificação do Animal: ");
            string idAnimal = Console.ReadLine();
            Console.Write("Equipe Responsável: ");
            string equipe = Console.ReadLine();
            Console.Write("Peso: ");
            decimal peso = Convert.ToDecimal(Console.ReadLine());
            Console.Write("Número do Microchip: ");
            int microchip = Convert.ToInt32(Console.ReadLine());
            Console.Write("Horário da Captura (HH:MM:SS): ");           
            TimeSpan horario = TimeSpan.Parse(Console.ReadLine());
            Console.Write("Local de Captura: ");
            string local = Console.ReadLine();
            Console.Write("Contato do Responsável: ");
            string contato = Console.ReadLine();
            Console.Write("Número de Identificação: ");
            int numId = Convert.ToInt32(Console.ReadLine());
            Console.Write("Data de Captura (AAAA-MM-DD): ");
            DateTime dataCaptura = DateTime.Parse(Console.ReadLine());
            Console.Write("Observações: ");
            string observacoes = Console.ReadLine();
            Console.Write("Instituição: ");
            string instituicao = Console.ReadLine();

            string sql = @"
                INSERT INTO Ficha_de_Campo (Identificacao_do_animal, Equipe_responsavel, Peso, Numero_do_microchip, Horario_da_captura, Local_de_captura, Contato_do_responsavel, Numero_de_identificacao, Data_de_captura, Observacoes, Instituicao) 
                VALUES (@idAnimal, @equipe, @peso, @microchip, @horario, @local, @contato, @numId, @dataCaptura, @observacoes, @instituicao)";

            using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@idAnimal", idAnimal);
            cmd.Parameters.AddWithValue("@equipe", equipe);
            cmd.Parameters.AddWithValue("@peso", peso);
            cmd.Parameters.AddWithValue("@microchip", microchip);
            cmd.Parameters.AddWithValue("@horario", horario);
            cmd.Parameters.AddWithValue("@local", local);
            cmd.Parameters.AddWithValue("@contato", contato);
            cmd.Parameters.AddWithValue("@numId", numId);
            cmd.Parameters.AddWithValue("@dataCaptura", dataCaptura);
            cmd.Parameters.AddWithValue("@observacoes", observacoes);
            cmd.Parameters.AddWithValue("@instituicao", instituicao);

            cmd.ExecuteNonQuery();
            Console.WriteLine("Dados inseridos com sucesso!");
            Console.ReadLine();
            Console.Clear();
        }

        static void BuscarTatuPeloID(NpgsqlConnection con)
        {
            Console.WriteLine("Digite a identificação do animal que deseja buscar");
            string idAnimal = Console.ReadLine();

            string sql = "SELECT * FROM Ficha_de_Campo WHERE Identificacao_do_animal = @idAnimal";
            using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@idAnimal", idAnimal);
            
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Console.WriteLine($"Identificação do Animal: {reader["Identificacao_do_animal"]}");
                Console.WriteLine($"Equipe Responsável: {reader["Equipe_responsavel"]}");
                Console.WriteLine($"Peso: {reader["Peso"]}");
                Console.WriteLine($"Número do Microchip: {reader["Numero_do_microchip"]}");
                Console.WriteLine($"Horário da Captura: {reader["Horario_da_captura"]}");
                Console.WriteLine($"Local de Captura: {reader["Local_de_captura"]}");
                Console.WriteLine($"Contato do Responsável: {reader["Contato_do_responsavel"]}");
                Console.WriteLine($"Número de Identificação: {reader["Numero_de_identificacao"]}");
                Console.WriteLine($"Data de Captura: {reader["Data_de_captura"]}");
                Console.WriteLine($"Observações: {reader["Observacoes"]}");
                Console.WriteLine($"Instituição: {reader["Instituicao"]}");
            }
            else
            {
                Console.WriteLine("Animal não encontrado.");
            }
            Console.ReadLine();
            Console.Clear();
        }

        static void EditarTatuPeloID(NpgsqlConnection con)
        {
            Console.Write("Digite a Identificação do Animal para atualizar: (Será necessário editar todas as informações do animal)");
            string idAnimal = Console.ReadLine();

            if (idAnimal != null)
            {
                Console.WriteLine("Atualizando dados na Ficha de Campo");
                Console.Write("Equipe Responsável: ");
                string equipe = Console.ReadLine();
                Console.Write("Peso: ");
                decimal peso = Convert.ToDecimal(Console.ReadLine());
                Console.Write("Número do Microchip: ");
                int microchip = Convert.ToInt32(Console.ReadLine());
                Console.Write("Horário da Captura (HH:MM:SS): ");
                TimeSpan horario = TimeSpan.Parse(Console.ReadLine());
                Console.Write("Local de Captura: ");
                string local = Console.ReadLine();
                Console.Write("Contato do Responsável: ");
                string contato = Console.ReadLine();
                Console.Write("Data de Captura (AAAA-MM-DD): ");
                DateTime dataCaptura = DateTime.Parse(Console.ReadLine());
                Console.Write("Observações: ");
                string observacoes = Console.ReadLine();
                Console.Write("Instituição: ");
                string instituicao = Console.ReadLine();

                string sql = @"
               UPDATE Ficha_de_Campo 
               SET Equipe_responsavel = @equipe, Peso = @peso, Numero_do_microchip = @microchip, Horario_da_captura = @horario, Local_de_captura = @local, Contato_do_responsavel = @contato, Data_de_captura = @dataCaptura, Observacoes = @observacoes, Instituicao = @instituicao 
               WHERE Identificacao_do_animal = @idAnimal";

                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@idAnimal", idAnimal);
                cmd.Parameters.AddWithValue("@equipe", equipe);
                cmd.Parameters.AddWithValue("@peso", peso);
                cmd.Parameters.AddWithValue("@microchip", microchip);
                cmd.Parameters.AddWithValue("@horario", horario);
                cmd.Parameters.AddWithValue("@local", local);
                cmd.Parameters.AddWithValue("@contato", contato);
                cmd.Parameters.AddWithValue("@dataCaptura", dataCaptura);
                cmd.Parameters.AddWithValue("@observacoes", observacoes);
                cmd.Parameters.AddWithValue("@instituicao", instituicao);

                cmd.ExecuteNonQuery();
                Console.WriteLine("Dados atualizados com sucesso!");
            }
            else
            {
                Console.WriteLine("Não existe nenhum animal com esta identificação no banco de dados");
            }
            Console.ReadLine();
            Console.Clear();
        }
        static void ExcluirTatuPeloID(NpgsqlConnection con)
        {
            Console.Write("Digite a Identificação do Animal para excluir: ");
            string idAnimal = Console.ReadLine();

            string sql = "DELETE FROM Ficha_de_Campo WHERE Identificacao_do_animal = @idAnimal";

            using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@idAnimal", idAnimal);

            int affectedRows = cmd.ExecuteNonQuery();
            if (affectedRows > 0)
            {
                Console.WriteLine("Animal excluído com sucesso!");
            }
            else
            {
                Console.WriteLine("Animal não encontrado.");
            }
            Console.ReadLine();
            Console.Clear();
        }
    }
}
