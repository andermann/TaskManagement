//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using FluentAssertions;
//using TaskManagement.Domain.Entities;
//using TaskManagement.Domain.Enums;
//using Xunit;

//namespace TaskManagement.Tests.Domain
//{
//    public class ProjectTests
//    {
//        [Fact]
//        public void Constructor_deve_inicializar_campos()
//        {
//            var project = new Project("Titulo", "Descricao", 10);

//            project.Title.Should().Be("Titulo");
//            project.Description.Should().Be("Descricao");
//            project.OwnerId.Should().Be(10);
//        }

//        [Fact]
//        public void CheckTaskLimit_deve_lancar_quando_ultrapassar_MaxTasks()
//        {
//            var project = new Project("Proj", "Desc", 1);

//            // Usa reflection porque _tasks é privado
//            var tasksField = typeof(Project)
//                .GetField("_tasks", BindingFlags.NonPublic | BindingFlags.Instance);

//            var list = new List<TaskItem>();

//            for (int i = 0; i < Project.MaxTasks; i++)
//            {
//                list.Add(new TaskItem
//                {
//                    Id = i + 1,
//                    ProjectId = project.Id,
//                    Title = $"Task {i + 1}",
//                    Description = "Desc",
//                    Status = TaskStatus.Pending
//                });
//            }

//            tasksField!.SetValue(project, list);

//            Action act = () =>
//            {
//                // chama o método que faz a checagem
//                typeof(Project)
//                    .GetMethod("CheckTaskLimit", BindingFlags.NonPublic | BindingFlags.Instance)!
//                    .Invoke(project, null);
//            };

//            act.Should().Throw<TargetInvocationException>()
//               .WithInnerException<InvalidOperationException>();
//        }

//        [Fact]
//        public void HasPendingTasks_deve_retornar_true_quando_existir_tarefa_pendente_ou_em_andamento()
//        {
//            var project = new Project("Proj", "Desc", 1);

//            var tasksField = typeof(Project)
//                .GetField("_tasks", BindingFlags.NonPublic | BindingFlags.Instance);

//            var list = new List<TaskItem>
//            {
//                new TaskItem { Status = TaskStatus.Completed },
//                new TaskItem { Status = TaskStatus.InProgress },
//            };

//            tasksField!.SetValue(project, list);

//            project.HasPendingTasks().Should().BeTrue();
//        }

//        [Fact]
//        public void HasPendingTasks_deve_retornar_false_quando_todas_completas()
//        {
//            var project = new Project("Proj", "Desc", 1);

//            var tasksField = typeof(Project)
//                .GetField("_tasks", BindingFlags.NonPublic | BindingFlags.Instance);

//            var list = new List<TaskItem>
//            {
//                new TaskItem { Status = TaskStatus.Completed },
//                new TaskItem { Status = TaskStatus.Completed },
//            };

//            tasksField!.SetValue(project, list);

//            project.HasPendingTasks().Should().BeFalse();
//        }
//    }
//}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using Xunit;

namespace TaskManagement.Tests.Domain
{
    public class ProjectTests
    {
        // Helper para setar tarefas privadas de forma segura
        private static void SetTasks(Project project, List<TaskItem> tasks)
        {
            var field = typeof(Project).GetField("_tasks", BindingFlags.NonPublic | BindingFlags.Instance);
            field!.SetValue(project, tasks);
        }

        [Fact]
        public void Constructor_deve_inicializar_campos()
        {
            var project = new Project("Titulo", "Descricao", 10);

            project.Title.Should().Be("Titulo");
            project.Description.Should().Be("Descricao");
            project.OwnerId.Should().Be(10);
        }

        [Fact]
        public void CheckTaskLimit_deve_lancar_quando_ultrapassar_MaxTasks()
        {
            var project = new Project("Proj", "Desc", 1);

            var tasks = Enumerable.Range(1, Project.MaxTasks)
                .Select(i => new TaskItem
                {
                    Id = i,
                    Title = $"Task {i}",
                    Description = "Desc",
                    Status = TaskStatus.Pending
                })
                .ToList();

            SetTasks(project, tasks);

            Action act = () => project.CheckTaskLimit();

            act.Should().Throw<InvalidOperationException>()
               .WithMessage("*limite máximo*");
        }

        [Fact]
        public void CheckTaskLimit_nao_deve_lancar_quando_abaixo_do_limite()
        {
            var project = new Project("Proj", "Desc", 1);

            var tasks = new List<TaskItem>
            {
                new TaskItem { Status = TaskStatus.Pending }
            };

            SetTasks(project, tasks);

            Action act = () => project.CheckTaskLimit();

            act.Should().NotThrow();
        }

        [Fact]
        public void HasPendingTasks_deve_retornar_true_quando_existir_tarefa_InProgress()
        {
            var project = new Project("Proj", "Desc", 1);

            var tasks = new List<TaskItem>
            {
                new TaskItem { Status = TaskStatus.Completed },
                new TaskItem { Status = TaskStatus.InProgress }
            };

            SetTasks(project, tasks);

            project.HasPendingTasks().Should().BeTrue();
        }

        [Fact]
        public void HasPendingTasks_deve_retornar_true_quando_existir_tarefa_Pendente()
        {
            var project = new Project("Proj", "Desc", 1);

            var tasks = new List<TaskItem>
            {
                new TaskItem { Status = TaskStatus.Pending }
            };

            SetTasks(project, tasks);

            project.HasPendingTasks().Should().BeTrue();
        }

        [Fact]
        public void HasPendingTasks_deve_retornar_false_quando_todas_completas()
        {
            var project = new Project("Proj", "Desc", 1);

            var tasks = new List<TaskItem>
            {
                new TaskItem { Status = TaskStatus.Completed },
                new TaskItem { Status = TaskStatus.Completed }
            };

            SetTasks(project, tasks);

            project.HasPendingTasks().Should().BeFalse();
        }

        [Fact]
        public void HasPendingTasks_deve_retornar_false_quando_lista_vazia()
        {
            var project = new Project("Proj", "Desc", 1);

            SetTasks(project, new List<TaskItem>());

            project.HasPendingTasks().Should().BeFalse();
        }
    }
}
