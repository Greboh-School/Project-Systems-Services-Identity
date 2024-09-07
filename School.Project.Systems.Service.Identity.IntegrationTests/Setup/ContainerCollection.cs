using School.Shared.Tools.Test.Containers;
using Xunit;

namespace School.Project.Systems.Service.Identity.IntegrationTests.Setup;

[CollectionDefinition("mysql")]
public class ContainerCollection : ICollectionFixture<MySQLContainerFixture>
{
    
}