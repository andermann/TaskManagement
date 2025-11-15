using FluentAssertions;
using TaskManagement.Domain.Entities;
using Xunit;

namespace TaskManagement.Tests.Domain
{
    public class UserTests
    {
        [Fact]
        public void Constructor_ShouldInitializeFieldsCorrectly()
        {
            var user = new User(1, "João", "joao@email.com", "Manager");

            user.Id.Should().Be(1);
            user.Name.Should().Be("João");
            user.Email.Should().Be("joao@email.com");
            user.Role.Should().Be("Manager");
        }

        [Fact]
        public void IsManager_ShouldReturnTrue_WhenRoleIsManager()
        {
            var user = new User(1, "A", "a@a.com", "manager");

            user.IsManager().Should().BeTrue();
        }

        [Fact]
        public void IsManager_ShouldBeCaseInsensitive()
        {
            var user = new User(1, "A", "a@a.com", "MaNaGeR");

            user.IsManager().Should().BeTrue();
        }

        [Fact]
        public void IsManager_ShouldReturnFalse_WhenRoleIsNotManager()
        {
            var user = new User(1, "A", "a@a.com", "user");

            user.IsManager().Should().BeFalse();
        }
    }
}
