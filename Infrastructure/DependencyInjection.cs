using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Neo4j.Driver;
using Neo4jClient;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // services.AddSingleton(GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "qwerty123")));
            var driver = GraphDatabase.Driver(
                configuration["Neo4j:Host"],
                AuthTokens.Basic(
                    configuration["Neo4j:User"],
                    configuration["Neo4j:Pass"])
            );
            services.AddSingleton<IDriver>(driver);

            return services;
        }
    }
}