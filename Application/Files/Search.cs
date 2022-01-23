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
    public class Search
    {
        public class Query : IRequest<Result<List<File>>>
        {
            public string Filename { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<File>>>
        {
            private readonly IDriver _driver;

            public Handler(IDriver driver)
            {
                _driver = driver;
            }

            public async Task<Result<List<File>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = @$"MATCH (folder:Folder)<-[:IN_FOLDER]-(file:File) 
                               WHERE file.name starts with '{request.Filename}'
                               RETURN file";

                // We use a 'Session' to perform our queries
                var session = _driver.AsyncSession();

                var results = await session.ReadTransactionAsync(async tx =>
                {
                    // This is where we actually execute our code
                    var cursor = await tx.RunAsync(query);
                    // Now we 'fetch' our data - if we don't have any, 'fetched' will be false.
                    var fetched = await cursor.FetchAsync();
                    // This is where we'll store our output.
                    var output = new List<File>();

                    // While we _have_ data...
                    while (fetched)
                    {
                        // Get the 'Node'
                        var node = cursor.Current["file"].As<INode>();

                        // Convert it into a File 
                        var folder = ConvertToFile(node);

                        // Add to our output
                        output.Add(folder);

                        // Do we have more?
                        fetched = await cursor.FetchAsync();
                    }

                    // Return our output from the Transaction Function
                    return output;
                });

                // Return the Transaction Function results to the caller.
                return Result<List<File>>.Success(results);
            }

            private File ConvertToFile(INode node)
            {
                return new File
                {
                    Name = node.Properties["name"].As<string>(),
                    Path = node.Properties["path"].As<string>()
                };
            }
        }
    }
}