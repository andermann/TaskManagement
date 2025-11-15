// DTO de Saída (View Model)
public class ProjectListDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string Description { get; set; }
    public int TaskCount { get; set; } // Pode ser útil
}

// Query de Entrada
public class GetProjectsByOwnerIdQuery
{
    public int OwnerId { get; set; } // O Id do usuário logado
}

public class GetProjectsByIdQuery
{
    public int Id { get; set; } // O Id do Projeto
}