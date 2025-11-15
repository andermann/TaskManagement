namespace TaskManagement.Domain.Entities
{
    // A Entidade User é crucial para referenciar o dono do projeto e o usuário que fez a alteração.
    // Não precisa de CRUD interno conforme requisito, mas precisa existir.
    public class User
    {
        public int Id { get; private set; }
        // Adicionar o operador '!' para suprimir o aviso, dizendo que o EF Core vai inicializar.
        public string Name { get; private set; } = null!; // << CORREÇÃO
        public string Email { get; private set; } = null!; // << CORREÇÃO
        public string Role { get; private set; } = null!; // << CORREÇÃO

        // Construtor para EF Core
        private User() { }

        // Construtor para uso interno
        public User(int id, string name, string email, string role)
        {
            Id = id;
            Name = name;
            Email = email;
            Role = role;
        }

        public bool IsManager() => Role.Equals("manager", StringComparison.OrdinalIgnoreCase);
    }
}