using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Domain.Entities;
using MediatR;
using Neo4j.Driver;

namespace Application.Files
{
    public class Create
    {
        public class Command : IRequest<Result<File>>
        {
            public string Path { get; set; }
            public string Filename { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<File>>
        {
            private readonly IDriver _driver;

            public Handler(IDriver driver)
            {
                _driver = driver;
            }

            public async Task<Result<File>> Handle(Command request, CancellationToken cancellationToken)
            {
                var command = "MATCH (fol:Folder {path: '" + request.Path + "'})" +
                               " CREATE (f:File {name: '" + request.Filename + "', path: '" + request.Path + "'})" +
                               " CREATE (f)-[:IN_FOLDER]->(fol)";

                // We use a 'Session' to perform our queries
                var session = _driver.AsyncSession();

                var results = await session.WriteTransactionAsync(async tx =>
                {
                    var folderExistsResult = await tx.RunAsync(command);

                    var result = folderExistsResult.ConsumeAsync();

                    return result;
                });

                return Result<File>.Success(new File { Path = request.Path, Name = request.Filename });
            }
        }
    }
}